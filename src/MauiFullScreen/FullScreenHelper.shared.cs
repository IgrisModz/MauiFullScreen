using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

/// <summary>
/// Provides helper methods for managing full screen mode for windows.
/// </summary>
/// <remarks>This static class offers extension methods and properties to facilitate toggling and tracking full
/// screen state in windowed applications. It is intended for use with window types that support full screen
/// transitions.</remarks>
public static partial class FullScreenHelper
{
	/// <summary>
	/// Gets a value indicating whether the application is currently in fullscreen mode.
	/// </summary>
	public static bool IsFullscreen { get; private set; }

	/// <summary>
	/// Toggles the specified window between full screen and windowed mode.
	/// </summary>
	/// <remarks>If the window is currently in full screen mode, this method restores it to windowed mode;
	/// otherwise, it switches the window to full screen. The method has no effect if the window is already in the desired
	/// state.</remarks>
	/// <param name="window">The window instance to toggle between full screen and windowed mode. Cannot be null.</param>
	public static void ToggleFullScreen(this Window window)
	{
		if (IsFullscreen)
		{
			DisableFullScreen(window);
		}
		else
		{
			EnableFullScreen(window);
		}
	}

	/// <summary>
	/// Enables or disables full-screen mode for the specified window.
	/// </summary>
	/// <param name="window">The window to modify. Cannot be null.</param>
	/// <param name="enable">true to enable full-screen mode; false to exit full-screen mode.</param>
	public static void SetFullScreen(this Window window, bool enable)
	{
		if (enable)
		{
			EnableFullScreen(window);
		}
		else
		{
			DisableFullScreen(window);
		}
	}
}
