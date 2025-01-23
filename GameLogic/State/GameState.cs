namespace GameLogic.State;

public class GameState
{
    public int HighScore { get; set; } = 0;
    public ActivePrompt ActivePrompt { get; set; } = ActivePrompt.None;
}

public enum ActivePrompt
{
    None,
    Restart,
    GameOverRestart,
    Quit
}