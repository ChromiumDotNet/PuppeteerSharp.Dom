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
            var element = await Page.EvaluateExpressionHandleAsync<HtmlElement>("document.body");
            
            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync<HtmlElement>("2");

            Assert.Null(aHandle);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNullForNull()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync<HtmlElement>("null");

            Assert.Null(aHandle);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await Page.SetContentAsync("<div>ee!</div>");

            var element = await Page.EvaluateExpressionHandleAsync<Text>("document.querySelector('div').firstChild");

            Assert.NotNull(element);

            var nodeType = await element.GetNodeTypeAsync();

            Assert.Equal(NodeType.Text, nodeType);
        }
    }
}
