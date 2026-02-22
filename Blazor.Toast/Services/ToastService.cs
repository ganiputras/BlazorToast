using System;
using System.Threading.Tasks;
using Blazor.Toast.Configuration;
using Microsoft.AspNetCore.Components;

namespace Blazor.Toast.Services;

/// <summary>
/// Default implementation of <see cref="IToastService"/>.
/// Keeps event hooks internal via <see cref="IToastServiceEvents"/> so the public API
/// is method-only and easier to evolve.
/// </summary>
public class ToastService : IToastService, IToastServiceEvents
{
    // Internal event backings (not part of public API)
    private Action<ToastLevel, RenderFragment, Action<ToastSettings>?>? _onShow;
    private Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? _onShowAsync;
    private Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? _onShowDetailedAsync;

    private Action? _onClearAll;
    private Action<ToastLevel>? _onClearToasts;
    private Action? _onClearCustomToasts;

    private Action<Type, ToastParameters?, Action<ToastSettings>?>? _onShowComponent;
    private Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? _onShowComponentAsync;
    private Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? _onShowComponentDetailedAsync;

    private Action? _onClearQueue;
    private Action<ToastLevel>? _onClearQueueToasts;

    // Explicit interface implementation to keep these events internal.
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?>? IToastServiceEvents.OnShow { add => _onShow += value; remove => _onShow -= value; }
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? IToastServiceEvents.OnShowAsync { add => _onShowAsync += value; remove => _onShowAsync -= value; }
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? IToastServiceEvents.OnShowDetailedAsync { add => _onShowDetailedAsync += value; remove => _onShowDetailedAsync -= value; }

    event Action? IToastServiceEvents.OnClearAll { add => _onClearAll += value; remove => _onClearAll -= value; }
    event Action<ToastLevel>? IToastServiceEvents.OnClearToasts { add => _onClearToasts += value; remove => _onClearToasts -= value; }
    event Action? IToastServiceEvents.OnClearCustomToasts { add => _onClearCustomToasts += value; remove => _onClearCustomToasts -= value; }

    event Action<Type, ToastParameters?, Action<ToastSettings>?>? IToastServiceEvents.OnShowComponent { add => _onShowComponent += value; remove => _onShowComponent -= value; }
    event Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? IToastServiceEvents.OnShowComponentAsync { add => _onShowComponentAsync += value; remove => _onShowComponentAsync -= value; }
    event Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? IToastServiceEvents.OnShowComponentDetailedAsync { add => _onShowComponentDetailedAsync += value; remove => _onShowComponentDetailedAsync -= value; }

    event Action? IToastServiceEvents.OnClearQueue { add => _onClearQueue += value; remove => _onClearQueue -= value; }
    event Action<ToastLevel>? IToastServiceEvents.OnClearQueueToasts { add => _onClearQueueToasts += value; remove => _onClearQueueToasts -= value; }

    // ---------- Public API (IToastService) ----------

    // RenderFragment-based APIs are the preferred surface for messages. String overloads removed.

    /// <summary>
    /// Shows an information toast (fire-and-forget).
    /// </summary>
    /// <param name="message">Render fragment used to render the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    public void ShowInfo(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToast(ToastLevel.Info, message, settings);

    /// <summary>
    /// Shows an information toast and returns a <see cref="ToastCloseReason"/> when it closes.
    /// </summary>
    /// <param name="message">Render fragment used to render the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    /// <returns>A task that completes with the close reason.</returns>
    public Task<ToastCloseReason> ShowInfoAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastAsync(ToastLevel.Info, message, settings);

    /// <summary>
    /// Shows an information toast and returns detailed information about the closed toast.
    /// </summary>
    /// <param name="message">Render fragment used to render the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    /// <returns>A task that completes with a <see cref="ToastResult"/> containing timestamps and reason.</returns>
    public Task<ToastResult> ShowInfoDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastDetailedAsync(ToastLevel.Info, message, settings);

    // ...

