using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics.CodeAnalysis;

namespace BlazorSsrTempDataExample8
{
    /// <summary>
    /// Used for Blazor static SSR redirects that may require data persistence across requests.
    /// </summary>
    public class BlazorSsrRedirectManager
    {
        private readonly ITempDataDictionary? _tempData;
        private readonly NavigationManager _navigationManager;
        private string CurrentPathKeepQuery => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).AbsoluteUri;
        private string CurrentPathWithoutQuery => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);

        public static string StatusMessageKey => "StatusMessage";
        public static string StatusSeverityKey => "StatusSeverity";

        public BlazorSsrRedirectManager(IHttpContextAccessor httpContextAccessor,
            ITempDataDictionaryFactory tempDataDirectoryFactory, NavigationManager navigationManager, ILogger<BlazorSsrRedirectManager> logger)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                logger.LogError("Null HttpContext detected. This class works in SSR only.");
            }

            else
            {
                _tempData = tempDataDirectoryFactory.GetTempData(httpContextAccessor.HttpContext);
            }

            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Redirects to the specified URI and persists data to TempData for the next request.
        /// </summary>
        /// <param name="uri">The destination URI.</param>
        /// <param name="data">The temp data to persist, subject to TempData limitations.</param>
        [DoesNotReturn]
        public void RedirectToWithTempData(string uri, IDictionary<string, object?> data)
        {
            if (_tempData != null)
            {
                // Note the same limitations of TempData in MVC/Razor pages apply, especially
                // cookie-based (cookie size limits, serialization type limits etc). 
                foreach (var item in data)
                {
                    _tempData[item.Key] = item.Value;
                }

                // **Explicitly save the data to the response cookie.**
                // Blazor won't save it automatically (possibly due to NavigationException thrown by the subsequent NavigateTo call).
                // Similarly, readers of the TempData must call Save() too to ensure the data/cookie is cleared.
                _tempData.Save();
            }

            RedirectTo(uri);
        }

        /// <summary>
        /// Redirect to a URI with just a status message and severity in TempData for the next request
        /// </summary>
        /// <param name="uri">The destination URI</param>
        /// <param name="message">The status message, which will have a key of "StatusMessage" in TempData</param>
        /// <param name="severity">The status message severity, which will have a key of "StatusSeverity" in TempData</param>
        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, Severity severity)
        {
            Dictionary<string, object?> data = new() {
                {StatusMessageKey, message },
                {StatusSeverityKey, severity}
                };

            RedirectToWithTempData(uri, data);
        }

        /// <summary>
        /// Redirect to a URI with a status message, severity and additional temp data for the next request
        /// </summary>
        /// <param name="uri">The destination URI</param>
        /// <param name="message">The status message, which will have a key of "StatusMessage" in TempData</param>
        /// <param name="severity">The status message severity, which will have a key of "StatusSeverity" in TempData</param>
        /// <param name="data">The additional temp data to persist, subject to TempData limitations.</param>
        [DoesNotReturn]
        public void RedirectToWithStatusAndTempData(string uri, string message, Severity severity, IDictionary<string, object?> data)
        {
            data["StatusMessage"] = message;
            data["StatusSeverity"] = severity;
            RedirectToWithTempData(uri, data);
        }

        /// <summary>
        /// Redirect to the current URI
        /// </summary>
        /// <param name="keepQuery"> If true, the query parameters will be kept in the redirect URI; otherwise, they will be removed.</param>
        [DoesNotReturn]
        public void RedirectToCurrentPage(bool keepQuery = true) =>
            RedirectTo(keepQuery ? CurrentPathKeepQuery : CurrentPathWithoutQuery);

        /// <summary>
        /// Redirects to the current URI and persists data to TempData for the next request.
        /// </summary>
        /// <param name="data">The temp data to persist, subject to TempData limitations.</param>
        /// <param name="keepQuery"> If true, the query parameters will be kept in the redirect URI; otherwise, they will be removed.</param>
        [DoesNotReturn]
        public void RedirectToCurrentPageWithTempData(IDictionary<string, object?> data, bool keepQuery = true)
            => RedirectToWithTempData(keepQuery ? CurrentPathKeepQuery : CurrentPathWithoutQuery, data);

        /// <summary>
        /// Redirect to the current URI with a status message and severity in Tempdata for the next request
        /// </summary>
        /// <param name="message">The status message, which will have a key of "StatusMessage" in TempData</param>
        /// <param name="severity">The status message severity, which will have a key of "StatusSeverity" in TempData</param>
        /// <param name="keepQuery"> If true, the query parameters will be kept in the redirect URI; otherwise, they will be removed.</param>
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, Severity severity, bool keepQuery = true)
            => RedirectToWithStatus(keepQuery ? CurrentPathKeepQuery : CurrentPathWithoutQuery, message, severity);

        /// <summary>
        /// Redirect to the current URI with a status message, severity, and additional TempData for the next request
        /// </summary>
        /// <param name="message">The status message, which will have a key of "StatusMessage" in TempData</param>
        /// <param name="severity">The status message severity, which will have a key of "StatusSeverity" in TempData</param>
        /// <param name="data">The additional temp data to persist, subject to TempData limitations.</param>
        /// <param name="keepQuery"> If true, the query parameters will be kept in the redirect URI; otherwise, they will be removed.</param>
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatusAndTempData(string message, Severity severity, IDictionary<string, object?> data, bool keepQuery = true)
            => RedirectToWithStatusAndTempData(keepQuery ? CurrentPathKeepQuery : CurrentPathWithoutQuery, message, severity, data);

        /// <summary>
        /// Redirect to a specified URI. 
        /// </summary>
        /// <param name="uri">The destination URI</param>
        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";

            // Prevent open redirects.
            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = _navigationManager.ToBaseRelativePath(uri);
            }

            // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
            // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
            _navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(BlazorSsrRedirectManager)} can only be used during static rendering.");
        }

        /// <summary>
        /// Redirect to a specified URI with specified query parameters.
        /// </summary>
        /// <param name="uri">The destination URI, WITHOUT query parameters (Note: any existing query parameters will be removed from this URI!)</param>
        /// <param name="queryParameters">The query parameters</param>
        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        public enum Severity
        {
            Normal,
            Info,
            Success,
            Warning,
            Error
        }

    }
}
