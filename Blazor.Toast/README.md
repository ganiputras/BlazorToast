# Blazor.Toast

A lightweight toast notification library for Blazor, built for .NET 10 and modern Razor Components.

> Independent fork — original project: https://github.com/Blazored/Toast (MIT). This fork adapts the library for .NET 10 and provides a streamlined API.

---

## Quick summary

- Lightweight toast library for Blazor apps
- Minimal, consistent public API (RenderFragment-first)
- Compatible with .NET 10 Razor Components
- Supports icons, progress bar, close button, component-based toasts, and awaitable results

---

## Installation

Add the NuGet package (when published) or reference the project directly:

```bash
dotnet add package Blazor.Toast
```

Or add a project reference to your solution.

---

## Service registration

Register the toast service in DI in your `Program.cs` (Server or WASM hosting as appropriate):

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // for interactive server render mode

builder.Services.AddBlazorToast();

var app = builder.Build();
// ... map components and run
```

The extension `AddBlazorToast()` registers the concrete `ToastService` as a singleton and exposes `IToastService` (public API) and the internal `IToastServiceEvents` used by the rendering component.

---

## Usage

1) Add helpful usings to `_Imports.razor`:

```razor
@using Blazor.Toast
@using Blazor.Toast.Services
@using Blazor.Toast.Configuration
```

2) Place the `<Toasts>` component somewhere in your layout (must be rendered in an interactive render mode):

```razor
@rendermode InteractiveServer
<Toasts Position="ToastPosition.TopRight" Timeout="8" ShowProgressBar="true" />
```

If you prefer, render a small child component that contains `<Toasts>` with an explicit `@rendermode` attribute.

3) Show a toast from any component by injecting `IToastService`:

RenderFragment-based (preferred):

```razor
@inject IToastService ToastService

@code {
    void Save()
    {
        ToastService.ShowSuccess(builder => builder.AddContent(0, "Saved"));
    }
}
```

String convenience overloads are provided as extension methods so older code that passes a `string` still works:

```csharp
ToastService.ShowSuccess("Saved"); // uses extension to wrap as RenderFragment
```

Quick comparison — fire-and-forget vs await simple vs await detailed:

```csharp
// 1) Fire-and-forget: show and continue immediately
ToastService.ShowInfo(builder => builder.AddContent(0, "Saved successfully"));

// 2) Await simple close reason (enum)
var reason = await ToastService.ShowInfoAsync(builder => builder.AddContent(0, "Confirm action?"));
if (reason == ToastCloseReason.Click)
{
    // user clicked the toast body
}

// 3) Await detailed result (includes timestamps and id)
var result = await ToastService.ShowInfoDetailedAsync(builder => builder.AddContent(0, "Please confirm"));
Console.WriteLine($"Toast {result.ToastId} closed with {result.Reason} after {result.Duration.TotalSeconds}s");
```

---

## Brief public API (high level)

IToastService focuses on RenderFragment messages and component-based toasts. Key methods:

- Message-based (RenderFragment)
  - `void ShowInfo(RenderFragment message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowInfoAsync(RenderFragment message, Action<ToastSettings>? settings = null)`
  - `Task<ToastResult> ShowInfoDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null)`

- Component-based
  - `void ShowToast<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent`
  - `Task<ToastCloseReason> ShowToastAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent`
  - `Task<ToastResult> ShowToastDetailedAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent`

- Clear / queue APIs
  - `void ClearAll()`, `void ClearToasts(ToastLevel level)`, `void ClearCustomToasts()`, `void ClearQueue()`, `void ClearQueueToasts(ToastLevel level)`

See sample pages in the `WebAppSample` project for many usage examples.

---

## ToastCloseReason and ToastResult

When you await `ShowXAsync(...)` you receive a `ToastCloseReason` (enum) indicating how the toast was closed:

- `Unknown`, `Timeout`, `CloseButton`, `Click`, `Programmatic`

When you await `ShowXDetailedAsync(...)` you receive a `ToastResult` with:

- `ToastId` (Guid), `Reason` (ToastCloseReason), `ShownAt`, `ClosedAt`, and computed `Duration`.

These let you react to user interactions or programmatic clears.

---

## Component-based toasts and cascading Toast

Render a Blazor component inside a toast and await the detailed `ToastResult`. The component receives a cascading `Toast` instance which it can use to close itself (`ParentToast.Close()`). See `WebAppSample` for examples.

---

## Backward compatibility & migration notes

- The public API was simplified to prefer `RenderFragment` messages. To avoid breaking callers that pass strings, extension helpers are included that wrap strings into `RenderFragment` automatically.
- Internal event hooks previously exposed are now internal (`IToastServiceEvents`) and should not be consumed directly. The public API methods should be used instead.
- If you relied on deprecated overloads that returned `ToastResult` via a `detailedResult` boolean, use the explicit `ShowXDetailedAsync` methods instead.

---

## Examples and samples

See the `WebAppSample` project for multiple example pages:
- `ToastGeneralExample.razor` — general usage
- `ToastCloseReasonExample.razor` — demonstrates all close reasons and awaitable close
- `ToastResultExample.razor` — demonstrates `ToastResult` detailed output

---

## Origin

This project is based on the following repository:

https://github.com/Blazored/Toast

This is an independent modification to support .NET 10. All original credit remains with the upstream contributors under the MIT License.

---

## License

MIT License  
See the `license.txt` file for details.
