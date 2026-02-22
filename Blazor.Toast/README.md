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

`IToastService`

- `ShowSuccess(string message, string? heading = null)`
- `ShowInfo(...)`
- `ShowWarning(...)`
- `ShowError(...)`

The `<Toasts>` component supports configuration options such as:

- `Position`
- `Timeout`
- `IconType`
- `ShowProgressBar`
- `ShowCloseButton`
- `MaxToastCount`

---

## Origin

This project is based on the following repository:

https://github.com/Blazored/Toast

This is an independent modification to support .NET 10. All original credit remains with the upstream contributors under the MIT License.

---

## License

MIT License  
See the `license.txt` file for details.
