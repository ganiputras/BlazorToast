namespace Blazor.Toast.Configuration;

/// <summary>
/// Reason why a toast was closed. Used to signal ShowAsync callers.
/// </summary>
public enum ToastCloseReason
{
    Unknown = 0,
    Timeout = 1,
    CloseButton = 2,
    Click = 3,
    Programmatic = 4
}
