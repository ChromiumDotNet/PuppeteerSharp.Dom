using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextXPathTests : PuppeteerPageBaseTest
    {
        public DevToolsContextXPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldQueryExistingElement()
        {
            await DevToolsContext.SetContentAsync("<section>test</section>");
            var elements = await DevToolsContext.XPathAsync("/html/body/section");
            Assert.NotNull(elements[0]);
            Assert.Single(elements);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            var elements = await DevToolsContext.XPathAsync("/html/body/non-existing-element");
            Assert.Empty(elements);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnMultipleElements()
        {
            await DevToolsContext.SetContentAsync("<div></div><div></div>");
            var elements = await DevToolsContext.XPathAsync("/html/body/div");
            Assert.Equal(2, elements.Length);
        }
    }
}
