using System.Threading.Tasks;
using PuppeteerSharp.Dom.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.InputTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InputTests : PuppeteerPageBaseTest
    {
        public InputTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerDomFact]
        public async Task ShouldUploadTheFile()
        {
            await WebView.CoreWebView2.NavigateToAsync(TestConstants.ServerUrl + "/input/fileupload.html");
            var filePath = TestConstants.FileToUpload;

            var input = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("input");
            await input.UploadFileAsync(filePath);

            var fileName = await DevToolsContext.EvaluateFunctionAsync<string>("e => e.files[0].name", input);

            Assert.Equal("file-to-upload.txt", fileName);

            var fileContents = await DevToolsContext.EvaluateFunctionAsync<string>(@"e => {
                const reader = new FileReader();
                const promise = new Promise(fulfill => reader.onload = fulfill);
                reader.readAsText(e.files[0]);
                return promise.then(() => reader.result);
            }", input);

            Assert.Equal("contents of the file", fileContents);
        }
    }
}
