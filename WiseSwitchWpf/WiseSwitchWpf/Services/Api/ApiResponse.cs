namespace WiseSwitchWpf.Services.Api
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

        public string Content { get; set; }
    }
}
