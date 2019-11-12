// <copyright file="INamingConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.NamingConventions
{
    public interface INamingConvention
    {
        System.StringComparer StringComparer { get; }

        string GetMemberName(string name);

        bool OperatorEquals(string testedOperator, string referenceOperator);
    }
}
