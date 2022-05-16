using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlAnchorElementTests : PuppeteerPageBaseTest
    {
        public HtmlAnchorElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await Page.QuerySelectorAsync<HtmlAnchorElement>("a");

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNull()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await Page.QuerySelectorAsync<HtmlAnchorElement>("a");

            Assert.Null(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetDisabled()
        {
            const string expected = "_blank";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await Page.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetTargetAsync(expected);

            var actual = await element.GetTargetAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetHref()
        {
            const string expected = "https://microsoft.com/";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await Page.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetHrefAsync(expected);

            var actual = await element.GetHrefAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetType()
        {
            const string expected = "text/html";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var element = await Page.QuerySelectorAsync<HtmlAnchorElement>("a");

            await element.SetTypeAsync(expected);

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }
    }
}
