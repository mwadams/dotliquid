// <copyright file="Case.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Tags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;
    using DotLiquid.Util;

    public class Case : DotLiquid.Block
    {
        private static readonly Regex Syntax = R.B(@"({0})", Liquid.QuotedFragment);
        private static readonly Regex WhenSyntax = R.B(@"({0})(?:(?:\s+or\s+|\s*\,\s*)({0}.*))?", Liquid.QuotedFragment);

        private List<Condition> blocks;
        private string left;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            this.blocks = new List<Condition>();

            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                this.left = syntaxMatch.Groups[1].Value;
            }
            else
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagSyntaxException"));
            }

            base.Initialize(tagName, markup, tokens);
        }

        public override void UnknownTag(string tag, string markup, List<string> tokens)
        {
            this.NodeList = new List<object>();
            switch (tag)
            {
                case "when":
                    this.RecordWhenCondition(markup);
                    break;
                case "else":
                    this.RecordElseCondition(markup);
                    break;
                default:
                    base.UnknownTag(tag, markup, tokens);
                    break;
            }
        }

        public override Task RenderAsync(Context context, TextWriter result)
        {
            return context.Stack(async () =>
            {
                bool executeElseBlock = true;
                foreach (Condition block in this.blocks)
                {
                    if (block.IsElse)
                    {
                        if (executeElseBlock)
                        {
                            await this.RenderAllAsync(block.Attachment, context, result).ConfigureAwait(false);
                            return;
                        }
                    }
                    else if (block.Evaluate(context, result.FormatProvider))
                    {
                        executeElseBlock = false;
                        await this.RenderAllAsync(block.Attachment, context, result).ConfigureAwait(false);
                    }
                }
            });
        }

        private void RecordWhenCondition(string markup)
        {
            while (markup != null)
            {
                // Create a new nodelist and assign it to the new block
                Match whenSyntaxMatch = WhenSyntax.Match(markup);
                if (!whenSyntaxMatch.Success)
                {
                    throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagWhenSyntaxException"));
                }

                markup = whenSyntaxMatch.Groups[2].Value;
                if (string.IsNullOrEmpty(markup))
                {
                    markup = null;
                }

                var block = new Condition(this.left, "==", whenSyntaxMatch.Groups[1].Value);
                block.Attach(this.NodeList);
                this.blocks.Add(block);
            }
        }

        private void RecordElseCondition(string markup)
        {
            if (markup.Trim() != string.Empty)
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("CaseTagElseSyntaxException"));
            }

            var block = new ElseCondition();
            block.Attach(this.NodeList);
            this.blocks.Add(block);
        }
    }
}
