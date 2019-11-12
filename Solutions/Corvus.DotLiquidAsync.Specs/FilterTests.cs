namespace DotLiquid.Tests
{
    using System.Collections;
    using System.Globalization;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class FilterTests
    {
        #region Classes used in tests

        private static class MoneyFilter
        {
            public static string Money(object input)
            {
                return string.Format(" {0:d}$ ", input);
            }

            public static string MoneyWithUnderscore(object input)
            {
                return string.Format(" {0:d}$ ", input);
            }
        }

        private static class CanadianMoneyFilter
        {
            public static string Money(object input)
            {
                return string.Format(" {0:d}$ CAD ", input);
            }
        }

        private static class FiltersWithArguments
        {
            public static string Adjust(int input, int offset = 10)
            {
                return string.Format("[{0:d}]", input + offset);
            }

            public static string AddSub(int input, int plus, int minus = 20)
            {
                return string.Format("[{0:d}]", input + plus - minus);
            }
        }

        private static class FiltersWithMulitpleMethodSignatures
        {
            public static string Concat(string one, string two)
            {
                return string.Concat(one, two);
            }

            public static string Concat(string one, string two, string three)
            {
                return string.Concat(one, two, three);
            }
        }

        private static class FiltersWithMultipleMethodSignaturesAndContextParam
        {
            public static string ConcatWithContext(string one, string two)
            {
                return string.Concat(one, two);
            }

            public static string ConcatWithContext(string one, string two, string three)
            {
                return string.Concat(one, two, three);
            }
        }

        private static class ContextFilters
        {
            public static string BankStatement(Context context, object input)
            {
                return string.Format(" " + context["name"] + " has {0:d}$ ", input);
            }
        }

        #endregion

        private Context _context;

        [OneTimeSetUp]
        public void SetUp()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
        }

        /*[Test]
        public void TestNonExistentFilter()
        {
            _context["var"] = 1000;
            Assert.Throws<FilterNotFoundException>(() => new Variable("var | syzzy").Render(_context));
        }*/

        [Test]
        public async Task TestLocalFilter()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(MoneyFilter));
            Assert.AreEqual(" 1000$ ", await new Variable("var | money").RenderAsync(this._context));
        }

        [Test]
        public async Task TestUnderscoreInFilterName()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(MoneyFilter));
            Assert.AreEqual(" 1000$ ", await new Variable("var | money_with_underscore").RenderAsync(this._context));
        }

        [Test]
        public async Task TestFilterWithNumericArgument()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(FiltersWithArguments));
            Assert.AreEqual("[1005]", await new Variable("var | adjust: 5").RenderAsync(this._context));
        }

        [Test]
        public async Task TestFilterWithNegativeArgument()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(FiltersWithArguments));
            Assert.AreEqual("[995]", await new Variable("var | adjust: -5").RenderAsync(this._context));
        }

        [Test]
        public async Task TestFilterWithDefaultArgument()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(FiltersWithArguments));
            Assert.AreEqual("[1010]", await new Variable("var | adjust").RenderAsync(this._context));
        }

        [Test]
        public async Task TestFilterWithTwoArguments()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(FiltersWithArguments));
            Assert.AreEqual("[1150]", await new Variable("var | add_sub: 200, 50").RenderAsync(this._context));
        }

        [Test]
        public async Task TestFilterWithMultipleMethodSignatures()
        {
            Template.RegisterFilter(typeof(FiltersWithMulitpleMethodSignatures));

            Assert.AreEqual("AB", await Template.Parse("{{'A' | concat : 'B'}}").RenderAsync());
            Assert.AreEqual("ABC", await Template.Parse("{{'A' | concat : 'B', 'C'}}").RenderAsync());
        }

        [Test]
        public async Task TestFilterWithMultipleMethodSignaturesAndContextParam()
        {
            Template.RegisterFilter(typeof(FiltersWithMultipleMethodSignaturesAndContextParam));

            Assert.AreEqual("AB", await Template.Parse("{{'A' | concat_with_context : 'B'}}").RenderAsync());
            Assert.AreEqual("ABC", await Template.Parse("{{'A' | concat_with_context : 'B', 'C'}}").RenderAsync());
        }

        /*/// <summary>
        /// ATM the trailing value is silently ignored. Should raise an exception?
        /// </summary>
        [Test]
        public void TestFilterWithTwoArgumentsNoComma()
        {
            _context["var"] = 1000;
            _context.AddFilters(typeof(FiltersWithArguments));
            Assert.AreEqual("[1150]", string.Join(string.Empty, new Variable("var | add_sub: 200 50").Render(_context));
        }*/

        [Test]
        public async Task TestSecondFilterOverwritesFirst()
        {
            this._context["var"] = 1000;
            this._context.AddFilters(typeof(MoneyFilter));
            this._context.AddFilters(typeof(CanadianMoneyFilter));
            Assert.AreEqual(" 1000$ CAD ", await new Variable("var | money").RenderAsync(this._context));
        }

        [Test]
        public async Task TestSize()
        {
            this._context["var"] = "abcd";
            this._context.AddFilters(typeof(MoneyFilter));
            Assert.AreEqual(4, await new Variable("var | size").RenderAsync(this._context));
        }

        [Test]
        public async Task TestJoin()
        {
            this._context["var"] = new[] { 1, 2, 3, 4 };
            Assert.AreEqual("1 2 3 4", await new Variable("var | join").RenderAsync(this._context));
        }

        [Test]
        public async Task TestSort()
        {
            this._context["value"] = 3;
            this._context["numbers"] = new[] { 2, 1, 4, 3 };
            this._context["words"] = new[] { "expected", "as", "alphabetic" };
            this._context["arrays"] = new[] { new[] { "flattened" }, new[] { "are" } };

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, await new Variable("numbers | sort").RenderAsync(this._context) as IEnumerable);
            CollectionAssert.AreEqual(new[] { "alphabetic", "as", "expected" }, await new Variable("words | sort").RenderAsync(this._context) as IEnumerable);
            CollectionAssert.AreEqual(new[] { 3 }, await new Variable("value | sort").RenderAsync(this._context) as IEnumerable);
            CollectionAssert.AreEqual(new[] { "are", "flattened" }, await new Variable("arrays | sort").RenderAsync(this._context) as IEnumerable);
        }

        [Test]
        public async Task TestSplit()
        {
            this._context["var"] = "a~b";
            Assert.AreEqual(new[] { "a", "b" }, await new Variable("var | split:'~'").RenderAsync(this._context));
        }

        [Test]
        public async Task TestStripHtml()
        {
            this._context["var"] = "<b>bla blub</a>";
            Assert.AreEqual("bla blub", await new Variable("var | strip_html").RenderAsync(this._context));
        }

        [Test]
        public async Task Capitalize()
        {
            this._context["var"] = "blub";
            Assert.AreEqual("Blub", await new Variable("var | capitalize").RenderAsync(this._context));
        }

        [Test]
        public async Task Slice()
        {
            this._context["var"] = "blub";
            Assert.AreEqual("b", await new Variable("var | slice: 0, 1").RenderAsync(this._context));
            Assert.AreEqual("bl", await new Variable("var | slice: 0, 2").RenderAsync(this._context));
            Assert.AreEqual("l", await new Variable("var | slice: 1").RenderAsync(this._context));
            Assert.AreEqual("", await new Variable("var | slice: 4, 1").RenderAsync(this._context));
            Assert.AreEqual("ub", await new Variable("var | slice: -2, 2").RenderAsync(this._context));
            Assert.AreEqual(null, await new Variable("var | slice: 5, 1").RenderAsync(this._context));
        }

        [Test]
        public async Task TestLocalGlobal()
        {
            Template.RegisterFilter(typeof(MoneyFilter));

            Assert.AreEqual(" 1000$ ", await Template.Parse("{{1000 | money}}").RenderAsync());
            Assert.AreEqual(" 1000$ CAD ", await Template.Parse("{{1000 | money}}").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { Filters = new[] { typeof(CanadianMoneyFilter) } }));
            Assert.AreEqual(" 1000$ CAD ", await Template.Parse("{{1000 | money}}").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { Filters = new[] { typeof(CanadianMoneyFilter) } }));
        }

        [Test]
        public async Task TestContextFilter()
        {
            this._context["var"] = 1000;
            this._context["name"] = "King Kong";
            this._context.AddFilters(typeof(ContextFilters));
            Assert.AreEqual(" King Kong has 1000$ ", await new Variable("var | bank_statement").RenderAsync(this._context));
        }
    }
}
