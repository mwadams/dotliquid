// <copyright file="Include.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Tags
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;
    using DotLiquid.FileSystems;
    using DotLiquid.Util;

    public class Include : DotLiquid.Block
    {
        private static readonly Regex Syntax = R.B(@"({0}+)(\s+(?:with|for)\s+({0}+))?", Liquid.QuotedFragment);

        private string templateName;
        private string variableName;
        private Dictionary<string, string> attributes;

        public override void Initialize(string tagName, string markup, List<string> tokens)
        {
            Match syntaxMatch = Syntax.Match(markup);
            if (syntaxMatch.Success)
            {
                this.templateName = syntaxMatch.Groups[1].Value;
                this.variableName = syntaxMatch.Groups[3].Value;
                if (this.variableName == string.Empty)
                {
                    this.variableName = null;
                }

                this.attributes = new Dictionary<string, string>(Template.NamingConvention.StringComparer);
                R.Scan(markup, Liquid.TagAttributes, (key, value) => this.attributes[key] = value);
            }
            else
            {
                throw new SyntaxException(Liquid.ResourceManager.GetString("IncludeTagSyntaxException"));
            }

            base.Initialize(tagName, markup, tokens);
        }

        protected override void Parse(List<string> tokens)
        {
        }

        public async override Task RenderAsync(Context context, TextWriter result)
        {
            IFileSystem fileSystem = context.Registers["file_system"] as IFileSystem ?? Template.FileSystem;
            var templateFileSystem = fileSystem as ITemplateFileSystem;
            Template partial = null;
            if (templateFileSystem != null)
            {
                partial = await templateFileSystem.GetTemplateAsync(context, this.templateName).ConfigureAwait(false);
            }

            if (partial == null)
            {
                string source = await fileSystem.ReadTemplateFileAsync(context, this.templateName).ConfigureAwait(false);
                partial = Template.Parse(source);
            }

            string shortenedTemplateName = this.templateName.Substring(1, this.templateName.Length - 2);
            object variable = context[this.variableName ?? shortenedTemplateName, this.variableName != null];

            await context.Stack(async () =>
            {
                foreach (KeyValuePair<string, string> keyValue in this.attributes)
                {
                    context[keyValue.Key] = context[keyValue.Value];
                }

                if (variable is IEnumerable)
                {
                    foreach (object v in ((IEnumerable)variable).Cast<object>().ToList())
                    {
                        context[shortenedTemplateName] = v;
                        await partial.RenderAsync(result, RenderParameters.FromContext(context, result.FormatProvider)).ConfigureAwait(false);
                    }

                    return;
                }

                context[shortenedTemplateName] = variable;
                await partial.RenderAsync(result, RenderParameters.FromContext(context, result.FormatProvider)).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}
