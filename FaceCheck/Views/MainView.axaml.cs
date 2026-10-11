using System.Net.NetworkInformation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FacePing.Sdk;
using FacePing.Sdk.Avalonia;

namespace FaceCheck.Views;

/// <summary>The same screen as the other FaceCheck apps: enrol your face, then verify it live, with or without a network.</summary>
public partial class MainView : UserControl
{
    static readonly IBrush Ink = Brush.Parse("#0F172A"), Green = Brush.Parse("#047857"), Red = Brush.Parse("#B91C1C");

    readonly IFacePing faceping = IFacePing.Current;
    readonly DispatcherTimer networkTimer;
    bool busy;

    public MainView()
    {
        InitializeComponent();
        Camera.CameraFailed += (_, message) => Show("Camera unavailable", message, Ink);
        // Whether the phone has a network, checked every couple of seconds (the checks never need one).
        networkTimer = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Background, (_, _) => ShowNetwork());
        ShowNetwork();
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        // Keep the buttons clear of the home indicator and the camera clear of the status bar.
        if (TopLevel.GetTopLevel(this)?.InsetsManager is { } insets)
        {
            insets.DisplayEdgeToEdgePreference = true;
            var safe = insets.SafeAreaPadding;
            Root.Margin = new Thickness(18 + safe.Left, 10 + safe.Top, 18 + safe.Right, 22 + safe.Bottom);
        }

        networkTimer.Start();
        // Activates the sandbox key on the first run, and a face enrolled earlier may still be on the phone (sandbox faces
        // are deleted 24 hours after enrolment).
        try { await faceping.SyncAsync(); }
        catch (FacePingSetupException ex) { Show("Sandbox key problem", ex.Message, Red); }
        catch (FacePingModelException ex) { Show("Face check unavailable", ex.Message, Red); }
        SetEnrolled(faceping.EnrolledIds.Contains("me"));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        networkTimer.Stop();
        base.OnDetachedFromVisualTree(e);
    }

    async void OnEnroll(object? sender, RoutedEventArgs e)
    {
        await Run(async () =>
        {
            Show("Hold still", "Taking the photo…", Ink);
            Camera.GuideState = FaceGuideState.Working;
            var photo = await Camera.CaptureAsync();
            var result = await faceping.EnrollAsync("me", photo);
            Camera.GuideState = FaceGuideState.Ready;
            if (result.Outcome == EnrollOutcome.Enrolled)
            {
                SetEnrolled(true);
                Show("Enrolled", Offline ? "No network. Tap Verify." : "Now tap Verify.", Ink);
            }
            else
            {
                Show("Try again", Describe(result.Outcome), Red);
            }
        });
    }

    async void OnVerify(object? sender, RoutedEventArgs e)
    {
        await Run(async () =>
        {
            Show("Turn your head", "Liveness check…", Ink);
            // A live check: matches your face, asks you to turn your head (liveness), then matches again.
            var result = await faceping.VerifyLiveAsync("me", Camera);
            var time = $"{result.Elapsed.TotalSeconds:0.00} s";
            if (result.IsMatch)
                Show("Match", $"Live face · {time} · {(Offline ? "no network used" : "on the phone")}", Green);
            else
                Show(Title(result.Outcome), "Tap Verify to try again.", Red);
        });
    }

    async Task Run(Func<Task> action)
    {
        if (busy)
            return;
        busy = true;
        EnrollButton.IsEnabled = VerifyButton.IsEnabled = false;
        try
        {
            await action();
        }
        catch (FacePingSetupException ex)
        {
            // The sandbox key is missing or revoked, or the very first run had no connection.
            Camera.GuideState = FaceGuideState.Ready;
            Show("Sandbox key problem", ex.Message, Red);
        }
        catch (FacePingModelException ex)
        {
            // The face models couldn't load on this phone: send people to your normal way in.
            Camera.GuideState = FaceGuideState.Ready;
            Show("Face check unavailable", ex.Message, Red);
        }
        catch (InvalidOperationException ex)
        {
            // For example the camera didn't deliver a picture.
            Camera.GuideState = FaceGuideState.Ready;
            Show("Try again", ex.Message, Red);
        }
        finally
        {
            busy = false;
            EnrollButton.IsEnabled = true;
            VerifyButton.IsEnabled = faceping.EnrolledIds.Contains("me");
        }
    }

    void SetEnrolled(bool enrolled)
    {
        VerifyButton.IsEnabled = enrolled;
        EnrollButton.Content = enrolled ? "Enrol again" : "Enrol my face";
    }

    void Show(string title, string note, IBrush color)
    {
        ResultTitle.Text = title;
        ResultTitle.Foreground = color;
        ResultNote.Text = note;
    }

    static bool Offline => !NetworkInterface.GetIsNetworkAvailable();

    void ShowNetwork()
    {
        var offline = Offline;
        NetworkText.Text = offline ? "Offline" : "Online";
        NetworkText.Foreground = Brush.Parse(offline ? "#B45309" : "#475569");
        NetworkPill.Background = Brush.Parse(offline ? "#FFF4E5" : "#EAECF0");
    }

    static string Title(VerifyOutcome outcome) => outcome switch
    {
        VerifyOutcome.NoMatch => "Not a match",
        VerifyOutcome.NotEnrolled => "Not enrolled",
        VerifyOutcome.NoFace => "No face seen",
        VerifyOutcome.LivenessFailed => "Not a live face",
        VerifyOutcome.ChallengeFailed => "Head turn not seen",
        VerifyOutcome.Expired => "Enrolment expired",
        _ => outcome.ToString()
    };

    static string Describe(EnrollOutcome outcome) => outcome switch
    {
        EnrollOutcome.NoFace => "No face seen. Look at the camera.",
        EnrollOutcome.MultipleFaces => "More than one face. Just you, please.",
        EnrollOutcome.FaceTooSmall => "Move a little closer.",
        EnrollOutcome.NotLive => "That looked like a photo or a screen.",
        EnrollOutcome.LimitReached => "The sandbox is full on this phone.",
        _ => outcome.ToString()
    };
}
