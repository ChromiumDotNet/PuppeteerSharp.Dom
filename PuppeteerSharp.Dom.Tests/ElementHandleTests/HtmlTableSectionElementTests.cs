using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableSectionElementTests : PuppeteerPageBaseTest
    {
        public HtmlTableSectionElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNull()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("#table2").GetBodyAsync();

            Assert.Null(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldInsertRow()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var row = await element.InsertRowAsync(-1);

            Assert.NotNull(row);
        }

        [PuppeteerDomFact]
        public async Task ShouldDeleteRow()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var initialLength = await element.GetRowsAsync().GetLengthAsync();
            var expected = initialLength - 1;

            await element.DeleteRowAsync(0);

            var actual = await element.GetRowsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldInsertThenDeleteRow()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var expected = await element.GetRowsAsync().GetLengthAsync();

            var row = await element.InsertRowAsync(-1);

            var rowIndex = await row.GetSectionRowIndexAsync();

            await element.DeleteRowAsync(rowIndex);

            var actual = await element.GetRowsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }
    }
}
