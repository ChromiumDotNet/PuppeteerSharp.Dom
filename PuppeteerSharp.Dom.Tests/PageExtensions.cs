using System.Linq;

namespace PuppeteerSharp.Dom.Tests
{
    public static class PageExtensions
    {
        public static Frame FirstChildFrame(this Page page) => page.Frames.FirstOrDefault(f => f.ParentFrame == page.MainFrame);
    }
}
