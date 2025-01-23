using GameLogic.Configuration;
using GameLogic.Input;
using Microsoft.Extensions.Options;

namespace GameLogic.Round;

public class RoundManager : IRoundManager
{
    public RoundState State { get; set; } = new();

    private readonly ITileSpawner _spawner;
    private readonly IInputManager _inputManager;
    private readonly IMerger _merger;
    private readonly GameConfiguration _settings;

    public RoundManager(
        ITileSpawner spawner,
        IInputManager inputManager,
        IMerger merger,
        IOptions<GameConfiguration> settings)
    {
        _spawner = spawner;
        _inputManager = inputManager;
        _merger = merger;
        _settings = settings.Value;

        Restart();
    }

    public void Restart()
    {
        var boardSize = _settings.BoardSize;
        State.Board = new int[boardSize, boardSize];
        State.IsGameOver = false;
        State.Score = 0;
        _spawner.SpawnStartingTiles(State.Board);
    }

    public void Tick()
    {
        State.LastTickScore = 0;
        State.LastMoveCommand = null;
        
        if (State.IsGameOver)
        {
            return;
        }

        BoardCommand command;
        while ((command = _inputManager.GetNextCommand()) != BoardCommand.None)
        {
            Shift(command);
        }
    }

    private void Shift(BoardCommand command)
    {
        var result = _merger.TryShift(State.Board, command);
        if (_merger.ShiftSuccess(result))
        {
            State.LastTickScore = result.Score;
            State.LastMoveCommand = command;
            State.Score += result.Score;
            _spawner.SpawnTile(State.Board);

            State.IsGameOver = !_merger.CanShift(State.Board) || HasWon();
        }
    }

    private bool HasWon()
    {
        for (int y = 0; y < State.Board.GetLength(0); y++)
        {
            for (int x = 0; x < State.Board.GetLength(1); x++)
            {
                if (State.Board[y, x] == _settings.WinTileValue)
                {
                    return true;
                }
            }
        }

        return false;
    }
}