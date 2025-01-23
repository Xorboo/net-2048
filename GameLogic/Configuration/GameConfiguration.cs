namespace GameLogic.Configuration;

// Assuming config values are correct, this should be checked on CI/CD
public class GameConfiguration
{
    public int FrameRate { get; init; }

    public int BoardSize { get; init; }
    public int StartingTilesCount { get; init; }
    public int[] SpawnChances { get; init; } = [];
    public int WinTileValue { get; init; }
    public string SaveDataFile { get; init; } = string.Empty;
}