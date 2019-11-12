// <copyright file="SyntaxException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class SyntaxException : LiquidException
    {
        public SyntaxException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }
    }
}
