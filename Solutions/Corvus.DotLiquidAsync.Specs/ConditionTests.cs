namespace DotLiquid.Tests
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DotLiquid.Exceptions;
    using DotLiquid.NamingConventions;
    using NUnit.Framework;

    [TestFixture]
    public class ConditionTests
    {
        private Context _context;

        [Test]
        public void TestBasicCondition()
        {
            Assert.AreEqual(false, new Condition("1", "==", "2").Evaluate(null, CultureInfo.InvariantCulture));
            Assert.AreEqual(true, new Condition("1", "==", "1").Evaluate(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void TestDefaultOperatorsEvaluateTrue()
        {
            this.AssertEvaluatesTrue("1", "==", "1");
            this.AssertEvaluatesTrue("1", "!=", "2");
            this.AssertEvaluatesTrue("1", "<>", "2");
            this.AssertEvaluatesTrue("1", "<", "2");
            this.AssertEvaluatesTrue("2", ">", "1");
            this.AssertEvaluatesTrue("1", ">=", "1");
            this.AssertEvaluatesTrue("2", ">=", "1");
            this.AssertEvaluatesTrue("1", "<=", "2");
            this.AssertEvaluatesTrue("1", "<=", "1");
        }

        [Test]
        public void TestDefaultOperatorsEvaluateFalse()
        {
            this.AssertEvaluatesFalse("1", "==", "2");
            this.AssertEvaluatesFalse("1", "!=", "1");
            this.AssertEvaluatesFalse("1", "<>", "1");
            this.AssertEvaluatesFalse("1", "<", "0");
            this.AssertEvaluatesFalse("2", ">", "4");
            this.AssertEvaluatesFalse("1", ">=", "3");
            this.AssertEvaluatesFalse("2", ">=", "4");
            this.AssertEvaluatesFalse("1", "<=", "0");
            this.AssertEvaluatesFalse("1", "<=", "0");
        }

        [Test]
        public void TestContainsWorksOnStrings()
        {
            this.AssertEvaluatesTrue("'bob'", "contains", "'o'");
            this.AssertEvaluatesTrue("'bob'", "contains", "'b'");
            this.AssertEvaluatesTrue("'bob'", "contains", "'bo'");
            this.AssertEvaluatesTrue("'bob'", "contains", "'ob'");
            this.AssertEvaluatesTrue("'bob'", "contains", "'bob'");

            this.AssertEvaluatesFalse("'bob'", "contains", "'bob2'");
            this.AssertEvaluatesFalse("'bob'", "contains", "'a'");
            this.AssertEvaluatesFalse("'bob'", "contains", "'---'");
        }

        [Test]
        public void TestContainsWorksOnArrays()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
            this._context["array"] = new[] { 1, 2, 3, 4, 5 };

            this.AssertEvaluatesFalse("array", "contains", "0");
            this.AssertEvaluatesTrue("array", "contains", "1");
            this.AssertEvaluatesTrue("array", "contains", "2");
            this.AssertEvaluatesTrue("array", "contains", "3");
            this.AssertEvaluatesTrue("array", "contains", "4");
            this.AssertEvaluatesTrue("array", "contains", "5");
            this.AssertEvaluatesFalse("array", "contains", "6");

            this.AssertEvaluatesFalse("array", "contains", "'1'");
        }

        [Test]
        public void TestContainsReturnsFalseForNilCommands()
        {
            this.AssertEvaluatesFalse("not_assigned", "contains", "0");
            this.AssertEvaluatesFalse("0", "contains", "not_assigned");
        }

        [Test]
        public void TestStartsWithWorksOnStrings()
        {
            this.AssertEvaluatesTrue("'dave'", "startswith", "'d'");
            this.AssertEvaluatesTrue("'dave'", "startswith", "'da'");
            this.AssertEvaluatesTrue("'dave'", "startswith", "'dav'");
            this.AssertEvaluatesTrue("'dave'", "startswith", "'dave'");

            this.AssertEvaluatesFalse("'dave'", "startswith", "'ave'");
            this.AssertEvaluatesFalse("'dave'", "startswith", "'e'");
            this.AssertEvaluatesFalse("'dave'", "startswith", "'---'");
        }

        [Test]
        public void TestStartsWithWorksOnArrays()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
            this._context["array"] = new[] { 1, 2, 3, 4, 5 };

            this.AssertEvaluatesFalse("array", "startswith", "0");
            this.AssertEvaluatesTrue("array", "startswith", "1");
        }

        [Test]
        public void TestStartsWithReturnsFalseForNilCommands()
        {
            this.AssertEvaluatesFalse("not_assigned", "startswith", "0");
            this.AssertEvaluatesFalse("0", "startswith", "not_assigned");
        }

        [Test]
        public void TestEndsWithWorksOnStrings()
        {
            this.AssertEvaluatesTrue("'dave'", "endswith", "'e'");
            this.AssertEvaluatesTrue("'dave'", "endswith", "'ve'");
            this.AssertEvaluatesTrue("'dave'", "endswith", "'ave'");
            this.AssertEvaluatesTrue("'dave'", "endswith", "'dave'");

            this.AssertEvaluatesFalse("'dave'", "endswith", "'dav'");
            this.AssertEvaluatesFalse("'dave'", "endswith", "'d'");
            this.AssertEvaluatesFalse("'dave'", "endswith", "'---'");
        }

        [Test]
        public void TestEndsWithWorksOnArrays()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
            this._context["array"] = new[] { 1, 2, 3, 4, 5 };

            this.AssertEvaluatesFalse("array", "endswith", "0");
            this.AssertEvaluatesTrue("array", "endswith", "5");
        }

        [Test]
        public void TestEndsWithReturnsFalseForNilCommands()
        {
            this.AssertEvaluatesFalse("not_assigned", "endswith", "0");
            this.AssertEvaluatesFalse("0", "endswith", "not_assigned");
        }

        [Test]
        public void TestDictionaryHasKey()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
            var testDictionary = new System.Collections.Generic.Dictionary<string, string>
            {
                { "dave", "0" },
                { "bob", "4" }
            };
            this._context["dictionary"] = testDictionary;

            this.AssertEvaluatesTrue("dictionary", "haskey", "'bob'");
            this.AssertEvaluatesFalse("dictionary", "haskey", "'0'");
        }

        [Test]
        public void TestDictionaryHasValue()
        {
            this._context = new Context(CultureInfo.InvariantCulture);
            var testDictionary = new System.Collections.Generic.Dictionary<string, string>
            {
                { "dave", "0" },
                { "bob", "4" }
            };
            this._context["dictionary"] = testDictionary;

            this.AssertEvaluatesTrue("dictionary", "hasvalue", "'0'");
            this.AssertEvaluatesFalse("dictionary", "hasvalue", "'bob'");
        }

        [Test]
        public void TestOrCondition()
        {
            var condition = new Condition("1", "==", "2");
            Assert.IsFalse(condition.Evaluate(null, CultureInfo.InvariantCulture));

            condition.Or(new Condition("2", "==", "1"));
            Assert.IsFalse(condition.Evaluate(null, CultureInfo.InvariantCulture));

            condition.Or(new Condition("1", "==", "1"));
            Assert.IsTrue(condition.Evaluate(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void TestAndCondition()
        {
            var condition = new Condition("1", "==", "1");
            Assert.IsTrue(condition.Evaluate(null, CultureInfo.InvariantCulture));

            condition.And(new Condition("2", "==", "2"));
            Assert.IsTrue(condition.Evaluate(null, CultureInfo.InvariantCulture));

            condition.And(new Condition("2", "==", "1"));
            Assert.IsFalse(condition.Evaluate(null, CultureInfo.InvariantCulture));
        }

        [Test]
        public void TestShouldAllowCustomProcOperator()
        {
            try
            {
                Condition.Operators["starts_with"] =
                    (left, right) => Regex.IsMatch(left.ToString(), string.Format("^{0}", right.ToString()));

                this.AssertEvaluatesTrue("'bob'", "starts_with", "'b'");
                this.AssertEvaluatesFalse("'bob'", "starts_with", "'o'");
            }
            finally
            {
                Condition.Operators.Remove("starts_with");
            }
        }

        [Test]
        public async Task TestCapitalInCustomOperator()
        {
            try
            {
                Condition.Operators["IsMultipleOf"] =
                    (left, right) => (int)left % (int)right == 0;

                // exact match
                this.AssertEvaluatesTrue("16", "IsMultipleOf", "4");
                this.AssertEvaluatesFalse("16", "IsMultipleOf", "5");

                // lower case: compatibility
                this.AssertEvaluatesTrue("16", "ismultipleof", "4");
                this.AssertEvaluatesFalse("16", "ismultipleof", "5");

                this.AssertEvaluatesTrue("16", "is_multiple_of", "4");
                this.AssertEvaluatesFalse("16", "is_multiple_of", "5");

                this.AssertError("16", "isMultipleOf", "4", typeof(ArgumentException));

                //Run tests through the template to verify that capitalization rules are followed through template parsing
                await Helper.AssertTemplateResultAsync(" TRUE ", "{% if 16 IsMultipleOf 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("", "{% if 14 IsMultipleOf 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync(" TRUE ", "{% if 16 ismultipleof 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("", "{% if 14 ismultipleof 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync(" TRUE ", "{% if 16 is_multiple_of 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("", "{% if 14 is_multiple_of 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("Liquid error: Unknown operator isMultipleOf", "{% if 16 isMultipleOf 4 %} TRUE {% endif %}");
            }
            finally
            {
                Condition.Operators.Remove("IsMultipleOf");
            }
        }

        [Test]
        public async Task TestCapitalInCustomCSharpOperator()
        {

            INamingConvention oldconvention = Template.NamingConvention;
            Template.NamingConvention = new CSharpNamingConvention();

            try
            {
                Condition.Operators["DivisibleBy"] =
                    (left, right) => (int)left % (int)right == 0;

                // exact match
                this.AssertEvaluatesTrue("16", "DivisibleBy", "4");
                this.AssertEvaluatesFalse("16", "DivisibleBy", "5");

                // lower case: compatibility
                this.AssertEvaluatesTrue("16", "divisibleby", "4");
                this.AssertEvaluatesFalse("16", "divisibleby", "5");

                this.AssertError("16", "divisibleBy", "4", typeof(ArgumentException));

                //Run tests through the template to verify that capitalization rules are followed through template parsing
                await Helper.AssertTemplateResultAsync(" TRUE ", "{% if 16 DivisibleBy 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("", "{% if 16 DivisibleBy 5 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync(" TRUE ", "{% if 16 divisibleby 4 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("", "{% if 16 divisibleby 5 %} TRUE {% endif %}");
                await Helper.AssertTemplateResultAsync("Liquid error: Unknown operator divisibleBy", "{% if 16 divisibleBy 4 %} TRUE {% endif %}");
            }
            finally
            {
                Condition.Operators.Remove("DivisibleBy");
            }

            Template.NamingConvention = oldconvention;
        }

        [Test]
        public async Task TestLessThanDecimal()
        {
            var model = new { value = new decimal(-10.5) };

            string output = await Template.Parse("{% if model.value < 0 %}passed{% endif %}")
                .RenderAsync(Hash.FromAnonymousObject(new { model }));

            Assert.AreEqual("passed", output);
        }

        [Test]
        public async Task TestCompareBetweenDifferentTypes()
        {
            var row = new System.Collections.Generic.Dictionary<string, object>();

            short id = 1;
            row.Add("MyID", id);

            string current = "MyID is {% if MyID == 1 %}1{%endif%}";
            var parse = DotLiquid.Template.Parse(current);
            string parsedOutput = await parse.RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = Hash.FromDictionary(row) });
            Assert.AreEqual("MyID is 1", parsedOutput);
        }

        [Test]
        public async Task TestShouldAllowCustomProcOperatorCapitalized()
        {
            try
            {
                Condition.Operators["StartsWith"] =
                    (left, right) => Regex.IsMatch(left.ToString(), string.Format("^{0}", right.ToString()));

                await Helper.AssertTemplateResultAsync("", "{% if 'bob' StartsWith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
                this.AssertEvaluatesTrue("'bob'", "StartsWith", "'b'");
                this.AssertEvaluatesFalse("'bob'", "StartsWith", "'o'");
            }
            finally
            {
                Condition.Operators.Remove("StartsWith");
            }
        }

        [Test]
        public async Task TestRuby_LowerCaseAccepted()
        {
            await Helper.AssertTemplateResultAsync("", "{% if 'bob' startswith 'B' %} YES {% endif %}");
            await Helper.AssertTemplateResultAsync(" YES ", "{% if 'Bob' startswith 'B' %} YES {% endif %}");
        }

        [Test]
        public async Task TestRuby_SnakeCaseAccepted()
        {
            await Helper.AssertTemplateResultAsync("", "{% if 'bob' starts_with 'B' %} YES {% endif %}");
            await Helper.AssertTemplateResultAsync(" YES ", "{% if 'Bob' starts_with 'B' %} YES {% endif %}");
        }

        [Test]
        public async Task TestRuby_PascalCaseNotAccepted()
        {
            await Helper.AssertTemplateResultAsync("Liquid error: Unknown operator StartsWith", "{% if 'bob' StartsWith 'B' %} YES {% endif %}");
        }

        [Test]
        public async Task TestCSharp_LowerCaseAccepted()
        {
            await Helper.AssertTemplateResultAsync("", "{% if 'bob' startswith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
            await Helper.AssertTemplateResultAsync(" YES ", "{% if 'Bob' startswith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
        }

        [Test]
        public async Task TestCSharp_PascalCaseAccepted()
        {
            await Helper.AssertTemplateResultAsync("", "{% if 'bob' StartsWith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
            await Helper.AssertTemplateResultAsync(" YES ", "{% if 'Bob' StartsWith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
        }

        [Test]
        public async Task TestCSharp_LowerPascalCaseAccepted()
        {
            await Helper.AssertTemplateResultAsync("", "{% if 'bob' startsWith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
            await Helper.AssertTemplateResultAsync(" YES ", "{% if 'Bob' startsWith 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
        }

        [Test]
        public async Task TestCSharp_SnakeCaseNotAccepted()
        {
            await Helper.AssertTemplateResultAsync("Liquid error: Unknown operator starts_with", "{% if 'bob' starts_with 'B' %} YES {% endif %}", null, new CSharpNamingConvention());
        }


        #region Helper methods

        private void AssertEvaluatesTrue(string left, string op, string right)
        {
            Assert.IsTrue(new Condition(left, op, right).Evaluate(this._context ?? new Context(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
                "Evaluated false: {0} {1} {2}", left, op, right);
        }

        private void AssertEvaluatesFalse(string left, string op, string right)
        {
            Assert.IsFalse(new Condition(left, op, right).Evaluate(this._context ?? new Context(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
                "Evaluated true: {0} {1} {2}", left, op, right);
        }

        private void AssertError(string left, string op, string right, System.Type errorType)
        {
            Assert.Throws(errorType, () => new Condition(left, op, right).Evaluate(this._context ?? new Context(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
