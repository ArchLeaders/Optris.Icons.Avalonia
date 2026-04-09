using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Optris.Icons.Avalonia.Models;
using SkiaSharp;
using Xunit;

namespace Optris.Icons.Avalonia.FontAwesome7.Test;

public class FontAwesome7IconProviderTest
{
    private readonly FontAwesome7IconProvider _iconProvider = new();
    private readonly FontAwesome7IconProvider _customIconProvider = new(new TestStreamProvider());

    [Theory]
    [InlineData("fa7-github")]
    [InlineData("fa7-arrow-left")]
    [InlineData("fa7-arrow-right")]
    [InlineData("fa7-brands fa7-github")]
    [InlineData("fa7-solid fa7-arrow-left")]
    [InlineData("fa7-regular fa7-address-book")]
    public void Icon_Should_Exist_And_Be_Valid_SVG_Path(string value)
    {
        // Act
        var icon = _iconProvider.GetIcon(value);
        var skiaPath = SKPath.ParseSvgPathData(icon.Path);

        // Assert
        skiaPath.Should().NotBeNull();
        skiaPath.Bounds.IsEmpty.Should().BeFalse();
    }

    [Theory]
    [InlineData("fa7-you-cant-find-me")]
    [InlineData("fa7")]
    public void IconProvider_Should_Throw_Exception_If_Icon_Does_Not_Exist(string value)
    {
        Assert.Throws<KeyNotFoundException>(() => _iconProvider.GetIcon(value));
    }

    [Theory]
    [InlineData("fa7-github-custom")]
    [InlineData("fa7-brands fa7-github-custom")]
    public void IconProvider_Should_Use_Custom_Stream_Provider(string value)
    {
        // Act
        var icon = _customIconProvider.GetIcon(value);

        // Assert
        icon.Should().NotBeNull();

        icon.Path.ToString()
            .Should()
            .StartWith("M165.9 397.4c0 2-2.3 3.6-5.2 3.6-3.3.3-5.6-1.3-5.6-3.6 0-2 2.3-3.6");
    }

    [Theory]
    [InlineData("fa7-github")]
    [InlineData("fa7-arrow-left")]
    public void IconProvider_Should_Throw_Exception_If_Icon_Does_Not_Exist_In_Custom_Stream(
        string value
    )
    {
        Assert.Throws<KeyNotFoundException>(() => _customIconProvider.GetIcon(value));
    }

    private sealed class TestStreamProvider : IFontAwesome7Utf8JsonStreamProvider
    {
        public Stream GetUtf8JsonStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{assembly.GetName().Name}.icons.json";
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
