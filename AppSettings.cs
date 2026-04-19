using System.Text.Json;

sealed class AppSettings
{
    static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

    public int Watch { get; set; } = 1000;

    public static AppSettings Load()
    {
        const string path = "setting.json";
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, _opts);
                if (loaded?.Watch > 0)
                    return loaded;
            }
        }
        catch { }
        return new AppSettings();
    }
}
