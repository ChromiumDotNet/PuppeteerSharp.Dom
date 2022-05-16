using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlButtonElementTests : PuppeteerPageBaseTest
    {
        public HtmlButtonElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            Assert.NotNull(button);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetDisabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetDisabledAsync(true);

            var actual = await button.GetDisabledAsync();

            Assert.True(actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetName()
        {
            const string expected = "buttonName";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetNameAsync(expected);

            var actual = await button.GetNameAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetType()
        {
            const HtmlButtonElementType expected = HtmlButtonElementType.Submit;

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetTypeAsync(expected);

            var actual = await button.GetTypeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetValue()
        {
            const string expected = "Test Button";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            await button.SetValueAsync(expected);

            var actual = await button.GetValueAsync();

            Assert.Equal(expected, actual);
        }
    }
}
