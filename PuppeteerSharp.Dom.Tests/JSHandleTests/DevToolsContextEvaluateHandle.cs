using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEvaluateHandle : PuppeteerPageBaseTest
    {
        public DevToolsContextEvaluateHandle(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldWork()
            => Assert.NotNull(await DevToolsContext.EvaluateFunctionHandleAsync("() => window"));

        [PuppeteerDomFact]
        public async Task ShouldAcceptObjectHandleAsAnArgument()
        {
            var navigatorHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => navigator");
            var text = await DevToolsContext.EvaluateFunctionAsync<string>(
                "(e) => e.userAgent",
                navigatorHandle);
            Assert.Contains("Mozilla", text);
        }

        [PuppeteerDomFact]
        public async Task ShouldAcceptObjectHandleToPrimitiveTypes()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => 5");
            var isFive = await DevToolsContext.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, 5)",
                aHandle);
            Assert.True(isFive);
        }

        [PuppeteerDomFact(Skip = "TODO: Fixme")]
        public async Task ShouldWarnOnNestedObjectHandles()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => document.body");
            var exception = await Assert.ThrowsAsync<WebView2DevToolsEvaluationFailedException>(() =>
                DevToolsContext.EvaluateFunctionHandleAsync("(opts) => opts.elem.querySelector('p')", new { aHandle }));
            Assert.Contains("Are you passing a nested JSHandle?", exception.Message);
        }

        [PuppeteerDomFact]
        public async Task ShouldAcceptObjectHandleToUnserializableValue()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => Infinity");
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>(
                "(e) => Object.is(e, Infinity)",
                aHandle));
        }

        [PuppeteerDomFact]
        public async Task ShouldUseTheSameJSWrappers()
        {
            var aHandle = await DevToolsContext.EvaluateFunctionHandleAsync(@"() => {
                globalThis.FOO = 123;
                return window;
            }");
            Assert.Equal(123, await DevToolsContext.EvaluateFunctionAsync<int>(
                "(e) => e.FOO",
                aHandle));
        }
    }
}
