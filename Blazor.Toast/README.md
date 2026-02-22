# Blazor.Toast

A lightweight toast notification library for Blazor, built for .NET 10 and modern Razor Components.

> This project is an independent fork and is not an official package from the Blazored team.

---

## Summary

- Lightweight toast library for Blazor apps
- Simple, easy-to-use API
- Compatible with .NET 10 Razor Components
- Supports icon configuration, progress bar, close button, and maximum toast count

---

## Installation

Add the NuGet package:

```bash
dotnet add package Blazor.Toast
```

Or add the project reference directly to your solution.

---

## Service Registration

Add the following in `Program.cs`:

```csharp
using Blazor.Toast;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorToast();

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

---

## Usage

### 1) Add namespaces to `_Imports.razor`

```razor
@using Blazor.Toast
@using Blazor.Toast.Services
@using Blazor.Toast.Configuration
```

---

### 2) Place the toast component in your layout (e.g. `MainLayout.razor`)

Blazor.Toast must be rendered in an interactive render mode. You can either place the `<Toasts>` component directly in a layout or put it into a small child component (the sample uses `Components/Layout/_FooterContent.razor`) and render that child with an explicit render mode.

Inline in the layout (ensure the layout uses an interactive render mode):

```razor
@rendermode InteractiveServer

<Toasts Position="ToastPosition.BottomRight"
        Timeout="10"
        IconType="IconType.Material"
        ErrorIcon="error_outline"
        InfoIcon="school"
        SuccessIcon="done_outline"
        WarningIcon="warning"
        ShowProgressBar="true"
        ShowCloseButton="true"
        MaxToastCount="3">
    <CloseButtonContent>
        <div>
            <span class="myCloseButtonStyleClass">&times;</span>
        </div>
    </CloseButtonContent>
</Toasts>
```

Or use a child component (sample): create `Components/Layout/_FooterContent.razor` with the `<Toasts>` markup and then render it from `MainLayout.razor` with a render-mode attribute:

```razor
<_FooterContent @rendermode="InteractiveServer" />
```

If your hosting model is WebAssembly, use `@rendermode InteractiveWebAssembly` instead. Components rendered statically will not provide client-side interactivity.

---

### 3) Invoke from another component

```razor
@inject IToastService ToastService

<button @onclick="ShowSuccess">Save</button>

@code {
    void ShowSuccess()
    {
        ToastService.ShowSuccess("Changes saved.");
    }
}
```

---

## Brief API

IToastService (selected methods)

- Synchronous display
  - `void ShowInfo(string message, Action<ToastSettings>? settings = null)`
  - `void ShowSuccess(string message, Action<ToastSettings>? settings = null)`
  - `void ShowWarning(string message, Action<ToastSettings>? settings = null)`
  - `void ShowError(string message, Action<ToastSettings>? settings = null)`

- Await close (simple)
  - `Task<ToastCloseReason> ShowInfoAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowSuccessAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowWarningAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowErrorAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowToastAsync(ToastLevel level, string message, Action<ToastSettings>? settings = null)`

- Await close (detailed)
  - `Task<ToastResult> ShowInfoDetailedAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastResult> ShowSuccessDetailedAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastResult> ShowWarningDetailedAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastResult> ShowErrorDetailedAsync(string message, Action<ToastSettings>? settings = null)`
  - `Task<ToastResult> ShowToastDetailedAsync(ToastLevel level, string message, Action<ToastSettings>? settings = null)`

- Component-based toasts
  - `void ShowToast<TComponent>() where TComponent : IComponent`
  - `Task<ToastCloseReason> ShowToastAsync<TComponent>(ToastParameters parameters) where TComponent : IComponent`
  - `Task<ToastResult> ShowToastDetailedAsync<TComponent>(ToastParameters parameters) where TComponent : IComponent`

The `<Toasts>` component supports configuration options such as:

- `Position`
- `Timeout`
- `IconType`
- `ShowProgressBar`
- `ShowCloseButton`
- `MaxToastCount`

---

## Awaiting toast result (detailed)

You can await a toast and receive a `ToastResult` with information about how and when the toast was closed. This is useful when you need to react after the user dismisses the notification or it times out.

```csharp
@inject Blazor.Toast.Services.IToastService ToastService

