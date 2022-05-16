using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableCellElementTests : PuppeteerPageBaseTest
    {
        public HtmlTableCellElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNull()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("#table2 td");

            Assert.Null(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetIndex()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            var index = await element.GetCellIndexAsync();

            Assert.True(index > -1);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetAbbr()
        {
            const string expected = "Testing";

            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetAbbrAsync(expected);
            var actual = await element.GetAbbrAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetScope()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            var actual = await element.GetScopeAsync();

            Assert.Equal(string.Empty, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetScope()
        {
            const string expected = "col";

            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetScopeAsync(expected);
            var actual = await element.GetScopeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetRowSpan()
        {
            const int expected = 3;

            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetRowSpanAsync(expected);
            var actual = await element.GetRowSpanAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetColSpan()
        {
            const int expected = 3;

            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableCellElement>("td");

            await element.SetColSpanAsync(expected);
            var actual = await element.GetColSpanAsync();

            Assert.Equal(expected, actual);
        }
    }
}
