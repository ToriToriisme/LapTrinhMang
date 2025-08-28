namespace BlogApi.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty; // tránh lỗi non-nullable
        public T? Data { get; set; }                        // cho phép null

        public static ApiResponse<T> Ok(T? data, string message = "Success") =>
            new ApiResponse<T> { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message) =>
            new ApiResponse<T> { Success = false, Message = message, Data = default };
    }
}
