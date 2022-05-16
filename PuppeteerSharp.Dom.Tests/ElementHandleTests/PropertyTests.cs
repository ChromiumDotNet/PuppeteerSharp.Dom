using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PropertyTests : PuppeteerPageBaseTest
    {
        public PropertyTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetInnerText()
        {
            const string expected = "Updated Inner Text";

            await Page.GoToAsync(TestConstants.ServerUrl + "/longText.html");
            var p = await Page.QuerySelectorAsync<HtmlParagraphElement>("p");
            await p.SetInnerTextAsync(expected);
            var actual = await p.GetInnerTextAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetInnerHtml()
        {
            const string expected = "<div>Welcome To Updated Inner Html</div>";

            await Page.GoToAsync(TestConstants.ServerUrl + "/longText.html");
            var p = await Page.QuerySelectorAsync<HtmlParagraphElement>("p");
            await p.SetInnerHtmlAsync(expected);
            var actual = await p.GetInnerHtmlAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetOuterHtml()
        {
            const string expected = "<div>Welcome To Updated Outer Html</div>";

            await Page.GoToAsync(TestConstants.ServerUrl + "/longText.html");
            var p = await Page.QuerySelectorAsync<HtmlParagraphElement>("p");
            await p.SetOuterHtmlAsync(expected);

            var div = await Page.QuerySelectorAsync<HtmlDivElement>("div");

            var actual = await div.GetOuterHtmlAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetOuterText()
        {
            const string expected = "Welcome To Updated Outer Text";

            await Page.GoToAsync(TestConstants.ServerUrl + "/longText.html");
            var p = await Page.QuerySelectorAsync<HtmlParagraphElement>("p");

            await p.SetInnerHtmlAsync("<div>Div for testing</div>");

            var div = await p.QuerySelectorAsync<HtmlDivElement>("div");

            await div.SetOuterTextAsync(expected);

            var actual = await p.GetOuterTextAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetTextContent()
        {
            const string expected = "Updated Text Content";

            await Page.GoToAsync(TestConstants.ServerUrl + "/longText.html");
            var p = await Page.QuerySelectorAsync<HtmlParagraphElement>("p");
            await p.SetTextContentAsync(expected);
            var actual = await p.GetTextContentAsync();

            Assert.Equal(expected, actual);
        }
    }
}
