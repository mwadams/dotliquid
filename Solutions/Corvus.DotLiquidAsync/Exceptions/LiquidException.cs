// <copyright file="LiquidException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public abstract class LiquidException :
#if CORE
        Exception
#else
        ApplicationException
#endif
    {
        protected LiquidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LiquidException(string message)
            : base(message)
        {
        }

        protected LiquidException()
        {
        }
    }
}
