using SkiaSharp;

const int size = 256;
const float cornerRadius = 48;
const float padding = 40;

// Brand color - Avalonia-inspired purple
var bgColor = new SKColor(0x7B, 0x2F, 0xBE);
var iconColor = SKColors.White;

var icons = new Dictionary<string, (string path, float viewBoxSize)>
{
    // Core: outlined palette shape (MD path used as outline)
    ["icon-core"] = (
        "M17.5,12A1.5,1.5 0 0,1 16,10.5A1.5,1.5 0 0,1 17.5,9A1.5,1.5 0 0,1 19,10.5A1.5,1.5 0 0,1 17.5,12M14.5,8A1.5,1.5 0 0,1 13,6.5A1.5,1.5 0 0,1 14.5,5A1.5,1.5 0 0,1 16,6.5A1.5,1.5 0 0,1 14.5,8M9.5,8A1.5,1.5 0 0,1 8,6.5A1.5,1.5 0 0,1 9.5,5A1.5,1.5 0 0,1 11,6.5A1.5,1.5 0 0,1 9.5,8M6.5,12A1.5,1.5 0 0,1 5,10.5A1.5,1.5 0 0,1 6.5,9A1.5,1.5 0 0,1 8,10.5A1.5,1.5 0 0,1 6.5,12M12,3A9,9 0 0,0 3,12A9,9 0 0,0 12,21A1.5,1.5 0 0,0 13.5,19.5C13.5,19.11 13.35,18.76 13.11,18.5C12.88,18.23 12.73,17.88 12.73,17.5A1.5,1.5 0 0,1 14.23,16H16A5,5 0 0,0 21,11C21,6.58 16.97,3 12,3Z",
        24f),

    // FontAwesome palette (filled style, viewBox 512)
    ["icon-fa"] = (
        "M512 256c0 .9 0 1.8 0 2.7c-.4 36.5-33.6 61.3-70.1 61.3L344 320c-26.5 0-48 21.5-48 48c0 3.4 .4 6.7 1 9.9c2.1 10.2 6.5 20 10.8 29.9c6.1 13.8 12.1 27.5 12.1 42c0 31.8-21.6 60.7-53.4 62c-3.5 .1-7 .2-10.6 .2C114.6 512 0 397.4 0 256S114.6 0 256 0S512 114.6 512 256zM128 288a32 32 0 1 0 -64 0 32 32 0 1 0 64 0zm0-96a32 32 0 1 0 0-64 32 32 0 1 0 0 64zM288 96a32 32 0 1 0 -64 0 32 32 0 1 0 64 0zm96 96a32 32 0 1 0 0-64 32 32 0 1 0 0 64z",
        512f),

    // MaterialDesign palette
    ["icon-md"] = (
        "M17.5,12A1.5,1.5 0 0,1 16,10.5A1.5,1.5 0 0,1 17.5,9A1.5,1.5 0 0,1 19,10.5A1.5,1.5 0 0,1 17.5,12M14.5,8A1.5,1.5 0 0,1 13,6.5A1.5,1.5 0 0,1 14.5,5A1.5,1.5 0 0,1 16,6.5A1.5,1.5 0 0,1 14.5,8M9.5,8A1.5,1.5 0 0,1 8,6.5A1.5,1.5 0 0,1 9.5,5A1.5,1.5 0 0,1 11,6.5A1.5,1.5 0 0,1 9.5,8M6.5,12A1.5,1.5 0 0,1 5,10.5A1.5,1.5 0 0,1 6.5,9A1.5,1.5 0 0,1 8,10.5A1.5,1.5 0 0,1 6.5,12M12,3A9,9 0 0,0 3,12A9,9 0 0,0 12,21A1.5,1.5 0 0,0 13.5,19.5C13.5,19.11 13.35,18.76 13.11,18.5C12.88,18.23 12.73,17.88 12.73,17.5A1.5,1.5 0 0,1 14.23,16H16A5,5 0 0,0 21,11C21,6.58 16.97,3 12,3Z",
        24f),
};

var outputDir = Path.Combine(args.Length > 0 ? args[0] : ".", "resources");
Directory.CreateDirectory(outputDir);

foreach (var (name, (pathData, viewBoxSize)) in icons)
{
    using var surface = SKSurface.Create(new SKImageInfo(size, size));
    var canvas = surface.Canvas;
    canvas.Clear(SKColors.Transparent);

    // Draw rounded rectangle background
    using var bgPaint = new SKPaint { Color = bgColor, IsAntialias = true };
    canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, size, size), cornerRadius), bgPaint);

    // Parse and draw the icon path
    var path = SKPath.ParseSvgPathData(pathData);
    if (path == null)
    {
        Console.Error.WriteLine($"Failed to parse SVG path for {name}");
        continue;
    }

    // Scale the path to fit within the padded area
    var pathBounds = path.Bounds;
    float availableSize = size - padding * 2;
    float scale = availableSize / Math.Max(pathBounds.Width, pathBounds.Height);

    var matrix = SKMatrix.Identity;
    // Translate to origin
    matrix = matrix.PostConcat(SKMatrix.CreateTranslation(-pathBounds.MidX, -pathBounds.MidY));
    // Scale
    matrix = matrix.PostConcat(SKMatrix.CreateScale(scale, scale));
    // Center in canvas
    matrix = matrix.PostConcat(SKMatrix.CreateTranslation(size / 2f, size / 2f));

    path.Transform(matrix);

    // For the core icon, draw as outline; for flavors, draw filled
    using var iconPaint = new SKPaint
    {
        Color = iconColor,
        IsAntialias = true,
        Style = name == "icon-core" ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
        StrokeWidth = name == "icon-core" ? 3f : 0f,
    };

    // FontAwesome palette uses even-odd fill rule for the cutout dots
    if (name == "icon-fa")
        path.FillType = SKPathFillType.EvenOdd;

    canvas.DrawPath(path, iconPaint);

    // Save PNG
    using var image = surface.Snapshot();
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    var filePath = Path.Combine(outputDir, $"{name}.png");
    using var stream = File.OpenWrite(filePath);
    data.SaveTo(stream);

    Console.WriteLine($"Generated: {filePath}");
}

Console.WriteLine("Done!");
