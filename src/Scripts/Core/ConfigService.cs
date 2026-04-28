using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EchoForest.Core;

/// <summary>
/// Pure-C# implementation of <see cref="IConfigService"/> that serialises
/// <see cref="UserConfig"/> to JSON via <see cref="IFileSystem"/>.
///
/// In production the file system is <see cref="GodotFileSystem"/> which
/// writes to <c>user://settings.json</c>. In tests it is
/// <see cref="MockFileSystem"/> — no Godot runtime required.
/// </summary>
public sealed class ConfigService : IConfigService
{
    private readonly IFileSystem _fs;
    private readonly string _path;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public ConfigService(IFileSystem fileSystem, string path = "user://settings.json")
    {
        _fs = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _path = path;
    }

    // ── IConfigService ────────────────────────────────────────────────────────

    public void Save(UserConfig config)
    {
        if (config is null) throw new ArgumentNullException(nameof(config));
        var json = JsonSerializer.Serialize(config, JsonOpts);
        _fs.WriteText(_path, json);
    }

    public UserConfig Load()
    {
        if (!_fs.Exists(_path))
            return GetDefaults();

        try
        {
            var json = _fs.ReadText(_path);
            return JsonSerializer.Deserialize<UserConfig>(json, JsonOpts) ?? GetDefaults();
        }
        catch (Exception)
        {
            // Corrupt or unreadable file — fall back to defaults gracefully.
            return GetDefaults();
        }
    }

    public UserConfig GetDefaults() => new();
}
