namespace PokeNX.Core.Models;

using Enums;

public class GameOffset
{
    public string BuildId { get; }

    public Version Version { get; }

    public uint PlayerPrefsProviderInstance { get; }

    public string MainPointer { get; }

    public GameOffset(string buildId, Version version, uint playerPrefsProviderInstance, string mainPointer)
    {
        BuildId = buildId;
        Version = version;
        PlayerPrefsProviderInstance = playerPrefsProviderInstance;
        MainPointer = mainPointer;
    }
}
