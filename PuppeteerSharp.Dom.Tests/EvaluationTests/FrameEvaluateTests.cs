using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.EvaluationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldHaveDifferentExecutionContexts()
        {
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, DevToolsContext.Frames.Count());

            var frame1 = DevToolsContext.MainFrame;
            var frame2 = DevToolsContext.FirstChildFrame();

            await frame1.EvaluateExpressionAsync("window.FOO = 'foo'");
            await frame2.EvaluateExpressionAsync("window.FOO = 'bar'");

            Assert.Equal("foo", await frame1.EvaluateExpressionAsync<string>("window.FOO"));
            Assert.Equal("bar", await frame2.EvaluateExpressionAsync<string>("window.FOO"));
        }

        [PuppeteerDomFact]
        public async Task ShouldExecuteAfterCrossSiteNavigation()
        {
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.EmptyPage);
            var mainFrame = DevToolsContext.MainFrame;
            Assert.Contains("devtools.test", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));

            await WebView.CoreWebView2.NavigateToAsync(TestConstants.CrossProcessHttpPrefix + "/empty.html");
            Assert.Contains("empty.html", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));
        }
    }
}
