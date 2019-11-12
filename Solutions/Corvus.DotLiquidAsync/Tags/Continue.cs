// <copyright file="Continue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Tags
{
    using System.IO;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;

    public class Continue : Tag
    {
        public override Task RenderAsync(Context context, TextWriter result)
        {
            return Task.FromException(new ContinueInterrupt());
        }
    }
}
