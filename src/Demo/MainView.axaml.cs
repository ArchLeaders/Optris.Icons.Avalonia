using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Demo;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new DemoViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
