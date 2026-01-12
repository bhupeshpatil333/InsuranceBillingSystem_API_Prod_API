namespace InsuranceBillingSystem_API_Prod.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> SuccessResponse(string message, T data)
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> ErrorResponse(string message)
            => new() { Success = false, Message = message, Data = default };
    }
}
