namespace C0deGeek.Result;

public static class ResultExtensions
{
    public static void ThrowFunctionalExceptionOnFailure(this Result result, bool treatNotFoundAsFailure = true)
    {
        if (result.IsFailure(treatNotFoundAsFailure))
        {
            throw new FunctionalException(result.Error);
        }
    }

    public static async Task<T> GetValueOrThrowFunctionalException<T>(this Task<Result<T>> resultTask)
        where T : notnull
    {
        var result = await resultTask;
        return result.GetValueOrThrowFunctionalException();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static T GetValueOrThrowFunctionalException<T>(this Result<T> result) where T : notnull
    {
        if (result.IsFailure())
        {
            throw new FunctionalException(result.Error);
        }

        return result.Value;
    }

    public static Result Combine(this Result initialResult, Result nextResult, bool treatNotFoundAsFailure = false)
    {
        return initialResult.IsFailure(treatNotFoundAsFailure) ? initialResult : nextResult;
    }

    public static Result<T> Combine<T>(this Result initialResult, Result<T> nextResult,
        bool treatNotFoundAsFailure = false) where T : notnull
    {
        return initialResult.IsFailure(treatNotFoundAsFailure) ? Result.Fail<T>(initialResult.Error) : nextResult;
    }

    public static Result OnSuccess<T>(this Result<T> initialResult, Func<T, Result> nextResult) where T : notnull
    {
        return initialResult.IsFailure() ? initialResult : nextResult(initialResult.Value);
    }

    public static async Task<Result<TResult>> OnSuccessAsync<TInput, TResult>(
        this Task<Result<TInput>> initialResultTask, Func<TInput, Result<TResult>> nextResult)
        where TInput : notnull
        where TResult : notnull
    {
        var initialResult = await initialResultTask;

        return initialResult.IsFailure() ? Result.Fail<TResult>(initialResult.Error) : nextResult(initialResult.Value);
    }
}