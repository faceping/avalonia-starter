using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using FacePing.Sdk.Avalonia;

namespace FaceCheck.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont()
            // Turns FacePing on with your sandbox key, or a device token (both in App.axaml.cs).
            .UseFacePing(o =>
            {
                if (App.DeviceMode) o.DeviceToken = App.DeviceToken;
                else o.SandboxKey = App.SandboxKey;
            });
}
