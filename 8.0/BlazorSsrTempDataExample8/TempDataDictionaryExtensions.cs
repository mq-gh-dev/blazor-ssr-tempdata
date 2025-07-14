using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BlazorSsrTempDataExample8
{
    public static class TempDataDictionaryExtensions
    {
        /// <summary>
        /// Attempts to get a value from TempData and cast it to type T.
        /// Returns true if the key exists in TempData, regardless of whether the type conversion succeeds.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <param name="tempData">The TempData dictionary.</param>
        /// <param name="key">The key of the TempData, case-sensitive.</param>
        /// <param name="value">When this method returns, contains the value cast to type T if the conversion succeeded; otherwise, the default value for the type.</param>
        /// <returns>true if the key exists in TempData; otherwise, false.</returns>
        /// <remarks>
        /// If the key exists but the value cannot be cast to type T, the method returns true with value set to default(T).
        /// This behavior matches the underlying ITempDataDictionary.TryGetValue semantics where the return value indicates key existence, not type compatibility.
        /// </remarks>
        public static bool TryGetValue<T>(this ITempDataDictionary tempData, string key, out T? value)
        {
            // Use the built-in TryGetValue which marks the item for deletion
            if (tempData.TryGetValue(key, out var rawValue))
            {
                // Key exists - we will return true regardless of type conversion

                // Handle null values
                if (rawValue == null)
                {
                    value = default;
                    return true;
                }

                // Try to cast the value to the requested type
                if (rawValue is T typedValue)
                {
                    value = typedValue;
                    return true;
                }

                // Special handling for enums - TempData stores them as integers
                var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                if (underlyingType.IsEnum && rawValue is int intValue)
                {
                    try
                    {
                        var enumValue = Enum.ToObject(underlyingType, intValue);
                        value = (T)(object)enumValue;
                        return true;
                    }
                    catch
                    {
                        // Invalid enum value - return default but still true
                        value = default;
                        return true;
                    }
                }

                // Type mismatch - return default but still true (key exists)
                value = default;
                return true;
            }

            // The key was not found
            value = default;
            return false;
        }
    }

}
