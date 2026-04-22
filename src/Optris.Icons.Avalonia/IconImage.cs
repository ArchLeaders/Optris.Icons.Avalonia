using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

#nullable enable

namespace Optris.Icons.Avalonia;

public class IconImage : DrawingImage, IImage
{
    public static readonly StyledProperty<string> ValueProperty = AvaloniaProperty.Register<
        IconImage,
        string
    >(nameof(Value), string.Empty);

    public static readonly StyledProperty<IBrush> BrushProperty = AvaloniaProperty.Register<
        IconImage,
        IBrush
    >(nameof(Brush), new SolidColorBrush(Colors.Black));

    public static readonly StyledProperty<IBrush?> SecondaryBrushProperty = AvaloniaProperty.Register<
        IconImage,
        IBrush?
    >(nameof(SecondaryBrush));

    public static readonly StyledProperty<IPen> PenProperty = AvaloniaProperty.Register<
        IconImage,
        IPen
    >(nameof(Pen), new ImmutablePen(Colors.Black.ToUInt32(), 0));

    public static readonly StyledProperty<IPen?> SecondaryPenProperty = AvaloniaProperty.Register<
        IconImage,
        IPen?
    >(nameof(SecondaryPen));

    private Rect _bounds;

    public IconImage()
        : this(string.Empty, new SolidColorBrush(Colors.Black)) { }

    public IconImage(string value, IBrush brush, IBrush? secondaryBrush = null)
    {
        Value = value;
        Brush = brush;
        SecondaryBrush = secondaryBrush;
    }

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public IBrush Brush
    {
        get => GetValue(BrushProperty);
        set => SetValue(BrushProperty, value);
    }

    public IBrush? SecondaryBrush
    {
        get => GetValue(SecondaryBrushProperty);
        set => SetValue(SecondaryBrushProperty, value);
    }

    public IPen Pen
    {
        get => GetValue(PenProperty);
        set => SetValue(PenProperty, value);
    }

    public IPen? SecondaryPen
    {
        get => GetValue(SecondaryPenProperty);
        set => SetValue(SecondaryPenProperty, value);
    }

    /// <inheritdoc>
    public new Size Size => _bounds.Size;

    /// <inheritdoc>
    Size IImage.Size => _bounds.Size;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty)
        {
            HandleValueChanged();
            RaiseInvalidated(EventArgs.Empty);
        }
        else if (change.Property == BrushProperty || change.Property == SecondaryBrushProperty)
        {
            HandleBrushChanged();
            RaiseInvalidated(EventArgs.Empty);
        }
        else if (change.Property == PenProperty || change.Property == SecondaryPenProperty)
        {
            HandlePenChanged();
            RaiseInvalidated(EventArgs.Empty);
        }
    }

    private void HandleValueChanged()
    {
        var iconModel = IconProvider.Current.GetIcon(Value);

        _bounds = new Rect(
            x: iconModel.ViewBox.X,
            y: iconModel.ViewBox.Y,
            width: iconModel.ViewBox.Width,
            height: iconModel.ViewBox.Height
        );

        var (primary, secondary) = GetGeometryDrawings();
        primary.Geometry = StreamGeometry.Parse(iconModel.Path.Primary);
        secondary.Geometry = StreamGeometry.Parse(iconModel.Path.Secondary);
    }

    private void HandleBrushChanged()
    {
        var (primary, secondary) = GetGeometryDrawings();
        primary.Brush = Brush;
        secondary.Brush = SecondaryBrush ?? Brush;
    }

    private void HandlePenChanged()
    {
        var (primary, secondary) = GetGeometryDrawings();
        primary.Pen = Pen;
        secondary.Pen = SecondaryPen ?? Pen;
    }

    private (GeometryDrawing Primary, GeometryDrawing Secondary) GetGeometryDrawings()
    {
        var drawing = (DrawingGroup)(Drawing ??= new DrawingGroup {
            Children = {
                new GeometryDrawing(),
                new GeometryDrawing()
            }
        });

        return (
            Primary: (GeometryDrawing)drawing.Children[0],
            Secondary: (GeometryDrawing)drawing.Children[1]
        );
    }

    /// <inheritdoc/>
    void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
        var drawing = Drawing;
        if (drawing == null)
        {
            return;
        }

        var bounds = _bounds;
        var scale = Matrix.CreateScale(
            destRect.Width / sourceRect.Width,
            destRect.Height / sourceRect.Height
        );
        var translate = Matrix.CreateTranslation(
            -sourceRect.X + destRect.X - bounds.X,
            -sourceRect.Y + destRect.Y - bounds.Y
        );

        using (context.PushClip(destRect))
        using (context.PushTransform(translate * scale))
        {
            drawing.Draw(context);
        }
    }
}
