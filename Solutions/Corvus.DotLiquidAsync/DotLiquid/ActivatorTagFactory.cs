// <copyright file="ActivatorTagFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    using System;

    /// <summary>
    /// Tag factory using System.Activator to instanciate the tag.
    /// </summary>
    public class ActivatorTagFactory : ITagFactory
    {
        private readonly Type tagType;

        /// <summary>
        /// Gets name of the tag.
        /// </summary>
        public string TagName { get; }

        /// <summary>
        /// Instanciates a new ActivatorTagFactory.
        /// </summary>
        /// <param name="tagType">Name of the tag.</param>
        /// <param name="tagName">Type of the tag. must inherit from DotLiquid.Tag.</param>
        public ActivatorTagFactory(Type tagType, string tagName)
        {
            this.tagType = tagType;
            this.TagName = tagName;
        }

        /// <summary>
        /// Creates the tag.
        /// </summary>
        /// <returns></returns>
        public Tag Create()
        {
            return (Tag)Activator.CreateInstance(this.tagType);
        }
    }
}
