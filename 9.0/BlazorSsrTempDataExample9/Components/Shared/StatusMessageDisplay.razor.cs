using static BlazorSsrTempDataExample9.BlazorSsrRedirectManager;

namespace BlazorSsrTempDataExample9.Components.Shared
{
    /// <summary>
    /// To be used in static SSR pages for displaying status messages set by a previous redirect that set a 
    /// StatusMessage and StatusSeverity in TempData 
    /// </summary>
    public partial class StatusMessageDisplay(TempDataAccessor tempDataAccessor)
    {
        private string? StatusMessage { get; set; }
        private Severity? StatusSeverity { get; set; }

        protected override void OnInitialized()
        {
            tempDataAccessor
                .TryGet<string?>("StatusMessage", out var msg, out _)
                .TryGet<Severity?>("StatusSeverity", out var severity, out _)
                .Save();

            StatusMessage = msg;
            StatusSeverity = severity;

            if (StatusSeverity == null && !string.IsNullOrWhiteSpace(StatusMessage))
            {
                // Try to infer severity if message is present but severity is null
                StatusSeverity = StatusMessage.Contains("Error", StringComparison.OrdinalIgnoreCase) ?
                    Severity.Error : Severity.Normal;
            }
        }

        private string GetAlertClass()
        {
            return StatusSeverity switch
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