using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using static BlazorSsrTempDataExample9.BlazorSsrRedirectManager;
namespace BlazorSsrTempDataExample9.Components.Pages;

public partial class Home(BlazorSsrRedirectManager redirectManager,
    ITempDataDictionaryFactory tempDataDictionaryFactory)
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private WeatherFormModel InputModel { get; set; } = new();

    private WeatherFormModel DisplayModel { get; set; } = new();
    private bool HasWeatherData { get; set; }


    protected override void OnInitialized()
    {
        if (HttpMethods.IsGet(HttpContext.Request.Method))
        {
            // Access TempData to retrieve previously saved weather data
            var tempData = tempDataDictionaryFactory.GetTempData(HttpContext);

            var hasWeatherDesc = tempData.TryGetValue<string>(nameof(InputModel.Description), out var desc);
            var hasDay = tempData.TryGetValue<DayOfWeek>(nameof(InputModel.SelectedDay), out var day);

            DisplayModel.Description = hasWeatherDesc ? desc! : string.Empty;
            DisplayModel.SelectedDay = hasDay ? day : default;
            HasWeatherData = hasWeatherDesc || hasDay;

            // Call this to ensure TempData is cleared after reading
            tempData.Save();
        }
    }

    private void HandleValidSubmit()
    {
        switch (InputModel.SubmitType)
        {
            case SubmitType.TempData:
                TestRedirectWithTempData();
                break;
            case SubmitType.Status:
                TestRedirectWithStatus();
                break;
            case SubmitType.StatusAndTempData:
                TestRedirectWithStatusAndTempData();
                break;
        }
    }

    private void TestRedirectWithTempData()
    {
        redirectManager.RedirectToCurrentPageWithTempData(
            new Dictionary<string, object?>
            {
                { nameof(InputModel.Description), InputModel.Description },
                { nameof(InputModel.SelectedDay), InputModel.SelectedDay }
            });
    }

    private void TestRedirectWithStatus()
    {
        redirectManager.RedirectToCurrentPageWithStatus("A simulated error occurred on the server!", Severity.Error);
    }

    private void TestRedirectWithStatusAndTempData()
    {
        redirectManager.RedirectToWithStatusAndTempData("/Forecast",
            "Weather data received successfully on a different page!",
            Severity.Success,
            new Dictionary<string, object?>
            {
                { nameof(InputModel.Description), InputModel.Description },
                { nameof(InputModel.SelectedDay), InputModel.SelectedDay }
            });
    }

    public class WeatherFormModel
    {
        [Required(ErrorMessage = "Please enter a weather description.")]
        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string Description { get; set; } = "Sunny with a chance of meatballs.";
        public DayOfWeek SelectedDay { get; set; }
        public SubmitType SubmitType { get; set; }
    }

    public enum SubmitType
    {
        [Display(Name = "Test Redirect with TempData")]
        TempData,
        [Display(Name = "Test Redirect with Status")]
        Status,
        [Display(Name = "Test Redirect with Status and TempData")]
        StatusAndTempData
    }

    private static string GetEnumDisplayName(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

}
