# FacePing Avalonia starter

**FaceCheck**: a small Avalonia app that enrols your face and verifies it live, on the phone, with a head-turn liveness check. Switch on airplane mode and it still matches. It can also run as a check-in device for your practice list, like the other FaceCheck starters.

It's a fuller version of the app you build in [Get started with Avalonia](https://docs.faceping.ai/start/avalonia/), built on [FacePing.Sdk.Avalonia](https://www.nuget.org/packages/FacePing.Sdk.Avalonia).

## Run it

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) with the Android workload (`dotnet workload install android`) and, for iPhone, the iOS workload and Xcode on a Mac. Then an Android phone (Android 7 or later, 64-bit) or an iPhone (iOS 15.1 or later) with a cable. Use a phone: the iOS simulator has no front camera, and an Android emulator has one only if you map your computer's webcam to it, which is slower and less reliable for the liveness check.

1. **Get a free sandbox key** at [app.faceping.ai/signup](https://app.faceping.ai/signup) (no card, no approval). In the dashboard: **Developers → Sandbox keys → Create sandbox key**.
2. **Paste it** into `FaceCheck/App.axaml.cs`, replacing `fp_test_…`.
3. **Run a Release build** on your phone:

   ```bash
   # Android
   dotnet build FaceCheck.Android -c Release -t:Run
   ```

   On iOS, change `ApplicationId` in `FaceCheck.iOS/FaceCheck.iOS.csproj` to your own, then run from Rider or Visual Studio, or see [Get started with Avalonia](https://docs.faceping.ai/start/avalonia/) for the command line.

4. Tap **Enrol my face**, then **Verify**: you should see **Match**. Now switch on airplane mode and tap **Verify** again.

Before you build your own app from this, change the app id (`com.example.facecheck.avalonia`) in both platform projects. The name under the icon on Android is the `Label` on `MainActivity` (`FaceCheck.Android/MainActivity.cs`); on iOS it's `CFBundleDisplayName` in `FaceCheck.iOS/Info.plist`.

## Try it as a check-in device

Once you've tried the server side with your sandbox key (a practice check-in list, an enrolment link, a device), this app can be the check-in device:

1. Enrol a person on your practice list with a `personId` (through an enrolment link, or from your server; see the [API quickstart](https://docs.faceping.ai/quickstart/)).
2. Add a device to the list (the list's **Devices** tab, or `POST /v1/lists/{listId}/devices`) and copy its `fpd_` token.
3. In `FaceCheck/App.axaml.cs`, set `DeviceToken` to the token and `PersonId` to the person you enrolled:

   ```csharp
   public const string DeviceToken = "fpd_yourtoken";
   public const string PersonId = "ACME-000123";
   ```

   With a device token set, the sandbox key is ignored. To keep the sandbox app on the phone as well, also give this copy its own `ApplicationId` in both platform projects (for example `com.yourcompany.facecheck.device`) and its own name.

4. Run it again. The app syncs the list when it starts and shows what it found: how many people synced and whether your person is there, or why not (a revoked or mistyped token, a deleted list, no connection). **Enrol my face** is hidden: people enrol on the server. Tap **Verify** once your person is here.

## What's where

| File | What it does |
|---|---|
| `FaceCheck/App.axaml.cs` | Your sandbox key, or a device token and the person to verify. |
| `FaceCheck.Android/Application.cs`, `FaceCheck.iOS/AppDelegate.cs` | Turn FacePing on (`UseFacePing`) in `CustomizeAppBuilder`. |
| `FaceCheck/Views/MainView.axaml` | The camera control (`FaceCameraView`) and the two buttons. |
| `FaceCheck/Views/MainView.axaml.cs` | `EnrollAsync` and `VerifyLiveAsync`, what each result means, and what each sync found on a check-in device. |
| `FaceCheck.Android/Resources/xml/` | Backup rules that keep FacePing's data on the phone (Android 12+ backups and device-to-device transfers). |

## The sandbox

Free, for building and testing: up to 25 faces per device, each deleted 24 hours after it was enrolled. Faces you enrol in the app never leave the phone; people on your practice list are held by FacePing, each for 24 hours. The app needs a connection the first time it runs, to activate your key; after that it works offline for 30 days.

To use FacePing with real people, go live from your [FacePing dashboard](https://app.faceping.ai). Your verify code doesn't change.

## Learn more

- [How the SDK works](https://docs.faceping.ai/sdk/): liveness, storage, sandbox and production
- [Avalonia reference](https://docs.faceping.ai/sdk/avalonia/): every type and method
- [Security](https://docs.faceping.ai/security/): the Rust core, encryption on the device
- Questions: support@faceping.ai

## Licence

This starter app is MIT licensed (see [LICENSE](LICENSE)): copy any of it into your own app. The FacePing SDK it uses has its own licence, included in the NuGet package.
