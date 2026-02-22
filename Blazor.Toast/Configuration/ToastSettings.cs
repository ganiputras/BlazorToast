namespace Blazor.Toast.Configuration;

public class ToastSettings
{
    /// <summary>
    /// Creates a new instance of <see cref="ToastSettings"/> with explicit values.
    /// </summary>
    /// <param name="additionalClasses">Additional CSS classes applied to the toast element.</param>
    /// <param name="iconType">Optional icon type to use for the toast.</param>
    /// <param name="icon">Optional icon name (for FontAwesome/Material).</param>
    /// <param name="showProgressBar">Whether to show the progress bar.</param>
    /// <param name="showCloseButton">Whether to show the close button.</param>
    /// <param name="onClick">Optional action invoked when the toast body is clicked.</param>
    /// <param name="timeout">Timeout in seconds before auto-close.</param>
    /// <param name="disableTimeout">When true, the toast will not auto-close.</param>
    /// <param name="pauseProgressOnHover">When true, hovering pauses progress/timer.</param>
    /// <param name="extendedTimeout">Extended timeout (seconds) after hover unpause.</param>
    /// <param name="toastPosition">Optional per-toast position override.</param>
    public ToastSettings(
        string additionalClasses,
        IconType? iconType,
        string icon,
        bool showProgressBar,
        bool showCloseButton,
        Action? onClick,
        int timeout,
        bool disableTimeout,
        bool pauseProgressOnHover,
        int extendedTimeout,
        ToastPosition? toastPosition)
    {
        AdditionalClasses = additionalClasses;
        IconType = iconType;
        Icon = icon;
        ShowProgressBar = showProgressBar;
        ShowCloseButton = showCloseButton;
        OnClick = onClick;
        Timeout = timeout;
        DisableTimeout = disableTimeout;
        PauseProgressOnHover = pauseProgressOnHover;
        ExtendedTimeout = extendedTimeout;
        Position = toastPosition;

        if (onClick is not null) AdditionalClasses += " blazor-toast-action";
    }

    internal ToastSettings()
    {
    }

    /// <summary>
    /// Additional CSS classes applied to the toast element.
    /// </summary>
    /// <remarks>
    /// Provide one or more CSS class names separated by spaces to customize toast styling.
    /// </remarks>
    public string AdditionalClasses { get; set; }

    /// <summary>
    /// Icon name (FontAwesome / Material) to use for the toast.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Optional icon type selection for the toast.
    /// </summary>
    public IconType? IconType { get; set; }

    /// <summary>
    /// When true, a progress bar is shown reflecting remaining timeout.
    /// </summary>
    public bool? ShowProgressBar { get; set; }

    /// <summary>
    /// When true, hovering the toast will pause the progress/timer (if supported).
    /// </summary>
    public bool? PauseProgressOnHover { get; set; }

    /// <summary>
    /// When true, a close button is displayed on the toast.
    /// </summary>
    public bool? ShowCloseButton { get; set; }

    /// <summary>
    /// Optional callback executed when the toast body is clicked.
    /// </summary>
    public Action? OnClick { get; set; }

    /// <summary>
    /// Timeout in seconds before the toast auto-closes. A value of 0 means "use global default".
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// When set, number of seconds to wait after hover unpause before closing.
    /// </summary>
    public int? ExtendedTimeout { get; set; }

    /// <summary>
    /// When true, prevents automatic closing (shows close button if enabled).
    /// </summary>
    public bool? DisableTimeout { get; set; }

    /// <summary>
    /// Optional per-toast position override.
    /// </summary>
    public ToastPosition? Position { get; set; }

    internal string PositionClass => $"position-{Position?.ToString().ToLower()}";
}
