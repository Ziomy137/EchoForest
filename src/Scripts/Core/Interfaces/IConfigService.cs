namespace EchoForest.Core;

/// <summary>
/// Persists and loads <see cref="UserConfig"/> to/from storage.
/// </summary>
public interface IConfigService
{
    /// <summary>Serialises <paramref name="config"/> and writes it to persistent storage.</summary>
    void Save(UserConfig config);

    /// <summary>
    /// Reads and deserialises config from persistent storage.
    /// Returns <see cref="GetDefaults"/> if the file does not exist or is corrupt.
    /// </summary>
    UserConfig Load();

    /// <summary>Returns factory-default settings.</summary>
    UserConfig GetDefaults();
}
