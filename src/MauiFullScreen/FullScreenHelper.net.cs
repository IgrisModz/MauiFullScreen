using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

public static partial class FullScreenHelper
{
	/// <summary>
	/// Enables full-screen mode for the specified window.
	/// </summary>
	/// <remarks>This method sets the application to full-screen mode. Once enabled, the window will occupy the
	/// entire screen and may hide system UI elements such as the taskbar or title bar. To exit full-screen mode, use the
	/// corresponding method provided by the application.</remarks>
	/// <param name="_">The window instance for which to enable full-screen mode. This parameter is not used directly.</param>
	public static void EnableFullScreen(this Window _)
	{
		IsFullscreen = true;
	}

	/// <summary>
	/// Disables full-screen mode for the specified window.
	/// </summary>
	/// <param name="_">The window instance for which to disable full-screen mode. This parameter is not used directly.</param>
	public static void DisableFullScreen(this Window _)
	{
		IsFullscreen = false;
	}
}
