// <copyright file="Assign.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
    /// Assign sets a variable in your template.
    ///
    /// {% assign foo = 'monkey' %}
    ///
    /// You can then use the variable later in the page.
    ///
    /// {{ foo }}.
    /// </summary>
    public class Assign : Tag
    {
        private static readonly Regex Syntax = R.B(R.Q(@"({0}+)\s*=\s*(.*)\s*"), Liquid.VariableSignature);

        private string to;
        private Variable from;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                this.to = syntaxMatch.Groups[1].Value;
                this.from = new Variable(syntaxMatch.Groups[2].Value);
            }
            else
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("AssignTagSyntaxException"));
            }

            base.Initialize(tagName, markup, tokens);
        }

        public async override Task RenderAsync(Context context, TextWriter result)
        {
            context.Scopes.Last()[this.to] = await this.from.RenderAsync(context).ConfigureAwait(false);
        }
    }
}
