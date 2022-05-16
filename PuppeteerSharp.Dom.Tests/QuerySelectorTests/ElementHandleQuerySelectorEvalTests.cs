using System;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleQuerySelectorEvalTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorEvalTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task QuerySelectorShouldWork()
        {
            await Page.SetContentAsync("<html><body><div class='tweet'><div class='like'>100</div><div class='retweets'>10</div></div></body></html>");
            var tweet = await Page.QuerySelectorAsync<HtmlDivElement>(".tweet");
            var content = await tweet.QuerySelectorAsync(".like")
                .AndThen(x => x.EvaluateFunctionAsync<string>("node => node.innerText"));
            Assert.Equal("100", content);
        }

        [PuppeteerDomFact]
        public async Task QuerySelectorShouldRetrieveContentFromSubtree()
        {
            var htmlContent = "<div class='a'>not-a-child-div</div><div id='myId'><div class='a'>a-child-div</div></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync<HtmlDivElement>("#myId");
            var content = await elementHandle.QuerySelectorAsync<HtmlElement>(".a")
                .AndThen(x => x.EvaluateFunctionAsync<string>("node => node.innerText"));
            Assert.Equal("a-child-div", content);
        }

        [PuppeteerDomFact]
        public async Task QuerySelectorShouldThrowInCaseOfMissingSelector()
        {
            var htmlContent = "<div class=\"a\">not-a-child-div</div><div id=\"myId\"></div>";
            await Page.SetContentAsync(htmlContent);
            var elementHandle = await Page.QuerySelectorAsync<HtmlDivElement>("#myId");
            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                () => elementHandle.QuerySelectorAsync<HtmlElement>(".a")
                    .AndThen(x => x.GetInnerTextAsync())
            );
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }
    }
}
