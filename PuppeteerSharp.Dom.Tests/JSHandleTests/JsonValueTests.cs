using WebView2.DevTools.Dom;
using PuppeteerSharp.Dom.Tests.Attributes;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System;
using System.Text.Json;

namespace PuppeteerSharp.Dom.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
        {
            var aHandle = await DevToolsContext.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.GetValueAsync<JsonElement>();

            var actual = json.GetProperty("foo").GetString();

            Assert.Equal("bar", actual);
        }

        [PuppeteerDomFact]
        public async Task WorksWithJsonValuesThatAreNotObjects()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => ['a', 'b']");
            var json = await aHandle.GetValueAsync<string[]>();
            Assert.Equal(new[] {"a","b" }, json);
        }

        [PuppeteerDomFact]
        public async Task WorksWithJsonValuesThatArePrimitives()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => 'foo'");
            var json = await aHandle.GetValueAsync<string>();
            Assert.Equal("foo", json);
        }

        [PuppeteerDomFact]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await DevToolsContext.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.Equal("{}", json.ToString());
        }

        [PuppeteerDomFact]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await DevToolsContext.EvaluateExpressionHandleAsync("window");
            var exception = await Assert.ThrowsAsync<WebView2DevToolsContextException>(()
                => windowHandle.JsonValueAsync());

            // Improve this when https://github.com/MicrosoftEdge/WebView2Feedback/issues/1609
            // is resolved.
            Assert.Contains("CallFunctionOnAsync failed", exception.Message);
        }
    }
}
