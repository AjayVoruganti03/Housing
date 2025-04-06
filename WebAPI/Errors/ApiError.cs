using System.Text.Json;


namespace WebAPI.Errors{
    public class ApiError
    {

        public ApiError(int errorCode, string errormessage, string errorDetails = "")
        {
            ErrorCode = errorCode;
            ErrorMessage = errormessage;
            ErrorDetails = errorDetails;
        }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;    

        public string ErrorDetails { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

    }
}