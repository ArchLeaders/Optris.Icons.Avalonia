using System.Collections.Generic;
using System.Text.Json;
using Optris.Icons.Avalonia.Models;

namespace Optris.Icons.Avalonia.FontAwesome.Models;

internal class Svg
{
    public JsonElement Path { get; set; }
    public IReadOnlyList<int> ViewBox { get; set; }

    public IconModel ToIconModel()
    {
        var viewBox = new ViewBoxModel(ViewBox[0], ViewBox[1], ViewBox[2], ViewBox[3]);
        var path = Path.ValueKind switch {
            JsonValueKind.Array => GetPathModelFromArray(Path),
            _ => Path.GetString(),
        };
        return new IconModel(viewBox, path);
    }

    private static PathModel GetPathModelFromArray(JsonElement element)
    {
        var enumerator = element.EnumerateArray();
        return new PathModel(
            enumerator.MoveNext() ? enumerator.Current.GetString() : null,
            enumerator.MoveNext() ? enumerator.Current.GetString() : null);
    }
}
