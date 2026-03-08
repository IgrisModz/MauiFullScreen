namespace MauiFullScreen.Tests;

public partial class FullScreenTests : IDisposable
{
	public FullScreenTests()
	{
		ResetIsFullscreen();
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			// Libération des ressources managées
			ResetIsFullscreen();
		}
	}

	static void ResetIsFullscreen()
	{
		typeof(FullScreenHelper).GetProperty("IsFullscreen")?.SetValue(null, false);
	}

	[Fact]
	public void DefaultIsFullscreenIsFalse()
	{
		// Act
		var result = FullScreenHelper.IsFullscreen;

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void EnableFullScreen_SetsIsFullscreenToTrue()
	{
		// Act
		FullScreenHelper.EnableFullScreen(null!);

		// Assert
		Assert.True(FullScreenHelper.IsFullscreen);
	}

	[Fact]
	public void DisableFullScreen_SetsIsFullscreenToFalse()
	{
		// Arrange
		FullScreenHelper.EnableFullScreen(null!); // Ensure it is true before disabling

		// Act
		FullScreenHelper.DisableFullScreen(null!);

		// Assert
		Assert.False(FullScreenHelper.IsFullscreen);
	}

	[Fact]
	public void ToggleFullScreen_TogglesIsFullscreenState()
	{
		// Ensure initial state is false for this test
		FullScreenHelper.DisableFullScreen(null!);
		Assert.False(FullScreenHelper.IsFullscreen);

		// Act - First toggle should to True
		FullScreenHelper.ToggleFullScreen(null!);
		var firstToggleState = FullScreenHelper.IsFullscreen;

		// Act - Second toggle should back to False
		FullScreenHelper.ToggleFullScreen(null!);
		var secondToggleState = FullScreenHelper.IsFullscreen;

		// Assert
		Assert.True(firstToggleState);
		Assert.False(secondToggleState);
	}

	[Fact]
	public void SetFullScreen_True_SetsIsFullscreenToTrue()
	{
		// Arrange
		ResetIsFullscreen(); // Ensure it is false before enabling

		// Act
		FullScreenHelper.SetFullScreen(null!, true);

		// Assert
		Assert.True(FullScreenHelper.IsFullscreen);
	}

	[Fact]
	public void SetFullScreen_False_SetsIsFullscreenToFalse()
	{
		// Arrange
		FullScreenHelper.EnableFullScreen(null!); // Ensure it is true before disabling

		// Act
		FullScreenHelper.SetFullScreen(null!, false);

		// Assert
		Assert.False(FullScreenHelper.IsFullscreen);
	}
}
