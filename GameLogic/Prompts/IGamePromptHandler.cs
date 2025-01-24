using GameLogic.State;

namespace GameLogic.Prompts;

public interface IGamePromptHandler
{
    bool HandlePrompts(GameState gameState);
    
    event Action OnRestart;
    event Action OnQuit;
}