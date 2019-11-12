namespace DotLiquid.Tests.Tags
{
    using DotLiquid.FileSystems;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    public class InheritanceTests
    {
        private class TestFileSystem : IFileSystem
        {
            public Task<string> ReadTemplateFileAsync(Context context, string templateName)
            {
                string templatePath = (string)context[templateName];

                return templatePath switch
                {
                    "simple" => Task.FromResult("test"),
                    "complex" => Task.FromResult(@"some markup here...
                             {% block thing %}
                                 thing block
                             {% endblock %}
                             {% block another %}
                                 another block
                             {% endblock %}
                             ...and some markup here"),
                    "nested" => Task.FromResult(@"{% extends 'complex' %}
                             {% block thing %}
                                another thing (from nested)
                             {% endblock %}"),
                    "outer" => Task.FromResult("{% block start %}{% endblock %}A{% block outer %}{% endblock %}Z"),
                    "middle" => Task.FromResult(@"{% extends 'outer' %}
                             {% block outer %}B{% block middle %}{% endblock %}Y{% endblock %}"),
                    "middleunless" => Task.FromResult(@"{% extends 'outer' %}
                             {% block outer %}B{% unless nomiddle %}{% block middle %}{% endblock %}{% endunless %}Y{% endblock %}"),
                    _ => Task.FromResult(@"{% extends 'complex' %}
                             {% block thing %}
                                thing block (from nested)
                             {% endblock %}"),
                };
            }
        }

        private IFileSystem _originalFileSystem;

        [OneTimeSetUp]
        public void SetUp()
        {
            this._originalFileSystem = Template.FileSystem;
            Template.FileSystem = new TestFileSystem();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Template.FileSystem = this._originalFileSystem;
        }

        [Test]
        public async Task CanOutputTheContentsOfTheExtendedTemplate()
        {
            var template = Template.Parse(
                                    @"{% extends 'simple' %}
                    {% block thing %}
                        yeah
                    {% endblock %}");

            StringAssert.Contains("test", await template.RenderAsync());
        }

        [Test]
        public async Task CanInherit()
        {
            var template = Template.Parse(@"{% extends 'complex' %}");

            StringAssert.Contains("thing block", await template.RenderAsync());
        }

        [Test]
        public async Task CanInheritAndReplaceBlocks()
        {
            var template = Template.Parse(
                                    @"{% extends 'complex' %}
                    {% block another %}
                      new content for another
                    {% endblock %}");

            StringAssert.Contains("new content for another", await template.RenderAsync());
        }

        [Test]
        public async Task CanProcessNestedInheritance()
        {
            var template = Template.Parse(
                                    @"{% extends 'nested' %}
                  {% block thing %}
                  replacing block thing
                  {% endblock %}");

            StringAssert.Contains("replacing block thing", await template.RenderAsync());
            StringAssert.DoesNotContain("thing block", await template.RenderAsync());
        }

        [Test]
        public async Task CanRenderSuper()
        {
            var template = Template.Parse(
                                    @"{% extends 'complex' %}
                    {% block another %}
                        {{ block.super }} + some other content
                    {% endblock %}");

            StringAssert.Contains("another block", await template.RenderAsync());
            StringAssert.Contains("some other content", await template.RenderAsync());
        }

        [Test]
        public async Task CanDefineBlockInInheritedBlock()
        {
            var template = Template.Parse(
                                    @"{% extends 'middle' %}
                  {% block middle %}C{% endblock %}");
            Assert.AreEqual("ABCYZ", await template.RenderAsync());
        }

        [Test]
        public async Task CanDefineContentInInheritedBlockFromAboveParent()
        {
            var template = Template.Parse(@"{% extends 'middle' %}
                  {% block start %}!{% endblock %}");
            Assert.AreEqual("!ABYZ", await template.RenderAsync());
        }

        [Test]
        public async Task CanRenderBlockContainedInConditional()
        {
            var template = Template.Parse(
                                    @"{% extends 'middleunless' %}
                  {% block middle %}C{% endblock %}");
            Assert.AreEqual("ABCYZ", await template.RenderAsync());

            template = Template.Parse(
                @"{% extends 'middleunless' %}
                  {% block start %}{% assign nomiddle = true %}{% endblock %}
                  {% block middle %}C{% endblock %}");
            Assert.AreEqual("ABYZ", await template.RenderAsync());
        }

        [Test]
        public async Task RepeatedRendersProduceSameResult()
        {
            var template = Template.Parse(
                                    @"{% extends 'middle' %}
                  {% block start %}!{% endblock %}
                  {% block middle %}C{% endblock %}");
            Assert.AreEqual("!ABCYZ", await template.RenderAsync());
            Assert.AreEqual("!ABCYZ", await template.RenderAsync());
        }

        [Test]
        public async Task TestExtendFromTemplateFileSystem()
        {
            var fileSystem = new IncludeTagTests.TestTemplateFileSystem(new TestFileSystem());
            Template.FileSystem = fileSystem;
            for (int i = 0; i < 2; ++i)
            {
                var template = Template.Parse(
                                    @"{% extends 'simple' %}
                    {% block thing %}
                        yeah
                    {% endblock %}");
                StringAssert.Contains("test", await template.RenderAsync());
            }
            Assert.AreEqual(fileSystem.CacheHitTimes, 1);
        }
    }
}
