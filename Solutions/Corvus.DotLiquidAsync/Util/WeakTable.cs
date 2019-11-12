// <copyright file="WeakTable.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace DotLiquid.Util
{
    using System;

    internal class WeakTable<TKey, TValue>
        where TValue : class
    {
        private struct Bucket
        {
            public TKey Key;
            public WeakReference Value;
        }

        private readonly Bucket[] buckets;

        public WeakTable(int size)
        {
            this.buckets = new Bucket[size];
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!this.TryGetValue(key, out TValue ret))
                {
                    throw new ArgumentException(Liquid.ResourceManager.GetString("WeakTableKeyNotFoundException"));
                }

                return ret;
            }

            set
            {
                int i = Math.Abs(key.GetHashCode()) % this.buckets.Length;
                this.buckets[i].Key = key;
                this.buckets[i].Value = new WeakReference(value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int i = Math.Abs(key.GetHashCode()) % this.buckets.Length;
            WeakReference wr;
            if ((wr = this.buckets[i].Value) == null || !this.buckets[i].Key.Equals(key))
            {
                value = null;
                return false;
            }

            value = (TValue)wr.Target;
            return wr.IsAlive;
        }

        public void Remove(TKey key)
        {
            int i = Math.Abs(key.GetHashCode()) % this.buckets.Length;
            if (this.buckets[i].Key.Equals(key))
            {
                this.buckets[i].Value = null;
            }
        }
    }
}
