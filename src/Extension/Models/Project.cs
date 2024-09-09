using Newtonsoft.Json;

namespace Extension.Models;

internal class Project
{
    public required string Id { get; set; }
    public int Number { get; set; }
    public required string Title { get; set; }

    [JsonIgnore]
    public required string Owner { get; set; }
}
