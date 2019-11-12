// <copyright file="LiquidTypeAttribute.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid
{
    using System;

    /// <summary>
    /// Specifies the type is safe to be rendered by DotLiquid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LiquidTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets an array of property and method names that are allowed to be called on the object.
        /// </summary>
        public string[] AllowedMembers { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="allowedMembers">An array of property and method names that are allowed to be called on the object.</param>
        public LiquidTypeAttribute(params string[] allowedMembers)
        {
            this.AllowedMembers = allowedMembers;
        }
    }
}
