using System.Collections.Generic;
using System.Text.Json.Serialization;
using Optris.Icons.Avalonia.FontAwesome7.Models;

namespace Optris.Icons.Avalonia.FontAwesome7;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Dictionary<string, FontAwesome7Icon>))]
internal partial class FontAwesome7IconsJsonContext : JsonSerializerContext
{
}
