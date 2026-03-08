using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using View = Android.Views.View;
using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

public static partial class FullScreenHelper
{
	static int defaultSystemUiVisibility;
	static bool wasSystemBarVisible;

	// Lists to manage multiple overlays (CommunityToolkit.Maui bug: StatusBar has no tag, so multiple overlays can be created)
	static readonly List<View> statusBarOverlays = [];
	static View? navigationBarOverlay;

	/// <summary>
	/// Enables full-screen mode for the specified window, hiding system UI elements to provide an immersive experience.
	/// </summary>
	/// <remarks>This method is intended for use on Android platforms. It hides system UI elements such as the
	/// status and navigation bars, and adjusts display cutout settings on supported Android versions. If the window is
	/// already in full-screen mode, the method has no effect. Repeated calls are safe and will not reapply full-screen
	/// mode if it is already active.</remarks>
	/// <param name="window">The window on which to enable full-screen mode. Must not be null.</param>
	public static void EnableFullScreen(this Window window)
	{
		if (IsFullscreen)
		{
			return;
		}

		RunOnUiThread(() =>
		{
			var currentWindow = (window.Handler?.PlatformView as Activity)?.Window ?? Platform.CurrentActivity?.Window;
			if (currentWindow is not { DecorView: { IsAttachedToWindow: true } decorView } aWindow)
			{
				return;
			}

			if (OperatingSystem.IsAndroidVersionAtLeast(28))
			{
				aWindow.Attributes?.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
			}

			ApplyHide(aWindow, decorView);

			// Delayed calls to work around aggressive OEMs (MIUI, etc.)
			foreach (var delay in (int[])[150, 500, 1000])
			{
				decorView.PostDelayed(() => ApplyHide(aWindow, decorView), delay);
			}

			IsFullscreen = true;
		});
	}

	/// <summary>
	/// Disables full-screen mode for the specified window if it is currently enabled.
	/// </summary>
	/// <remarks>This method has no effect if the window is not currently in full-screen mode. It must be called
	/// from a context where UI thread access is available.</remarks>
	/// <param name="window">The window instance for which to disable full-screen mode. Cannot be null.</param>
	public static void DisableFullScreen(this Window window)
	{
		if (!IsFullscreen)
		{
			return;
		}

		RunOnUiThread(() =>
		{
			var currentWindow = (window.Handler?.PlatformView as Activity)?.Window ?? Platform.CurrentActivity?.Window;
			if (currentWindow is not { DecorView: { IsAttachedToWindow: true } decorView } aWindow)
			{
				return;
			}

			RestoreSystemBars(aWindow, decorView);
			IsFullscreen = false;
		});
	}

	static void ApplyHide(Android.Views.Window window, View decorView)
	{
		var controller = WindowCompat.GetInsetsController(window, decorView);
		var barTypes = WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.NavigationBars();

		// Android 35+: Hide CommunityToolkit.Maui overlays
		if (OperatingSystem.IsAndroidVersionAtLeast(35))
		{
			var decorGroup = (ViewGroup)decorView;

			// Hide ALL StatusBar overlays (CTK bug: multiple can exist because no tag is set)
			foreach (var overlay in FindAllStatusBarOverlays(decorGroup))
			{
				overlay.Visibility = ViewStates.Gone;
				if (!statusBarOverlays.Contains(overlay))
				{
					statusBarOverlays.Add(overlay);
				}
			}

			if ((navigationBarOverlay = decorGroup.FindViewWithTag("NavigationBarOverlay")) is not null)
			{
				navigationBarOverlay.Visibility = ViewStates.Gone;
			}

			window.ClearFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
		}

		WindowCompat.SetDecorFitsSystemWindows(window, false);

		if (OperatingSystem.IsAndroidVersionAtLeast(30))
		{
			if (decorView.RootWindowInsets is not { } insets)
			{
				return;
			}

			wasSystemBarVisible = insets.IsVisible(WindowInsetsCompat.Type.NavigationBars()) || insets.IsVisible(WindowInsetsCompat.Type.StatusBars());
			if (wasSystemBarVisible)
			{
				window.InsetsController?.Hide(WindowInsets.Type.SystemBars());
			}
		}
		else
		{
			defaultSystemUiVisibility = (int)decorView.SystemUiFlags;
			decorView.SystemUiFlags = SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation
				| SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation
				| SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky;
		}

		window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.LayoutNoLimits);
		window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
		controller?.Hide(barTypes);
		controller?.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
	}

	static void RestoreSystemBars(Android.Views.Window window, View decorView)
	{
		var controller = WindowCompat.GetInsetsController(window, decorView);
		var barTypes = WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.NavigationBars();

		if (OperatingSystem.IsAndroidVersionAtLeast(35))
		{
			window.ClearFlags(WindowManagerFlags.LayoutNoLimits);
			window.SetFlags(WindowManagerFlags.DrawsSystemBarBackgrounds, WindowManagerFlags.DrawsSystemBarBackgrounds);

			// Restore ALL StatusBar overlays
			foreach (var overlay in statusBarOverlays)
			{
				overlay.Visibility = ViewStates.Visible;
			}

			statusBarOverlays.Clear();

			navigationBarOverlay?.Visibility = ViewStates.Visible;
			navigationBarOverlay = null;
		}

		if (OperatingSystem.IsAndroidVersionAtLeast(30))
		{
			if (wasSystemBarVisible)
			{
				window.InsetsController?.Show(WindowInsets.Type.SystemBars());
			}
		}
		else
		{
			decorView.SystemUiFlags = (SystemUiFlags)defaultSystemUiVisibility;
		}

		controller?.Show(barTypes);
		controller?.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorDefault;

		window.AddFlags(WindowManagerFlags.ForceNotFullscreen);
		window.ClearFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.LayoutNoLimits);

		if (OperatingSystem.IsAndroidVersionAtLeast(28))
		{
			window.Attributes?.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
		}

		WindowCompat.SetDecorFitsSystemWindows(window, true);
	}

	#region Helpers

	static void RunOnUiThread(Action action) => Platform.CurrentActivity?.RunOnUiThread(() =>
	{
		try { action(); }
		catch (Exception ex) { Log.Error("FullscreenService", ex.Message); }
	});

	/// <summary>
	/// Finds ALL StatusBarOverlays from CommunityToolkit.Maui.
	/// CTK bug: StatusBar has no tag defined, so FindViewWithTag doesn't work
	/// and a new overlay is created on each color change → multiple stacked overlays.
	/// </summary>
	static List<View> FindAllStatusBarOverlays(ViewGroup decorGroup)
	{
		var overlays = new List<View>();
		var resources = Platform.CurrentActivity?.Resources;
		if (resources is null)
		{
			return overlays;
		}

		var heightId = resources.GetIdentifier("status_bar_height", "dimen", "android");
		var expectedHeight = (heightId > 0 ? resources.GetDimensionPixelSize(heightId) : 0) + 3;

		for (var i = 0; i < decorGroup.ChildCount; i++)
		{
			if (decorGroup.GetChildAt(i) is { LayoutParameters: FrameLayout.LayoutParams { Gravity: GravityFlags.Top, Width: ViewGroup.LayoutParams.MatchParent } lp } child
				&& lp.Height == expectedHeight)
			{
				overlays.Add(child);
			}
		}
		return overlays;
	}

	#endregion
}
