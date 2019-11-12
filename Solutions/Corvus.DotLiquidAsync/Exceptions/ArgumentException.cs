// <copyright file="ArgumentException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class ArgumentException : LiquidException
    {
        public ArgumentException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }

        public ArgumentException()
        {
        }
    }
}
