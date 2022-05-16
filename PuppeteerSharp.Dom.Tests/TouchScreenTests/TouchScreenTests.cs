using System.Threading.Tasks;
using WebView2.DevTools.Dom.Mobile;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.TouchScreenTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TouchScreenTests : PuppeteerPageBaseTest
    {
        public TouchScreenTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldTapTheButton()
        {
            var iPhone6 = Emulate.Device(DeviceName.IPhone6);

            await DevToolsContext.EmulateAsync(iPhone6);
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.TapAsync("button");

            var actual = await DevToolsContext.EvaluateExpressionAsync<string>("result");

            Assert.Equal("Clicked", actual);
        }

        [PuppeteerDomFact]
        public async Task ShouldReportTouches()
        {
            var iPhone6 = Emulate.Device(DeviceName.IPhone6);

            await DevToolsContext.EmulateAsync(iPhone6);

            await WebView.CoreWebView2.NavigateToAsync(TestConstants.ServerUrl + "/input/touches.html");

            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("button");
            await button.TapAsync();

            var actual = await DevToolsContext.EvaluateExpressionAsync<string[]>("getResult()");

            Assert.Equal(new string[] {
                "Touchstart: 0",
                "Touchend: 0"
            }, actual);
        }
    }
}
