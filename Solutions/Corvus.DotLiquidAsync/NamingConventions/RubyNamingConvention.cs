// <copyright file="RubyNamingConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.NamingConventions
{
    using System;
    using System.Text.RegularExpressions;
    using DotLiquid.Util;

    /// <summary>
    /// Converts C# member names to Ruby-style names for access by Liquid templates.
    /// </summary>
    /// <example>
    /// Input: Text
    /// Output: text
    ///
    /// Input: ScopesAsArray
    /// Output: scopes_as_array.
    /// </example>
    public class RubyNamingConvention : INamingConvention
    {
        private static readonly Regex Regex1 = R.C(@"([A-Z]+)([A-Z][a-z])");
        private static readonly Regex Regex2 = R.C(@"([a-z\d])([A-Z])");

        public StringComparer StringComparer
        {
            get { return StringComparer.OrdinalIgnoreCase; }
        }

        public string GetMemberName(string name)
        {
            // Replace any capital letters, apart from the first character, with _x, the same way Ruby does
            return Regex2.Replace(Regex1.Replace(name, "$1_$2"), "$1_$2").ToLowerInvariant();
        }

        public bool OperatorEquals(string testedOperator, string referenceOperator)
        {
            return this.GetMemberName(testedOperator).Equals(referenceOperator);
        }
    }
}
