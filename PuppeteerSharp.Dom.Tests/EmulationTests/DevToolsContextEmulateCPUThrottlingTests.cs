using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEmulateCPUThrottlingTests : PuppeteerPageBaseTest
    {
        public DevToolsContextEmulateCPUThrottlingTests(ITestOutputHelper output) : base(output)
        {
        }

        public async Task ShouldChangeTheCPUThrottlingRateSuccessfully()
        {
            await DevToolsContext.EmulateCPUThrottlingAsync(100);
            await DevToolsContext.EmulateCPUThrottlingAsync();
        }
    }
}
