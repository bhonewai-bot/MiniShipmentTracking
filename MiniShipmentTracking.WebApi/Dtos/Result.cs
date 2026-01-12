namespace MiniShipmentTracking.WebApi.Dtos;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    
    public EnumResponseType Type { get; set; }
    
    public T Data { get; set; }
    
    public string Message { get; set; }

    public bool IsValidatorError
    {
        get { return Type == EnumResponseType.ValidationError; }
    }

    public bool IsSystemError
    {
        get { return Type == EnumResponseType.SystemError; }
    }

    public static Result<T> Success(T data, string message = "Success.")
    {
        return new Result<T>()
        {
            IsSuccess = true,
            Data = data,
            Type = EnumResponseType.Success,
            Message = message
        };
    }

    public static Result<T> ValidationError(string message, T? data = default)
    {
        return new Result<T>()
        {
            IsSuccess = false,
            Data = data,
            Type = EnumResponseType.ValidationError,
            Message = message
        };
    }
    
    public static Result<T> SystemError(string message, T? data = default)
    {
        return new Result<T>()
        {
            IsSuccess = false,
            Type = EnumResponseType.SystemError,
            Data = data,
            Message = message
        };
    }
}

public enum EnumResponseType
{
    None,
    Success,
    ValidationError,
    SystemError
}