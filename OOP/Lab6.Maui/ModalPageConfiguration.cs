#if MACCATALYST
using UIKit;
#endif

namespace Lab6;

internal static class ModalPageConfiguration
{
    internal static void ConfigureAsDialog(Page page)
    {
        page.BackgroundColor = Colors.White;
#if MACCATALYST || IOS
        page.HandlerChanged += OnDialogHandlerChanged;
#endif
    }

#if MACCATALYST
    private static void OnDialogHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not Page page || page.Handler?.PlatformView is not UIViewController viewController)
            return;

        viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
        page.HandlerChanged -= OnDialogHandlerChanged;
    }
#endif
}
