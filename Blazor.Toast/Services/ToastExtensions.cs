using Microsoft.AspNetCore.Components;
using Blazor.Toast.Configuration;

namespace Blazor.Toast.Services
{
    /// <summary>
    /// Convenience extension methods that accept string messages and wrap them as <see cref="RenderFragment"/>.
    /// These helpers make it easy to migrate existing code that passes plain strings.
    /// </summary>
    public static class ToastExtensions
    {
        // Info
        public static void ShowInfo(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowInfo(builder => builder.AddContent(0, message), settings);

        public static Task<ToastCloseReason> ShowInfoAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowInfoAsync(builder => builder.AddContent(0, message), settings);

        public static Task<ToastResult> ShowInfoDetailedAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowInfoDetailedAsync(builder => builder.AddContent(0, message), settings);

        // Success
        public static void ShowSuccess(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowSuccess(builder => builder.AddContent(0, message), settings);

        public static Task<ToastCloseReason> ShowSuccessAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowSuccessAsync(builder => builder.AddContent(0, message), settings);

        public static Task<ToastResult> ShowSuccessDetailedAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowSuccessDetailedAsync(builder => builder.AddContent(0, message), settings);

        // Warning
        public static void ShowWarning(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowWarning(builder => builder.AddContent(0, message), settings);

        public static Task<ToastCloseReason> ShowWarningAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowWarningAsync(builder => builder.AddContent(0, message), settings);

        public static Task<ToastResult> ShowWarningDetailedAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowWarningDetailedAsync(builder => builder.AddContent(0, message), settings);

        // Error
        public static void ShowError(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowError(builder => builder.AddContent(0, message), settings);

        public static Task<ToastCloseReason> ShowErrorAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowErrorAsync(builder => builder.AddContent(0, message), settings);

        public static Task<ToastResult> ShowErrorDetailedAsync(this IToastService service, string message, Action<ToastSettings>? settings = null)
            => service.ShowErrorDetailedAsync(builder => builder.AddContent(0, message), settings);

        // Generic level-based
        public static void ShowToast(this IToastService service, ToastLevel level, string message, Action<ToastSettings>? settings = null)
            => service.ShowToast(level, builder => builder.AddContent(0, message), settings);

        public static Task<ToastCloseReason> ShowToastAsync(this IToastService service, ToastLevel level, string message, Action<ToastSettings>? settings = null)
            => service.ShowToastAsync(level, builder => builder.AddContent(0, message), settings);

        public static Task<ToastResult> ShowToastDetailedAsync(this IToastService service, ToastLevel level, string message, Action<ToastSettings>? settings = null)
            => service.ShowToastDetailedAsync(level, builder => builder.AddContent(0, message), settings);
    }
}
