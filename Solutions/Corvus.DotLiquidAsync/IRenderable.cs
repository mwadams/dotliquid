// <copyright file="IRenderable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Object that can render itslef.
    /// </summary>
    internal interface IRenderable
    {
        Task RenderAsync(Context context, TextWriter result);
    }
}
