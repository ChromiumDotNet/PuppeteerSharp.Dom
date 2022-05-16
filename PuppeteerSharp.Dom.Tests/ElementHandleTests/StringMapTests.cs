using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class StringMapTests : PuppeteerPageBaseTest
    {
        public StringMapTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/dataattributes.html");
            var dataSet = await Page.QuerySelectorAsync<HtmlElement>("h1").AndThen(x => x.GetDatasetAsync());

            Assert.NotNull(dataSet);

            var data = await dataSet.ToArrayAsync();

            Assert.NotNull(data);
            Assert.NotEmpty(data);

            Assert.Equal("testing", data[0].Key);
            Assert.Equal("Test1", data[0].Value);
            Assert.Equal("extra", data[1].Key);
            Assert.Equal("Test2", data[1].Value);
        }
    }
}
