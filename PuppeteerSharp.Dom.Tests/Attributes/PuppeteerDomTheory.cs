using Xunit;

namespace PuppeteerSharp.Dom.Tests.Attributes
{
    /// <summary>
    /// WebView2Context Theory
    /// </summary>
    public class PuppeteerDomTheory : TheoryAttribute
    {
        /// <summary>
        /// Creates a new <seealso cref="PuppeteerDomTheory"/>
        /// </summary>
        public PuppeteerDomTheory()
        {
            Timeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultTestTimeout;
        }
    }
}
