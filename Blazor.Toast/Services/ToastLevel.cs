namespace Blazor.Toast.Services;

/// <summary>
/// Specifies the level (severity) of a toast notification.
/// </summary>
public enum ToastLevel
{
    /// <summary>
    /// Informational message (neutral tone).
    /// </summary>
    Info,

    /// <summary>
    /// Indicates a successful operation.
    /// </summary>
    Success,

    /// <summary>
    /// Indicates a warning that may require attention.
    /// </summary>
    Warning,

    /// <summary>
    /// Indicates an error or failure.
    /// </summary>
    Error
}
