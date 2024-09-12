using Newtonsoft.Json;

namespace Extension.Models;

public class Option
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}
