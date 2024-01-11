namespace WiseSwitchWpf.Services.Data
{
    public class DataResponse
    {
        public bool IsSuccess { get; set; }

        public bool IsClientError { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }


        public static DataResponse Success(object result, string message = null)
        {
            return new DataResponse
            {
                IsSuccess = true,
                Message = message,
                Result = result,
            };
        }

        public static DataResponse Error(string message, bool isClientError = false, object result = null)
        {
            return new DataResponse
            {
                IsSuccess = false,
                IsClientError = isClientError,
                Message = message,
                Result = result,
            };
        }
    }
}
