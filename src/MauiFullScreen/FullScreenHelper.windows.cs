using Microsoft.UI.Windowing;
using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

public static partial class FullScreenHelper
{
    static readonly Dictionary<Window, bool> wasMaximizedMap = [];

    static OverlappedPresenter? GetPresenter(Window window) =>
        (window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window)?.AppWindow?.Presenter as OverlappedPresenter;

	/// <summary>
	/// Enables full-screen mode for the specified window, removing window borders and the title bar.
	/// </summary>
	/// <remarks>If the window is currently maximized, it is first restored before entering full-screen mode. The
	/// previous maximized state is tracked and can be used to restore the window later if needed.</remarks>
	/// <param name="window">The window to be displayed in full-screen mode. Cannot be null.</param>
    public static void EnableFullScreen(this Window window)
    {
        IsFullscreen = true;

        if (GetPresenter(window) is { } presenter)
        {
            var isMaximized = presenter.State == OverlappedPresenterState.Maximized;
            wasMaximizedMap[window] = isMaximized;

            if (isMaximized)
            {
                presenter.Restore();
            }

            presenter.SetBorderAndTitleBar(false, false);
            presenter.Maximize();
        }
    }

	/// <summary>
	/// Restores the specified window from full screen mode, re-enabling its border and title bar.
	/// </summary>
	/// <remarks>If the window was maximized before entering full screen, it will be maximized after restoration;
	/// otherwise, it will be restored to its previous size and position.</remarks>
	/// <param name="window">The window instance to restore from full screen mode. Cannot be null.</param>
    public static void DisableFullScreen(this Window window)
    {
        IsFullscreen = false;

        if (GetPresenter(window) is { } presenter)
        {
            presenter.SetBorderAndTitleBar(true, true);
            
            if (wasMaximizedMap.TryGetValue(window, out bool wasMaximized) && wasMaximized)
            {
                presenter.Maximize();
            }
            else
            {
                presenter.Restore();
            }

            wasMaximizedMap.Remove(window);
        }
    }
}
