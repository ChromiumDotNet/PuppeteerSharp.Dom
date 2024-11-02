using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using PuppeteerSharp.Dom.Tests.Attributes;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AttributeTests : PuppeteerPageBaseTest
    {
        public AttributeTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldGetAttribute()
        {
            const string expected = "checkbox";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await Page.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await checkbox.GetAttributeAsync("type");

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetAttribute()
        {
            const string expected = "1676";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await Page.QuerySelectorAsync<HtmlInputElement>("#agree");
            await checkbox.SetAttributeAsync("data-custom", expected);

            var actual = await checkbox.GetAttributeAsync("data-custom");

            Assert.Equal(expected, actual);
        }
    }
}
