using System.Diagnostics.CodeAnalysis;

namespace C0deGeek.Result;

/// <summary>
/// Represents the result of an operation, indicating success, failure, or not found status.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets the status of the result.
    /// </summary>
    private ResultStatus Status { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string Error { get; }
    
    /// <summary>
    /// Gets metadata about the operation result.
    /// </summary>
    public ResultMetadata Metadata { get; } = new();

    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="status">The status of the result.</param>
    /// <param name="error">The error message if the operation failed.</param>
    /// <exception cref="InvalidOperationException">Thrown when the status and error message are inconsistent.</exception>
    protected Result(ResultStatus status, string error)
    {
        switch (status)
        {
            case ResultStatus.Success when error != string.Empty:
                throw new InvalidOperationException("Success result cannot have an error message");
            case ResultStatus.Failure when error == string.Empty:
                throw new InvalidOperationException("Failure result must have an error message");
            case ResultStatus.NotFound:
            default:
                Status = status;
                Error = error;
                break;
        }
    }
    
    /// <summary>
    /// Creates a success result with warnings.
    /// </summary>
    public static Result OkWithWarnings(params string[] warnings)
    {
        var result = Ok();
        result.Metadata.Warnings.AddRange(warnings);
        return result;
    }
    
    /// <summary>
    /// Creates a result from multiple results, succeeding only if all succeed.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure()).ToList();
        if (failures.Count != 0)
        {
            return Fail(string.Join(Environment.NewLine, failures.Select(f => f.Error)));
        }

        var warnings = results.SelectMany(r => r.Metadata.Warnings).ToList();
        if (warnings.Count != 0)
        {
            return OkWithWarnings(warnings.ToArray());
        }

        return Ok();
    }

    /// <summary>
    /// Ensures the result is successful or throws with a custom exception factory.
    /// </summary>
    public Result EnsureSuccess(Func<string, Exception> exceptionFactory)
    {
        if (IsFailure())
        {
            throw exceptionFactory(Error);
        }
        return this;
    }
    
    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A failure result.</returns>
    public static Result Fail(string message) => new(ResultStatus.Failure, message);

    /// <summary>
    /// Creates a typed failure result with the specified error message.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="message">The error message.</param>
    /// <returns>A typed failure result.</returns>
    public static Result<T> Fail<T>(string message) where T : notnull =>
        new(default, ResultStatus.Failure, message);

    /// <summary>
    /// Determines if the result represents a successful operation.
    /// </summary>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public virtual bool IsSuccess(bool treatNotFoundAsFailure = true)
    {
        return treatNotFoundAsFailure ? Status != ResultStatus.NotFound : Status == ResultStatus.Success;
    }

    /// <summary>
    /// Determines if the result represents a failed operation.
    /// </summary>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>True if the operation failed; otherwise, false.</returns>
    public virtual bool IsFailure(bool treatNotFoundAsFailure = true)
    {
        return treatNotFoundAsFailure ? Status == ResultStatus.NotFound : Status != ResultStatus.Success;
    }

    /// <summary>
    /// Gets a value indicating whether the result represents a not found status.
    /// </summary>
    public bool IsNotFound => Status == ResultStatus.NotFound;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Ok() => new(ResultStatus.Success, string.Empty);

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value to include in the result.</param>
    /// <returns>A successful result containing the specified value.</returns>
    public static Result<T> Ok<T>(T value) where T : notnull =>
        new(value, ResultStatus.Success, string.Empty);

    /// <summary>
    /// Creates a not found result with the specified message.
    /// </summary>
    /// <param name="message">The message describing what was not found.</param>
    /// <returns>A not found result.</returns>
    public static Result NotFound(string message) =>
        new(ResultStatus.NotFound, message);

    /// <summary>
    /// Creates a not found result with the specified value and message.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value associated with the not found result.</param>
    /// <param name="message">The message describing what was not found.</param>
    /// <returns>A not found result containing the specified value.</returns>
    public static Result<T> NotFound<T>(T value, string message) where T : notnull =>
        new(value, ResultStatus.NotFound, message);
}

/// <summary>
/// Represents a result containing a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value contained in the result.</typeparam>
public class Result<T> : Result where T : notnull
{
    /// <summary>
    /// Gets the value contained in the result.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the Result{T} class.
    /// </summary>
    /// <param name="value">The value to include in the result.</param>
    /// <param name="status">The status of the result.</param>
    /// <param name="error">The error message if the operation failed.</param>
    protected internal Result(T? value, ResultStatus status, string error)
        : base(status, error)
    {
        Value = value;
    }

    /// <summary>
    /// Determines if the result represents a successful operation and contains a non-null value.
    /// </summary>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>True if the operation was successful and the value is non-null; otherwise, false.</returns>
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess(bool treatNotFoundAsFailure = true)
    {
        return base.IsSuccess(treatNotFoundAsFailure);
    }

    /// <summary>
    /// Determines if the result represents a failed operation or contains a null value.
    /// </summary>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>True if the operation failed or the value is null; otherwise, false.</returns>
    [MemberNotNullWhen(false, nameof(Value))]
    public override bool IsFailure(bool treatNotFoundAsFailure = true)
    {
        return base.IsFailure(treatNotFoundAsFailure);
    }
}