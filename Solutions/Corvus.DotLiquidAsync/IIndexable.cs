// <copyright file="IIndexable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid
{
    public interface IIndexable
    {
        object this[object key] { get; }

        bool ContainsKey(object key);
    }
}
