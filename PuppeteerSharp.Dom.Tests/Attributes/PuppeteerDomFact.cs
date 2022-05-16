using Xunit;

namespace PuppeteerSharp.Dom.Tests.Attributes
{
    /// <summary>
    /// WebView2Context Fact
    /// </summary>
    public class PuppeteerDomFact : FactAttribute
    {
        /// <summary>
        /// Creates a new <seealso cref="PuppeteerDomFact"/>
        /// </summary>
        public PuppeteerDomFact()
        {
            Timeout = System.Diagnostics.Debugger.IsAttached ? TestConstants.DebuggerAttachedTestTimeout : TestConstants.DefaultTestTimeout;
        }
    }
}
