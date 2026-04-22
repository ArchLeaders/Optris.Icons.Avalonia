namespace Optris.Icons.Avalonia.Models;

public readonly record struct PathModel(string Primary, string Secondary = null)
{
    public static implicit operator PathModel(string path) => new(path);
}
