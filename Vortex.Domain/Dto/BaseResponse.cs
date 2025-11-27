namespace Vortex.Domain.Dto;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    // Helper for Success
    public static BaseResponse<T> SuccessResponse(T data, string message = "Operation successful")
    {
        return new BaseResponse<T> { Success = true, Data = data, Message = message };
    }

    // Helper for Failure
    public static BaseResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null)
    {
        return new BaseResponse<T> { Success = false, Message = message, Errors = errors };
    }
}