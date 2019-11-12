namespace DotLiquid.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class OutputTests
    {
        private static class FunnyFilter
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for reflection.")]
            public static string MakeFunny(string input)
            {
                return "LOL";
            }

            public static string CiteFunny(string input)
            {
                return "LOL: " + input;
            }

            public static string AddSmiley(string input, string smiley = ":-)")
            {
                return input + " " + smiley;
            }

            public static string AddTag(string input, string tag = "p", string id = "foo")
            {
                return string.Format("<{0} id=\"{1}\">{2}</{0}>", tag, id, input);
            }

            public static string Paragraph(string input)
            {
                return string.Format("<p>{0}</p>", input);
            }

            public static string LinkTo(string name, string url)
            {
                return string.Format("<a href=\"{0}\">{1}</a>", url, name);
            }
        }

        private Hash _assigns;

        [OneTimeSetUp]
        public void SetUp()
        {
            this._assigns = Hash.FromAnonymousObject(new {
                best_cars = "bmw",
                car = Hash.FromAnonymousObject(new { bmw = "good", gm = "bad" }),
                number = 3.145
            });
        }

        [Test]
        public async Task TestVariable()
        {
            Assert.AreEqual(" bmw ", await Template.Parse(" {{best_cars}} ").RenderAsync(this._assigns));
        }


        Task<string> RenderAsync(CultureInfo culture)
        {

            var renderParams = new RenderParameters(culture)
            {
                LocalVariables = _assigns
            };
            return Template.Parse("{{number}}").RenderAsync(renderParams);
        }

        [Test]
        public async Task TestSeperator_Comma()
        {

            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = ","
            };
            var c = new CultureInfo("en-US")
            {
                NumberFormat = nfi
            };
            Assert.AreEqual("3,145", await this.RenderAsync(c));

        }
        [Test]
        public async Task TestSeperator_Decimal()
        {
            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            var c = new CultureInfo("en-US")
            {
                NumberFormat = nfi
            };
            Assert.AreEqual("3.145", await this.RenderAsync(c));

        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action _Action;

            public ActionDisposable(Action action) => this._Action = action;

            public void Dispose() => this._Action();
        }
        IDisposable SetCulture(CultureInfo ci)
        {
            CultureInfo old = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = ci;
            return new ActionDisposable(() => CultureInfo.CurrentCulture = old);
        }


        [Test]
        public async Task ParsingWithCommaDecimalSeparatorShouldWorkWhenPassedCultureIsDifferentToCurrentCulture()
        {
            var ci = new CultureInfo(CultureInfo.CurrentCulture.Name)
            {
                NumberFormat =
                      {
                          NumberDecimalSeparator = ","
                          , NumberGroupSeparator = "."
                      }
            };
            using (this.SetCulture(ci))
            {
                var t = Template.Parse("{{2.5}}");
                string result = await t.RenderAsync(new Hash(), CultureInfo.InvariantCulture);

                Assert.AreEqual(result, "2.5");
            }
        }

        [Test]
        public void ParsingWithInvariantCultureShouldWork()
        {
            var ci = new CultureInfo(CultureInfo.CurrentCulture.Name)
            {
                NumberFormat =
                                  {
                                      NumberDecimalSeparator = ","
                                      , NumberGroupSeparator = "."
                                  }
            };
            using (this.SetCulture(ci))
            {
                float.TryParse("2.5", NumberStyles.Number, CultureInfo.InvariantCulture, out float result);

                Assert.AreEqual(2.5, result);
            }
        }

        [Test]
        public void ParsingWithExplicitCultureShouldWork()
        {
            var ci = new CultureInfo(CultureInfo.CurrentCulture.Name)
            {
                NumberFormat =
                                     {
                                         NumberDecimalSeparator = ","
                                         , NumberGroupSeparator = "."
                                     }
            };
            using (this.SetCulture(ci))
            {
                CultureInfo.CurrentCulture = ci;
                float.TryParse("2.5", NumberStyles.Number, ci, out float result);

                Assert.AreEqual(25, result);
            }
        }


        [Test]
        public void ParsingWithDefaultCultureShouldWork()
        {
            var ci = new CultureInfo(CultureInfo.CurrentCulture.Name)
            {
                NumberFormat =
                                  {
                                      NumberDecimalSeparator = ","
                                      , NumberGroupSeparator = "."
                                  }
            };
            using (this.SetCulture(ci))
            {
                float.TryParse("2.5", out float result);
                Assert.AreEqual(25, result);
            }
        }

        [Test]
        public async Task TestVariableTraversing()
        {
            Assert.AreEqual(" good bad good ", await Template.Parse(" {{car.bmw}} {{car.gm}} {{car.bmw}} ").RenderAsync(this._assigns));
        }

        [Test]
        public async Task TestVariablePiping()
        {
            Assert.AreEqual(" LOL ", await Template.Parse(" {{ car.gm | make_funny }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestVariablePipingWithInput()
        {
            Assert.AreEqual(" LOL: bad ", await Template.Parse(" {{ car.gm | cite_funny }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestVariablePipingWithArgs()
        {
            Assert.AreEqual(" bad :-( ", await Template.Parse(" {{ car.gm | add_smiley : ':-(' }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestVariablePipingWithNoArgs()
        {
            Assert.AreEqual(" bad :-) ", await Template.Parse(" {{ car.gm | add_smiley }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestMultipleVariablePipingWithArgs()
        {
            Assert.AreEqual(" bad :-( :-( ", await Template.Parse(" {{ car.gm | add_smiley : ':-(' | add_smiley : ':-(' }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestVariablePipingWithArgs2()
        {
            Assert.AreEqual(" <span id=\"bar\">bad</span> ", await Template.Parse(" {{ car.gm | add_tag : 'span', 'bar' }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestVariablePipingWithWithVariableArgs()
        {
            Assert.AreEqual(" <span id=\"good\">bad</span> ", await Template.Parse(" {{ car.gm | add_tag : 'span', car.bmw }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestMultiplePipings()
        {
            Assert.AreEqual(" <p>LOL: bmw</p> ", await Template.Parse(" {{ best_cars | cite_funny | paragraph }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }

        [Test]
        public async Task TestLinkTo()
        {
            Assert.AreEqual(" <a href=\"http://typo.leetsoft.com\">Typo</a> ", await Template.Parse(" {{ 'Typo' | link_to: 'http://typo.leetsoft.com' }} ").RenderAsync(new RenderParameters(CultureInfo.InvariantCulture) { LocalVariables = _assigns, Filters = new[] { typeof(FunnyFilter) } }));
        }
    }
}
