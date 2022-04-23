using System.Text.Json.Serialization;

namespace cw2backend.Models;

public class Modifier
{
    [JsonPropertyName("division")]
    public string Division { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
}