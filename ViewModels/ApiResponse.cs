namespace WebApplication1.ViewModels
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public object? Errors { get; set; }

        public static ApiResponse Ok(object? data = null, string? message = null)
        {
            return new ApiResponse { Success = true, Data = data, Message = message };
        }

        public static ApiResponse Fail(string message, object? errors = null)
        {
            return new ApiResponse { Success = false, Message = message, Errors = errors };
        }
    }
}
