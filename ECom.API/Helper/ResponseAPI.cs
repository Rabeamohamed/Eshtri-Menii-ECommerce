namespace ECom.API.Helper
{
    public class ResponseAPI
    {
        public ResponseAPI(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message?? GetMessageFromStatusCode(StatusCode);
        }
        private string GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done",
                201 => "Created",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                _=> null
            };
        }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        
    }
}
