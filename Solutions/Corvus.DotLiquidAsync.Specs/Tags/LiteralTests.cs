namespace DotLiquid.Tests.Tags
{
    using NUnit.Framework;
    using DotLiquid.Tags;
    using System.Threading.Tasks;

    [TestFixture]
    public class LiteralTests
    {
        [Test]
        public async Task TestEmptyLiteral()
        {
            var t = Template.Parse("{% literal %}{% endliteral %}");
            Assert.AreEqual(string.Empty, await t.RenderAsync());
            t = Template.Parse("{{{}}}");
            Assert.AreEqual(string.Empty, await t.RenderAsync());
        }

        [Test]
        public async Task TestSimpleLiteralValue()
        {
            var t = Template.Parse("{% literal %}howdy{% endliteral %}");
            Assert.AreEqual("howdy", await t.RenderAsync());
        }

        [Test]
        public async Task TestLiteralsIgnoreLiquidMarkup()
        {
            var t = Template.Parse("{% literal %}{% if 'gnomeslab' contains 'liquid' %}yes{ % endif %}{% endliteral %}");
            Assert.AreEqual("{% if 'gnomeslab' contains 'liquid' %}yes{ % endif %}", await t.RenderAsync());
        }

        [Test]
        public async Task TestShorthandSyntax()
        {
            var t = Template.Parse("{{{{% if 'gnomeslab' contains 'liquid' %}yes{ % endif %}}}}");
            Assert.AreEqual("{% if 'gnomeslab' contains 'liquid' %}yes{ % endif %}", await t.RenderAsync());
        }

        [Test]
        public async Task TestLiteralsDontRemoveComments()
        {
            var t = Template.Parse("{{{ {# comment #} }}}");
            Assert.AreEqual("{# comment #}", await t.RenderAsync());
        }

        [Test]
        public void TestFromShorthand()
        {
            Assert.AreEqual("{% literal %}gnomeslab{% endliteral %}", Literal.FromShortHand("{{{gnomeslab}}}"));
        }

        [Test]
        public void TestFromShorthandIgnoresImproperSyntax()
        {
            Assert.AreEqual("{% if 'hi' == 'hi' %}hi{% endif %}", Literal.FromShortHand("{% if 'hi' == 'hi' %}hi{% endif %}"));
        }
    }
}
