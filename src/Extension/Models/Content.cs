using Newtonsoft.Json;

namespace Extension.Models;

public class Content
{
    [JsonProperty("body")]
    public required string Body { get; set; }

    [JsonProperty("number")]
    public long Number { get; set; }

    [JsonProperty("repository")]
    public required string Repository { get; set; }

    [JsonProperty("title")]
    public required string Title { get; set; }

    [JsonProperty("type")]
    public required string Type { get; set; }

    [JsonProperty("url")]
    public required Uri Url { get; set; }
}