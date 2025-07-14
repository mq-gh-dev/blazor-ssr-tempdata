using Microsoft.AspNetCore.Components;
using static BlazorSsrTempDataExample8.BlazorSsrRedirectManager;

namespace BlazorSsrTempDataExample8.Components.Shared
{
    /// <summary>
    /// To be used in static SSR pages for displaying status messages set by a previous redirect that set a 
    /// StatusMessage and StatusSeverity in TempData 
    /// </summary>
    public partial class StatusMessageDisplay()
    {
        [Inject]
        private TempDataAccessor TempDataAccessor { get; set; } = default!;
        private string? StatusMessage { get; set; }
        private Severity? StatusSeverity { get; set; }

        protected override void OnInitialized()
        {
            TempDataAccessor
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