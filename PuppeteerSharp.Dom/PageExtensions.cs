using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Dom
{
    /// <summary>
    /// Page Extensions 
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Creates a Html Element from the <see cref="IJSHandle"/>
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="jsHandle">JSHandle</param>
        /// <returns>Element or null</returns>
        public static T ToDomHandle<T>(this IJSHandle jsHandle)
            where T : DomHandle
        {
            if (jsHandle == null)
            {
                return default;
            }

            var remoteObject = jsHandle.RemoteObject;

            // Types like FileList are of SubType other
            if (remoteObject.Type == Messaging.RemoteObjectType.Object &&
                (remoteObject.Subtype == Messaging.RemoteObjectSubtype.Node ||
                remoteObject.Subtype == Messaging.RemoteObjectSubtype.Array ||
                remoteObject.Subtype == Messaging.RemoteObjectSubtype.Other))
            {
                return HtmlObjectFactory.CreateObject<T>(remoteObject.ClassName, jsHandle);
            }

            return default;
        }

        /// <summary>
        /// Passes an expression to the <see cref="IPage.EvaluateExpressionHandleAsync(string)"/>, returns a <see cref="Task"/>, then <see cref="IPage.EvaluateExpressionHandleAsync(string)"/> would wait for the <see cref="Task"/> to resolve and return its value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var handle = await Page.EvaluateExpressionHandleAsync<HtmlElement>("button");
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="page">Page</param>
        /// <param name="script">Expression to be evaluated in the <seealso cref="ExecutionContext"/></param>
        /// <returns>Resolves to the return value of <paramref name="script"/></returns>
        public static async Task<T> EvaluateExpressionHandleAsync<T>(this IPage page, string script)
            where T : DomHandle
        {
            var handle = await page.EvaluateExpressionHandleAsync(script).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="page">Page</param>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <returns>Created element</returns>
        public static async Task<T> CreateHtmlElementAsync<T>(this IPage page, string tagName)
            where T : HtmlElement
        {
            var handle = await page.EvaluateFunctionHandleAsync(
                @"(tagName) => {
                    return document.createElement(tagName);
                }",
                tagName).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// Creates the HTML element specified
        /// </summary>
        /// <typeparam name="T">HtmlElementType</typeparam>
        /// <param name="page">Page</param>
        /// <param name="tagName">
        /// A string that specifies the type of element to be created.
        /// The nodeName of the created element is initialized with the
        /// value of tagName. Don't use qualified names (like "html:a")
        /// with this method.
        /// </param>
        /// <param name="id">element id</param>
        /// <returns>Created element</returns>
        public static async Task<T> CreateHtmlElementAsync<T>(this IPage page, string tagName, string id)
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

            return handle.ToDomHandle<T>();
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
        public static Task<T> QuerySelectorAsync<T>(this IPage page, string querySelector)
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
        public static async Task<T> QuerySelectorAsync<T>(this IFrame frame, string querySelector)
            where T : Element
        {
            var handle = await frame.QuerySelectorAsync(querySelector).ConfigureAwait(false);

            return handle.ToDomHandle<T>();
        }

        /// <summary>
        /// Evaluates the XPath expression
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Element"/> or derived type</typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="expression">Expression to evaluate <see href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"/></param>
        /// <returns>Task which resolves to an array of <typeparamref name="T"/> </returns>
        /// <seealso cref="IFrame.XPathAsync(string)"/>
        public static async Task<T[]> XPathAsync<T>(this IPage page, string expression)
            where T : Element
        {
            var elements = await page.XPathAsync(expression).ConfigureAwait(false);

            var list = new List<T>();

            foreach (var element in elements)
            {
                list.Add(element.ToDomHandle<T>());
            }

            return list.ToArray();
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
        /// <seealso cref="IFrame.QuerySelectorAsync(string)"/>
        public static Task<HtmlElement> QuerySelectorAsync(this IPage page, string querySelector)
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
        public static Task<HtmlElement> QuerySelectorAsync(this IFrame frame, string querySelector)
            => frame.QuerySelectorAsync<HtmlElement>(querySelector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public static Task<HtmlElement[]> QuerySelectorAllAsync(this IPage page, string querySelector)
            => page.QuerySelectorAllAsync<HtmlElement>(querySelector);

        /// <summary>
        /// Runs <c>document.querySelectorAll</c> within the page. If no elements match the selector, the return value resolve to <see cref="Array.Empty{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="querySelector">A selector to query page for</param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        /// <seealso cref="Frame.QuerySelectorAllAsync(string)"/>
        public static async Task<T[]> QuerySelectorAllAsync<T>(this IPage page, string querySelector)
            where T : Element
        {
            var elements = await page.QuerySelectorAllAsync(querySelector).ConfigureAwait(false);

            var list = new List<T>();

            foreach(var element in elements)
            {
                list.Add(element.ToDomHandle<T>());
            }

            return list.ToArray();
        }

        /// <summary>
        /// Waits for a selector to be added to the DOM
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="selector">A selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task that resolves when element specified by selector string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.</returns>
        /// <seealso cref="IPage.WaitForXPathAsync(string, WaitForSelectorOptions)"/>
        public static async Task<T> WaitForSelectorAsync<T>(this IPage page, string selector, WaitForSelectorOptions options = null)
            where T : Element
        {
            var element = await page.WaitForSelectorAsync(selector, options).ConfigureAwait(false);

            return element.ToDomHandle<T>();
        }

        /// <summary>
        /// Waits for a xpath selector to be added to the DOM
        /// </summary>
        /// <typeparam name="T">Type derived from <see cref="Element"/></typeparam>
        /// <param name="page">Puppeteer Page</param>
        /// <param name="xpath">A xpath selector of an element to wait for</param>
        /// <param name="options">Optional waiting parameters</param>
        /// <returns>A task which resolves when element specified by xpath string is added to DOM.
        /// Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.</returns>
        /// <seealso cref="IPage.WaitForSelectorAsync(string, WaitForSelectorOptions)"/>
        public static async Task<T> WaitForXPathAsync<T>(this IPage page, string xpath, WaitForSelectorOptions options = null)
            where T : Element
        {
            var element = await page.WaitForXPathAsync(xpath, options).ConfigureAwait(false);

            return element.ToDomHandle<T>();
        }
    }
}
