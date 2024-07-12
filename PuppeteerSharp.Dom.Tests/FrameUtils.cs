using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PuppeteerSharp.Dom.Tests
{
    public static class FrameUtils
    {
        public static async Task<IFrame> AttachFrameAsync(IPage page, string frameId, string url)
        {
            var handle = await page.EvaluateFunctionHandleAsync(@" async (frameId, url) => {
              const frame = document.createElement('iframe');
              frame.src = url;
              frame.id = frameId;
              document.body.appendChild(frame);
              await new Promise(x => frame.onload = x);
              return frame
            }", frameId, url) as ElementHandle;
            return await handle.ContentFrameAsync();
        }

        public static async Task DetachFrameAsync(Page page, string frameId)
        {
            await page.EvaluateFunctionAsync(@"function detachFrame(frameId) {
              const frame = document.getElementById(frameId);
              frame.remove();
            }", frameId);
        }

        public static IEnumerable<string> DumpFrames(IFrame frame, string indentation = "")
        {
            var description = indentation + Regex.Replace(frame.Url, @":\d{4}", ":<PORT>");
#pragma warning disable CS0618 // Type or member is obsolete
            var name = frame.Name;
#pragma warning restore CS0618 // Type or member is obsolete
            if (!string.IsNullOrEmpty(name))
            {
                description += $" ({name})";
            }
            var result = new List<string>() { description };
            foreach (var child in frame.ChildFrames)
            {
                result.AddRange(DumpFrames(child, "    " + indentation));
            }

            return result;
        }

        internal static async Task NavigateFrameAsync(Page page, string frameId, string url)
        {
            await page.EvaluateFunctionAsync(@"function navigateFrame(frameId, url) {
              const frame = document.getElementById(frameId);
              frame.src = url;
              return new Promise(x => frame.onload = x);
            }", frameId, url);
        }
    }
}
