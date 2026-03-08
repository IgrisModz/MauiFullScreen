namespace MauiFullScreen.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		SafeAreaEdges = SafeAreaEdges.Default;
		MainRoot.SafeAreaEdges = SafeAreaEdges.Default;
		FullScreenBtn.Text = $"FullScreen {(FullScreenHelper.IsFullscreen ? "[On]" : "[Off]")}";

#if WINDOWS
		static Button? CreateButton(string text)
		{
			if (Application.Current?.Resources.TryGetValue("TitleBarButton", out var titleBarButton) == true && titleBarButton is Style titleBarButtonStyle)
			{
				var btn = new Button
				{
					Text = text,
					Style = titleBarButtonStyle
				};
				return btn;
			}
			return null;
		}

		var backBtn = CreateButton("\uE72B");
		var forwardBtn = CreateButton("\uE72A");
		var reloadBtn = CreateButton("\uE72C");

		backBtn?.Clicked += (s, e) => GoBack();
		forwardBtn?.Clicked += (s, e) => GoForward();
		reloadBtn?.Clicked += (s, e) => MainWebView.Reload();

		Loaded += (s, e) =>
		{
			Window.TitleBar = new TitleBar
			{
				Content = new HorizontalStackLayout
				{
					HorizontalOptions = LayoutOptions.Start,
					VerticalOptions = LayoutOptions.Center,
					Children = 
					{
						backBtn,
						forwardBtn,
						reloadBtn
					}
				}
			};
		};
#endif

		MainWebView.Navigated += (_, _) =>
		{
			if (MainRefreshView.IsRefreshing)
			{
				MainRefreshView.IsRefreshing = false;
			}
#if WINDOWS
			backBtn?.IsEnabled = MainWebView.CanGoBack;
			forwardBtn?.IsEnabled = MainWebView.CanGoForward;
#endif
		};

		MainRefreshView.Refreshing += (_, _) => MainWebView.Reload();
		FullScreenBtn.Clicked += OnFullScreenClicked;
	}

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

	protected override bool OnBackButtonPressed()
	{
		return GoBack() || base.OnBackButtonPressed();
	}

	public bool GoBack()
	{
		if (MainWebView.CanGoBack)
		{
			MainWebView.GoBack();
			return true;
		}
		return false;
	}

	public bool GoForward()
	{
		if (MainWebView.CanGoForward)
		{
			MainWebView.GoForward();
			return true;
		}
		return false;
	}
}
