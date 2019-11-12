// <copyright file="ObjectExtensionMethods.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Util
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ObjectExtensionMethods
    {
        public static bool RespondTo(this object value, string member, bool ensureNoParameters = true)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type type = value.GetType();

            MethodInfo methodInfo = type.GetRuntimeMethod(member, Type.EmptyTypes);
            if (methodInfo != null && (!ensureNoParameters || !methodInfo.GetParameters().Any()))
            {
                return true;
            }

            PropertyInfo propertyInfo = type.GetRuntimeProperty(member);
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                return true;
            }

            return false;
        }

        public static object Send(this object value, string member, object[] parameters = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type type = value.GetType();

            MethodInfo methodInfo = type.GetRuntimeMethod(member, Type.EmptyTypes);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(value, parameters);
            }

            PropertyInfo propertyInfo = type.GetRuntimeProperty(member);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(value, null);
            }

            return null;
        }
    }
}
