// <copyright file="BlankFileSystem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid.FileSystems
{
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;

    public class BlankFileSystem : IFileSystem
    {
        public Task<string> ReadTemplateFileAsync(Context context, string templateName)
        {
            throw new FileSystemException(Liquid.ResourceManager.GetString("BlankFileSystemDoesNotAllowIncludesException"));
        }
    }
}
