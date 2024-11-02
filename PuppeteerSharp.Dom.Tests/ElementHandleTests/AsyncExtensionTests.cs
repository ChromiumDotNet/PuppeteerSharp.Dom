using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;
using System;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsyncExtensionTests : PuppeteerPageBaseTest
    {
        public AsyncExtensionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldAndThenFunction()
        {
            const string expected = "checkbox";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = Page.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await element.AndThen(x => x.GetAttributeAsync("type"));

            Assert.Equal(expected, actual);
            Assert.True(element.Result.IsDisposed);
        }

        [PuppeteerDomFact]
        public async Task ShouldAndThenFunctionChain()
        {
            const string expected = "checkbox";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = Page.QuerySelectorAsync<HtmlBodyElement>("body");
            var actual = await element
                .AndThen(x => x.QuerySelectorAsync<HtmlInputElement>("#agree"))
                .AndThen(x => x.GetAttributeAsync("type"));

            Assert.Equal(expected, actual);
            Assert.True(element.Result.IsDisposed);
        }

        [PuppeteerDomFact]
        public async Task ShouldAndThenFunctionWithoutDispose()
        {
            const string expected = "checkbox";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = Page.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await element.AndThen(x => x.GetAttributeAsync("type"), dispose: false);

            Assert.Equal(expected, actual);
            Assert.False(element.Result.IsDisposed);
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowNullReferenceException()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");

            var element = await Page.QuerySelectorAsync<HtmlBodyElement>("body");

            var exception = await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                var content = await element.QuerySelectorAsync<HtmlDivElement>(".MyElement")
                .AndThen(x => x.GetTextContentAsync());
            });

            Assert.NotNull(exception);
        }
    }
}
