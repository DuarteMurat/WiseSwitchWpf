using Newtonsoft.Json;
using WiseSwitchWpf.Services.Api;

namespace WiseSwitchWpf.Services.Data
{
    public class DataService
    {
        private readonly ApiService _apiService;

        public DataService(ApiService apiService)
        {
            _apiService = apiService;
        }


        // Get data.
        public async Task<DataResponse> GetAsync<T>(string url, object value = null)
        {
            var apiResponse = await _apiService.GetDataAsync(url + value);

            if (apiResponse.IsSuccess)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<T>(apiResponse.Content);

                    return DataResponse.Success(result);
                }
                catch
                {
                    return DataResponse.Error("Could not deserialize JSON object from the API response.");
                }
            }

            var message = apiResponse.Content;
            var isClientError = IsClientError(apiResponse.StatusCode);

            return DataResponse.Error(message, isClientError);
        }

        private bool IsClientError(int statusCode)
        {
            return statusCode >= 400 && statusCode <= 499;
        }
    }
}
