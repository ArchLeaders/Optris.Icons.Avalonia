using System.IO;

namespace Optris.Icons.Avalonia.FontAwesome7;

/// <summary>
/// Provides an UTF-8 encoded JSON stream to deserialize the Font Awesome 7 icon collection from.
/// </summary>
public interface IFontAwesome7Utf8JsonStreamProvider
{
    Stream GetUtf8JsonStream();
}
