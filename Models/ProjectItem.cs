using Newtonsoft.Json;

namespace Extension.Models;

public class ProjectItem
{
    [JsonProperty("content")]
    public required Content Content { get; set; }

    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("labels")]
    public required string[] Labels { get; set; }

    [JsonProperty("repository")]
    public required Uri Repository { get; set; }

    [JsonProperty("title")]
    public required string Title { get; set; }
}
