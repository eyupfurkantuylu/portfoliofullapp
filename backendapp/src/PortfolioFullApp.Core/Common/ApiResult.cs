namespace PortfolioFullApp.Core.Common;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }

    public ApiResult()
    {
        Success = true;
        Errors = new List<string>();
    }

    public static ApiResult<T> SuccessResult(T data, string message = "İşlem başarılı")
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResult<T> ErrorResult(string message, List<string> errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}