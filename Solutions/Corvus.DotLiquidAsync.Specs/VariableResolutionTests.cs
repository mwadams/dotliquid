namespace DotLiquid.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class VariableResolutionTests
    {
        [Test]
        public async Task TestSimpleVariable()
        {
            var template = Template.Parse("{{test}}");
            Assert.AreEqual("worked", await template.RenderAsync(Hash.FromAnonymousObject(new { test = "worked" })));
            Assert.AreEqual("worked wonderfully", await template.RenderAsync(Hash.FromAnonymousObject(new { test = "worked wonderfully" })));
        }

        [Test]
        public async Task TestSimpleWithWhitespaces()
        {
            var template = Template.Parse("  {{ test }}  ");
            Assert.AreEqual("  worked  ", await template.RenderAsync(Hash.FromAnonymousObject(new { test = "worked" })));
            Assert.AreEqual("  worked wonderfully  ", await template.RenderAsync(Hash.FromAnonymousObject(new { test = "worked wonderfully" })));
        }

        [Test]
        public async Task TestIgnoreUnknown()
        {
            var template = Template.Parse("{{ test }}");
            Assert.AreEqual("", await template.RenderAsync());
        }

        [Test]
        public async Task TestHashScoping()
        {
            var template = Template.Parse("{{ test.test }}");
            Assert.AreEqual("worked", await template.RenderAsync(Hash.FromAnonymousObject(new { test = new { test = "worked" } })));
        }

        [Test]
        public async Task TestPresetAssigns()
        {
            var template = Template.Parse("{{ test }}");
            template.Assigns["test"] = "worked";
            Assert.AreEqual("worked", await template.RenderAsync());
        }

        [Test]
        public async Task TestReuseParsedTemplate()
        {
            var template = Template.Parse("{{ greeting }} {{ name }}");
            template.Assigns["greeting"] = "Goodbye";
            Assert.AreEqual("Hello Tobi", await template.RenderAsync(Hash.FromAnonymousObject(new { greeting = "Hello", name = "Tobi" })));
            Assert.AreEqual("Hello ", await template.RenderAsync(Hash.FromAnonymousObject(new { greeting = "Hello", unknown = "Tobi" })));
            Assert.AreEqual("Hello Brian", await template.RenderAsync(Hash.FromAnonymousObject(new { greeting = "Hello", name = "Brian" })));
            Assert.AreEqual("Goodbye Brian", await template.RenderAsync(Hash.FromAnonymousObject(new { name = "Brian" })));
            CollectionAssert.AreEqual(Hash.FromAnonymousObject(new { greeting = "Goodbye" }), template.Assigns);
        }

        [Test]
        public async Task TestAssignsNotPollutedFromTemplate()
        {
            var template = Template.Parse("{{ test }}{% assign test = 'bar' %}{{ test }}");
            template.Assigns["test"] = "baz";
            Assert.AreEqual("bazbar", await template.RenderAsync());
            Assert.AreEqual("bazbar", await template.RenderAsync());
            Assert.AreEqual("foobar", await template.RenderAsync(Hash.FromAnonymousObject(new { test = "foo" })));
            Assert.AreEqual("bazbar", await template.RenderAsync());
        }

        [Test]
        public async Task TestHashWithDefaultProc()
        {
            var template = Template.Parse("Hello {{ test }}");
            var assigns = new Hash((h, k) => { throw new Exception("Unknown variable '" + k + "'"); })
            {
                ["test"] = "Tobi"
            };
            Assert.AreEqual("Hello Tobi", await template.RenderAsync(new RenderParameters(CultureInfo.InvariantCulture)
            {
                LocalVariables = assigns,
                ErrorsOutputMode = ErrorsOutputMode.Rethrow
            }));
            assigns.Remove("test");
            Exception ex = Assert.Throws<Exception>(() => template.RenderAsync(new RenderParameters(CultureInfo.InvariantCulture)
            {
                LocalVariables = assigns,
                ErrorsOutputMode = ErrorsOutputMode.Rethrow
            }).GetAwaiter().GetResult());
            Assert.AreEqual("Unknown variable 'test'", ex.Message);
        }
    }
}
