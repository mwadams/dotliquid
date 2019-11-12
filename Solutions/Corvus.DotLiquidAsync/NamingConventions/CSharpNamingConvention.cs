// <copyright file="CSharpNamingConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.NamingConventions
{
    using System;

    public class CSharpNamingConvention : INamingConvention
    {
        public StringComparer StringComparer
        {
            get { return StringComparer.Ordinal; }
        }

        public string GetMemberName(string name)
        {
            return name;
        }

        public bool OperatorEquals(string testedOperator, string referenceOperator)
        {
            return UpperFirstLetter(testedOperator).Equals(referenceOperator)
                    || LowerFirstLetter(testedOperator).Equals(referenceOperator);
        }

        private static string UpperFirstLetter(string word)
        {
            return char.ToUpperInvariant(word[0]) + word.Substring(1);
        }

        private static string LowerFirstLetter(string word)
        {
            return char.ToUpperInvariant(word[0]) + word.Substring(1);
        }
    }
}
