using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests
{
    public class PuppeteerBrowserContextBaseTest : PuppeteerBrowserBaseTest
    {
        public PuppeteerBrowserContextBaseTest(ITestOutputHelper output) : base(output)
        {
        }

        protected IBrowserContext Context { get; set; }
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Context = await Browser.CreateBrowserContextAsync();
        }
    }
}
