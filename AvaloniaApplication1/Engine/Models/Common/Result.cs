namespace AvaloniaApplication1.Engine.Models.Common;

public readonly struct Result<TValue, TError>
{
    public TValue? Value { get; }
    
    public TError? Error { get; }
    
    public bool IsSuccess { get; }
    
    private Result(TValue? value)
    {
        Value = value;
        Error = default;
        IsSuccess = true;
    }

    private Result(TError error)
    {
        Value = default;
        Error = error;
        IsSuccess = false;
    }

    public static Result<TValue, TError> Success(TValue value) => new(value);
    
    public static Result<Unit, TError> Success() => new(Unit.Value);

    public static Result<TValue, TError> Failure(TError error) => new(error);
}