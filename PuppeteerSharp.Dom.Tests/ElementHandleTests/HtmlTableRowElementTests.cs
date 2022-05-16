using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableRowElementTests : PuppeteerPageBaseTest
    {
        public HtmlTableRowElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableRowElement>("tr");

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNull()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableRowElement>("#table2 tr");

            Assert.Null(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetIndex()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var rows = await element.GetRowsAsync();
            var row = await rows.ItemAsync(0);

            var index = await row.GetRowIndexAsync();

            Assert.True(index > 0);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetSelectionIndex()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table").GetBodyAsync();

            var rows = await element.GetRowsAsync();
            var row = await rows.ItemAsync(0);

            var index = await row.GetSectionRowIndexAsync();

            Assert.Equal(0, index);
        }

        [PuppeteerDomFact]
        public async Task ShouldDeleteCell()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableRowElement>("tr");

            var cells = await element.GetCellsAsync();
            var expected = await cells.GetLengthAsync() - 1;

            await element.DeleteCellAsync(0);

            var actual = await cells.GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldInsertThenDeleteCell()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableRowElement>("tr");

            var cell = await element.InsertCellAsync(-1, "Testing");

            var expected = await element.GetCellsAsync().GetLengthAsync() - 1;

            await element.DeleteCellAsync(cell);

            var actual = await element.GetCellsAsync().GetLengthAsync();

            Assert.Equal(expected, actual);
        }
    }
}
