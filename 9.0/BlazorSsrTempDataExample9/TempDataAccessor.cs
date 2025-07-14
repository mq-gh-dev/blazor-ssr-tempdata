using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BlazorSsrTempDataExample9
{
    /// <summary>
    /// Provides a fluent API for retrieving multiple values from TempData. You should call .Save() at the end of the chain to ensure cleanup.
    /// </summary>
    public class TempDataAccessor
    {
        private readonly ITempDataDictionary? _tempData;
        private bool _hasAnyData = false;

        public TempDataAccessor(IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataDirectoryFactory, ILogger<TempDataAccessor> logger)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                logger.LogError("Null HttpContext detected. This class works in SSR only.");
            }
            else
            {
                _tempData = tempDataDirectoryFactory.GetTempData(httpContextAccessor.HttpContext);
            }
        }

        /// <summary>
        /// Tries to get a value from TempData with type safety and success flag.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key to look up in TempData.</param>
        /// <param name="value">When this method returns, contains the value from TempData cast to type T if the conversion succeeded; otherwise, the default value.</param>
        /// <param name="hasData">When this method returns, contains true if the key exists in TempData; otherwise, false.</param>
        /// <param name="defaultValue">The default value to use if the key is not found. Defaults to default(T).</param>
        /// <returns>The current TempDataAccessor instance for method chaining. You should call Save() at the end of the chain.</returns>
        /// <remarks>
        /// If the key exists but the value cannot be cast to type T, hasData will be true but value will be default(T).
        /// This behavior ensures hasData accurately reflects whether the key exists in TempData.
        /// </remarks>
        public TempDataAccessor TryGet<T>(string key, out T? value, out bool hasData, T? defaultValue = default)
        {
            if (_tempData != null)
            {
                hasData = _tempData.TryGetValue<T>(key, out value);

                if (!hasData)
                {
                    value = defaultValue;
                }
                _hasAnyData |= hasData;
            }
            else
            {
                hasData = false;
                value = defaultValue;
            }
            return this;
        }

        /// <summary>
        /// Gets whether any data was successfully retrieved.
        /// </summary>
        public bool HasAnyData => _hasAnyData;

        /// <summary>
        /// Saves TempData to clear retrieved values.
        /// </summary>
        public void Save()
        {
            _tempData?.Save();
        }
    }
}