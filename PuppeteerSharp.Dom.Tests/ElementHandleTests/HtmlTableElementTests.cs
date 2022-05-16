using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlTableElementTests : PuppeteerPageBaseTest
    {
        public HtmlTableElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            Assert.NotNull(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnNull()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("#table2");

            Assert.Null(element);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetTHead()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var thead = await element.GetTHeadAsync();

            Assert.NotNull(thead);
        }

        [PuppeteerDomFact]
        public async Task ShouldCreateTHead()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var thead = await element.CreateTHeadAsync();

            Assert.NotNull(thead);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetRowsFromTHead()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var thead = await element.GetTHeadAsync();

            Assert.NotNull(thead);

            var rows = await thead.GetRowsAsync();

            Assert.Equal(1, await rows.GetLengthAsync());

            await foreach(var row in rows)
            {
                Assert.Equal("HTMLTableRowElement", row.ClassName);

                await foreach (var cell in await row.GetCellsAsync())
                {
                    Assert.Equal("HTMLTableCellElement", cell.ClassName);
                }
            }
        }

        [PuppeteerDomFact]
        public async Task ShouldGetRowsAsArrayFromTHead()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var thead = await element.GetTHeadAsync();

            Assert.NotNull(thead);

            var rows = await thead.GetRowsAsync().ToArrayAsync();

            Assert.Single(rows);

            foreach (var row in rows)
            {
                Assert.Equal("HTMLTableRowElement", row.ClassName);

                await foreach (var cell in await row.GetCellsAsync())
                {
                    Assert.Equal("HTMLTableCellElement", cell.ClassName);
                }
            }
        }

        [PuppeteerDomFact]
        public async Task ShouldGetTFoot()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var tfoot = await element.GetTFootAsync();

            Assert.NotNull(tfoot);
        }

        [PuppeteerDomFact]
        public async Task ShouldCreateTFoot()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var tfoot = await element.CreateTFootAsync();

            Assert.NotNull(tfoot);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetRowsFromTFoot()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var tfoot = await element.GetTFootAsync();

            Assert.NotNull(tfoot);

            var rows = await tfoot.GetRowsAsync();

            Assert.Equal(1, await rows.GetLengthAsync());

            await foreach (var row in rows)
            {
                Assert.Equal("HTMLTableRowElement", row.ClassName);

                await foreach (var cell in await row.GetCellsAsync())
                {
                    Assert.Equal("HTMLTableCellElement", cell.ClassName);
                }
            }
        }

        [PuppeteerDomFact]
        public async Task CanAddToTable()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var rows = await element.GetRowsAsync();
            var rowCount = await rows.GetLengthAsync();

            var expected = rowCount + 2;

            var newRow1 = await element.InsertRowAsync();

            var newCell1 = await newRow1.InsertCellAsync(0, "New Cell #1");
            var newCell2 = await newRow1.InsertCellAsync(1, "New Cell #2");

            Assert.Equal("New Cell #1", await newCell1.GetInnerTextAsync());
            Assert.Equal("New Cell #2", await newCell2.GetInnerTextAsync());

            var newRow2 = await element.InsertRowAsync(1);

            var newCell3 = await newRow2.InsertCellAsync(0, "New Cell #3");
            var newCell4 = await newRow2.InsertCellAsync(1, "New Cell #4");

            Assert.Equal("New Cell #3", await newCell3.GetInnerTextAsync());
            Assert.Equal("New Cell #4", await newCell4.GetInnerTextAsync());

            var actual = await rows.GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetTBodies()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var tbodies = await element.GetTBodiesAsync();

            Assert.NotNull(tbodies);
            Assert.IsType<HtmlCollection<HtmlTableSectionElement>>(tbodies);
        }

        [PuppeteerDomFact]
        public async Task ShouldEnumerateTBodies()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var element = await Page.QuerySelectorAsync<HtmlTableElement>("table");

            var tbodies = await element.GetTBodiesAsync();
            var length = await tbodies.GetLengthAsync();

            Assert.Equal(1, length);

            await foreach(var e in tbodies)
            {
                Assert.IsType<HtmlTableSectionElement>(e);
            }
        }
    }
}
