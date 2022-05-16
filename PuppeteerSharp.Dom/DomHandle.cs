using Microsoft.Extensions.Logging;
using PuppeteerSharp.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp.Dom
{
    public class DomHandle : IAsyncDisposable
    {
        /// <summary>
        /// Class Name
        /// </summary>
        public string ClassName { get; private set; }
        /// <summary>
        /// Javascript Handle
        /// </summary>
        public JSHandle Handle { get; private set; }

        internal DomHandle(string className, JSHandle jSHandle)
        {
            ClassName = className;
            Handle = jSHandle;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RemoteObject"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed
        {
            get { return Handle.Disposed; }
        }

        /// <summary>
        /// Disposes the Handle. It will mark the JSHandle as disposed and release the <see cref="RemoteObject.RemoteObj"/>
        /// </summary>
        /// <returns>The async.</returns>
        public async ValueTask DisposeAsync()
        {
            if (IsDisposed)
            {
                return;
            }

            GC.SuppressFinalize(this);

            await Handle.DisposeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <typeparam name="T">The type to deserialize the result to</typeparam>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="RemoteObject"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal Task<T> EvaluateFunctionInternalAsync<T>(string script, params object[] args) => Handle.EvaluateFunctionAsync<T>(script, args);

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="RemoteObject"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal Task EvaluateFunctionInternalAsync(string script, params object[] args) => Handle.EvaluateFunctionAsync<object>(script, args);

        internal async Task<T> EvaluateFunctionHandleInternalAsync<T>(string script, params object[] args)
            where T : DomHandle
        {
            var handle = await Handle.EvaluateFunctionHandleAsync(script, args).ConfigureAwait(false);

            if (handle == null)
            {
                return default;
            }

            // If Puppeteer addds RemoteObject.ClassName we can skip this call.
            var result = await handle.EvaluateFunctionAsync("e => e[Symbol.toStringTag]").ConfigureAwait(false);
            var className = result.ToString();

            var domHandle = HtmlObjectFactory.CreateObject<T>(className, handle);

            return domHandle;
        }

        internal async Task<IEnumerable<T>> GetArray<T>()
            where T : DomHandle
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<T>();

            foreach (var property in response)
            {
                //if (property.Enumerable == false)
                //{
                //    continue;
                //}

                if (property.Value.RemoteObject.Value == null)
                {
                    result.Add(default);

                    continue;
                }

                var obj = (T)HtmlObjectFactory.CreateObject<T>("", property.Value);

                result.Add(obj);
            }
            return result;
        }

        internal async Task<IEnumerable<string>> GetStringArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<string>();

            foreach (var property in response.Values)
            {
                if (property.RemoteObject.Value == null)
                {
                    result.Add(default);

                    continue;
                }

                result.Add(property.RemoteObject.Value.ToString());
            }
            return result;
        }

        internal async Task<IEnumerable<KeyValuePair<string, string>>> GetStringMapArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<KeyValuePair<string, string>>();

            foreach (var property in response)
            {
                //if (property.Enumerable == false)
                //{
                //    continue;
                //}

                var val = property.Value.RemoteObject.Value;

                if (val == null)
                {
                    result.Add(default);

                    continue;
                }

                var kvp = new KeyValuePair<string, string>(property.Key, val.ToString());

                result.Add(kvp);
            }
            return result;
        }
    }
}
