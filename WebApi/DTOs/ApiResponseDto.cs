namespace ProductCatalogAPI.DTOs;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponseDto<T> SuccessResult(T? data, string message = "Success")
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponseDto<T> ErrorResult(string error, string message = "Something went wrong")
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Error = error
        };
    }
}