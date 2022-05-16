using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PuppeteerSharp.Dom
{
    /// <summary>
    /// Page Extensions 
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Creates a Html Element from the <see cref="JSHandle"/>
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="jsHandle">JSHandle</param>
        /// <returns>Element or null</returns>
        public static async Task<T> ToDomHandleAsync<T>(this JSHandle jsHandle)
            where T : DomHandle
        {
            if (jsHandle == null)
            {
                return default;
            }

            var remoteObject = jsHandle.RemoteObject;

            // Types like FileList are of SubType other
            if (remoteObject.Type == Messaging.RemoteObjectType.Object &&
                (remoteObject.Subtype == Messaging.RemoteObjectSubtype.Node || remoteObject.Subtype == Messaging.RemoteObjectSubtype.Other))
            {
                // If Puppeteer addds RemoteObject.ClassName we can skip this call.
                var className = await jsHandle.EvaluateFunctionAsync<string>("e => e[Symbol.toStringTag]").ConfigureAwait(false);

                return HtmlObjectFactory.CreateObject<T>(className, jsHandle);
            }

            return default;
        }

        /// <summary>
        /// Passes an expression to the <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/>, returns a <see cref="Task"/>, then <see cref="ExecutionContext.EvaluateExpressionHandleAsync(string)"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var frame = devToolsContext.MainFrame;
        /// var handle = await Page.EvaluateExpressionHandleAsync<HtmlElement>("button");
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>Resolves to the return value of <paramref name="script"/></returns>
        /// <param name="script">Expression to be evaluated in the <seealso cref="ExecutionContext"/></param>
        public static async Task<T> EvaluateExpressionHandleAsync<T>(this Page page, string script)
            where T : DomHandle
        {
            var handle = await page.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

            return await handle.ToDomHandleAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <returns>Created element</returns>
        public static async Task<T> CreateHtmlElementAsync<T>(this Page page, string tagName)
            where T : HtmlElement
        {
            var handle = await page.EvaluateFunctionHandleAsync(
                @"(tagName) => {
                    return document.createElement(tagName);
                }",
                tagName).ConfigureAwait(false);

            return await handle.ToDomHandleAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <param name="id">element id</param>
        /// <returns>Created element</returns>
        public static async Task<T> CreateHtmlElementAsync<T>(this Page page, string tagName, string id)
            where T : HtmlElement
        {
            var handle = await page.EvaluateFunctionHandleAsync(
                @"(tagName, id) => {
                    let e = document.createElement(tagName);
                    e.id = id;
                    return e;
                }",
                tagName,
                id).ConfigureAwait(false);

            return await handle.ToDomHandleAsync<T>().ConfigureAwait(false);
        }

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

            return await handle.ToDomHandleAsync<T>().ConfigureAwait(false);
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
        public static async Task<T[]> QuerySelectorAllAsync<T>(this Page page, string querySelector)
            where T : Element
        {
            var elements = await page.QuerySelectorAllAsync(querySelector).ConfigureAwait(false);

            var list = new List<T>();

            foreach(var element in elements)
            {
                list.Add(await element.ToDomHandleAsync<T>());
            }

            return list.ToArray();
        }
    }
}
