# FacePing Avalonia starter

**FaceCheck**: a small Avalonia app that enrols your face and verifies it live, on the phone, with a head-turn liveness check. Switch on airplane mode and it still matches.

It's the finished app from the [Avalonia quickstart](https://docs.faceping.ai/start/avalonia/), built on [FacePing.Sdk.Avalonia](https://www.nuget.org/packages/FacePing.Sdk.Avalonia).

## Run it

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) with the Android workload (`dotnet workload install android`) and, for iPhone, the iOS workload and Xcode on a Mac. Then an Android phone (Android 7 or later, 64-bit) or an iPhone (iOS 15.1 or later) with a cable. Simulators and emulators have no front camera.

1. **Get a free sandbox key** at [app.faceping.ai/signup](https://app.faceping.ai/signup) (no card, no approval). In the dashboard: **Developers → Sandbox keys → Create sandbox key**.
2. **Paste it** into `FaceCheck/App.axaml.cs`, replacing `fp_test_…`.
3. **Run a Release build** on your phone:

   ```bash
   # Android
   dotnet build FaceCheck.Android -c Release -t:Run
   ```

   On iOS, change `ApplicationId` in `FaceCheck.iOS/FaceCheck.iOS.csproj` to your own, then run from Rider or Visual Studio, or see [the quickstart](https://docs.faceping.ai/start/avalonia/) for the command line.

4. Tap **Enrol my face**, then **Verify**: you should see **Match**. Now switch on airplane mode and tap **Verify** again.

Before you build your own app from this, change the app id (`com.example.facecheck.avalonia`) in both platform projects.

## What's where

| File | What it does |
|---|---|
| `FaceCheck/App.axaml.cs` | Your sandbox key. |
| `FaceCheck.Android/Application.cs`, `FaceCheck.iOS/AppDelegate.cs` | Turn FacePing on (`UseFacePing`) in `CustomizeAppBuilder`. |
| `FaceCheck/Views/MainView.axaml` | The camera control (`FaceCameraView`) and the two buttons. |
| `FaceCheck/Views/MainView.axaml.cs` | `EnrollAsync` and `VerifyLiveAsync`, and what each result means. |

## The sandbox

Free, for building and testing: up to 25 faces per device, each deleted 24 hours after it was enrolled. Faces never leave the phone. The app needs a connection the first time it runs, to activate your key; after that it works offline for 30 days.

To use FacePing with real people, go live from your [FacePing dashboard](https://app.faceping.ai). Your verify code doesn't change.

## Learn more

- [How the SDK works](https://docs.faceping.ai/sdk/): liveness, storage, sandbox and production
- [Avalonia reference](https://docs.faceping.ai/sdk/avalonia/): every type and method
- [Security](https://docs.faceping.ai/security/): the Rust core, encryption on the device
- Questions: support@faceping.ai

## Licence

This starter app is MIT licensed (see [LICENSE](LICENSE)): copy any of it into your own app. The FacePing SDK it uses has its own licence, included in the NuGet package.
