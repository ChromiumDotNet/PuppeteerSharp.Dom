using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsElementTests : PuppeteerPageBaseTest
    {
        public AsElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            var element = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("document.body");
            
            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("2");

            Assert.Null(aHandle);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNullForNull()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync<HtmlElement>("null");

            Assert.Null(aHandle);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await DevToolsContext.SetContentAsync("<div>ee!</div>");

            var element = await DevToolsContext.EvaluateExpressionHandleAsync<Text>("document.querySelector('div').firstChild");

            Assert.NotNull(element);
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("e => e.nodeType === HTMLElement.TEXT_NODE", element));
        }
    }
}
