using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Dom
{
    /// <inheritdoc />
    public abstract class DomHandle : IDomHandle, IJSHandle
    {
        /// <inheritdoc />
        public string ClassName { get; private set; }

        /// <inheritdoc />
        public IJSHandle Handle { get; private set; }

        /// <inheritdoc />
        public bool IsDisposed
        {
            get { return Handle.Disposed; }
        }

        bool IJSHandle.Disposed
        {
            get { return Handle?.Disposed == true; }
        }

        RemoteObject IJSHandle.RemoteObject
        {
            get { return Handle?.RemoteObject; }
        }

        internal DomHandle(string className, IJSHandle jSHandle)
        {
            ClassName = className;
            Handle = jSHandle;
        }

        /// <summary>
        /// Disposes the underlying <see cref="Handle"/>.
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
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal async Task<T> EvaluateFunctionInternalAsync<T>(string script, params object[] args)
        {
            JsonElement? result = await Handle.EvaluateFunctionAsync(script, GetArguments(args)).ConfigureAwait(false);
            return ParseJSValueTo<T>(result);
        }

        /// <summary>
        /// Executes a function in browser context
        /// </summary>
        /// <param name="script">Script to be evaluated in browser context</param>
        /// <param name="args">Arguments to pass to script</param>
        /// <remarks>
        /// If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
        /// <see cref="IDomHandle"/> instances can be passed as arguments
        /// </remarks>
        /// <returns>Task which resolves to script return value</returns>
        internal Task EvaluateFunctionInternalAsync(string script, params object[] args)
        {
            return Handle.EvaluateFunctionAsync(script, GetArguments(args));
        }

        internal async Task<T> EvaluateFunctionHandleInternalAsync<T>(string script, params object[] args)
            where T : DomHandle
        {
            var handle = await Handle.EvaluateFunctionHandleAsync(script, GetArguments(args)).ConfigureAwait(false);

            if (handle == null)
            {
                return default;
            }

            return handle.ToDomHandle<T>();
        }

        internal async Task<IEnumerable<T>> GetArray<T>()
            where T : DomHandle
        {
            var properties = await Handle.GetPropertiesAsync().ConfigureAwait(false);
            var result = new List<T>();

            foreach (var jSHandle in properties.Values)
            {
                if (jSHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                var obj = jSHandle.ToDomHandle<T>();

                result.Add(obj);
            }
            return result;
        }

        internal async Task<IEnumerable<string>> GetStringArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<string>();

            foreach (var jsHandle in response.Values)
            {
                if (jsHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                result.Add(jsHandle.RemoteObject.Value.ToString());
            }
            return result;
        }

        internal async Task<IEnumerable<KeyValuePair<string, string>>> GetStringMapArray()
        {
            var response = await Handle.GetPropertiesAsync().ConfigureAwait(false);

            var result = new List<KeyValuePair<string, string>>();

            foreach (var kvp in response)
            {
                var jsHandle = kvp.Value;

                if (jsHandle == null)
                {
                    result.Add(default);

                    continue;
                }

                result.Add(new KeyValuePair<string, string>(kvp.Key, jsHandle.RemoteObject.Value.ToString()));

                await jsHandle.DisposeAsync().ConfigureAwait(false);
            }
            return result;
        }

        private static object[] GetArguments(object[] args)
        {
            return args
                .Select(arg => (arg is IDomHandle domHandle) ? domHandle.Handle : arg)
                .ToArray();
        }

        Task<JsonElement?> IJSHandle.EvaluateFunctionAsync(string script, params object[] args)
        {
            return Handle.EvaluateFunctionAsync<JsonElement?>(script, args);
        }

        async Task<T> IJSHandle.EvaluateFunctionAsync<T>(string script, params object[] args)
        {
            var result = await Handle.EvaluateFunctionAsync<JsonElement?>(script, args).ConfigureAwait(false);;
            return ParseJSValueTo<T>(result);
        }

        Task<IJSHandle> IJSHandle.EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
        {
            return Handle.EvaluateFunctionHandleAsync(pageFunction, args);
        }

        Task<Dictionary<string, IJSHandle>> IJSHandle.GetPropertiesAsync()
        {
            return Handle.GetPropertiesAsync();
        }

        Task<IJSHandle> IJSHandle.GetPropertyAsync(string propertyName)
        {
            return Handle.GetPropertyAsync(propertyName);
        }

        Task<object> IJSHandle.JsonValueAsync()
        {
            return Handle.JsonValueAsync();
        }

        Task<T> IJSHandle.JsonValueAsync<T>()
        {
            return Handle.JsonValueAsync<T>();
        }

        /// <summary>
        /// Parse value of element to designated type
        /// </summary>
        /// <typeparam name="T">Designated type</typeparam>
        /// <param name="value">Value element</param>
        /// <returns>Value of element of designated type</returns>
        /// <exception cref="InvalidOperationException">Value not found (null).</exception>
        /// <exception cref="FormatException">Type not recognized and value was unable to be parsed.</exception>
        public static T ParseJSValueTo<T>(JsonElement? value)
        {
            var returnType = typeof(T);
            if (returnType == typeof(object) || returnType == typeof(JsonElement?))
            {
                return (T)(object)value;
            }
            var valueIsNull = value == null || value.Value.ValueKind == JsonValueKind.Null || value.Value.ValueKind == JsonValueKind.Undefined;
            var nullable = returnType.IsClass;
            if (Nullable.GetUnderlyingType(returnType) != null)
            {
                nullable = true;
                returnType = Nullable.GetUnderlyingType(returnType);
            }

            if (valueIsNull)
            {
                if (nullable)
                {
                    return (T)(object)null;
                }
                throw new InvalidOperationException($"Result of evaluating a script is null when type {typeof(T)} expected.");
            }
            else if (returnType == typeof(string))
            {
                return (T)(object)value.ToString();
            }
            else if (returnType.IsEnum)
            {
                var typeConverter = TypeDescriptor.GetConverter(returnType);
                return (T)typeConverter.ConvertFrom(value.ToString());
            }
            else if (returnType == typeof(int))
            {
                if (int.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(bool))
            {
                if (value.Value.ValueKind == JsonValueKind.True)
                {
                    return (T)(object)true;
                }
                if (value.Value.ValueKind == JsonValueKind.False)
                {
                    return (T)(object)false;
                }

                if (bool.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(double))
            {
                if (double.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(DateTime))
            {
                if (DateTime.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(float))
            {
                if (float.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(decimal))
            {
                if (decimal.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(long))
            {
                if (long.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(short))
            {
                if (short.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }
            else if (returnType == typeof(byte))
            {
                if (byte.TryParse(value.ToString(), out var result))
                { return (T)(object)result; }
            }

            throw new FormatException($"Failed to parse '{value}' to {typeof(T)}");
        }

        /// <summary>
        /// Implicit operator, convert <see cref="DomHandle"/> to <see cref="JSHandle"/>
        /// </summary>
        /// <param name="domHandle">DomHandle</param>
        public static implicit operator JSHandle(DomHandle domHandle) => (JSHandle)domHandle.Handle;
    }
}
