using System;
using Blazor.Toast.Configuration;
using Microsoft.AspNetCore.Components;

namespace Blazor.Toast.Services;

/// <summary>
/// Defines the public contract for displaying and managing toast notifications.
/// </summary>
/// <remarks>
/// This interface represents the stable API surface exposed to consumers.
/// Internal event-based communication is handled separately via
/// <c>IToastServiceEvents</c>.
/// </remarks>
/// <summary>
/// Public toast service used by application code to display notifications.
/// </summary>
/// <remarks>
/// The API is RenderFragment-first: prefer providing a <see cref="RenderFragment"/> for message
/// content so callers can pass markup or rich content. Async variants return either a
/// <see cref="ToastCloseReason"/> (simple) or a <see cref="ToastResult"/> (detailed) that
/// describes how and when the toast was closed.
/// </remarks>
public interface IToastService
{
    // Minimal, modern public API (RenderFragment + component-based). String overloads are
    // provided via extension helpers in `IToastServiceExtensions`.

    /// <summary>
    /// Shows an information toast without waiting for it to close.
    /// </summary>
    /// <param name="message">A <see cref="RenderFragment"/> that renders the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    void ShowInfo(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows an information toast and returns the <see cref="ToastCloseReason"/> when it closes.
    /// </summary>
    /// <param name="message">A <see cref="RenderFragment"/> that renders the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    /// <returns>A task that completes with the close reason.</returns>
    Task<ToastCloseReason> ShowInfoAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows an information toast and returns a detailed <see cref="ToastResult"/> when it closes.
    /// </summary>
    /// <param name="message">A <see cref="RenderFragment"/> that renders the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    /// <returns>A task that completes with detailed information about the closed toast.</returns>
    Task<ToastResult> ShowInfoDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a success toast without waiting for it to close.
    /// </summary>
    void ShowSuccess(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a success toast and returns the close reason when it closes.
    /// </summary>
    Task<ToastCloseReason> ShowSuccessAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a success toast and returns a detailed result when it closes.
    /// </summary>
    Task<ToastResult> ShowSuccessDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a warning toast without waiting for it to close.
    /// </summary>
    void ShowWarning(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a warning toast and returns the close reason when it closes.
    /// </summary>
    Task<ToastCloseReason> ShowWarningAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a warning toast and returns a detailed result when it closes.
    /// </summary>
    Task<ToastResult> ShowWarningDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows an error toast without waiting for it to close.
    /// </summary>
    void ShowError(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows an error toast and returns the close reason when it closes.
    /// </summary>
    Task<ToastCloseReason> ShowErrorAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows an error toast and returns a detailed result when it closes.
    /// </summary>
    Task<ToastResult> ShowErrorDetailedAsync(RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a toast with the specified <paramref name="level"/> and content.
    /// </summary>
    /// <param name="level">The toast level (Info/Success/Warning/Error).</param>
    /// <param name="message">A <see cref="RenderFragment"/> that renders the toast content.</param>
    /// <param name="settings">Optional per-toast settings to override defaults.</param>
    void ShowToast(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a toast for the specified <paramref name="level"/> and returns the close reason when it closes.
    /// </summary>
    Task<ToastCloseReason> ShowToastAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a toast for the specified <paramref name="level"/> and returns a detailed result when it closes.
    /// </summary>
    Task<ToastResult> ShowToastDetailedAsync(ToastLevel level, RenderFragment message, Action<ToastSettings>? settings = null);

    /// <summary>
    /// Shows a toast that renders a Blazor component of type <typeparamref name="TComponent"/>.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render inside the toast.</typeparam>
    /// <param name="parameters">Optional parameters to pass to the component.</param>
    /// <param name="settings">Optional per-toast settings.</param>
    void ShowToast<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent;

    /// <summary>
    /// Shows a component-based toast and returns the close reason when it closes.
    /// </summary>
    Task<ToastCloseReason> ShowToastAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent;

    /// <summary>
    /// Shows a component-based toast and returns detailed information when it closes.
    /// </summary>
    Task<ToastResult> ShowToastDetailedAsync<TComponent>(ToastParameters? parameters = null, Action<ToastSettings>? settings = null) where TComponent : IComponent;

    // Clear APIs
    /// <summary>
    /// Removes all active toasts immediately (not queued ones only).
    /// Awaiting callers will receive <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    void ClearAll();

    /// <summary>
    /// Removes all active toasts for the specified <paramref name="toastLevel"/>.
    /// Awaiting callers will receive <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    void ClearToasts(ToastLevel toastLevel);

    /// <summary>
    /// Removes all custom component toasts.
    /// </summary>
    void ClearCustomToasts();

    /// <summary>
    /// Clears the queued (not-yet-shown) toasts. Awaiting callers are notified with <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    void ClearQueue();

    /// <summary>
    /// Clears queued toasts of the specified level.
    /// </summary>
    void ClearQueueToasts(ToastLevel toastLevel);
}
