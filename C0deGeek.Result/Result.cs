using System.Diagnostics.CodeAnalysis;

namespace C0deGeek.Result
{
    public class Result
    {
        private ResultStatus Status { get; }
        public string Error { get; }

        protected Result(ResultStatus status, string error)
        {
            switch (status)
            {
                case ResultStatus.Success when error != string.Empty:
                    throw new InvalidOperationException();
                case ResultStatus.Failure when error == string.Empty:
                    throw new InvalidOperationException();
                case ResultStatus.NotFound:
                default:
                    Status = status;
                    Error = error;
                    break;
            }
        }

        protected virtual bool IsSuccess(bool treatNotFoundAsFailure = true)
        {
            return treatNotFoundAsFailure ? Status != ResultStatus.NotFound : Status == ResultStatus.Success;
        }

        public virtual bool IsFailure(bool treatNotFoundAsFailure = true)
        {
            return treatNotFoundAsFailure ? Status == ResultStatus.NotFound : Status != ResultStatus.Success;
        }

        public bool IsNotFound => Status == ResultStatus.NotFound;

        public static Result Fail(string message)
        {
            return new Result(ResultStatus.Failure, message);
        }

        public static Result<T> Fail<T>(string message) where T : notnull
        {
            return new Result<T>(default, ResultStatus.Failure, message);
        }

        public static Result Ok()
        {
            return new Result(ResultStatus.Success, string.Empty);
        }

        public static Result<T> Ok<T>(T value) where T : notnull
        {
            return new Result<T>(value, ResultStatus.Success, string.Empty);
        }

        public static Result NotFound(string message)
        {
            return new Result(ResultStatus.NotFound, message);
        }

        public static Result<T> NotFound<T>(T value, string message) where T : notnull
        {
            return new Result<T>(value, ResultStatus.NotFound, message);
        }
    }

    public class Result<T> : Result where T : notnull
    {
        public T? Value { get; }

        protected internal Result(T? value, ResultStatus status, string error)
            : base(status, error)
        {
            Value = value;
        }

        [MemberNotNullWhen(true, nameof(Value))]
        protected override bool IsSuccess(bool treatNotFoundAsFailure = true)
        {
            return base.IsSuccess(treatNotFoundAsFailure);
        }

        [MemberNotNullWhen(false, nameof(Value))]
        public override bool IsFailure(bool treatNotFoundAsFailure = true)
        {
            return base.IsFailure(treatNotFoundAsFailure);
        }
    }
}