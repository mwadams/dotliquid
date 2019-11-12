// <copyright file="FilterNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
