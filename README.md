# Blazor SSR TempData Example

A clean, minimal example of TempData usage in Blazor Static Server-Side Rendering (SSR) for the post-redirect-get pattern. Built from the official empty Blazor SSR template with no other dependencies.

## Overview

The solution includes a simple `BlazorSsrRedirectManager` service that handles redirecting and saving TempData, a `TempDataAccessor` service that retrieves TempData, and a helper `StatusMessageDisplay` component for TempData-based status messages. An example `Home` form demonstrates their use. 

TempData is an alternative to other mechanisms such as query strings and flash cookies, which are less ideal for privacy, security and reliability concerns in many use cases.

## Live Demo

[View Live Demo Here](https://blazor-ssr-temp-data-demo.azurewebsites.net/)

## Availability

While Blazor's official templates don't include built-in TempData support yet, it can be added via:

```csharp
// Program.cs
// Register minimum services to make ITempDataDictionary and default cookie provider available via DI.
// AddRazorPages or AddControllersWithViews also work (especially if you need those in your project)
builder.Services.AddMvcCore().AddViews();
```
Please consider posting in [this Github issue](https://github.com/dotnet/aspnetcore/issues/49683) to request official TempData support in Blazor SSR from Microsoft.

## Usage Example

```csharp
// Redirect with status message and TempData
redirectManager.RedirectToWithStatusAndTempData("/profile", "Please complete your profile", Severity.Info,
    new Dictionary<string, object?> 
    {
        { "EmailAddress", email },
        { "UserId", userId }
    });
```

```csharp
// Access TempData via TempDataAccessor
tempDataAccessor
   .TryGet<string?>("EmailAddress", out var email, out bool hasEmail)
   .TryGet<Guid?>("UserId", out var userId, out bool hasId)
   .Save();

// Note: You can also inject ITempDataDictionaryFactory and access TempData manually, 
// as shown in Forecast.razor.cs
```

## Known Limitations

- **Explicit Save() Required**: You must call `Save()` after using `TempDataAccessor` (or `ITempDataDictionary`). The auto `SaveTempDataFilter` isn't triggered likely due to the framework's `NavigationException` mechanism for static SSR (could be fixed in .NET 10).
- **TempData Restrictions**: Subject to the same TempData restrictions as in Razor Pages/MVC. Use simple types and safe serializable objects, etc.
- **Size Limits**: Subject to cookie size limitations when using the default cookie provider. You should be able to configure session provider if needed (untested).

For complete TempData limitations and best practices, refer to the [official ASP.NET Core TempData documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0#tempdata).

## Requirements

- .NET 8 or .NET 9 Blazor Web App.

## Contributing

Feel free to submit issues, fork the repo, and create pull requests for any improvements.
