using C0deGeek.Result.Exceptions;

namespace C0deGeek.Result.Extensions;

/// <summary>
/// Provides extension methods for the Result class.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Throws a FunctionalException if the result represents a failure.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <exception cref="FunctionalException">Thrown when the result represents a failure.</exception>
    public static void ThrowFunctionalExceptionOnFailure(this Result result, bool treatNotFoundAsFailure = true)
    {
        if (result.IsFailure(treatNotFoundAsFailure))
        {
            throw new FunctionalException(result.Error);
        }
    }

    /// <summary>
    /// Gets the value from an asynchronous result or throws a FunctionalException if the result represents a failure.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="resultTask">The task representing the asynchronous result.</param>
    /// <returns>The value contained in the result if successful.</returns>
    /// <exception cref="FunctionalException">Thrown when the result represents a failure.</exception>
    public static async Task<T> GetValueOrThrowFunctionalException<T>(this Task<Result<T>> resultTask)
        where T : notnull
    {
        var result = await resultTask;
        return result.GetValueOrThrowFunctionalException();
    }

    /// <summary>
    /// Gets the value from a result or throws a FunctionalException if the result represents a failure.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="result">The result containing the value.</param>
    /// <returns>The value contained in the result if successful.</returns>
    /// <exception cref="FunctionalException">Thrown when the result represents a failure.</exception>
    public static T GetValueOrThrowFunctionalException<T>(this Result<T> result) where T : notnull
    {
        if (result.IsFailure())
        {
            throw new FunctionalException(result.Error);
        }

        return result.Value;
    }

    /// <summary>
    /// Combines two results, where the second result is only evaluated if the first is successful.
    /// </summary>
    /// <param name="initialResult">The first result to evaluate.</param>
    /// <param name="nextResult">The second result to evaluate if the first is successful.</param>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>The combined result.</returns>
    public static Result Combine(this Result initialResult, Result nextResult, bool treatNotFoundAsFailure = false)
    {
        return initialResult.IsFailure(treatNotFoundAsFailure) ? initialResult : nextResult;
    }

    /// <summary>
    /// Combines an initial result with a typed result, where the second result is only evaluated if the first is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value in the second result.</typeparam>
    /// <param name="initialResult">The first result to evaluate.</param>
    /// <param name="nextResult">The second result to evaluate if the first is successful.</param>
    /// <param name="treatNotFoundAsFailure">If true, treats NotFound status as a failure.</param>
    /// <returns>The combined result.</returns>
    public static Result<T> Combine<T>(this Result initialResult, Result<T> nextResult,
        bool treatNotFoundAsFailure = false) where T : notnull
    {
        return initialResult.IsFailure(treatNotFoundAsFailure) ? Result.Fail<T>(initialResult.Error) : nextResult;
    }

    /// <summary>
    /// Executes a function on the value of a successful result.
    /// </summary>
    /// <typeparam name="T">The type of the value in the initial result.</typeparam>
    /// <param name="initialResult">The result containing the value.</param>
    /// <param name="nextResult">The function to execute on the value.</param>
    /// <returns>The result of the executed function or the initial failure.</returns>
    public static Result OnSuccess<T>(this Result<T> initialResult, Func<T, Result> nextResult) where T : notnull
    {
        return initialResult.IsFailure() ? initialResult : nextResult(initialResult.Value);
    }

    /// <summary>
    /// Executes an asynchronous function on the value of a successful result.
    /// </summary>
    /// <typeparam name="TInput">The type of the value in the initial result.</typeparam>
    /// <typeparam name="TResult">The type of the value in the final result.</typeparam>
    /// <param name="initialResultTask">The task representing the asynchronous initial result.</param>
    /// <param name="nextResult">The function to execute on the value.</param>
    /// <returns>The result of the executed function or the initial failure.</returns>
    public static async Task<Result<TResult>> OnSuccessAsync<TInput, TResult>(
        this Task<Result<TInput>> initialResultTask,
        Func<TInput, Result<TResult>> nextResult)
        where TInput : notnull
        where TResult : notnull
    {
        var initialResult = await initialResultTask;
        return initialResult.IsFailure()
            ? Result.Fail<TResult>(initialResult.Error)
            : nextResult(initialResult.Value);
    }


    /// <summary>
    /// Executes an action only if the result is successful.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <param name="action">The action to execute if successful.</param>
    /// <returns>The original result.</returns>
    public static Result Tap(this Result result, Action action)
    {
        if (result.IsSuccess())
        {
            action();
        }

        return result;
    }

    /// <summary>
    /// Executes an action with the value if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the value in the result.</typeparam>
    /// <param name="result">The result containing the value.</param>
    /// <param name="action">The action to execute with the value if successful.</param>
    /// <returns>The original result.</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action) where T : notnull
    {
        if (result.IsSuccess())
        {
            action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Maps a Result{T} to a Result{TNew} using a mapping function.
    /// </summary>
    /// <typeparam name="T">The source type of the result value.</typeparam>
    /// <typeparam name="TNew">The target type to map the result value to.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <param name="mapper">The function to map the value from T to TNew.</param>
    /// <returns>A new result containing the mapped value if successful, or the original error if failed.</returns>
    public static Result<TNew> Map<T, TNew>(this Result<T> result, Func<T, TNew> mapper)
        where T : notnull
        where TNew : notnull
    {
        return result.IsSuccess()
            ? Result.Ok(mapper(result.Value))
            : Result.Fail<TNew>(result.Error);
    }

    /// <summary>
    /// Adds context information to the result.
    /// </summary>
    /// <param name="result">The result to add context to.</param>
    /// <param name="key">The key for the context information.</param>
    /// <param name="value">The context value to add.</param>
    /// <returns>The original result with added context.</returns>
    public static Result WithContext(this Result result, string key, object value)
    {
        result.Metadata.Context[key] = value;
        return result;
    }
}