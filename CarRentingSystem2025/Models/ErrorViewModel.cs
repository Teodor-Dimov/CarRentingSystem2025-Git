namespace CarRentingSystem2025.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorDetails { get; set; }
        public int? StatusCode { get; set; }
        public string? OriginalPath { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);
        public bool ShowErrorDetails => !string.IsNullOrEmpty(ErrorDetails);
        
        public string GetStatusMessage()
        {
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                500 => "Internal Server Error",
                503 => "Service Unavailable",
                _ => "Error"
            };
        }
    }
}
