using System;
using Blazor.Toast.Configuration;
using Microsoft.AspNetCore.Components;

namespace Blazor.Toast.Services;

/// <summary>
/// Provides internal event hooks used by the toast infrastructure.
/// </summary>
/// <remarks>
/// <para>
/// This interface is not part of the public API surface. It defines the 
/// event-based communication channel between the <c>IToastService</c> 
/// implementation and the <c>Toasts</c> rendering component.
/// </para>
/// <para>
/// The separation between the public service contract and this internal
/// event mechanism helps keep the external API small, stable, and easier
/// to evolve without breaking consumers.
/// </para>
/// </remarks>
internal interface IToastServiceEvents
{
    /// <summary>
    /// Raised to request that a simple (non-awaitable) toast be shown.
    /// Parameters: <see cref="ToastLevel"/>, <see cref="RenderFragment"/> message, optional per-toast settings.
    /// </summary>
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?>? OnShow;

    /// <summary>
    /// Raised to request that an awaitable toast be shown. The attached <see cref="TaskCompletionSource{ToastCloseReason}"/>
    /// is completed when the toast is closed with the corresponding <see cref="ToastCloseReason"/>.
    /// </summary>
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? OnShowAsync;

    /// <summary>
    /// Raised to request that an awaitable toast be shown which returns a detailed <see cref="ToastResult"/>.
    /// The attached <see cref="TaskCompletionSource{ToastResult}"/> is completed when the toast is closed.
    /// </summary>
    event Action<ToastLevel, RenderFragment, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? OnShowDetailedAsync;

    /// <summary>
    /// Raised to request that all active toasts be cleared programmatically.
    /// Awaiting callers will be notified with <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    event Action? OnClearAll;

    /// <summary>
    /// Raised to request that active toasts for a specific <see cref="ToastLevel"/> be cleared.
    /// </summary>
    event Action<ToastLevel>? OnClearToasts;

    /// <summary>
    /// Raised to request that custom (component) toasts be cleared.
    /// </summary>
    event Action? OnClearCustomToasts;

    /// <summary>
    /// Raised to request showing a component-based toast. Parameters: component <see cref="Type"/>, optional
    /// <see cref="ToastParameters"/>, and optional per-toast settings.
    /// </summary>
    event Action<Type, ToastParameters?, Action<ToastSettings>?>? OnShowComponent;

    /// <summary>
    /// Raised to request showing an awaitable component-based toast that returns a <see cref="ToastCloseReason"/>.
    /// </summary>
    event Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastCloseReason>?>? OnShowComponentAsync;

    /// <summary>
    /// Raised to request showing an awaitable component-based toast that returns a detailed <see cref="ToastResult"/>.
    /// </summary>
    event Action<Type, ToastParameters?, Action<ToastSettings>?, TaskCompletionSource<ToastResult>?>? OnShowComponentDetailedAsync;

    /// <summary>
    /// Raised to request clearing of all queued (not yet shown) toasts.
    /// Awaiting callers for queued toasts will be completed with <see cref="ToastCloseReason.Programmatic"/>.
    /// </summary>
    event Action? OnClearQueue;

    /// <summary>
    /// Raised to request clearing queued toasts for a specific <see cref="ToastLevel"/>.
    /// </summary>
    event Action<ToastLevel>? OnClearQueueToasts;
}
