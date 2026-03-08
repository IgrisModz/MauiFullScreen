using System.Runtime.InteropServices;
using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

public static partial class FullScreenHelper
{
	// Imports de l'Objective-C Runtime pour discuter avec AppKit
	[LibraryImport("/usr/lib/libobjc.dylib", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint objc_getClass(string name);

	[LibraryImport("/usr/lib/libobjc.dylib", StringMarshalling = StringMarshalling.Utf8)]
	private static partial nint sel_registerName(string name);

	[LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
	private static partial nint objc_msgSend(nint receiver, nint selector);

	[LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
	private static partial void objc_msgSend_Void(nint receiver, nint selector, nint arg1);

	[LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
	private static partial nuint objc_msgSend_nuint(nint receiver, nint selector);

	// NSWindowStyleMaskFullScreen = 1 << 14
	const nuint nsWindowStyleMaskFullScreen = 16384;

	/// <summary>
	/// Enables full-screen mode for the specified window.
	/// </summary>
	/// <remarks>On macOS, the actual full-screen state is managed by the operating system. This method requests the
	/// window to enter full-screen mode, but the final state may depend on user interaction or system behavior.</remarks>
	/// <param name="_">The window instance to enable full-screen mode for.</param>
	public static void EnableFullScreen(this Window _)
	{
		// On pourrait mettre IsFullscreen = true ici s'il est utilisé ailleurs,
		// mais l'état réel sera dicté par macOS.
		SetFullScreenState(true);
	}

	/// <summary>
	/// Disables full-screen mode for the specified window.
	/// </summary>
	/// <param name="_">The window instance for which to disable full-screen mode.</param>
	public static void DisableFullScreen(this Window _)
	{
		SetFullScreenState(false);
	}

	static void SetFullScreenState(bool wantFullScreen)
	{
		try
		{
			// 1. Récupérer NSApplication.sharedApplication
			var nsApplicationClass = objc_getClass("NSApplication");
			var sharedApplicationSel = sel_registerName("sharedApplication");
			var nsApp = objc_msgSend(nsApplicationClass, sharedApplicationSel);

			// 2. Obtenir NSApplication.sharedApplication.mainWindow
			var mainWindowSel = sel_registerName("mainWindow");
			var nsWindow = objc_msgSend(nsApp, mainWindowSel);

			if (nsWindow != IntPtr.Zero)
			{
				// 3. Vérifier le styleMask actuel de la fenêtre
				var styleMaskSel = sel_registerName("styleMask");
				var styleMask = objc_msgSend_nuint(nsWindow, styleMaskSel);

				var isFullScreen = (styleMask & nsWindowStyleMaskFullScreen) != 0;

				// 4. On appelle toggleFullScreen: UNIQUEMENT si l'état doit changer
				if (isFullScreen != wantFullScreen)
				{
					var toggleFullScreenSel = sel_registerName("toggleFullScreen:");
					objc_msgSend_Void(nsWindow, toggleFullScreenSel, IntPtr.Zero);
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Trace.TraceError($"Erreur lors de la modification de l'état plein écran MacCatalyst: {ex.Message}");
		}
	}
}
