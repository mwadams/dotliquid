// <copyright file="VariableNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class VariableNotFoundException : LiquidException
    {
        public VariableNotFoundException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }

        public VariableNotFoundException(string message)
            : base(message)
        {
        }
    }
}