async Task ShowAndWait()
{
    // Show an info toast and await a detailed result
    var result = await ToastService.ShowInfoAsync("Please confirm action", settings: null, detailedResult: true);

    Console.WriteLine($"Toast {result.ToastId} closed with reason: {result.Reason}");
    Console.WriteLine($"Shown at: {result.ShownAt}, Closed at: {result.ClosedAt}, Duration: {result.Duration}");
}
```

There are overloads for `ShowSuccessAsync`, `ShowWarningAsync`, `ShowErrorAsync`, `ShowToastAsync` and component based toasts that also return `ToastResult` when `detailedResult` is used.

## Component-based toasts (await result)

You can render a Blazor component inside a toast and await a `ToastResult` when it is closed. The component receives a cascading `Toast` instance which it can use to close itself (call `ParentToast.Close()`).

Example component (`ConfirmToast.razor`):

```razor
<div class="confirm-toast">
    <p>@Message</p>
    <button @onclick="Confirm">Yes</button>
    <button @onclick="Cancel">No</button>
</div>

@code {
    [Parameter] public string Message { get; set; } = "Are you sure?";
    [CascadingParameter] public Toast ParentToast { get; set; } = default!;

    void Confirm() => ParentToast.Close();
    void Cancel() => ParentToast.Close();
}
```

Invoke and await the component-based toast from another component/page:

```csharp
var parameters = new ToastParameters();
parameters.Add("Message", "Delete this item?");

// Show the ConfirmToast component inside a toast and await detailed result
var result = await ToastService.ShowToastAsync<ConfirmToast>(parameters, detailedResult: true);

Console.WriteLine($"Toast {result.ToastId} closed with reason: {result.Reason}, duration: {result.Duration}");
```

Note: the custom component can dismiss the toast by calling the cascaded `ParentToast.Close()` method. The awaited `ToastResult` will contain timestamps and the close reason.

## Events and backward compatibility

Previously the library exposed public events on `IToastService` (for example `OnShow`, `OnShowAsync`) that consumers could subscribe to. Those events are now considered internal and have been marked `[Obsolete]` to discourage direct subscription. Prefer the service API methods above (`ShowInfoAsync`, `ShowInfoDetailedAsync`, `ShowToastAsync<TComponent>`, `ShowToastDetailedAsync<TComponent>`, etc.).

Backward-compatibility: overloads that accepted a `bool detailedResult` flag are kept as `[Obsolete]` aliases and will forward to the explicit methods. Example (still supported but obsolete):

```csharp
// Obsolete alias (works but use explicit methods instead)
var result = await ToastService.ShowInfoAsync("Please confirm", settings: null, detailedResult: true);

// Preferred explicit method
var detailed = await ToastService.ShowInfoDetailedAsync("Please confirm");
```

This change makes the API clearer and reduces the need for consumers to subscribe to internal events.

## Registration and migration notes

To use the library register the service in DI (example for a Blazor Server or WebAssembly host):

```csharp
builder.Services.AddBlazorToast();
```

This registers `ToastService` as a singleton and exposes both the public `IToastService` and the internal `IToastServiceEvents` used by the internal `<Toasts>` component.

Migration notes (breaking change):
- Public events previously exposed on `IToastService` were removed. Consumers should use the `IToastService` methods (`ShowInfoAsync`, `ShowInfoDetailedAsync`, etc.) instead of subscribing to events.
- The `<Toasts>` component subscribes to the internal `IToastServiceEvents` so it continues to work.

If you need a compatibility shim (to continue subscribing to events from application code) ask to add an adapter.


## Origin

This project is based on the following repository:

https://github.com/Blazored/Toast

This is an independent modification to support .NET 10. All original credit remains with the upstream contributors under the MIT License.

---

## License

MIT License  
See the `license.txt` file for details.
