using System.Text.Json.Serialization;

namespace CodingTest.Models
{
    public class StoryResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("postedBy")]
        public string PostedBy { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }
    }
}
