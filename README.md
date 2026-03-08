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
```

## Compatibility

This project targets **.NET 10** and is designed to work on the platforms supported by .NET MAUI (Android, iOS, Windows, MacCatalyst). Platform-specific implementations (such as on Android) automatically handle screen notches and the hiding of system bars.

> **Note regarding iOS and MacCatalyst:** 
> These platforms have not been tested. Developing, building, and testing applications for Apple platforms strictly requires Apple-branded hardware (a Mac) according to Apple's End User License Agreement (EULA). Since I do not currently possess the appropriate legal Apple hardware, these platforms remain untested. Contributions, bug reports, and pull requests from developers with macOS environments are highly appreciated!
