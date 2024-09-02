using Newtonsoft.Json;

namespace Extension.Models;

public class ProjectField
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("type")]
    public ProjectFieldType Type { get; set; }

    [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
    public required Option[] Options { get; set; }
}