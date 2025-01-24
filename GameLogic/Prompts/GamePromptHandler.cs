using GameLogic.Input;
using GameLogic.State;

namespace GameLogic.Prompts;

public class GamePromptHandler : IGamePromptHandler
{
    private readonly IInputManager _inputManager;

    public event Action? OnRestart;
    public event Action? OnQuit;

    public GamePromptHandler(IInputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public bool HandlePrompts(GameState gameState)
    {
        return gameState.ActivePrompt switch
        {
            ActivePrompt.None => HandleNewPrompt(gameState),
            ActivePrompt.Restart or ActivePrompt.GameOverRestart => HandleRestartPrompt(gameState),
            ActivePrompt.Quit => HandleQuitPrompt(gameState),
            _ => false
        };
    }

    private bool HandleNewPrompt(GameState gameState)
    {
        if (_inputManager.IsRestartPressed())
        {
            gameState.ActivePrompt = ActivePrompt.Restart;
            return true;
        }

        if (_inputManager.IsQuitPressed())
        {
            gameState.ActivePrompt = ActivePrompt.Quit;
            return true;
        }

        return false;
    }

    private bool HandleRestartPrompt(GameState gameState)
    {
        switch (_inputManager.GetPromptResponse())
        {
            case PromptResponse.No:
                gameState.ActivePrompt = ActivePrompt.None;
                return false;
            
            case PromptResponse.Yes:
                gameState.ActivePrompt = ActivePrompt.None;
                OnRestart?.Invoke();
                return true;
            
            default:
                return false;
        }
    }

    private bool HandleQuitPrompt(GameState gameState)
    {
        switch (_inputManager.GetPromptResponse())
        {
            case PromptResponse.No:
                gameState.ActivePrompt = ActivePrompt.None;
                return false;
            
            case PromptResponse.Yes:
                OnQuit?.Invoke();
                return true;
            
            default:
                return false;
        }
    }
}