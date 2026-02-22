using Blazor.Toast.Configuration;
using Blazor.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazor.Toast;

public partial class Toasts
{
    [Inject] private IToastServiceEvents ToastServiceEvents { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter] public IconType IconType { get; set; } = IconType.Default;
    [Parameter] public string? InfoClass { get; set; }
    [Parameter] public string? InfoIcon { get; set; }
    [Parameter] public string? SuccessClass { get; set; }
    [Parameter] public string? SuccessIcon { get; set; }
    [Parameter] public string? WarningClass { get; set; }
    [Parameter] public string? WarningIcon { get; set; }
    [Parameter] public string? ErrorClass { get; set; }
    [Parameter] public string? ErrorIcon { get; set; }
    [Parameter] public ToastPosition Position { get; set; } = ToastPosition.TopRight;
    [Parameter] public int Timeout { get; set; } = 5;
    [Parameter] public int MaxToastCount { get; set; } = int.MaxValue;
    [Parameter] public bool RemoveToastsOnNavigation { get; set; }
    [Parameter] public bool ShowProgressBar { get; set; }
    [Parameter] public RenderFragment? CloseButtonContent { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public bool DisableTimeout { get; set; }
    [Parameter] public bool PauseProgressOnHover { get; set; }
    [Parameter] public int ExtendedTimeout { get; set; }

    private List<ToastInstance> ToastList { get; } = new();
    private Queue<ToastInstance> ToastWaitingQueue { get; set; } = new();

    protected override void OnInitialized()
    {
        ToastServiceEvents.OnShow += ShowToast;
        ToastServiceEvents.OnShowAsync += ShowToastAsync;
        ToastServiceEvents.OnShowDetailedAsync += ShowToastDetailedAsync;
        ToastServiceEvents.OnShowComponent += ShowCustomToast;
        ToastServiceEvents.OnShowComponentAsync += ShowCustomToastAsync;
        ToastServiceEvents.OnShowComponentDetailedAsync += ShowCustomToastDetailedAsync;
        ToastServiceEvents.OnClearAll += ClearAll;
        ToastServiceEvents.OnClearToasts += ClearToasts;
        ToastServiceEvents.OnClearCustomToasts += ClearCustomToasts;
        ToastServiceEvents.OnClearQueue += ClearQueue;
        ToastServiceEvents.OnClearQueueToasts += ClearQueueToasts;

        if (RemoveToastsOnNavigation) NavigationManager.LocationChanged += ClearToasts;

        if (IconType == IconType.Custom
            && string.IsNullOrWhiteSpace(InfoIcon)
            && string.IsNullOrWhiteSpace(SuccessIcon)
            && string.IsNullOrWhiteSpace(WarningIcon)
            && string.IsNullOrWhiteSpace(ErrorIcon))
            throw new ArgumentException("IconType is Custom but icon parameters are not set.");
    }

    private void ShowToastDetailedAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? toastSettings, TaskCompletionSource<ToastResult>? completionSource)
    {
        InvokeAsync(() =>
        {
            var settings = BuildToastSettings(level, toastSettings);
            var toast = new ToastInstance(message, level, settings, null, completionSource);

            if (ToastList.Count < MaxToastCount)
            {
                ToastList.Add(toast);

                StateHasChanged();
            }
            else
            {
                ToastWaitingQueue.Enqueue(toast);
            }
        });
    }

    private ToastSettings BuildCustomToastSettings(Action<ToastSettings>? settings)
    {
        var instanceToastSettings = new ToastSettings();
        settings?.Invoke(instanceToastSettings);
        instanceToastSettings.Timeout = instanceToastSettings.Timeout == 0 ? Timeout : instanceToastSettings.Timeout;
        instanceToastSettings.DisableTimeout ??= DisableTimeout;
        instanceToastSettings.PauseProgressOnHover ??= PauseProgressOnHover;
        instanceToastSettings.ExtendedTimeout ??= ExtendedTimeout;
        instanceToastSettings.Position ??= Position;
        instanceToastSettings.ShowProgressBar ??= ShowProgressBar;

        return instanceToastSettings;
    }

