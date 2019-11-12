// <copyright file="Cycle.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;
    using DotLiquid.Util;

    /// <summary>
    /// Cycle is usually used within a loop to alternate between values, like colors or DOM classes.
    ///
    ///   {% for item in items %}
    ///    &lt;div class="{% cycle 'red', 'green', 'blue' %}"&gt; {{ item }} &lt;/div&gt;
    ///   {% end %}
    ///
    ///    &lt;div class="red"&gt; Item one &lt;/div&gt;
    ///    &lt;div class="green"&gt; Item two &lt;/div&gt;
    ///    &lt;div class="blue"&gt; Item three &lt;/div&gt;
    ///    &lt;div class="red"&gt; Item four &lt;/div&gt;
    ///    &lt;div class="green"&gt; Item five&lt;/div&gt;.
    /// </summary>
    public class Cycle : Tag
    {
        private static readonly Regex SimpleSyntax = R.B(R.Q(@"^{0}+"), Liquid.QuotedFragment);
        private static readonly Regex NamedSyntax = R.B(R.Q(@"^({0})\s*\:\s*(.*)"), Liquid.QuotedFragment);
        private static readonly Regex QuotedFragmentRegex = R.B(R.Q(@"\s*({0})\s*"), Liquid.QuotedFragment);

        private string[] variables;
        private string name;

        /// <summary>
        /// Initializes the cycle tag.
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="markup"></param>
        /// <param name="tokens"></param>
        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match match = NamedSyntax.Match(markup);
            if (match.Success)
            {
                this.variables = VariablesFromString(match.Groups[2].Value);
                this.name = match.Groups[1].Value;
            }
            else
            {
                match = SimpleSyntax.Match(markup);
                if (match.Success)
                {
                    this.variables = VariablesFromString(markup);
                    this.name = "'" + string.Join(string.Empty, this.variables) + "'";
                }
                else
                {
                    throw new SyntaxException(Liquid.ResourceManager.GetString("CycleTagSyntaxException"));
                }
            }

            base.Initialize(tagName, markup, tokens);
        }

        private static string[] VariablesFromString(string markup)
        {
            return markup.Split(',').Select(var =>
            {
                Match match = QuotedFragmentRegex.Match(var);
                return (match.Success && !string.IsNullOrEmpty(match.Groups[1].Value))
                    ? match.Groups[1].Value
                    : null;
            }).ToArray();
        }

        /// <summary>
        /// Renders the cycle tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task RenderAsync(Context context, TextWriter result)
        {
            context.Registers["cycle"] = context.Registers["cycle"] ?? new Hash(0);

            context.Stack(async () =>
            {
                string key = context[this.name].ToString();
                int iteration = (int)(((Hash)context.Registers["cycle"])[key] ?? 0);
                await result.WriteAsync(context[this.variables[iteration]].ToString()).ConfigureAwait(false);
                ++iteration;
                if (iteration >= this.variables.Length)
                {
                    iteration = 0;
                }

                ((Hash)context.Registers["cycle"])[key] = iteration;
            });

            return Task.CompletedTask;
        }
    }
}
