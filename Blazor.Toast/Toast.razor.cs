using Blazor.Toast.Configuration;
using Blazor.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Blazor.Toast;

public partial class Toast : IDisposable
{
    private CountdownTimer? _countdownTimer;
    private int _progress = 100;
    [CascadingParameter] private Toasts ToastsContainer { get; set; } = default!;

    [Parameter] [EditorRequired] public Guid ToastId { get; set; }
    [Parameter] [EditorRequired] public ToastSettings Settings { get; set; } = default!;
    [Parameter] public ToastLevel? Level { get; set; }
    [Parameter] public RenderFragment? Message { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private RenderFragment? CloseButtonContent => ToastsContainer.CloseButtonContent;

    public void Dispose()
    {
        _countdownTimer?.Dispose();
        _countdownTimer = null;
    }

    protected override async Task OnInitializedAsync()
    {
        if (Settings.DisableTimeout ?? false) return;

        if (Settings.ShowProgressBar!.Value)
            _countdownTimer = new CountdownTimer(Settings.Timeout, Settings.ExtendedTimeout!.Value)
                .OnTick(CalculateProgressAsync)
                .OnElapsed(CloseByTimeout);
        else
            _countdownTimer = new CountdownTimer(Settings.Timeout, Settings.ExtendedTimeout!.Value)
                .OnElapsed(CloseByTimeout);

        await _countdownTimer.StartAsync();
    }

    /// <summary>
    ///     Closes the toast
    /// </summary>
    public void Close()
    {
        ToastsContainer.RemoveToast(ToastId, Configuration.ToastCloseReason.CloseButton);
    }

    private void CloseByTimeout()
    {
        ToastsContainer.RemoveToast(ToastId, Configuration.ToastCloseReason.Timeout);
    }

    private void TryPauseCountdown()
    {
        if (Settings.PauseProgressOnHover!.Value)
        {
            Settings.ShowProgressBar = false;
            _countdownTimer?.Pause();
        }
    }

    private void TryResumeCountdown()
    {
        if (Settings.PauseProgressOnHover!.Value)
        {
            Settings.ShowProgressBar = true;
            _countdownTimer?.UnPause();
        }
    }

    private async Task CalculateProgressAsync(int percentComplete)
    {
        _progress = 100 - percentComplete;
        await InvokeAsync(StateHasChanged);
    }

    private void ToastClick()
    {
        try
        {
            Settings.OnClick?.Invoke();
        }
        finally
        {
            // Ensure clicking the toast body closes it and reports Click reason
            ToastsContainer.RemoveToast(ToastId, Configuration.ToastCloseReason.Click);
        }
    }

    private bool ShowIconDiv()
    {
        return Settings.IconType switch
        {
            IconType.None => false,
            IconType.Default => true,
            IconType.FontAwesome => !string.IsNullOrWhiteSpace(Settings.Icon),
            IconType.Material => !string.IsNullOrWhiteSpace(Settings.Icon),
            _ => false
        };
    }
}