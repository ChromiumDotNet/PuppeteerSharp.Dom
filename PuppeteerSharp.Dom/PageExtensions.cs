using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Dom
{
    /// <summary>
    /// Page Extensions 
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HtmlElement"/> or derived type</typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="HtmlElement"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public static Task<T> QuerySelectorAsync<T>(this Page page, string querySelector)
            where T : Element
        {
            return page.MainFrame.QuerySelectorAsync<T>(querySelector);
        }

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HtmlElement"/> or derived type</typeparam>
        /// <param name="frame">Puppeteer frame</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="HtmlElement"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public static async Task<T> QuerySelectorAsync<T>(this Frame frame, string querySelector)
            where T : Element
        {
            var handle = await frame.QuerySelectorAsync(querySelector);

            // If Puppeteer addds RemoteObject.ClassName we can skip this call.
            var result = await handle.EvaluateFunctionAsync("e => e[Symbol.toStringTag]").ConfigureAwait(false);
            var className = result.ToString();

            var domHandle = HtmlObjectFactory.CreateObject<T>(className, handle);

            return domHandle;
        }

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="Element"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public static Task<HtmlElement> QuerySelectorAsync(this Page page, string querySelector)
            => page.QuerySelectorAsync<HtmlElement>(querySelector);

        /// <summary>
        /// The method runs <c>document.querySelector</c> within the page. If no element matches the selector, the return value resolve to <c>null</c>.
        /// </summary>
        /// <param name="frame">Puppeteer Frame</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to <see cref="Element"/> pointing to the frame element</returns>
        /// <remarks>
        /// Shortcut for <c>page.MainFrame.QuerySelectorAsync(selector)</c>
        /// </remarks>
        /// <seealso cref="Frame.QuerySelectorAsync(string)"/>
        public static Task<HtmlElement> QuerySelectorAsync(this Frame frame, string querySelector)
            => frame.QuerySelectorAsync<HtmlElement>(querySelector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public static Task<HtmlElement[]> QuerySelectorAllAsync(this Page page, string querySelector)
            => page.QuerySelectorAllAsync<HtmlElement>(querySelector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public static Task<T[]> QuerySelectorAllAsync<T>(this Page page, string querySelector)
            where T : Element
        {
            throw new NotImplementedException();
        }
    }
}
