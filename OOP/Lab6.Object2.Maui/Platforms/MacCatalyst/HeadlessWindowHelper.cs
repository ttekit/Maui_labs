#if MACCATALYST
using UIKit;

namespace Lab6Object2;

internal static class HeadlessWindowHelper
{
    internal static void HideApplicationWindows()
    {
        foreach (UIScene scene in UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is not UIWindowScene windowScene)
                continue;

            foreach (UIWindow window in windowScene.Windows)
                window.Hidden = true;
        }
    }
}
#endif
