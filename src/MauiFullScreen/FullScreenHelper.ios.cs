using UIKit;
using Window = Microsoft.Maui.Controls.Window;

namespace MauiFullScreen;

public static partial class FullScreenHelper
{
	/// <summary>
	/// Enables full-screen mode for the specified window.
	/// </summary>
	/// <param name="window">The window to enable full-screen mode on. Cannot be null.</param>
	public static void EnableFullScreen(this Window window)
	{
		UpdateStatusBarVisibility(window, true);
	}

	/// <summary>
	/// Disables full screen mode for the specified window.
	/// </summary>
	/// <param name="window">The window instance for which to disable full screen mode. Cannot be null.</param>
	public static void DisableFullScreen(this Window window)
	{
		UpdateStatusBarVisibility(window, false);
	}

	static void UpdateStatusBarVisibility(Window window, bool isFullscreen)
	{
		IsFullscreen = isFullscreen;

		if (window.Handler?.PlatformView is not UIWindow uiWindow)
		{
			return;
		}

		// Si le RootViewController est déjà notre wrapper, on met à jour l'état
		if (uiWindow.RootViewController is FullScreenWrapperViewController wrapper)
		{
			wrapper.IsFullScreen = isFullscreen;
		}
		else if (uiWindow.RootViewController != null)
		{
			// Bonne pratique iOS : on enveloppe le View Controller principal de MAUI 
			// dans notre propre View Controller pour pouvoir y surcharger PrefersStatusBarHidden()
			// sans avoir à utiliser les anciennes API dépréciées de UIApplication.
			var originalRoot = uiWindow.RootViewController;
			wrapper = new FullScreenWrapperViewController(originalRoot)
			{
				IsFullScreen = isFullscreen
			};
			uiWindow.RootViewController = wrapper;
		}
	}

	class FullScreenWrapperViewController : UIViewController
	{
		bool isFullScreen;

		public bool IsFullScreen
		{
			get => isFullScreen;
			set
			{
				if (isFullScreen != value)
				{
					isFullScreen = value;
					SetNeedsStatusBarAppearanceUpdate();
				}
			}
		}

		public FullScreenWrapperViewController(UIViewController rootViewController)
		{
			// Transfert de la hiérarchie au wrapper
			AddChildViewController(rootViewController);
			View!.AddSubview(rootViewController.View!);

			// On s'assure que la vue englobée prend tout l'espace
			rootViewController.View?.Frame = View.Bounds;
			rootViewController.View?.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

			rootViewController.DidMoveToParentViewController(this);
		}

		public override bool PrefersStatusBarHidden() => isFullScreen;

		public override UIStatusBarAnimation PreferredStatusBarUpdateAnimation => UIStatusBarAnimation.Fade;

		// Obligatoire pour forcer iOS à lire PrefersStatusBarHidden sur CETTE classe 
		// plutôt que de déléguer automatiquement aux enfants (le controlleur natif MAUI).
		public override UIViewController? ChildViewControllerForStatusBarHidden() => null;
		public override UIViewController? ChildViewControllerForStatusBarStyle() => null;
	}
}
