// <copyright file="ITagFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    /// <summary>
    /// Interface for tag factory.
    /// </summary>
    /// <remarks>Can be usefull when the tag needs a parameter and can't be created with parameterless constructor.</remarks>
    public interface ITagFactory
    {
        /// <summary>
        /// Gets name of the tag.
        /// </summary>
        string TagName { get; }

        /// <summary>
        /// Creates the tag.
        /// </summary>
        /// <returns></returns>
        Tag Create();
    }
}
