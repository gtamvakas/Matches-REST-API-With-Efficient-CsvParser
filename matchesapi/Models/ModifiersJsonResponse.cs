using System.Text.Json.Serialization;

namespace cw2backend.Models;

public class ModifiersJsonResponse
{
    [JsonPropertyName("modifiers")]
    public List<Modifier> Modifiers { get; set; }
}