// <copyright file="IRenderable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
