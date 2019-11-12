// <copyright file="MaximumIterationsExceededException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Exceptions
{
    internal class MaximumIterationsExceededException : RenderException
    {
        public MaximumIterationsExceededException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }

        public MaximumIterationsExceededException()
        {
        }
    }
}
