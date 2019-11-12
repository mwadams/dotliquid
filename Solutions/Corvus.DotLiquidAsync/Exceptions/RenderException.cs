// <copyright file="RenderException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public abstract class RenderException :
#if CORE
        Exception
#else
        ApplicationException
#endif
    {
        protected RenderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RenderException(string message)
            : base(message)
        {
        }

        protected RenderException()
        {
        }
    }
}
