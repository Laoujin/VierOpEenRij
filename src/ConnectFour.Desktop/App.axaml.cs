using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ConnectFour.Desktop.Services;
using ConnectFour.Desktop.ViewModels;
using ConnectFour.Desktop.Views;

namespace ConnectFour.Desktop;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ISoundService sound = OperatingSystem.IsWindows()
                ? new WindowsSoundService()
                : new NoOpSoundService();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(sound)
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
