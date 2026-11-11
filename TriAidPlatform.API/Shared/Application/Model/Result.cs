namespace TriAidPlatform.API.Shared.Application.Model;

/// <summary>
///     Represents a domain error.
/// </summary>
/// <param name="Code">The unique error code.</param>
/// <param name="Message">The error message.</param>
/// <param name="Type">The error classification type.</param>
public record AppError(string Code, string Message, string Type = "Failure")
{
    public static readonly AppError None = new(string.Empty, string.Empty);
    public static AppError Failure(string code, string message) => new(code, message, "Failure");
    public static AppError Unauthorized(string code, string message) => new(code, message, "Unauthorized");
    public static AppError NotFound(string code, string message) => new(code, message, "NotFound");
    public static AppError Conflict(string code, string message) => new(code, message, "Conflict");
    public static AppError Forbidden(string code, string message) => new(code, message, "Forbidden");
}

/// <summary>
///     Generic Result class for Command Handlers in the Application Layer.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class Result<T>
{
    protected Result(bool isSuccess, T? value, string message, AppError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        DomainError = error ?? (isSuccess ? AppError.None : AppError.Failure("Error", message));
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string Message { get; }
    public AppError DomainError { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, string.Empty, AppError.None);
    }

    public static Result<T> Failure(string code, string message)
    {
        return new Result<T>(false, default, message, AppError.Failure(code, message));
    }

    public static Result<T> Failure(AppError error)
    {
        return new Result<T>(false, default, error.Message, error);
    }

    public static Result<T> Failure(Enum errorEnum, string message)
    {
        return new Result<T>(false, default, message, AppError.Failure(errorEnum.ToString(), message));
    }

    public static Result<T> Failure(Domain.Model.Errors.Error domainError, Enum errorEnum)
    {
        return new Result<T>(false, default, domainError.Message, AppError.Failure(domainError.Code, domainError.Message));
    }

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<AppError, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(DomainError);
}

/// <summary>
///     Non-generic Result class for Command Handlers.
/// </summary>
public class Result : Result<object>
{
    private Result(bool isSuccess, string message, AppError? error)
        : base(isSuccess, null, message, error)
    {
    }

    public static Result Success()
    {
        return new Result(true, string.Empty, AppError.None);
    }

    public static Result Failure(string code, string message)
    {
        return new Result(false, message, AppError.Failure(code, message));
    }

    public static Result Failure(AppError error)
    {
        return new Result(false, error.Message, error);
    }

    public static Result Failure(Enum errorEnum, string message)
    {
        return new Result(false, message, AppError.Failure(errorEnum.ToString(), message));
    }

    public static Result Failure(Domain.Model.Errors.Error domainError, Enum errorEnum)
    {
        return new Result(false, domainError.Message, AppError.Failure(domainError.Code, domainError.Message));
    }
}
