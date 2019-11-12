// <copyright file="Drop.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
// Derived from code under the Apache 2 License from https://github.com/dotliquid/dotliquid

namespace DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using DotLiquid.NamingConventions;
    using DotLiquid.Util;

    /// <summary>
    ///     Configurable typing metadata collection.
    /// </summary>
    internal class TypeResolution
    {
        public Dictionary<string, MethodInfo> CachedMethods { get; private set; }

        public Dictionary<string, PropertyInfo> CachedProperties { get; private set; }

        public TypeResolution(Type type, Func<MemberInfo, bool> filterMemberCallback)
        {
            // Cache all methods and properties of this object, but don't include those
            // defined at or above the base Drop class.
            this.CachedMethods = this.GetMemberDictionary(
                GetMethodsWithoutDuplicateNames(type, mi => mi.GetParameters().Length == 0),
                mi => filterMemberCallback(mi));

            this.CachedProperties = this.GetMemberDictionary(GetPropertiesWithoutDuplicateNames(type), mi => filterMemberCallback(mi));
        }

        private Dictionary<string, T> GetMemberDictionary<T>(IEnumerable<T> members, Func<T, bool> filterMemberCallback)
            where T : MemberInfo
        {
            return members.Where(filterMemberCallback)
                          .ToDictionary(mi => Template.NamingConvention.GetMemberName(mi.Name), Template.NamingConvention.StringComparer);
        }

        /// <summary>
        ///     Gets all of the properties for a type, filtering out properties with duplicate names by choosing the property with
        ///     the most derived declaring type.
        /// </summary>
        /// <param name="type">Type to get properties for.</param>
        /// <param name="bindingFlags">Binding flags for properties.</param>
        /// <param name="predicate">Any additional filtering on properties.</param>
        /// <returns>Filtered properties.</returns>
        private static IEnumerable<PropertyInfo> GetPropertiesWithoutDuplicateNames(Type type, Func<PropertyInfo, bool> predicate = null)
        {
            IList<MemberInfo> properties = predicate != null
                                               ? type.GetRuntimeProperties()
                                                     .Where(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)
                                                     .Where(predicate)
                                                     .Cast<MemberInfo>()
                                                     .ToList()
                                               : type.GetRuntimeProperties()
                                                     .Where(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsStatic)
                                                     .Cast<MemberInfo>()
                                                     .ToList();

            return GetMembersWithoutDuplicateNames(properties)
                .Cast<PropertyInfo>();
        }

        /// <summary>
        ///     Gets all of the methods for a type, filtering out methods with duplicate names by choosing the method with the most
        ///     derived declaring type.
        /// </summary>
        /// <param name="type">Type to get methods for.</param>
        /// <param name="bindingFlags">Binding flags for methods.</param>
        /// <param name="predicate">Any additional filtering on methods.</param>
        /// <returns>Filtered methods.</returns>
        private static IEnumerable<MethodInfo> GetMethodsWithoutDuplicateNames(Type type, Func<MethodInfo, bool> predicate = null)
        {
            IList<MemberInfo> methods = predicate != null
                                            ? type
                                                  .GetRuntimeMethods()
                                                  .Where(m => m.IsPublic && !m.IsStatic)
                                                  .Where(predicate)
                                                  .Cast<MemberInfo>()
                                                  .ToList()
                                            : type
                                                  .GetRuntimeMethods()
                                                  .Where(m => m.IsPublic && !m.IsStatic)
                                                  .Cast<MemberInfo>()
                                                  .ToList();

            return GetMembersWithoutDuplicateNames(methods)
                .Cast<MethodInfo>();
        }

        /// <summary>
        ///     Filters a collection of MemberInfos by removing MemberInfos with duplicate names. If duplicate names exist, the
        ///     MemberInfo with the most derived DeclaringType will be chosen.
        /// </summary>
        /// <param name="members">Collection of MemberInfos to filter.</param>
        /// <returns>Filtered MemberInfos.</returns>
        private static IEnumerable<MemberInfo> GetMembersWithoutDuplicateNames(ICollection<MemberInfo> members)
        {
            IEnumerable<IGrouping<string, MemberInfo>> duplicatesGroupings = members.GroupBy(x => x.Name)
                                             .Where(g => g.Count() > 1);

            foreach (IGrouping<string, MemberInfo> duplicatesGrouping in duplicatesGroupings)
            {
                var duplicates = duplicatesGrouping.Select(g => g)
                                                   .ToList();
                var declaringTypes = duplicates.Select(d => d.DeclaringType)
                                               .ToList();

                Type mostDerived = declaringTypes.Single(t => !declaringTypes.Any(o => t.GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && (o != t)));

                foreach (MemberInfo duplicate in duplicates)
                {
                    if (duplicate.DeclaringType != mostDerived)
                    {
                        members.Remove(duplicate);
                    }
                }
            }

            return members;
        }
    }

    internal static class TypeResolutionCache
    {
        [ThreadStatic]
        private static WeakTable<Type, TypeResolution> cache;

        public static WeakTable<Type, TypeResolution> Instance
        {
            get { return cache ?? (cache = new WeakTable<Type, TypeResolution>(32)); }
        }
    }

    /// <summary>
    /// A drop in liquid is a class which allows you to to export DOM like things to liquid
    /// Methods of drops are callable.
    /// The main use for liquid drops is the implement lazy loaded objects.
    /// If you would like to make data available to the web designers which you don't want loaded unless needed then
    /// a drop is a great way to do that
    ///     Example:
    ///     class ProductDrop &lt; Liquid::Drop
    ///     def top_sales
    ///     Shop.current.products.find(:all, :order => 'sales', :limit => 10 )
    ///     end
    ///     end
    ///     tmpl = Liquid::Template.parse( ' {% for product in product.top_sales %} {{ product.name }} {%endfor%} ' )
    ///     tmpl.render('product' => ProductDrop.new ) # will invoke top_sales query.
    ///
    /// Your drop can either implement the methods sans any parameters or implement the before_method(name) method which is a
    /// catch all.
    /// </summary>
    public abstract class DropBase : ILiquidizable, IIndexable, IContextAware
    {
        internal TypeResolution TypeResolution
        {
            get
            {
                Type dropType = this.GetObject().GetType();
                if (!TypeResolutionCache.Instance.TryGetValue(dropType, out TypeResolution resolution))
                {
                    TypeResolutionCache.Instance[dropType] = resolution = this.CreateTypeResolution(dropType);
                }

                return resolution;
            }
        }

        public Context Context { get; set; }

        /// <summary>
        /// Just an alias for InvokeDrop - but the presence of the indexer
        /// means that Liquid will access Drop objects as though they are
        /// dictionaries or hashes.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual object this[object method]
        {
            get { return this.InvokeDrop(method); }
        }

        public virtual bool ContainsKey(object name)
        {
            return true;
        }

        public virtual object ToLiquid()
        {
            return this;
        }

        internal abstract object GetObject();

        internal abstract TypeResolution CreateTypeResolution(Type type);

        /// <summary>
        /// Catch all for the method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual object BeforeMethod(string method)
        {
            // Quite a common (and easy) mistake is to use C#-style property names,
            // without realising that the default naming convention is Ruby-style.
            // To try to help with this, we check if the given name *would* match,
            // if we were using Ruby-style names.
            if (Template.NamingConvention is RubyNamingConvention)
            {
                string rubyMethod = Template.NamingConvention.GetMemberName(method);
                if (this.TypeResolution.CachedMethods.TryGetValue(rubyMethod, out _) || this.TypeResolution.CachedProperties.TryGetValue(rubyMethod, out _))
                {
                    return string.Format(Liquid.ResourceManager.GetString("DropWrongNamingConventionMessage"), rubyMethod);
                }
            }

            return null;
        }

        /// <summary>
        ///     Called by liquid to invoke a drop.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object InvokeDrop(object name)
        {
            string method = (string)name;

            if (this.TypeResolution.CachedMethods.TryGetValue(method, out MethodInfo mi))
            {
                object result = mi.Invoke(this.GetObject(), null);
                if (result is Task t)
                {
                    t.GetAwaiter().GetResult();
                    Type taskType = t.GetType();
                    if (taskType.IsGenericType)
                    {
                        return taskType.GetProperty("Result").GetValue(t);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return result;
                }
            }

            if (this.TypeResolution.CachedProperties.TryGetValue(method, out PropertyInfo pi))
            {
                return pi.GetValue(this.GetObject(), null);
            }

            return this.BeforeMethod(method);
        }
    }

    public abstract class Drop : DropBase
    {
        internal override object GetObject()
        {
            return this;
        }

        internal override TypeResolution CreateTypeResolution(Type type)
        {
            return new TypeResolution(type, mi => mi.DeclaringType.GetTypeInfo().BaseType != null && typeof(Drop).GetTypeInfo().IsAssignableFrom(mi.DeclaringType.GetTypeInfo().BaseType.GetTypeInfo()));
        }
    }

    /// <summary>
    /// Proxy for types not derived from DropBase.
    /// </summary>
    public class DropProxy : DropBase, IValueTypeConvertible
    {
        private readonly string[] allowedMembers;
        private readonly object proxiedObject;
        private readonly Func<object, object> value;
        private readonly bool allowAllMembers;

        /// <summary>
        /// Create a new DropProxy object.
        /// </summary>
        /// <param name="obj">The object to create a proxy for.</param>
        /// <param name="allowedMembers">An array of property and method names that are allowed to be called on the object.</param>
        public DropProxy(object obj, string[] allowedMembers)
        {
            this.proxiedObject = obj;
            this.allowedMembers = allowedMembers;

            // Allow all member if the list of allowed members is size 1 and is a wild card
            this.allowAllMembers = this.allowedMembers?.Length == 1 && this.allowedMembers[0] == "*";
        }

        /// <summary>
        /// Create a new DropProxy object.
        /// </summary>
        /// <param name="obj">The object to create a proxy for.</param>
        /// <param name="allowedMembers">An array of property and method names that are allowed to be called on the object.</param>
        /// <param name="value">Function that converts the specified type into a Liquid Drop-compatible object (eg, implements ILiquidizable).</param>
        public DropProxy(object obj, string[] allowedMembers, Func<object, object> value)
            : this(obj, allowedMembers)
        {
            this.value = value;
        }

        public virtual object ConvertToValueType()
        {
            if (this.value == null)
            {
                return null;
            }

            return this.value(this.proxiedObject);
        }

        internal override object GetObject()
        {
            return this.proxiedObject;
        }

        internal override TypeResolution CreateTypeResolution(Type type)
        {
            return new TypeResolution(type, mi => this.allowAllMembers || this.allowedMembers.Contains(mi.Name));
        }
    }
}
