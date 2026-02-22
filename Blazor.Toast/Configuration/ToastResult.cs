using System;

namespace Blazor.Toast.Configuration;

/// <summary>
/// Detailed result returned when awaiting a toast closure.
/// </summary>
public class ToastResult
{
    public Guid ToastId { get; init; }
    public ToastCloseReason Reason { get; init; }
    public DateTime ShownAt { get; init; }
    public DateTime ClosedAt { get; init; }

    public TimeSpan Duration => ClosedAt - ShownAt;
}
