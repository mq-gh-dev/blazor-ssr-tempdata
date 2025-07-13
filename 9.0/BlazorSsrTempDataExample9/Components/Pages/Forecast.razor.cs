using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static BlazorSsrTempDataExample9.Components.Pages.Home;

namespace BlazorSsrTempDataExample9.Components.Pages
{
    public partial class Forecast(BlazorSsrRedirectManager redirectManager,
    ITempDataDictionaryFactory tempDataDictionaryFactory)
    {
        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;
        private bool HasWeatherData { get; set; }
        private WeatherFormModel DisplayModel { get; set; } = new();


        protected override void OnInitialized()
        {
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Access TempData to retrieve previously saved weather data
                var tempData = tempDataDictionaryFactory.GetTempData(HttpContext);

                var hasWeatherDesc = tempData.TryGetValue<string>(nameof(DisplayModel.Description), out var desc);
                var hasDay = tempData.TryGetValue<DayOfWeek>(nameof(DisplayModel.SelectedDay), out var day);

                DisplayModel.Description = hasWeatherDesc ? desc! : string.Empty;
                DisplayModel.SelectedDay = hasDay ? day : default;
                HasWeatherData = hasWeatherDesc || hasDay;

                // Call this to ensure TempData is cleared after reading
                tempData.Save();
            }
        }


    }
}