namespace OrderService.Application.Common;

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Ok() => new() { Success = true };
    public static Result Ok(string message) => new() { Success = true, Message = message };
    public static Result Failure(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };
}

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result<T> Ok(T data, string message = "") =>
        new() { Success = true, Data = data, Message = message };
    public static Result<T> Failure(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new() };
}
