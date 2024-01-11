using System.Net.Http;
using System.Net.Http.Headers;

namespace WiseSwitchWpf.Services.Api
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseAddress;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _apiBaseAddress = "https://localhost:7179/";

            ApiServiceConfiguration();
        }

        private void ApiServiceConfiguration()
        {
            // Headers.
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Set timeout for the request.
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ApiResponse> GetDataAsync(string apiEndpoint)
        {
            HttpResponseMessage httpResponseMessage;

            // Try Get.
            try
            {
                httpResponseMessage = await _httpClient.GetAsync(_apiBaseAddress + apiEndpoint);
            }
            catch
            {
                return ApiErrorResponse.ApiCallFailed;
            }

            return new ApiResponse
            {
                IsSuccess = httpResponseMessage.IsSuccessStatusCode,
                StatusCode = (int)httpResponseMessage.StatusCode,
                Content = await httpResponseMessage.Content.ReadAsStringAsync(),
            };
        }
    }
}
