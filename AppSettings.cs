using System.Text.Json;

sealed class AppSettings
{
    public int Watch { get; set; } = 1000;

    public static AppSettings Load()
    {
        const string path = "setting.json";
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loaded = JsonSerializer.Deserialize<AppSettings>(json, opts);
                if (loaded?.Watch > 0)
                    return loaded;
            }
        }
        catch { }
        return new AppSettings();
    }
}
