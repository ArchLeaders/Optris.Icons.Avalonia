using System.IO;
using System.Reflection;

namespace Optris.Icons.Avalonia.FontAwesome7;

public sealed class FontAwesome7FreeUtf8JsonStreamProvider : IFontAwesome7Utf8JsonStreamProvider
{
    public Stream GetUtf8JsonStream()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Assets.icons.json";
        return assembly.GetManifestResourceStream(resourceName);
    }
}
