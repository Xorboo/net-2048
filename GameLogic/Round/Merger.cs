using GameLogic.Configuration;
using GameLogic.Input;
using Microsoft.Extensions.Options;

namespace GameLogic.Round;

public class Merger: IMerger
{
    private readonly List<int> _rowStorage = new();
    private readonly int[,] _tempBoard;

    public Merger(IOptions<GameConfiguration> settings)
    {
        int boardSize = settings.Value.BoardSize;
        _tempBoard = new int[boardSize, boardSize];
    }
    
    public (int Score, bool Shifted) TryShift(int[,] board, BoardCommand command)
    {
        (int Score, bool Shifted) result = (0, false);

        switch (command)
        {
            case BoardCommand.Up:
            case BoardCommand.Down:
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    var shiftResult = ShiftColumn(board, x, command == BoardCommand.Down);
                    result.Score += shiftResult.Score;
                    result.Shifted |= shiftResult.Shifted;;
                }

                break;
            case BoardCommand.Left:
            case BoardCommand.Right:
                for (int y = 0; y < board.GetLength(0); y++)
                {
                    var shiftResult = ShiftRow(board, y, command == BoardCommand.Right);
                    result.Score += shiftResult.Score;
                    result.Shifted |= shiftResult.Shifted;;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }

        return result;
    }
    
    public bool CanShift(int[,] board)
    {
        Array.Copy(board, _tempBoard, board.Length);
        return ShiftSuccess(TryShift(_tempBoard, BoardCommand.Up)) ||
               ShiftSuccess(TryShift(_tempBoard, BoardCommand.Down)) ||
               ShiftSuccess(TryShift(_tempBoard, BoardCommand.Left)) ||
               ShiftSuccess(TryShift(_tempBoard, BoardCommand.Right));
    }

    public bool ShiftSuccess((int Score, bool Shifted) shiftResult)
    {
        return shiftResult.Score > 0 || shiftResult.Shifted;
    }
    
    private (int Score, bool Shifted) ShiftColumn(int[,] board, int x, bool reverse)
    {
        _rowStorage.Clear();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            _rowStorage.Add(board[y, x]);
        }

        if (reverse)
        {
            _rowStorage.Reverse();
        }
        
        var result = ShiftLeft(_rowStorage);
        if (ShiftSuccess(result))
        {
            if (reverse)
            {
                _rowStorage.Reverse();
            }

            for (int y = 0; y < board.GetLength(0); y++)
            {
                board[y, x] = _rowStorage[y];
            }
        }

        return result;
    }

    private (int Score, bool Shifted) ShiftRow(int[,] board, int y, bool reverse)
    {
        _rowStorage.Clear();
        for (int x = 0; x < board.GetLength(1); x++)
        {
            _rowStorage.Add(board[y, x]);
        }

        if (reverse)
        {
            _rowStorage.Reverse();
        }

        var result = ShiftLeft(_rowStorage);
        if (ShiftSuccess(result))
        {
            if (reverse)
            {
                _rowStorage.Reverse();
            }

            for (int x = 0; x < board.GetLength(1); x++)
            {
                board[y, x] = _rowStorage[x];
            }
        }

        return result;
    }

    private (int Score, bool Shifted) ShiftLeft(List<int> row)
    {
        int score = 0;
        bool shifted = false;
        
        int lastValue = -1;
        int lastPos = -1;

        // Combine pairs first
        for (int i = 0; i < row.Count; i++)
        {
            int value = row[i];
            if (value == 0)
            {
                continue;
            }

            if (lastValue == value)
            {
                // Merge into the pair and lock the number from further merges
                int newValue = lastValue * 2;
                row[lastPos] = newValue;
                row[i] = 0;
                lastValue = -1;

                score += newValue;
            }
            else
            {
                // Shift left if possible
                lastValue = value;
                lastPos += 1;
                if (lastPos != i)
                {
                    row[i] = 0;
                    row[lastPos] = value;
                    shifted = true;
                }
            }
        }

        return (score, shifted);
    }
}