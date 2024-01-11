namespace WiseSwitchWpf.Services.Api
{
    public class ApiErrorResponse : ApiResponse
    {
        public static ApiErrorResponse ApiCallFailed =>
            new ApiErrorResponse
            {
                IsSuccess = false,
                StatusCode = 500,
                Content = "API call failed.",
            };
    }
}
