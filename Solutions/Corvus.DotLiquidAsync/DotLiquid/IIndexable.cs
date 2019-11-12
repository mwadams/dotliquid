// <copyright file="IIndexable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    public interface IIndexable
    {
        object this[object key] { get; }

        bool ContainsKey(object key);
    }
}
