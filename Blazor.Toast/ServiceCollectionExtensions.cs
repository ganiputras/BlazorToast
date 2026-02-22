using Blazor.Toast.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Toast;


/// <summary>
/// Extension methods to register Blazor.Toast services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the toast service and required interfaces.
    /// Use this in your host app to enable toasts.
    /// </summary>
    public static IServiceCollection AddBlazorToast(this IServiceCollection services)
    {
        // Register concrete ToastService as a singleton and expose it as both IToastService and internal IToastServiceEvents
        services.AddSingleton<ToastService>();
        services.AddSingleton<IToastService>(sp => sp.GetRequiredService<ToastService>());
        services.AddSingleton<IToastServiceEvents>(sp => sp.GetRequiredService<ToastService>());

        return services;
    }
}