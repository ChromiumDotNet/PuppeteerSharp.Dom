using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Dom;
using PuppeteerSharp.Dom.Tests.Attributes;
using PuppeteerSharp.Input;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.ClickTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickSvg()
        {
            await Page.SetContentAsync($@"
                <svg height=""100"" width=""100"">
                  <circle onclick=""javascript:window.__CLICKED=42"" cx=""50"" cy=""50"" r=""40"" stroke=""black"" stroke-width=""3"" fill=""red""/>
                </svg>
            ");
            await Page.ClickAsync("circle");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.__CLICKED"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheButtonIfWindowNodeIsRemoved()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickOnASpanWithAnInlineElementInside()
        {
            await Page.SetContentAsync($@"
                <style>
                span::before {{
                    content: 'q';
                }}
                </style>
                <span onclick='javascript:window.CLICKED=42'></span>
            ");
            await Page.ClickAsync("span");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheButtonAfterNavigation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickWhenOneOfInlineBoxChildrenIsOutsideOfViewport()
        {
            await Page.SetContentAsync($@"
            <style>
            i {{
                position: absolute;
                top: -1000px;
            }}
            </style>
            <span onclick='javascript:window.CLICKED = 42;'><i>woof</i><b>doggo</b></span>
            ");

            await Page.ClickAsync("span");
            Assert.Equal(42, await Page.EvaluateFunctionAsync<int>("() => window.CLICKED"));
        }

        [PuppeteerDomFact]
        public async Task ShouldSelectTheTextByTripleClicking()
        {
            const string expected = "This is the text that we are going to try to select. Let's see how it goes.";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await Page.FocusAsync("textarea");

            await Page.Keyboard.TypeAsync(expected);
            await Page.ClickAsync("textarea");
            await Page.ClickAsync("textarea", new ClickOptions { Count = 2 });
            await Page.ClickAsync("textarea", new ClickOptions { Count = 3 });

            var actual = await Page.EvaluateFunctionAsync<string>(@"() => {
                const textarea = document.querySelector('textarea');
                return textarea.value.substring(
                    textarea.selectionStart,
                    textarea.selectionEnd
                );
            }");

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldClickWrappedLinks()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/wrappedlink.html");
            await Page.ClickAsync("a");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("window.__clicked"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickOnCheckboxInputAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await Page.EvaluateExpressionAsync<object>("result.check"));
            await Page.ClickAsync("input#agree");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "mouseover",
                "mouseenter",
                "mousemove",
                "mousedown",
                "mouseup",
                "click",
                "input",
                "change"
            }, await Page.EvaluateExpressionAsync<string[]>("result.events"));
            await Page.ClickAsync("input#agree");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("result.check"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickOnCheckboxLabelAndToggle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            Assert.Null(await Page.EvaluateExpressionAsync<object>("result.check"));
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.True(await Page.EvaluateExpressionAsync<bool>("result.check"));
            Assert.Equal(new[] {
                "click",
                "input",
                "change"
            }, await Page.EvaluateExpressionAsync<string[]>("result.events"));
            await Page.ClickAsync("label[for=\"agree\"]");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("result.check"));
        }

        [PuppeteerDomFact]
        public async Task ShouldFailToClickAMissingButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var exception = await Assert.ThrowsAsync<SelectorException>(()
                => Page.ClickAsync("button.does-not-exist"));
            Assert.Equal("No node found for selector: button.does-not-exist", exception.Message);
            Assert.Equal("button.does-not-exist", exception.Selector);
        }

        [PuppeteerDomFact]
        public async Task ShouldScrollAndClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-5");
            Assert.Equal("clicked", await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-5\").textContent"));
            await Page.ClickAsync("#button-80");
            Assert.Equal("clicked", await Page.EvaluateExpressionAsync<string>("document.querySelector(\"#button-80\").textContent"));
        }

        [PuppeteerDomFact]
        public async Task ShouldDoubleClickTheButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync(@"{
               window.double = false;
               const button = document.querySelector('button');
               button.addEventListener('dblclick', event => {
                 window.double = true;
               });
            }");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync(new ClickOptions { Count = 2 });
            Assert.True(await Page.EvaluateExpressionAsync<bool>("double"));
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickAPartiallyObscuredButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.EvaluateExpressionAsync(@"{
                const button = document.querySelector('button');
                button.textContent = 'Some really long text that will go offscreen';
                button.style.position = 'absolute';
                button.style.left = '368px';
            }");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickARotatedButton()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/rotatedButton.html");
            await Page.ClickAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldFireContextmenuEventOnRightClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.ClickAsync("#button-8", new ClickOptions { Button = MouseButton.Right });
            Assert.Equal("context menu", await Page.EvaluateExpressionAsync<string>("document.querySelector('#button-8').textContent"));
        }

        // @see https://github.com/GoogleChrome/puppeteer/issues/206
        [PuppeteerDomFact]
        public async Task ShouldClickLinksWhichCauseNavigation()
        {
            await Page.SetContentAsync($"<a href=\"{TestConstants.EmptyPage}\">empty.html</a>");
            // This await should not hang.
            await Page.ClickAsync("a");
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheButtonInsideAnIframe()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheButtonWithDeviceScaleFactorSet()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 400, DeviceScaleFactor = 5 });
            Assert.Equal(5, await Page.EvaluateExpressionAsync<int>("window.devicePixelRatio"));
            await Page.SetContentAsync("<div style=\"width:100px;height:100px\">spacer</div>");
            await FrameUtils.AttachFrameAsync(Page, "button-test", TestConstants.ServerUrl + "/input/button.html");
            var frame = Page.FirstChildFrame();
            var button = await frame.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await frame.EvaluateExpressionAsync<string>("window.result"));
        }
    }
}
