# Blazor.Toast

A lightweight toast notification library for Blazor (Razor Components), updated for .NET 10.

This repository provides a small, RenderFragment-first API surface for showing toast notifications in Blazor Server and Blazor WebAssembly apps. It is an independent fork inspired by the original Blazored.Toast project.

Status: functional for .NET 10 apps. See `WebAppSample` for working examples.
![Legacy Example](legacy-example.jpg)
## Key features

- RenderFragment-first API (preferred): pass markup or components as toast content
- Component-based toasts (render a Blazor component inside a toast)
- Awaitable APIs that return a simple close reason or a detailed result object
- Progress bar, close button, custom icons, position and timeout configuration
- Queueing and programmatic clearing of toasts

## Requirements

- .NET 10 SDK
- Blazor Server or Blazor WebAssembly (Razor Components)

## Installation

1) When distributed as a NuGet package:

```bash
dotnet add package Blazor.Toast
```

2) Or add a project reference to `Blazor.Toast` in your solution.

## Register the service

In `Program.cs` register the toast service. Examples below show both server and generic host setup.

Blazor Server (interactive server components):

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorToast();

var app = builder.Build();
// map components/endpoints and run
```

Blazor WebAssembly (Client) — typical pattern: add service in the host project DI container:

```csharp
builder.Services.AddBlazorToast();
```

The `AddBlazorToast()` extension registers `ToastService` as a singleton and exposes the public `IToastService`.

## Add imports

Add these to your `_Imports.razor` for convenient usage:

```razor
@using Blazor.Toast
@using Blazor.Toast.Services
@using Blazor.Toast.Configuration
```

## Render the toast container

Place the `<Toasts>` component in your main layout or a root component that is rendered in an interactive render mode (Server: add `@rendermode InteractiveServer` where required).

Basic example:

```razor
@rendermode InteractiveServer
<Toasts Position="ToastPosition.BottomRight"
            Timeout="5"
            IconType="IconType.Default"
            ErrorIcon="info"
            InfoIcon="info"
            SuccessIcon="info"
            WarningIcon="info"
            ShowProgressBar="@true"
            ShowCloseButton="@true"
            MaxToastCount="3">
        <CloseButtonContent>
            <div>
                <span class="myCloseButtonStyleClass">&times;</span>
            </div>
        </CloseButtonContent>
    </Toasts>
```

Common parameters:

- `Position` — `ToastPosition` (TopRight, BottomRight, TopLeft, BottomLeft, TopCenter, BottomCenter)
- `Timeout` — seconds (default timeout for toasts)
- `ShowProgressBar` — true/false
- `ShowCloseButton` — true/false
- `MaxToastCount` — maximum visible toasts before queueing
- `IconType` and per-level icons/classes for customizing visuals

## Show a toast (RenderFragment-first)

Inject `IToastService` in any component or page and call the preferred RenderFragment APIs:

```razor
@inject IToastService ToastService

@code {
    void Save()
    {
        ToastService.ShowSuccess(builder => builder.AddContent(0, "Saved"));
    }

    async Task Confirm()
    {
        var reason = await ToastService.ShowInfoAsync(builder => builder.AddContent(0, "Confirm action?"));
        if (reason == ToastCloseReason.Click) { /* handle click */ }
    }
}
```

String convenience overloads are available (extensions wrap the string into a `RenderFragment`):

```csharp
ToastService.ShowSuccess("Saved");
```

## Component-based toasts

Render a custom component inside a toast and optionally await a detailed result:

```csharp
var result = await ToastService.ShowToastDetailedAsync<MyToastComponent>(parameters, settings);
```

The component rendered inside the toast receives a cascading `Toast` instance which can be used to close itself programmatically.

## Clearing and queue management

- `ToastService.ClearAll()` — remove all visible toasts (awaiting callers receive Programmatic)
- `ToastService.ClearToasts(ToastLevel.Warning)` — remove visible toasts by level
- `ToastService.ClearCustomToasts()` — remove component toasts
- `ToastService.ClearQueue()` and `ClearQueueToasts(...)` — remove queued toasts

## API summary

Public surface (high level):

- Message-based (RenderFragment):
  - `void ShowInfo(RenderFragment message, Action<ToastSettings>? settings = null)`
  - `Task<ToastCloseReason> ShowInfoAsync(...)` / `Task<ToastResult> ShowInfoDetailedAsync(...)`
  - Same for `ShowSuccess`, `ShowWarning`, `ShowError`

- Component-based:
  - `void ShowToast<TComponent>(...)` / `Task<ToastCloseReason> ShowToastAsync<TComponent>(...)`
  - `Task<ToastResult> ShowToastDetailedAsync<TComponent>(...)`

- Clear/queue APIs: `ClearAll`, `ClearToasts`, `ClearCustomToasts`, `ClearQueue`, `ClearQueueToasts`

Key types:

- `ToastLevel` — Info, Success, Warning, Error
- `ToastCloseReason` — Unknown, Timeout, CloseButton, Click, Programmatic
- `ToastResult` — { ToastId (Guid), Reason, ShownAt, ClosedAt, Duration }

## Samples

Open `WebAppSample` in this solution for working examples and usage pages:

- `ToastGeneraLegacyExample.razor` — legacy usage
- `ToastGeneralExample.razor` — general usage
- `ToastCloseReasonExample.razor` — demonstrates awaitable close reasons
- `ToastResultExample.razor` — demonstrates detailed `ToastResult`

## Contributing

Contributions, bug reports and pull requests are welcome. Follow the coding conventions in the repo and keep changes minimal and focused.

## License

MIT. See `license.txt` in the repository for details.

## Origin

This project is based on the following repository:

https://github.com/Blazored/Toast

This is an independent modification to support .NET 10. All original credit remains with the upstream contributors under the MIT License.

