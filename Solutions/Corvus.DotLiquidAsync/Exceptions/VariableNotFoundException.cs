// <copyright file="VariableNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

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
