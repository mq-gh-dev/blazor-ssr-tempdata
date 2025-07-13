using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static BlazorSsrTempDataExample9.BlazorSsrRedirectManager;

namespace BlazorSsrTempDataExample9.Components.Shared
{
    /// <summary>
    /// To be used in static SSR pages for displaying status messages set by a previous redirect that set a 
    /// StatusMessage and StatusSeverity in TempData (No need to set component parameters). 
    /// If Message or MessageSeverity are manually passed as component parameters, TempData won't be accessed.
    /// </summary>
    public partial class StatusMessage(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        [Parameter]
        public string? Message { get; set; }

        [Parameter]
        public Severity? MessageSeverity { get; set; }

        protected override void OnInitialized()
        {
            if (httpContextAccessor.HttpContext != null && (string.IsNullOrWhiteSpace(Message) || MessageSeverity is null))
            {
                var tempData = tempDataDictionaryFactory.GetTempData(httpContextAccessor.HttpContext);

                if (string.IsNullOrWhiteSpace(Message))
                {
                    tempData.TryGetValue<string>("StatusMessage", out var message);
                    Message = message;
                }
                if (MessageSeverity is null)
                {
                    tempData.TryGetValue<Severity>("StatusSeverity", out var severity);
                    MessageSeverity = severity;
                }

                // Call this to ensure TempData is cleared after reading
                tempData.Save();
            }


            // In case message is present but severity is not set, infer severity from message content
            if (MessageSeverity == null && !string.IsNullOrWhiteSpace(Message))
            {
                MessageSeverity = Message.Contains("Error", StringComparison.OrdinalIgnoreCase) ?
                    Severity.Error : Severity.Normal;
            }
        }

        private string GetAlertClass()
        {
            return MessageSeverity switch
            {
                Severity.Success => "alert-success",
                Severity.Info => "alert-info",
                Severity.Warning => "alert-warning",
                Severity.Error => "alert-danger",
                _ => "alert-secondary"
            };
        }
    }
}