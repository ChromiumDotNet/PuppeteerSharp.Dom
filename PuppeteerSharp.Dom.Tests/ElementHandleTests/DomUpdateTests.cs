using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DomUpdateTests : PuppeteerPageBaseTest
    {
        public DomUpdateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            const string expected = "Testing123";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");

            var before = await button.GetTextContentAsync();

            Assert.Equal("Click target", before);

            await button.SetTextContentAsync(expected);

            var actual = await button.GetTextContentAsync();

            Assert.Equal(expected, actual);
        }
    }
}
