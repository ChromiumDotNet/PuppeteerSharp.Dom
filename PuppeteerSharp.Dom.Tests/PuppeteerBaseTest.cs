using PuppeteerSharp.Dom.TestServer;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PuppeteerSharp.Dom.Tests
{
    public class PuppeteerBaseTest
    {
        protected string BaseDirectory { get; set; }

        protected SimpleServer Server => PuppeteerLoaderFixture.Server;
        protected SimpleServer HttpsServer => PuppeteerLoaderFixture.HttpsServer;

        public PuppeteerBaseTest(ITestOutputHelper output)
        {
            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "workspace");
            var dirInfo = new DirectoryInfo(BaseDirectory);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            Initialize();
        }

        protected void Initialize()
        {
            Server.Reset();
            HttpsServer.Reset();
        }

        protected static Task<JsonElement?> WaitEvent(CDPSession emitter, string eventName)
        {
            var completion = new TaskCompletionSource<JsonElement?>();
            void handler(object sender, MessageEventArgs e)
            {
                if (e.MessageID != eventName)
                {
                    return;
                }
                emitter.MessageReceived -= handler;
                completion.SetResult(e.MessageData);
            }

            emitter.MessageReceived += handler;
            return completion.Task;
        }

        protected static Task WaitForBrowserDisconnect(Browser browser)
        {
            var disconnectedTask = new TaskCompletionSource<bool>();
            browser.Disconnected += (_, _) => disconnectedTask.TrySetResult(true);
            return disconnectedTask.Task;
        }
    }
}
