namespace C0deGeek.Result;

/// <summary>
/// Represents the status of a Result object.
/// </summary>
public enum ResultStatus
{
    /// <summary>
    /// Indicates that the operation was successful.
    /// </summary>
    Success,

    /// <summary>
    /// Indicates that the operation failed.
    /// </summary>
    Failure,

    /// <summary>
    /// Indicates that the requested resource was not found.
    /// </summary>
    NotFound
}
