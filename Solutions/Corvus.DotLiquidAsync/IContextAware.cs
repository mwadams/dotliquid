// <copyright file="IContextAware.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid
{
    public interface IContextAware
    {
        Context Context { set; }
    }
}