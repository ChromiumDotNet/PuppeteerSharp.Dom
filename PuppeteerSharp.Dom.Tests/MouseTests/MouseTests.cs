using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using PuppeteerSharp.Input;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.MouseTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class MouseTests : PuppeteerPageBaseTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private const string Dimensions = @"function dimensions() {
            const rect = document.querySelector('textarea').getBoundingClientRect();
            return {
                x: rect.left,
                y: rect.top,
                width: rect.width,
                height: rect.height
            };
        }";

        public MouseTests(ITestOutputHelper output) : base(output)
        {
            _testOutputHelper = output;
        }

        [PuppeteerDomFact]
        public async Task ShouldClickTheDocument()
        {
            await Page.EvaluateFunctionAsync(@"() => {
                globalThis.clickPromise = new Promise((resolve) => {
                    document.addEventListener('click', (event) => {
                    resolve({
                        type: event.type,
                        detail: event.detail,
                        clientX: event.clientX,
                        clientY: event.clientY,
                        isTrusted: event.isTrusted,
                        button: event.button,
                    });
                    });
                });
            }");
            await Page.Mouse.ClickAsync(50, 60);
            var e = await Page.EvaluateFunctionAsync<MouseEvent>("() => globalThis.clickPromise");

            Assert.Equal("click", e.Type);
            Assert.Equal(1, e.Detail);
            Assert.Equal(50, e.ClientX);
            Assert.Equal(60, e.ClientY);
            Assert.True(e.IsTrusted);
            Assert.Equal(0, e.Button);
        }

        [PuppeteerDomFact]
        public async Task ShouldResizeTheTextarea()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            var mouse = Page.Mouse;
            await mouse.MoveAsync(dimensions.X + dimensions.Width - 4, dimensions.Y + dimensions.Height - 4);
            await mouse.DownAsync();
            await mouse.MoveAsync(dimensions.X + dimensions.Width + 100, dimensions.Y + dimensions.Height + 100);
            await mouse.UpAsync();
            var newDimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);

            Assert.Equal(dimensions.Width + 104, newDimensions.Width, 0);
            Assert.Equal(dimensions.Height + 104, newDimensions.Height, 0);
        }

        [PuppeteerDomFact]
        public async Task ShouldSelectTheTextWithMouse()
        {
            const string expectedText = "This is the text that we are going to try to select. Let's see how it goes.";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textArea = await Page.QuerySelectorAsync<HtmlTextAreaElement>("textarea");
            await textArea.FocusAsync();
            await textArea.TypeAsync(expectedText);

            await Page.WaitForFunctionAsync(@"(text) => document.querySelector('textarea').value === text", expectedText);

            var dimensions = await Page.EvaluateFunctionAsync<Dimensions>(Dimensions);
            await Page.Mouse.MoveAsync(dimensions.X + 2, dimensions.Y + 2);
            await Page.Mouse.DownAsync();
            await Page.Mouse.MoveAsync(dimensions.Width, dimensions.Height + 20);
            await Page.Mouse.UpAsync();

            var selectionStart = await textArea.GetSelectionStartAsync();
            var selectionEnd = await textArea.GetSelectionEndAsync();
            var value = await textArea.GetValueAsync();
            var actualText = value.Substring(selectionStart, selectionEnd);

            Assert.Equal(expectedText, actualText);
        }

        [PuppeteerDomFact]
        public async Task ShouldTriggerHoverState()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.HoverAsync("#button-6");
            Assert.Equal("button-6", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-2");
            Assert.Equal("button-2", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
            await Page.HoverAsync("#button-91");
            Assert.Equal("button-91", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
        }

        [PuppeteerDomFact]
        public async Task ShouldTriggerHoverStateWithRemovedWindowNode()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("delete window.Node");
            await Page.HoverAsync("#button-6");
            Assert.Equal("button-6", await Page.EvaluateExpressionAsync<string>("document.querySelector('button:hover').id"));
        }

        [PuppeteerDomFact]
        public async Task ShouldSetModifierKeysOnClick()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            await Page.EvaluateExpressionAsync("document.querySelector('#button-3').addEventListener('mousedown', e => window.lastEvent = e, true)");
            var modifiers = new Dictionary<string, string> { ["Shift"] = "shiftKey", ["Control"] = "ctrlKey", ["Alt"] = "altKey", ["Meta"] = "metaKey" };
            foreach (var modifier in modifiers)
            {
                await Page.Keyboard.DownAsync(modifier.Key);
                await Page.ClickAsync("#button-3");
                if (!(await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value)))
                {
                    Assert.Fail($"{modifier.Value} should be true");
                }

                await Page.Keyboard.UpAsync(modifier.Key);
            }
            await Page.ClickAsync("#button-3");
            foreach (var modifier in modifiers)
            {
                if (await Page.EvaluateFunctionAsync<bool>("mod => window.lastEvent[mod]", modifier.Value))
                {
                    Assert.Fail($"{modifiers.Values} should be false");
                }
            }
        }

        [PuppeteerDomFact]
        public async Task ShouldSendMouseWheelEvents()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/wheel.html");
            var elem = await Page.QuerySelectorAsync<HtmlDivElement>("div");
            var boundingBoxBefore = await elem.BoundingBoxAsync();
            Assert.Equal(115, boundingBoxBefore.Width);
            Assert.Equal(115, boundingBoxBefore.Height);

            await Page.Mouse.MoveAsync(
                boundingBoxBefore.X + (boundingBoxBefore.Width / 2),
                boundingBoxBefore.Y + (boundingBoxBefore.Height / 2)
            );

            await Page.Mouse.WheelAsync(0, -100);
            var boundingBoxAfter = await elem.BoundingBoxAsync();

            Assert.Equal(259, boundingBoxAfter.Width, 0);
            Assert.Equal(259, boundingBoxAfter.Height, 0);
        }

        [PuppeteerDomFact]
        public async Task ShouldTweenMouseMovement()
        {
            await Page.Mouse.MoveAsync(100, 100);
            await Page.EvaluateExpressionAsync(@"{
                window.result = [];
                document.addEventListener('mousemove', event => {
                    window.result.push([event.clientX, event.clientY]);
                });
            }");
            await Page.Mouse.MoveAsync(200, 300, new MoveOptions { Steps = 5 });
            Assert.Equal(new[] {
                new[]{ 120, 140 },
                new[]{ 140, 180 },
                new[]{ 160, 220 },
                new[]{ 180, 260 },
                new[]{ 200, 300 }
            }, await Page.EvaluateExpressionAsync<int[][]>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldWorkWithMobileViewportsAndCrossProcessNavigations()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 360,
                Height = 640,
                IsMobile = true
            });
            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/mobile.html");

            await Page.EvaluateFunctionAsync(@"() => {
                document.addEventListener('click', event => {
                    window.result = { x: event.clientX, y: event.clientY };
                });
            }");

            await Page.Mouse.ClickAsync(30, 40);

            Assert.Equal(new DomPointInternal()
            {
                X = 30,
                Y = 40
            }, await Page.EvaluateExpressionAsync<DomPointInternal>("result"));
        }

        internal struct WheelEventInternal
        {
            public WheelEventInternal(decimal deltaX, decimal deltaY)
            {
                DeltaX = deltaX;
                DeltaY = deltaY;
            }

            public decimal DeltaX { get; set; }

            public decimal DeltaY { get; set; }

            public override string ToString() => $"({DeltaX}, {DeltaY})";
        }

        internal struct DomPointInternal
        {
            public decimal X { get; set; }

            public decimal Y { get; set; }

            public override string ToString() => $"({X}, {Y})";

            public void Scroll(decimal deltaX, decimal deltaY)
            {
                X = Math.Max(0, X + deltaX);
                Y = Math.Max(0, Y + deltaY);
            }
        }

        internal struct MouseEvent
        {
            public string Type { get; set; }

            public int Detail { get; set; }

            public int ClientX { get; set; }

            public int ClientY { get; set; }

            public bool IsTrusted { get; set; }

            public int Button { get; set; }
        }
    }
}
