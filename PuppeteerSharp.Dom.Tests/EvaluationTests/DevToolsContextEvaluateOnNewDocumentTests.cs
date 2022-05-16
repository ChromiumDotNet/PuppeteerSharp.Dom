using System.Threading.Tasks;
using WebView2.DevTools.Dom;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.EvaluationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public DevToolsContextEvaluateOnNewDocumentTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.result"));
        }

        [PuppeteerDomFact]
        public async Task ShouldWorkWithCSP()
        {
            //Server.SetCSP("/empty.html", "script-src " + TestConstants.ServerUrl);
            await DevToolsContext.EvaluateFunctionOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.EmptyPage);
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.injected"));

            // Make sure CSP works.
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.e = 10;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Equal(10, await DevToolsContext.EvaluateExpressionAsync<int>("window.e"));
        }

        [PuppeteerDomFact]
        public async Task ShouldWorkWithExpressions()
        {
            await DevToolsContext.EvaluateExpressionOnNewDocumentAsync("window.injected = 123;");
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await DevToolsContext.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
