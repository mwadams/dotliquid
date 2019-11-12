// <copyright file="ITemplateFileSystem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.FileSystems
{
    using System.Threading.Tasks;

    /// <summary>
    /// This interface allow you return a Template instance,
    /// it can reduce the template parsing time in some cases.
    /// Please also provide the implementation of ReadTemplateFile for fallback purpose.
    /// </summary>
    public interface ITemplateFileSystem : IFileSystem
    {
        /// <summary>
        /// Called by Liquid to retrieve a template instance.
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        Task<Template> GetTemplateAsync(Context context, string templateName);
    }
}
