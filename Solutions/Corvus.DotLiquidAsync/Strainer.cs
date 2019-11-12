// <copyright file="Strainer.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using DotLiquid.Exceptions;

    internal static class DictionaryExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1314:Type parameter names should begin with T", Justification = "Matches pattern for dictionary.")]
        public static V TryAdd<K, V>(this IDictionary<K, V> dic, K key, Func<V> factory)
        {
            if (!dic.TryGetValue(key, out V found))
            {
                return dic[key] = factory();
            }

            return found;
        }
    }

    /// <summary>
    /// Strainer is the parent class for the filters system.
    /// New filters are mixed into the strainer class which is then instanciated for each liquid template render run.
    ///
    /// One of the strainer's responsibilities is to keep malicious method calls out.
    /// </summary>
    public class Strainer
    {
        private static readonly Dictionary<string, Type> Filters = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Tuple<object, MethodInfo>> FilterFuncs = new Dictionary<string, Tuple<object, MethodInfo>>();

        public static void GlobalFilter(Type filter)
        {
            Filters[filter.AssemblyQualifiedName] = filter;
        }

        public static void GlobalFilter(string rawName, object target, MethodInfo methodInfo)
        {
            string name = Template.NamingConvention.GetMemberName(rawName);

            FilterFuncs[name] = Tuple.Create(target, methodInfo);
        }

        public static Strainer Create(Context context)
        {
            var strainer = new Strainer(context);

            foreach (KeyValuePair<string, Type> keyValue in Filters)
            {
                strainer.Extend(keyValue.Value);
            }

            foreach (KeyValuePair<string, Tuple<object, MethodInfo>> keyValue in FilterFuncs)
            {
                strainer.AddMethodInfo(keyValue.Key, keyValue.Value.Item1, keyValue.Value.Item2);
            }

            return strainer;
        }

        private readonly Context context;
        private readonly Dictionary<string, IList<Tuple<object, MethodInfo>>> methods = new Dictionary<string, IList<Tuple<object, MethodInfo>>>();

        public IEnumerable<MethodInfo> Methods
        {
            get { return this.methods.Values.SelectMany(m => m.Select(x => x.Item2)); }
        }

        public Strainer(Context context)
        {
            this.context = context;
        }

        /// <summary>
        /// In this C# implementation, we can't use mixins. So we grab all the static
        /// methods from the specified type and use them instead.
        /// </summary>
        /// <param name="type"></param>
        public void Extend(Type type)
        {
            // From what I can tell, calls to Extend should replace existing filters. So be it.
            IEnumerable<MethodInfo> methods = type.GetRuntimeMethods().Where(m => m.IsPublic && m.IsStatic);
            IEnumerable<string> methodNames = methods.Select(m => Template.NamingConvention.GetMemberName(m.Name));

            foreach (string methodName in methodNames)
            {
                this.methods.Remove(methodName);
            }

            foreach (MethodInfo methodInfo in methods)
            {
                this.AddMethodInfo(methodInfo.Name, null, methodInfo);
            } // foreach
        }

        public void AddFunction<TIn, TOut>(string rawName, Func<TIn, TOut> func)
        {
            this.AddMethodInfo(rawName, func.Target, func.GetMethodInfo());
        }

        public void AddFunction<TIn, TIn2, TOut>(string rawName, Func<TIn, TIn2, TOut> func)
        {
            this.AddMethodInfo(rawName, func.Target, func.GetMethodInfo());
        }

        public void AddMethodInfo(string rawName, object target, MethodInfo method)
        {
            string name = Template.NamingConvention.GetMemberName(rawName);
            this.methods.TryAdd(name, () => new List<Tuple<object, MethodInfo>>()).Add(Tuple.Create(target, method));
        }

        public bool RespondTo(string method)
        {
            return this.methods.ContainsKey(method);
        }

        public object Invoke(string method, List<object> args)
        {
            // First, try to find a method with the same number of arguments minus context which we set automatically further down.
            Tuple<object, MethodInfo> methodInfo = this.methods[method].FirstOrDefault(m =>
                m.Item2.GetParameters().Where(p => p.ParameterType != typeof(Context)).Count() == args.Count);

            // If we failed to do so, try one with max numbers of arguments, hoping
            // that those not explicitly specified will be taken care of
            // by default values
            if (methodInfo == null)
            {
                methodInfo = this.methods[method].OrderByDescending(m => m.Item2.GetParameters().Length).First();
            }

            ParameterInfo[] parameterInfos = methodInfo.Item2.GetParameters();

            // If first parameter is Context, send in actual context.
            if (parameterInfos.Length > 0 && parameterInfos[0].ParameterType == typeof(Context))
            {
                args.Insert(0, this.context);
            }

            // Add in any default parameters - .NET won't do this for us.
            if (parameterInfos.Length > args.Count)
            {
                for (int i = args.Count; i < parameterInfos.Length; ++i)
                {
                    if ((parameterInfos[i].Attributes & ParameterAttributes.HasDefault) != ParameterAttributes.HasDefault)
                    {
                        throw new SyntaxException(Liquid.ResourceManager.GetString("StrainerFilterHasNoValueException"), method, parameterInfos[i].Name);
                    }

                    args.Add(parameterInfos[i].DefaultValue);
                }
            }

            try
            {
                return methodInfo.Item2.Invoke(methodInfo.Item1, args.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