    /// <summary>
    /// Shows a success toast (fire-and-forget).
    /// </summary>
    public void ShowSuccess(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToast(ToastLevel.Success, message, settings);

    /// <summary>
    /// Shows a success toast and returns the close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowSuccessAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastAsync(ToastLevel.Success, message, settings);

    /// <summary>
    /// Shows a success toast and returns a detailed result when it closes.
    /// </summary>
    public Task<ToastResult> ShowSuccessDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastDetailedAsync(ToastLevel.Success, message, settings);

    // ...

    /// <summary>
    /// Shows a warning toast (fire-and-forget).
    /// </summary>
    public void ShowWarning(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToast(ToastLevel.Warning, message, settings);

    /// <summary>
    /// Shows a warning toast and returns the close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowWarningAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastAsync(ToastLevel.Warning, message, settings);

    /// <summary>
    /// Shows a warning toast and returns a detailed result when it closes.
    /// </summary>
    public Task<ToastResult> ShowWarningDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastDetailedAsync(ToastLevel.Warning, message, settings);

    // ...

    /// <summary>
    /// Shows an error toast (fire-and-forget).
    /// </summary>
    public void ShowError(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToast(ToastLevel.Error, message, settings);

    /// <summary>
    /// Shows an error toast and returns the close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowErrorAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastAsync(ToastLevel.Error, message, settings);

    /// <summary>
    /// Shows an error toast and returns a detailed result when it closes.
    /// </summary>
    public Task<ToastResult> ShowErrorDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null)
        => ShowToastDetailedAsync(ToastLevel.Error, message, settings);

    // String convenience overloads removed — use RenderFragment variants instead.

    /// <summary>
    /// Shows a toast with the specified level and content.
    /// </summary>
    /// <param name="level">The toast level.</param>
    /// <param name="message">Render fragment used to render the toast content.</param>
    /// <param name="settings">Optional per-toast settings.</param>
    public void ShowToast(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null)
        => _onShow?.Invoke(level, message, settings);

    /// <summary>
    /// Shows a toast and returns the simple close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowToastAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null)
    {
        var tcs = new TaskCompletionSource<ToastCloseReason>();
        _onShowAsync?.Invoke(level, message, settings, tcs);
        return tcs.Task;
    }

    /// <summary>
    /// Shows a toast and returns detailed information when it closes.
    /// </summary>
    public Task<ToastResult> ShowToastDetailedAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null)
    {
        var tcs = new TaskCompletionSource<ToastResult>();
        _onShowDetailedAsync?.Invoke(level, message, settings, tcs);
        return tcs.Task;
    }

    // Simplified component-based API: single generic method set with optional params/settings
    /// <summary>
    /// Shows a component-based toast.
    /// </summary>
    public void ShowToast<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null)
        where TComponent : IComponent => ShowToast(typeof(TComponent), parameters, settings);

    /// <summary>
    /// Shows a component-based toast and returns the close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowToastAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null)
        where TComponent : IComponent => ShowToastAsync(typeof(TComponent), parameters, settings);

    /// <summary>
    /// Shows a component-based toast and returns detailed result when it closes.
    /// </summary>
    public Task<ToastResult> ShowToastDetailedAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null)
        where TComponent : IComponent => ShowToastDetailedAsync(typeof(TComponent), parameters, settings);

    /// <summary>
    /// Clears all active toasts. Awaiting callers will receive <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    public void ClearAll() => _onClearAll?.Invoke();

    /// <summary>
    /// Clears active toasts matching the given level.
    /// </summary>
    public void ClearToasts(ToastLevel toastLevel) => _onClearToasts?.Invoke(toastLevel);

    /// <summary>
    /// Clears custom (component) toasts.
    /// </summary>
    public void ClearCustomToasts() => _onClearCustomToasts?.Invoke();

    /// <summary>
    /// Clears queued toasts (not yet shown). Awaiting callers are notified with <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    public void ClearQueue() => _onClearQueue?.Invoke();

    /// <summary>
    /// Clears queued toasts of the specified level.
    /// </summary>
    public void ClearQueueToasts(ToastLevel toastLevel) => _onClearQueueToasts?.Invoke(toastLevel);

    /// <summary>
    /// Shows a component-based toast by component type.
    /// </summary>
    /// <param name="contentComponent">Component type to render inside the toast.</param>
    /// <param name="parameters">Optional parameters for the component.</param>
    /// <param name="settings">Optional per-toast settings.</param>
    public void ShowToast(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings)
    {
        if (!typeof(IComponent).IsAssignableFrom(contentComponent))
            throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");

        _onShowComponent?.Invoke(contentComponent, parameters, settings);
    }

    /// <summary>
    /// Shows a component-based toast and returns the simple close reason when it closes.
    /// </summary>
    public Task<ToastCloseReason> ShowToastAsync(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings)
    {
        if (!typeof(IComponent).IsAssignableFrom(contentComponent))
            throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");

        var tcs = new TaskCompletionSource<ToastCloseReason>();
        _onShowComponentAsync?.Invoke(contentComponent, parameters, settings, tcs);
        return tcs.Task;
    }

    /// <summary>
    /// Shows a component-based toast and returns detailed result when it closes.
    /// </summary>
    public Task<ToastResult> ShowToastDetailedAsync(Type contentComponent, ToastParameters? parameters, Action<ToastSettings>? settings)
    {
        if (!typeof(IComponent).IsAssignableFrom(contentComponent))
            throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");

        var tcs = new TaskCompletionSource<ToastResult>();
        _onShowComponentDetailedAsync?.Invoke(contentComponent, parameters, settings, tcs);
        return tcs.Task;
    }
}
