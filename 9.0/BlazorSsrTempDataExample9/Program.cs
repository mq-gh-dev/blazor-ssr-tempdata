using BlazorSsrTempDataExample9;
using BlazorSsrTempDataExample9.Components;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

// Register minimum services to make ITempDataDictionary and default cookie provider available via DI.
// AddRazorPages or AddControllersWithViews also work (especially if you need those in your project).
// Note that automatic TempData Save() calls (SaveTempDataFilter) aren't triggered in Blazor (most likely due to static SSR's 
// NavigationException during redirection), so we just need to call it manually after setting/accessing TempData.
// Hopefully this will be resolved automatically in .NET 10 which no longer uses NavigationException for SSR.
builder.Services.AddMvcCore().AddViews();

builder.Services.AddHttpContextAccessor();

// A conveneint redirect manager that can carry TempData and TempData-based status messages
builder.Services.AddScoped<BlazorSsrRedirectManager>();

// Some extra configs for cookie-based TempData for security.
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
