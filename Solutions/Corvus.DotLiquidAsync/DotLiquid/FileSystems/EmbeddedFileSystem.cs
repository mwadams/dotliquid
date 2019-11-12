// <copyright file="EmbeddedFileSystem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.FileSystems
{
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;

    /// <summary>
    /// This implements a file system which retrieves template files from embedded resources in .NET assemblies.
    ///
    /// Its behavior is the same as with the Local File System, except this uses namespaces and embedded resources
    /// instead of directories and files.
    ///
    /// Example:
    ///
    /// var fileSystem = new EmbeddedFileSystem("My.Base.Namespace");
    ///
    /// fileSystem.FullPath("mypartial") # => "My.Base.Namespace._mypartial.liquid"
    /// fileSystem.FullPath("dir/mypartial") # => "My.Base.Namespace.dir._mypartial.liquid".
    /// </summary>
    public class EmbeddedFileSystem : IFileSystem
    {
        protected System.Reflection.Assembly Assembly { get; private set; }

        public string Root { get; private set; }

        public EmbeddedFileSystem(System.Reflection.Assembly assembly, string root)
        {
            this.Assembly = assembly;
            this.Root = root;
        }

        public Task<string> ReadTemplateFileAsync(Context context, string templateName)
        {
            string templatePath = (string)context[templateName];
            string fullPath = this.FullPath(templatePath);

            Stream stream = this.Assembly.GetManifestResourceStream(fullPath);
            if (stream == null)
            {
                throw new FileSystemException(
                    Liquid.ResourceManager.GetString("LocalFileSystemTemplateNotFoundException"), templatePath);
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEndAsync();
        }

        public string FullPath(string templatePath)
        {
            if (templatePath == null || !Regex.IsMatch(templatePath, @"^[^.\/][a-zA-Z0-9_\/]+$"))
            {
                throw new FileSystemException(
                    Liquid.ResourceManager.GetString("LocalFileSystemIllegalTemplateNameException"), templatePath);
            }

            string basePath = templatePath.Contains("/")
                ? Path.Combine(this.Root, Path.GetDirectoryName(templatePath))
                : this.Root;

            string fileName = string.Format("_{0}.liquid", Path.GetFileName(templatePath));

            string fullPath = Regex.Replace(Path.Combine(basePath, fileName), @"\\|/", ".");

            return fullPath;
        }
    }
}
