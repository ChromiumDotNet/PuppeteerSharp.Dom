using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CssStyleTests : PuppeteerPageBaseTest
    {
        public CssStyleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            Assert.NotNull(style);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetPriorityFalse()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            var priority = await style.GetPropertyPriorityAsync("border");

            Assert.False(priority);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetPriorityTrue()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border", "1px solid red", true);

            var priority = await style.GetPropertyPriorityAsync("border");

            Assert.True(priority);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetPropertyAsString()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border", "1px solid red", true);

            var actual = await style.GetPropertyValueAsync<string>("border-width");

            Assert.Equal("1px", actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetPropertyAsInt()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("z-index", 10);

            var actual = await style.GetPropertyValueAsync<int>("z-index");

            Assert.Equal(10, actual);
        }

        [InlineData("auto")]
        [InlineData("1")]
        [PuppeteerDomTheory]
        public async Task ShouldGetPropertyZIndexAsObject(object expected)
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("z-index", expected);

            var actual = await style.GetPropertyValueAsync<object>("z-index");

            Assert.IsType<JsonElement>(actual);

            var actualJE = (JsonElement)actual;
            Assert.Equal(JsonValueKind.String, actualJE.ValueKind);
            Assert.Equal(expected, actualJE.GetString());
        }

        [PuppeteerDomFact]
        public async Task ShouldGetPropertyNameByIndex()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border", "1px solid red", true);

            var propertyName = await style.ItemAsync(0);

            Assert.Equal("border-top-width", propertyName);
        }

        [PuppeteerDomFact]
        public async Task ShouldRemovePropertyByName()
        {
            const string expectedValue = "1px";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync<HtmlButtonElement>("button");

            var style = await button.GetStyleAsync();

            await style.SetPropertyAsync("border-top-width", expectedValue, true);
            
            var actualValue = await style.RemovePropertyAsync("border-top-width");

            Assert.Equal(expectedValue, actualValue);
        }
    }
}
