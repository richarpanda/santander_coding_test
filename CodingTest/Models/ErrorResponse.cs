using System.Text.Json.Serialization;

namespace CodingTest.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}