    private void ShowCustomToastDetailedAsync(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings, TaskCompletionSource<ToastResult>? completionSource)
    {
        InvokeAsync(() =>
        {
            var childContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                if (parameters is not null)
                    foreach (var parameter in parameters.Parameters)
                        builder.AddAttribute(i++, parameter.Key, parameter.Value);

                builder.CloseComponent();
            });

            var toastSettings = BuildCustomToastSettings(settings);
            var toastInstance = new ToastInstance(childContent, toastSettings, null, completionSource);

            ToastList.Add(toastInstance);

            StateHasChanged();
        });
    }

    private ToastSettings BuildToastSettings(ToastLevel level, Action<ToastSettings>? settings)
    {
        var toastInstanceSettings = new ToastSettings();
        settings?.Invoke(toastInstanceSettings);

        return level switch
        {
            ToastLevel.Error => BuildToastSettings(toastInstanceSettings, "blazor-toast-error", ErrorIcon,
                ErrorClass),
            ToastLevel.Info => BuildToastSettings(toastInstanceSettings, "blazor-toast-info", InfoIcon, InfoClass),
            ToastLevel.Success => BuildToastSettings(toastInstanceSettings, "blazor-toast-success", SuccessIcon,
                SuccessClass),
            ToastLevel.Warning => BuildToastSettings(toastInstanceSettings, "blazor-toast-warning", WarningIcon,
                WarningClass),
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }

    private ToastSettings BuildToastSettings(ToastSettings toastInstanceSettings, string cssClassForLevel,
        string? configIcon, string? configAdditionalClasses)
    {
        var additonalClasses = string.IsNullOrEmpty(toastInstanceSettings.AdditionalClasses)
            ? configAdditionalClasses
            : toastInstanceSettings.AdditionalClasses;

        return new ToastSettings(
            $"{cssClassForLevel} {additonalClasses}",
            toastInstanceSettings.IconType ?? IconType,
            toastInstanceSettings.Icon ?? configIcon ?? "",
            toastInstanceSettings.ShowProgressBar ?? ShowProgressBar,
            toastInstanceSettings.ShowCloseButton ?? ShowCloseButton,
            toastInstanceSettings.OnClick,
            toastInstanceSettings.Timeout == 0 ? Timeout : toastInstanceSettings.Timeout,
            toastInstanceSettings.DisableTimeout ?? DisableTimeout,
            toastInstanceSettings.PauseProgressOnHover ?? PauseProgressOnHover,
            toastInstanceSettings.ExtendedTimeout ?? ExtendedTimeout,
            toastInstanceSettings.Position ?? Position);
    }

    private void ShowToast(ToastLevel level, RenderFragment message, Action<ToastSettings>? toastSettings)
    {
        InvokeAsync(() =>
        {
            var settings = BuildToastSettings(level, toastSettings);
            var toast = new ToastInstance(message, level, settings);

            if (ToastList.Count < MaxToastCount)
            {
                ToastList.Add(toast);

                StateHasChanged();
            }
            else
            {
                ToastWaitingQueue.Enqueue(toast);
            }
        });
    }

    private void ShowToastAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? toastSettings, TaskCompletionSource<ToastCloseReason>? completionSource)
    {
        InvokeAsync(() =>
        {
            var settings = BuildToastSettings(level, toastSettings);
            var toast = new ToastInstance(message, level, settings, completionSource);

            if (ToastList.Count < MaxToastCount)
            {
                ToastList.Add(toast);

                StateHasChanged();
            }
            else
            {
                ToastWaitingQueue.Enqueue(toast);
            }
        });
    }

    private void ShowCustomToast(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings)
    {
        InvokeAsync(() =>
        {
            var childContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                if (parameters is not null)
                    foreach (var parameter in parameters.Parameters)
                        builder.AddAttribute(i++, parameter.Key, parameter.Value);

                builder.CloseComponent();
            });

            var toastSettings = BuildCustomToastSettings(settings);
            var toastInstance = new ToastInstance(childContent, toastSettings);

            ToastList.Add(toastInstance);

            StateHasChanged();
        });
    }

    private void ShowCustomToastAsync(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings, TaskCompletionSource<ToastCloseReason>? completionSource)
    {
        InvokeAsync(() =>
        {
            var childContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                if (parameters is not null)
                    foreach (var parameter in parameters.Parameters)
                        builder.AddAttribute(i++, parameter.Key, parameter.Value);

                builder.CloseComponent();
            });

            var toastSettings = BuildCustomToastSettings(settings);
            var toastInstance = new ToastInstance(childContent, toastSettings, completionSource);

            ToastList.Add(toastInstance);

            StateHasChanged();
        });
    }

    private void ShowEnqueuedToast()
    {
        InvokeAsync(() =>
        {
            var toast = ToastWaitingQueue.Dequeue();

            ToastList.Add(toast);

            StateHasChanged();
        });
    }

    public void RemoveToast(Guid toastId, ToastCloseReason reason = ToastCloseReason.Unknown)
    {
        InvokeAsync(() =>
        {
            var toastInstance = ToastList.SingleOrDefault(x => x.Id == toastId);

            if (toastInstance is not null)
            {
                ToastList.Remove(toastInstance);
                try
                {
                    toastInstance.CompletionSource?.TrySetResult(reason);
                }
                catch
                {
                    // ignore
                }

                try
                {
                    if (toastInstance.CompletionSourceResult is not null)
                    {
                        var result = new ToastResult
                        {
                            ToastId = toastInstance.Id,
                            Reason = reason,
                            ShownAt = toastInstance.TimeStamp,
                            ClosedAt = DateTime.Now
                        };

                        toastInstance.CompletionSourceResult.TrySetResult(result);
                    }
                }
                catch
                {
                    // ignore
                }

                StateHasChanged();
            }

            if (ToastWaitingQueue.Any()) ShowEnqueuedToast();
        });
    }

    private void ClearToasts(object? sender, LocationChangedEventArgs args)
    {
        InvokeAsync(() =>
        {
            // Programmatically remove all visible toasts (report Programmatic close reason)
            var ids = ToastList.Select(t => t.Id).ToList();
            foreach (var id in ids)
                RemoveToast(id, ToastCloseReason.Programmatic);

            // Clear queued toasts and notify any awaiting callers as Programmatic
            while (ToastWaitingQueue.Count > 0)
            {
                var queued = ToastWaitingQueue.Dequeue();
                try { queued.CompletionSource?.TrySetResult(ToastCloseReason.Programmatic); } catch { }
                try
                {
                    if (queued.CompletionSourceResult is not null)
                    {
                        queued.CompletionSourceResult.TrySetResult(new ToastResult
                        {
                            ToastId = queued.Id,
                            Reason = ToastCloseReason.Programmatic,
                            ShownAt = queued.TimeStamp,
                            ClosedAt = DateTime.Now
                        });
                    }
                }
                catch { }
            }

            StateHasChanged();
        });
    }

    private void ClearAll()
    {
        InvokeAsync(() =>
        {
            var ids = ToastList.Select(t => t.Id).ToList();
            foreach (var id in ids)
                RemoveToast(id, ToastCloseReason.Programmatic);

            // Clear queued toasts and notify awaiting callers
            while (ToastWaitingQueue.Count > 0)
            {
                var queued = ToastWaitingQueue.Dequeue();
                try { queued.CompletionSource?.TrySetResult(ToastCloseReason.Programmatic); } catch { }
                try
                {
                    if (queued.CompletionSourceResult is not null)
                    {
                        queued.CompletionSourceResult.TrySetResult(new ToastResult
                        {
                            ToastId = queued.Id,
                            Reason = ToastCloseReason.Programmatic,
                            ShownAt = queued.TimeStamp,
                            ClosedAt = DateTime.Now
                        });
                    }
                }
                catch { }
            }

            StateHasChanged();
        });
    }

    private void ClearToasts(ToastLevel toastLevel)
    {
        InvokeAsync(() =>
        {
            var ids = ToastList.Where(x => x.CustomComponent is null && x.Level == toastLevel).Select(x => x.Id).ToList();
            foreach (var id in ids) RemoveToast(id, ToastCloseReason.Programmatic);

            // clear queued matching
            var remainingQueue = new Queue<ToastInstance>(ToastWaitingQueue.Where(x => x.Level != toastLevel));
            var removed = ToastWaitingQueue.Where(x => x.Level == toastLevel).ToList();
            foreach (var q in removed)
            {
                try { q.CompletionSource?.TrySetResult(ToastCloseReason.Programmatic); } catch { }
                try
                {
                    if (q.CompletionSourceResult is not null)
                    {
                        q.CompletionSourceResult.TrySetResult(new ToastResult
                        {
                            ToastId = q.Id,
                            Reason = ToastCloseReason.Programmatic,
                            ShownAt = q.TimeStamp,
                            ClosedAt = DateTime.Now
                        });
                    }
                }
                catch { }
            }

            ToastWaitingQueue = remainingQueue;

            StateHasChanged();
        });
    }

    private void ClearCustomToasts()
    {
        InvokeAsync(() =>
        {
            var ids = ToastList.Where(x => x.CustomComponent is not null).Select(x => x.Id).ToList();
            foreach (var id in ids) RemoveToast(id, ToastCloseReason.Programmatic);
            StateHasChanged();
        });
    }

    private void ClearQueue()
    {
        InvokeAsync(() =>
        {
            while (ToastWaitingQueue.Count > 0)
            {
                var queued = ToastWaitingQueue.Dequeue();
                try { queued.CompletionSource?.TrySetResult(ToastCloseReason.Programmatic); } catch { }
                try
                {
                    if (queued.CompletionSourceResult is not null)
                    {
                        queued.CompletionSourceResult.TrySetResult(new ToastResult
                        {
                            ToastId = queued.Id,
                            Reason = ToastCloseReason.Programmatic,
                            ShownAt = queued.TimeStamp,
                            ClosedAt = DateTime.Now
                        });
                    }
                }
                catch { }
            }

            StateHasChanged();
        });
    }

    private void ClearQueueToasts(ToastLevel toastLevel)
    {
        InvokeAsync(() =>
        {
            var remaining = new Queue<ToastInstance>(ToastWaitingQueue.Where(x => x.Level != toastLevel));
            var removed = ToastWaitingQueue.Where(x => x.Level == toastLevel).ToList();
            foreach (var q in removed)
            {
                try { q.CompletionSource?.TrySetResult(ToastCloseReason.Programmatic); } catch { }
                try
                {
                    if (q.CompletionSourceResult is not null)
                    {
                        q.CompletionSourceResult.TrySetResult(new ToastResult
                        {
                            ToastId = q.Id,
                            Reason = ToastCloseReason.Programmatic,
                            ShownAt = q.TimeStamp,
                            ClosedAt = DateTime.Now
                        });
                    }
                }
                catch { }
            }

            ToastWaitingQueue = remaining;
            StateHasChanged();
        });
    }
}