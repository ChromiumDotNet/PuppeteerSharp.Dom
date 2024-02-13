using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageContextXPathTests : PuppeteerPageBaseTest
    {
        public PageContextXPathTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
#pragma warning disable CS0618 // Type or member is obsolete
            var elements = await Page.XPathAsync("/html/body/section");
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.NotNull(elements[0]);
            Assert.Single(elements);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var elements = await Page.XPathAsync("/html/body/non-existing-element");
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Empty(elements);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div></div><div></div>");
#pragma warning disable CS0618 // Type or member is obsolete
            var elements = await Page.XPathAsync("/html/body/div");
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal(2, elements.Length);
        }
    }
}
