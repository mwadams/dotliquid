// <copyright file="INamingConvention.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.NamingConventions
{
    public interface INamingConvention
    {
        System.StringComparer StringComparer { get; }

        string GetMemberName(string name);

        bool OperatorEquals(string testedOperator, string referenceOperator);
    }
}
