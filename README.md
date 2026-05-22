# MauiFullScreen

MauiFullScreen is a simple and efficient .NET MAUI library to manage full-screen mode in your applications.

## Features

- **Enable / Disable**: Change the full-screen state on the fly.
- **Toggle**: Switch between full-screen and windowed mode.
- **State Tracking**: Easily check if the application is currently in full-screen mode.

The methods directly extend the `Microsoft.Maui.Controls.Window` object.

## Basic Usage

```csharp
using MauiFullScreen;

// Check the current state
bool isFull = FullScreenHelper.IsFullscreen;

// Enable full-screen mode
Window?.EnableFullScreen();

// Disable full-screen mode
Window?.DisableFullScreen();

// Toggle between full-screen and windowed mode
Window?.ToggleFullScreen();

// Set the full-screen state directly
// (enable only if not already in the desired state)
Window?.SetFullScreen(isFull);

// Hide the status bar (Android)
Window?.HideStatusBar();

// Show the status bar (Android)
Window?.ShowStatusBar();

// Hide the navigation bar (Android)
Window?.HideNavigationBar();

// Show the navigation bar (Android)
Window?.ShowNavigationBar();
```

## Integration Example (MauiFullScreen.Sample)

The following example shows how to integrate the state change with a button and adjust the `SafeAreaEdges` for a truly immersive rendering, particularly on mobile devices:

```csharp
void OnFullScreenClicked(object? sender, EventArgs e)
{
	var enable = !FullScreenHelper.IsFullscreen;

	SafeAreaEdges = MainRoot.SafeAreaEdges = enable
        ? SafeAreaEdges.None
        : SafeAreaEdges.Default;

	// Window?.ToggleFullScreen();
	Window?.SetFullScreen(enable);

	FullScreenBtn.Text = enable ? "FullScreen [On]" : "FullScreen [Off]";
}

bool statusBarHidden = false, navigationBarHidden = false;

void OnStatusBarVisibilityClicked(object? sender, EventArgs e)
{
    statusBarHidden = !statusBarHidden;
    if (statusBarHidden)
        Window?.HideStatusBar();
    else
        Window?.ShowStatusBar();
    SafeAreaEdges = MainRoot.SafeAreaEdges = statusBarHidden
        ? SafeAreaEdges.None
        : SafeAreaEdges.Default;
    StatusBarBtn.Text = statusBarHidden ? "Status Bar [Hidden]" : "Status Bar [Visible]";
}

void OnNavigationBarVisibilityClicked(object? sender, EventArgs e)
{
    navigationBarHidden = !navigationBarHidden;
    if (navigationBarHidden)
        Window?.HideNavigationBar();
    else
        Window?.ShowNavigationBar();
    // Safe Area not affected by navigation bar visibility on Android, so we don't adjust it here.
    NavigationBarBtn.Text = navigationBarHidden ? "Navigation Bar [Hidden]" : "Navigation Bar [Visible]";
}
```

## Compatibility

This project targets **.NET 10** and is designed to work on the platforms supported by .NET MAUI (Android, iOS, Windows, MacCatalyst). Platform-specific implementations (such as on Android) automatically handle screen notches and the hiding of system bars.

> **Note regarding iOS and MacCatalyst:** 
> These platforms have not been tested. Developing, building, and testing applications for Apple platforms strictly requires Apple-branded hardware (a Mac) according to Apple's End User License Agreement (EULA). Since I do not currently possess the appropriate legal Apple hardware, these platforms remain untested. Contributions, bug reports, and pull requests from developers with macOS environments are highly appreciated!
