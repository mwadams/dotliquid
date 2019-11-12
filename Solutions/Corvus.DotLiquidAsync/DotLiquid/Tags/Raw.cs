// <copyright file="Raw.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Tags
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using DotLiquid.Util;

    /// <summary>
    /// Raw
    /// Raw outputs text as is, usefull if your template contains Liquid syntax.
    ///
    /// {% raw %}{% if user = 'tobi' %}hi{% endif %}{% endraw %}.
    /// </summary>
    public class Raw : DotLiquid.Block
    {
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
