using System.Linq;

namespace PuppeteerSharp.Dom.Tests
{
    public static class PageExtensions
    {
        public static IFrame FirstChildFrame(this IPage page) => page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
    }
}
