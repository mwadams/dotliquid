// <copyright file="ContextException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class ContextException : LiquidException
    {
        public ContextException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }

        public ContextException()
        {
        }
    }
}
