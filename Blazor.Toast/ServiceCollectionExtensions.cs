using Blazor.Toast.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Toast;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorToast(this IServiceCollection services)
    {
        return services.AddScoped<IToastService, ToastService>();
    }
}