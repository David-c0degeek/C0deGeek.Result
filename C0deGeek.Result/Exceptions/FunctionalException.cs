namespace C0deGeek.Result.Exceptions;

/// <summary>
/// Represents an exception that occurs during functional operations.
/// This exception is typically thrown when a Result object represents a failure.
/// </summary>
public class FunctionalException : Exception
{
    /// <summary>
    /// Initializes a new instance of the FunctionalException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FunctionalException(string message) : base(message)
    {
    }
}