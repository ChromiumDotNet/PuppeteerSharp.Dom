using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BoundingBoxTests : PuppeteerPageBaseTest
    {
        public BoundingBoxTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            var elementHandle = await Page.QuerySelectorAsync<HtmlElement>(".box:nth-of-type(13)");
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(100, 50, 50, 50), box);
        }

        [PuppeteerDomFact]
        public async Task ShouldHandleNestedFrames()
        {
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 500,
                Height = 500
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/nested-frames.html");
            var childFrame = Page.Frames.First(f => f.Url.Contains("two-frames.html"));
            var nestedFrame = childFrame.ChildFrames.Last();
            var elementHandle = await nestedFrame.QuerySelectorAsync<HtmlElement>("div");
            var box = await elementHandle.BoundingBoxAsync();

            Assert.Equal(new BoundingBox(28, 182, 264, 18), box);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNullForInvisibleElements()
        {
            await Page.SetContentAsync("<div style='display:none'>hi</div>");
            var elementHandle = await Page.QuerySelectorAsync<HtmlDivElement>("div");
            Assert.Null(await elementHandle.BoundingBoxAsync());
        }

        [PuppeteerDomFact]
        public async Task ShouldForceALayout()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync("<div style='width: 100px; height: 100px'>hello</div>");
            var elementHandle = await Page.QuerySelectorAsync<HtmlDivElement>("div");
            await Page.EvaluateFunctionAsync("element => element.style.height = '200px'", elementHandle);
            var box = await elementHandle.BoundingBoxAsync();
            Assert.Equal(new BoundingBox(8, 8, 100, 200), box);
        }

        [PuppeteerDomFact]
        public async Task ShouldWworkWithSVGNodes()
        {
            await Page.SetContentAsync(@"
                <svg xmlns=""http://www.w3.org/2000/svg"" width=""500"" height=""500"">
                  <rect id=""theRect"" x=""30"" y=""50"" width=""200"" height=""300""></rect>
                </svg>
            ");

            var element = await Page.QuerySelectorAsync<HtmlElement>("#therect");
            var pptrBoundingBox = await element.BoundingBoxAsync();
            var webBoundingBox = await Page.EvaluateFunctionAsync<BoundingBox>(@"e =>
            {
                const rect = e.getBoundingClientRect();
                return { x: rect.x, y: rect.y, width: rect.width, height: rect.height};
            }", element);
            Assert.Equal(webBoundingBox, pptrBoundingBox);
        }
    }
}
