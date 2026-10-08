#if MACCATALYST
using UIKit;
#endif

namespace Lab1;

/// <summary>
/// Applies platform-specific settings so modal pages appear as dialogs without dimming the main window.
/// </summary>
internal static class ModalPageConfiguration
{
    internal static void ConfigureAsDialog(ContentPage page)
    {
        page.BackgroundColor = Colors.Transparent;
#if MACCATALYST || IOS
        page.HandlerChanged += OnDialogHandlerChanged;
#endif
    }

#if MACCATALYST
    private static void OnDialogHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not ContentPage page || page.Handler?.PlatformView is not UIViewController viewController)
        {
            return;
        }

        viewController.ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
        viewController.View!.BackgroundColor = UIColor.Clear;
        page.HandlerChanged -= OnDialogHandlerChanged;
    }
#endif
}
