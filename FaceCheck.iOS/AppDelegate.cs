using Avalonia;
using Avalonia.iOS;
using FacePing.Sdk.Avalonia;
using Foundation;

namespace FaceCheck.iOS;

[Register("AppDelegate")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public partial class AppDelegate : AvaloniaAppDelegate<App>
#pragma warning restore CA1711
{
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
