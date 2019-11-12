// <copyright file="Literal.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Tags
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using DotLiquid.Util;

    /// <summary>
    /// Literal
    /// Literal outputs text as is, usefull if your template contains Liquid syntax.
    ///
    /// {% literal %}{% if user = 'tobi' %}hi{% endif %}{% endliteral %}
    ///
    /// or (shorthand version)
    ///
    /// {{{ {% if user = 'tobi' %}hi{% endif %} }}}.
    /// </summary>
    public class Literal : DotLiquid.Block
    {
        private static readonly Regex LiteralRegex = R.C(Liquid.LiteralShorthand);

        /// <summary>
        /// Creates a literal from shorthand.
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string FromShortHand(string @string)
        {
            if (@string == null)
            {
                return @string;
            }

            Match match = LiteralRegex.Match(@string);
            return match.Success ? string.Format(@"{{% literal %}}{0}{{% endliteral %}}", match.Groups[1].Value) : @string;
        }

        /// <summary>
        /// Parses the tag.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void Parse(List<string> tokens)
        {
            this.NodeList ??= new List<object>();
            this.NodeList.Clear();

            string token;
            while ((token = tokens.Shift()) != null)
            {
                Match fullTokenMatch = FullToken.Match(token);
                if (fullTokenMatch.Success && this.BlockDelimiter == fullTokenMatch.Groups[1].Value)
                {
                    this.EndTag();
                    return;
                }
                else
                {
                    this.NodeList.Add(token);
                }
            }

            this.AssertMissingDelimitation();
        }
    }
}
