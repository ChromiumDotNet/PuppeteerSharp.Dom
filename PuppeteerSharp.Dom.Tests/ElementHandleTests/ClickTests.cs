using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ClickTests : PuppeteerPageBaseTest
    {
        public ClickTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldWorkForShadowDomV1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = await Page.EvaluateExpressionHandleAsync("button") as ElementHandle;
            await buttonHandle.ClickAsync();
            Assert.True(await Page.EvaluateExpressionAsync<bool>("clicked"));
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowForDetachedNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.RemoveAsync();
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is detached from document", exception.Message);
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowForHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.GetStyleAsync()
                .AndThen(x => x.SetPropertyAsync("display", "none"));
            //await Page.EvaluateFunctionAsync("button => button.style.display = 'none'", (JSHandle)button);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not clickable or not an Element", exception.Message);
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowForRecursivelyHiddenNodes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.GetParentElementAsync<HtmlElement>()
                .AndThen(x => x.GetStyleAsync())
                .AndThen(x => x.SetPropertyAsync("display", "none"));
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await button.ClickAsync());
            Assert.Equal("Node is either not clickable or not an Element", exception.Message);
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowForBrElements()
        {
            await Page.SetContentAsync("hello<br>goodbye");
            var br = await Page.QuerySelectorAsync<HtmlElement>("br");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await br.ClickAsync());
            Assert.Equal("Node is either not clickable or not an Element", exception.Message);
        }
    }
}
