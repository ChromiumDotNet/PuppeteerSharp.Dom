using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit.Abstractions;
using Xunit;

namespace PuppeteerSharp.Dom.Tests.ElementHandleTests
{

    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HtmlSelectElementTests : PuppeteerPageBaseTest
    {
        public HtmlSelectElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var button = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            Assert.NotNull(button);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetDisabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetDisabledAsync(true);

            var actual = await element.GetDisabledAsync();

            Assert.True(actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldSetThenGetName()
        {
            const string expected = "selectName";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetNameAsync(expected);

            var actual = await element.GetNameAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetTypeSingle()
        {
            const string expected = "select-one";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetTypeMultiple()
        {
            const string expected = "select-multiple";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetMultipleAsync(true);

            var actual = await element.GetTypeAsync();

            Assert.Equal(expected, actual);
        }


        [PuppeteerDomFact]
        public async Task ShouldSetThenGetValue()
        {
            const string expected = "indigo";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            await element.SetValueAsync(expected);

            var actual = await element.GetValueAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldMoveOption()
        {
            const string expected = "green";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            var itemSixth = await element.ItemAsync(5);

            await element.AddAsync(itemSixth, 0);

            var itemFirst = await element.ItemAsync(0);

            var actual = await itemFirst.GetValueAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldAddOption()
        {
            const string expected = "darkBlue";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            var newOption = await element.AddAsync(expected, "Dark Blue");

            Assert.NotNull(newOption);

            var length = await element.GetLengthAsync();

            var lastElement = await element.ItemAsync(length - 1);

            var actual1 = await lastElement.GetValueAsync();
            var actual2 = await newOption.GetValueAsync();

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual2);
        }

        [PuppeteerDomFact]
        public async Task ShouldRemoveLastOption()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            var length = await element.GetLengthAsync();

            var expected = length - 1;

            await element.RemoveAsync(length - 1);

            var actual = await element.GetLengthAsync();

            Assert.Equal(expected, actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldGetOptionByName()
        {
            const string expected = "red";

            await Page.GoToAsync(TestConstants.ServerUrl + "/input/select.html");
            var element = await Page.QuerySelectorAsync<HtmlSelectElement>("select");

            var option = await element.NamedItemAsync(expected);

            var actual = await option.GetValueAsync();

            Assert.Equal(expected, actual);
        }
    }
}
