// <copyright file="TableRow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Tags.Html
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;
    using DotLiquid.Util;

    /// <summary>
    /// TablerRow tag.
    /// </summary>
    /// <example>
    /// &lt;table&gt;
    ///   {% tablerow product in collection.products %}
    ///     {{ product.title }}
    ///   {% endtablerow %}
    /// &lt;/table&gt;.
    /// </example>
    public class TableRow : DotLiquid.Block
    {
        private static readonly Regex Syntax = R.B(R.Q(@"(\w+)\s+in\s+({0}+)"), Liquid.VariableSignature);

        private string variableName;
        private string collectionName;
        private Dictionary<string, string> attributes;

        /// <summary>
        /// Initializes the tablerow tag.
        /// </summary>
        /// <param name="tagName">Name of the parsed tag.</param>
        /// <param name="markup">Markup of the parsed tag.</param>
        /// <param name="tokens">Toeksn of the parsed tag.</param>
        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                this.variableName = syntaxMatch.Groups[1].Value;
                this.collectionName = syntaxMatch.Groups[2].Value;
                this.attributes = new Dictionary<string, string>(Template.NamingConvention.StringComparer);
                R.Scan(markup, Liquid.TagAttributes, (key, value) => this.attributes[key] = value);
            }
            else
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("TableRowTagSyntaxException"));
            }

            base.Initialize(tagName, markup, tokens);
        }

        /// <summary>
        /// Renders the tablerow tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async override Task RenderAsync(Context context, TextWriter result)
        {
            object coll = context[this.collectionName];

            if (!(coll is IEnumerable))
            {
                return;
            }

            IEnumerable<object> collection = ((IEnumerable)coll).Cast<object>();

            if (this.attributes.ContainsKey("offset"))
            {
                int offset = Convert.ToInt32(this.attributes["offset"]);
                collection = collection.Skip(offset);
            }

            if (this.attributes.ContainsKey("limit"))
            {
                int limit = Convert.ToInt32(this.attributes["limit"]);
                collection = collection.Take(limit);
            }

            collection = collection.ToList();
            int length = collection.Count();

            int cols = Convert.ToInt32(context[this.attributes["cols"]]);

            int row = 1;
            int col = 0;

            result.WriteLine("<tr class=\"row1\">");
            await context.Stack(async () =>
            {
                int index = 0;

                foreach (object item in collection)
                {
                    context[this.variableName] = item;
                    context["tablerowloop"] = Hash.FromAnonymousObject(
                        new
                        {
                            length = length,
                            index = index + 1,
                            index0 = index,
                            col = col + 1,
                            col0 = col,
                            rindex = length - index,
                            rindex0 = length - index - 1,
                            first = index == 0,
                            last = index == length - 1,
                            col_first = col == 0,
                            col_last = col == cols - 1,
                        });

                    ++col;

                    using (TextWriter temp = new StringWriter(result.FormatProvider))
                    {
                        await this.RenderAllAsync(this.NodeList, context, temp).ConfigureAwait(false);
                        result.Write("<td class=\"col{0}\">{1}</td>", col, temp.ToString());
                    }

                    if (col == cols && index != length - 1)
                    {
                        col = 0;
                        ++row;
                        result.WriteLine("</tr>");
                        result.Write("<tr class=\"row{0}\">", row);
                    }

                    ++index;
                }
            }).ConfigureAwait(false);
            result.WriteLine("</tr>");
        }
    }
}
