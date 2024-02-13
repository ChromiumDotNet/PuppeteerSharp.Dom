using System;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.WaitForTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageWaitForTests : PuppeteerPageBaseTest
    {
        public PageWaitForTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWaitForSelector()
        {
            var found = false;
            var waitFor = Page.WaitForSelectorAsync<HtmlDivElement>("div").ContinueWith(_ => found = true);
            await Page.GoToAsync(TestConstants.EmptyPage);

            Assert.False(found);

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await waitFor;

            Assert.True(found);
        }

        [PuppeteerDomFact]
        public async Task ShouldWaitForAnXpath()
        {
            var found = false;
#pragma warning disable CS0618 // Type or member is obsolete
            var waitFor = Page.WaitForXPathAsync<HtmlDivElement>("//div").ContinueWith(_ => found = true);
#pragma warning restore CS0618 // Type or member is obsolete
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(found);
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await waitFor;
            Assert.True(found);
        }

        [PuppeteerDomFact]
        public async Task ShouldNotAllowYouToSelectAnElementWithSingleSlashXpath()
        {
            await Page.SetContentAsync("<div>some text</div>");

            var exception = await Assert.ThrowsAsync<WaitTaskTimeoutException>(() =>
                Page.WaitForSelectorAsync<HtmlDivElement>("/html/body/div"));

            Assert.NotNull(exception);
        }
    }
}
