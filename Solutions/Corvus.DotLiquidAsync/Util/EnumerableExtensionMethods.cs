// <copyright file="EnumerableExtensionMethods.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class EnumerableExtensionMethods
    {
        public static IEnumerable Flatten(this IEnumerable array)
        {
            foreach (object item in array)
            {
                if (item is string || !(item is IEnumerable))
                {
                    yield return item;
                }
                else
                {
                    foreach (object subitem in Flatten((IEnumerable)item))
                    {
                        yield return subitem;
                    }
                }
            }
        }

        public static void EachWithIndex(this IEnumerable<object> array, Action<object, int> callback)
        {
            int index = 0;
            foreach (object item in array)
            {
                callback(item, index);
                ++index;
            }
        }
    }
}
