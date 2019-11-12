// <copyright file="FileSystemException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Exceptions
{
    using System;

#if !CORE
    [Serializable]
#endif
    public class FileSystemException : LiquidException
    {
        public FileSystemException(string message, params string[] args)
            : base(string.Format(message, args))
        {
        }
    }
}
