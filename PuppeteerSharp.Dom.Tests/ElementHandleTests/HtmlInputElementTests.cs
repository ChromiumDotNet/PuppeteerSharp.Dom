using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlInputElementTests : PuppeteerPageBaseTest
    {
        public HtmlInputElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await Page.QuerySelectorAsync<HtmlInputElement>("#agree");

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetChecked()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await Page.QuerySelectorAsync<HtmlInputElement>("#agree");

            await element.SetCheckedAsync(true);

            var actual = await element.GetCheckedAsync();

            Assert.True(actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetIndeterminate()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var element = await Page.QuerySelectorAsync<HtmlInputElement>("#agree");

            await element.SetIndeterminateAsync(true);

            var actual = await element.GetIndeterminateAsync();

            Assert.True(actual);
        }
    }
}
