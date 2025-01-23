using GameLogic.Configuration;
using GameLogic.Round.Utils;

namespace GameLogic.Round;

public class TileSpawner: ITileSpawner
{
    private readonly Random _random = new();
    private readonly List<Coordinate> _emptyTiles = new();
    private readonly GameConfiguration _settings;
    private readonly WeightedRandom<int> _spawnChances;

    public TileSpawner(Microsoft.Extensions.Options.IOptions<GameConfiguration> settings)
    {
        _settings = settings.Value;
        var tileChances = _settings.SpawnChances
            .Select((chance, index) => (Item: 1 << (index + 1), Chance: chance));
        _spawnChances = new(tileChances);
    }

    public void SpawnStartingTiles(int[,] board)
    {
        FindEmptyTiles(board);
        for (int i = 0; i < _settings.StartingTilesCount; i++)
        {
            if (!PlaceNewTile(board))
            {
                throw new Exception($"Can't place starting tile [{i}]");
            }
        }
    }

    public bool SpawnTile(int[,] board)
    {
        FindEmptyTiles(board);
        PlaceNewTile(board);
        return _emptyTiles.Count > 0;
    }

    private bool PlaceNewTile(int[,] board)
    {
        if (_emptyTiles.Count == 0)
        {
            return false;
        }
        
        var pos = GetRandomEmptyTile();
        var value = _spawnChances.GetRandomItem();
        board[pos.Y, pos.X] = value;
        return true;
    }
    
    private Coordinate GetRandomEmptyTile()
    {
        var tileIndex = _random.Next(0, _emptyTiles.Count);
        var tile = _emptyTiles[tileIndex];
        _emptyTiles.RemoveAt(tileIndex);
        return tile;
    }

    private void FindEmptyTiles(int[,] board)
    {
        _emptyTiles.Clear();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] == 0)
                {
                    _emptyTiles.Add(new Coordinate(x, y));
                }
            }
        }
    }
}
