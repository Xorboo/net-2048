using GameLogic.Input;

namespace GameLogic.Round;

public class RoundState
{
    public int Score { get; set; } = 0;
    public bool IsGameOver { get; set; } = false;
    public int[,] Board { get; set; } = new int[0, 0];
    
    public int LastTickScore { get; set; } = 0;
    public BoardCommand? LastMoveCommand { get; set; } = null;
}