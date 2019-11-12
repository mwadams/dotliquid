// <copyright file="ErrorsOutputModeEnum.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    /// <summary>
    /// Errors output mode.
    /// </summary>
    public enum ErrorsOutputMode
    {
        /// <summary>
        /// Rethrow the errors
        /// </summary>
        Rethrow,

        /// <summary>
        /// Suppress the errors
        /// </summary>
        Suppress,

        /// <summary>
        /// DIsplay the errors
        /// </summary>
        Display,
    }
}
