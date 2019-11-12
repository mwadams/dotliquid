namespace DotLiquid.Tests
{
    using DotLiquid.NamingConventions;
    using NUnit.Framework;

    [TestFixture]
    public class NamingConventionTests
    {
        [Test]
        public void TestRubySimpleName()
        {
            var namingConvention = new RubyNamingConvention();
            Assert.AreEqual("test", namingConvention.GetMemberName("Test"));
        }

        [Test]
        public void TestRubyComplexName()
        {
            var namingConvention = new RubyNamingConvention();
            Assert.AreEqual("hello_world", namingConvention.GetMemberName("HelloWorld"));
        }

        [Test]
        public void TestRubyMoreComplexName()
        {
            var namingConvention = new RubyNamingConvention();
            Assert.AreEqual("hello_cruel_world", namingConvention.GetMemberName("HelloCruelWorld"));
        }

        [Test]
        public void TestRubyFullUpperCase()
        {
            var namingConvention = new RubyNamingConvention();
            Assert.AreEqual("id", namingConvention.GetMemberName("ID"));
            Assert.AreEqual("hellocruelworld", namingConvention.GetMemberName("HELLOCRUELWORLD"));
        }

        [Test]
        public void TestRubyWithTurkishCulture()
        {
            using (CultureHelper.SetCulture("tr-TR"))
            {
                var namingConvention = new RubyNamingConvention();

                // in Turkish ID.ToLower() returns a localized i, and this fails
                Assert.AreEqual("id", namingConvention.GetMemberName("ID"));
            }
        }

        [Test]
        public void TestCSharpConventionDoesNothing()
        {
            var namingConvention = new CSharpNamingConvention();
            Assert.AreEqual("Test", namingConvention.GetMemberName("Test"));
        }
    }
}
