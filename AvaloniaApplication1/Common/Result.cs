using System;

namespace AvaloniaApplication1.Common;

public readonly struct Result<TValue, TError>
{
    public bool IsSuccess { get; }
    
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Result is not successful but tried to get value");
    
    private readonly TError? _error;
    
    public TError Error => !IsSuccess 
            ? _error! 
            : throw new InvalidOperationException("Result is successful but tried to get error");
    
    private Result(bool isSuccess, TValue? value = default, TError? error = default)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }
    

    public static Result<TValue, TError> Success(TValue value) => new(true, value);
    
    public static Result<Unit, TError> Success() => Result<Unit, TError>.Success(Unit.Value);

    public static Result<TValue, TError> Failure(TError error) => new(false, error: error);
}