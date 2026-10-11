using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FaceCheck.Views;

namespace FaceCheck;

public partial class App : Application
{
    // Your sandbox key: app.faceping.ai, Developers, Sandbox keys, Create sandbox key. The Android and iOS projects pass
    // it to .UseFacePing(...) in CustomizeAppBuilder.
    public const string SandboxKey = "fp_test_…";

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IActivityApplicationLifetime activity)
            activity.MainViewFactory = () => new MainView();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new MainView();

        base.OnFrameworkInitializationCompleted();
    }
}
