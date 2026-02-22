using Blazor.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Toast.Configuration;

internal class ToastInstance
{
    public ToastInstance(RenderFragment message, ToastLevel level, ToastSettings toastSettings, TaskCompletionSource<ToastCloseReason>? completionSource = null, TaskCompletionSource<ToastResult>? completionSourceResult = null)
    {
        Message = message;
        Level = level;
        ToastSettings = toastSettings;
        CompletionSource = completionSource;
        CompletionSourceResult = completionSourceResult;
    }

    public ToastInstance(RenderFragment customComponent, ToastSettings settings, TaskCompletionSource<ToastCloseReason>? completionSource = null, TaskCompletionSource<ToastResult>? completionSourceResult = null)
    {
        CustomComponent = customComponent;
        ToastSettings = settings;
        CompletionSource = completionSource;
        CompletionSourceResult = completionSourceResult;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public DateTime TimeStamp { get; } = DateTime.Now;
    public RenderFragment? Message { get; set; }
    public ToastLevel Level { get; }
    public ToastSettings ToastSettings { get; }
    public RenderFragment? CustomComponent { get; }
    public TaskCompletionSource<ToastCloseReason>? CompletionSource { get; }
    public TaskCompletionSource<ToastResult>? CompletionSourceResult { get; }
}
