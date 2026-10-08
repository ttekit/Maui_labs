namespace Lab5;

internal static class CoolPaintFileTypes
{
    public static PickOptions CreatePickOptions() => new()
    {
        PickerTitle = "Оберіть файли .coolPaint",
        FileTypes = DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? null
            : Create(),
    };

    private static FilePickerFileType Create() => new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.iOS, new[] { "public.plain-text", "public.data" } },
        { DevicePlatform.MacCatalyst, new[] { "public.plain-text", "public.data" } },
        { DevicePlatform.WinUI, new[] { ".coolPaint", ".txt" } },
        { DevicePlatform.Android, new[] { "text/plain", "application/octet-stream" } },
    });
}
