// <copyright file="IFileSystem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.FileSystems
{
    using System.Threading.Tasks;

    /// <summary>
    /// A Liquid file system is way to let your templates retrieve other templates for use with the include tag.
    ///
    /// You can implement subclasses that retrieve templates from the database, from the file system using a different
    /// path structure, you can provide them as hard-coded inline strings, or any manner that you see fit.
    ///
    /// You can add additional instance variables, arguments, or methods as needed.
    ///
    /// Example:
    ///
    /// Liquid::Template.file_system = Liquid::LocalFileSystem.new(template_path)
    /// liquid = Liquid::Template.parse(template)
    ///
    /// This will parse the template with a LocalFileSystem implementation rooted at 'template_path'.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Called by Liquid to retrieve a template file.
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        Task<string> ReadTemplateFileAsync(Context context, string templateName);
    }
}
