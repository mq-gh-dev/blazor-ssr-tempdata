# Blazor SSR TempData Example

A clean, minimal example demonstrating how to use TempData in Blazor Static Server-Side Rendering (SSR) applications for the post-redirect-get pattern. It's made from the empty official Blazor SSR template with no additional dependencies.

## 🎯 Overview

The solution includes a simple-to-use `BlazorSsrRedirectManager` service that handles redirections with TempData storage. A helper `StatusMessage` component displays TempData-based status messages after redirects. An example weather forecast form demonstrates access to TempData in Blazor components. 

TempData is an alternative to mechanisms such as query strings and flash cookies, which are less ideal for privacy, security and reliability concerns in many use cases.

## 🚀 Live Demo

[View Live Demo Here](https://blazor-ssr-temp-data-demo.azurewebsites.net/)

## 📋 Availability

While Blazor's official templates don't include built-in TempData support, it can be added via:

```csharp
// Program.cs
// Register minimum services to make ITempDataDictionary and default cookie provider available via DI.
// AddRazorPages or AddControllersWithViews also work (especially if you need those in your project)
builder.Services.AddMvcCore().AddViews();
```

## 🔧 Usage Example

```csharp
// Redirect with custom TempData
redirectManager.RedirectToWithStatusAndTempData("/profile", "Please complete your profile",
    new Dictionary<string, object?> 
    {
        { "EmailAddress", email },
        { "UserId", userId }
    });
```

```csharp
// Access TempData in a component or class
 var tempData = tempDataFact.GetTempData(httpContext);      

 tempData.TryGetValue<string>("EmailAddress", out var email);
 tempData.TryGetValue<Guid>("UserId", out var userId);

tempData.Save();
```

## ⚠️ Known Limitations

- **Explicit Save() Required**: You must call `Save()` on your `ITempDataDictionary` after read/write (The auto `SaveTempDataFilter` isn't triggered likely due to the framework's `NavigationException` mechanism for static SSR).
- **TempData Restrictions**: Subject to the same TempData restrictions in Razor Pages/MVC. Use simple types and safe serializable objects, etc.
- **Size Limits**: Subject to cookie size limitations when using the default cookie provider. You should be able to configure session provider if needed (untested).

For complete TempData limitations and best practices, refer to the [official ASP.NET Core TempData documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state#tempdata).

## 📦 Requirements

- .NET 8 or .NET 9 Blazor Web App (this template is built on .NET 9, but .NET 8 should work the same except for some language syntax tweaks)

## 🤝 Contributing

Feel free to submit issues, fork the repo, and create pull requests for any improvements.
