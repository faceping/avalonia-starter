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

    // To run as a check-in device for your practice list instead: the device token from the list's Devices tab, and the
    // personId you enrolled on the list. A token wins over the sandbox key.
    public const string DeviceToken = "";

    // Who this app verifies: "me" in the sandbox; on a check-in device, the personId you enrolled on the list.
    public const string PersonId = "me";

    public static bool DeviceMode => DeviceToken.Length > 0;

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
