namespace GameLogic.Round;

public interface ITileSpawner
{
    void SpawnStartingTiles(int[,] board);
    bool SpawnTile(int[,] board);
}