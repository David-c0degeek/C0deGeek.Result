namespace C0deGeek.Result;

/// <summary>
/// Represents additional metadata about an operation's result.
/// </summary>
public class ResultMetadata
{
    /// <summary>
    /// Gets the timestamp when the result was created.
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets any warnings that occurred during the operation.
    /// </summary>
    public List<string> Warnings { get; } = [];

    /// <summary>
    /// Gets additional context about the operation.
    /// </summary>
    public Dictionary<string, object> Context { get; } = new();
}