namespace DotLiquid.Tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    [TestFixture]
    public class LazyHashTest
    {

        public class LazyHash : Hash
        {
            #region Fields

            private Lazy<Dictionary<string, PropertyInfo>> lazyProperties = null;
            private Dictionary<string, PropertyInfo> PropertyInfos => this.lazyProperties.Value;


            private object ObjectWithLazyProperty { get; set; }

            #endregion

            #region Constructors

            public LazyHash(object bo)
            {
                this.ObjectWithLazyProperty = bo;
                this.Initialize(bo);
            }

            private void Initialize(object bo)
            {
                this.lazyProperties = new Lazy<Dictionary<string, PropertyInfo>>(delegate () {
                    var boProperties = new Dictionary<string, PropertyInfo>();
                    foreach (PropertyInfo pi in bo.GetType().GetProperties())
                    {
                        if (!boProperties.ContainsKey(pi.Name.ToLower()))
                        {
                            boProperties.Add(pi.Name.ToLower(), pi);
                        }
                    }
                    return boProperties;
                });

            }

            #endregion

            protected override object GetValue(string key)
            {
                if (this.PropertyInfos.ContainsKey(key.ToLower()))
                {
                    return this.PropertyInfos[key.ToLower()].GetValue(this.ObjectWithLazyProperty, null);
                }
                return base.GetValue(key);
            }

            public override bool Contains(object key)
            {
                string dicKey = key.ToString().ToLower();
                if (this.PropertyInfos.ContainsKey(dicKey))
                    return true;
                return base.Contains(key);
            }
        }



        public class TestLazyObject
        {
            public Lazy<string> _lazyProperty1 => new Lazy<string>(() =>
            {
                return "LAZY_PROPERTY_1";
            });
            public string LazyProperty1 => this._lazyProperty1.Value;

            public Lazy<string> _lazyProperty2 => new Lazy<string>(() =>
            {
                return "LAZY_PROPERTY_2";
            });
            public string LazyProperty2 => this._lazyProperty2.Value;

            public string StaticProperty => "STATIC_PROPERTY";
        }

        [Test]
        public async Task TestLazyHashProperty1WithoutAccessingProperty2()
        {
            var lazyObject = new TestLazyObject();
            var template = Template.Parse("{{LazyProperty1}}");
            string output = await template.RenderAsync(new LazyHash(lazyObject));
            Assert.AreEqual("LAZY_PROPERTY_1", output);
            Assert.IsFalse(lazyObject._lazyProperty2.IsValueCreated, "LazyObject LAZY_PROPERTY_2 has been created");
        }

        [Test]
        public async Task TestLazyHashProperty2WithoutAccessingProperty1()
        {
            var lazyObject = new TestLazyObject();
            var template = Template.Parse("{{LazyProperty2}}");
            string output = await template.RenderAsync(new LazyHash(lazyObject));
            Assert.AreEqual("LAZY_PROPERTY_2", output);
            Assert.IsFalse(lazyObject._lazyProperty1.IsValueCreated, "LazyObject LAZY_PROPERTY_1 has been created");
        }

        [Test]
        public async Task TestLazyHashWithoutAccessingAny()
        {
            var lazyObject = new TestLazyObject();
            var template = Template.Parse("{{StaticProperty}}");
            string output = await template.RenderAsync(new LazyHash(lazyObject));
            Assert.AreEqual("STATIC_PROPERTY", output);
            Assert.IsFalse(lazyObject._lazyProperty1.IsValueCreated, "LazyObject LAZY_PROPERTY_1 has been created");
            Assert.IsFalse(lazyObject._lazyProperty2.IsValueCreated, "LazyObject LAZY_PROPERTY_2 has been created");
        }

        [Test]
        public async Task TestLazyHashWithAccessingAllProperties()
        {
            var lazyObject = new TestLazyObject();
            var template = Template.Parse("{{LazyProperty1}}-{{LazyProperty2}}-{{StaticProperty}}");
            string output = await template.RenderAsync(new LazyHash(lazyObject));
            Assert.AreEqual($"LAZY_PROPERTY_1-LAZY_PROPERTY_2-STATIC_PROPERTY", output);
        }
    }
}
