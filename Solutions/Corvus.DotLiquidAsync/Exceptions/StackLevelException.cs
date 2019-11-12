// <copyright file="StackLevelException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class StackLevelException : LiquidException
    {
        public StackLevelException(string message)
            : base(string.Format(message))
        {
        }
    }
}
