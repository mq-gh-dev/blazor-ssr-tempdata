using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics.CodeAnalysis;

namespace BlazorSsrTempDataExample9
{
    public static class TempDataDictionaryExtensions
    {
        /// <summary>
        /// Helper extension method to directly retrieve TempData as type T without additional casting in code.
        /// </summary>
        /// <param name="key">The key of the TempData, case-sensitive.</param>
        public static bool TryGetValue<T>(this ITempDataDictionary tempData, string key, [MaybeNullWhen(false)] out T value)
        {
            // Use the built-in TryGetValue which marks the item for deletion
            if (tempData.TryGetValue(key, out var rawValue) && rawValue != null)
            {
                // Try to cast the value to the requested type
                if (rawValue is T typedValue)
                {
                    value = typedValue;
                    return true;
                }

                // Special handling for enums - TempData stores them as integers
                if (typeof(T).IsEnum && rawValue is int intValue)
                {
                    try
                    {
                        value = (T)Enum.ToObject(typeof(T), intValue);
                        return true;
                    }
                    catch
                    {
                        // Invalid enum value
                    }
                }
            }

            // The key was not found or the value could not be cast to type T
            value = default;
            return false;
        }
    }

}
