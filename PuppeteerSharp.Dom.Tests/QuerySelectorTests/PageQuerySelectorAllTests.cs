using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Dom.Tests.Attributes;

namespace PuppeteerSharp.Dom.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageQuerySelectorAllTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorAllTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public async Task Usage()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            #region QuerySelectorAll

            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.

            // Add using PuppeteerSharp.Dom to access QuerySelectorAllAsync<T> extension method
            // Get elements by tag name
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelectorAll
            var inputElements = await page.QuerySelectorAllAsync<HtmlInputElement>("input");

            foreach (var element in inputElements)
            {
                var name = await element.GetNameAsync();
                var id = await element.GetIdAsync();

                var value = await element.GetValueAsync<int>();
            }

            #endregion
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var elements = await Page.QuerySelectorAllAsync<HtmlTableRowElement>("tr");

            Assert.NotNull(elements);
            Assert.Equal(4, elements.Length);
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnEmptyArray()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/table.html");
            var elements = await Page.QuerySelectorAllAsync<HtmlTableRowElement>("#table2 tr");

            Assert.NotNull(elements);
            Assert.Empty(elements);
        }

        [PuppeteerDomFact]
        public async Task ShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<div>A</div><br/><div>B</div>");
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Equal(2, elements.Length);
            var tasks = elements.Select(element => Page.EvaluateFunctionAsync<string>("e => e.textContent", element));
            Assert.Equal(new[] { "A", "B" }, await Task.WhenAll(tasks));
        }

        [PuppeteerDomFact]
        public async Task ShouldReturnEmptyArrayIfNothingIsFound()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var elements = await Page.QuerySelectorAllAsync("div");
            Assert.Empty(elements);
        }
    }
}
