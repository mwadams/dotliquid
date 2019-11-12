// <copyright file="TypeUtility.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Util
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeUtility
    {
        private const TypeAttributes AnonymousTypeAttributes = TypeAttributes.NotPublic;

        public static bool IsAnonymousType(Type t)
        {
            return t.GetTypeInfo().GetCustomAttribute<CompilerGeneratedAttribute>() != null
                && t.GetTypeInfo().IsGenericType
                    && (t.Name.Contains("AnonymousType") || t.Name.Contains("AnonType"))
                        && (t.Name.StartsWith("<>") || t.Name.StartsWith("VB$"))
                            && (t.GetTypeInfo().Attributes & AnonymousTypeAttributes) == AnonymousTypeAttributes;
        }
    }
}
