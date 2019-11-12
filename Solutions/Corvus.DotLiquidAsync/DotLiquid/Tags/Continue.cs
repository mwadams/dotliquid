// <copyright file="Continue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

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
