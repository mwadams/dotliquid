// <copyright file="FilterNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class FilterNotFoundException : LiquidException
    {
        public FilterNotFoundException(string message, FilterNotFoundException innerException)
            : base(message, innerException)
        {
        }

        public FilterNotFoundException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }

        public FilterNotFoundException(string message)
            : base(message)
        {
        }
    }
}
