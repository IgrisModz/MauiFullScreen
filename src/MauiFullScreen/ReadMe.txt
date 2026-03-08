MauiFullScreen

MauiFullScreen is a powerful and easy-to-use library to manage full-screen capabilities in your .NET MAUI applications.

How to use:

Ensure your setup allows window manipulation. Simply call the extension methods on your current Window instance:

// Turn Full-Screen Mode On
Window?.EnableFullScreen();

// Turn Full-Screen Mode Off
Window?.DisableFullScreen();

// Check if Full-Screen is active
bool isFullScreen = FullScreenHelper.IsFullscreen;

Enjoy seamless full-screen toggling across platforms!