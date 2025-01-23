using GameLogic.Input;

namespace GameLogic.Round;

public interface IMerger
{
    (int Score, bool Shifted) TryShift(int[,] board, BoardCommand command);
    bool CanShift(int[,] board);
    bool ShiftSuccess((int Score, bool Shifted) shiftResult);
}