using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Render;
using GameLogic.Round;
using GameLogic.State;
using GameLogic.UserData;
using Microsoft.Extensions.Options;

namespace GameLogic;

public class GameManager: IGameManager
{
    private readonly IRoundManager _roundManager;
    private readonly IInputManager _inputManager;
    private readonly IRenderer _renderer;
    private readonly IUserDataStorage _userDataStorage;
    
    private readonly GameConfiguration _settings;
    private readonly GameState _gameState = new();
    private bool _quitRequested = false;
    
    public GameManager(
        IRoundManager roundManager,
        IInputManager inputManager, 
        IRenderer renderer,
        IUserDataStorage userDataStorage,
        IOptions<GameConfiguration> settings)
    {
        _roundManager = roundManager;
        _inputManager = inputManager;
        _renderer = renderer;
        _userDataStorage = userDataStorage;
        
        _settings = settings.Value;

        _gameState.HighScore = _userDataStorage.GetHighScore();
    }
    
    public void Run()
    {
        float frameTime = 1000f / _settings.FrameRate;
        
        while (!_quitRequested)
        {
            long frameStartTime = Environment.TickCount64;
            
            _inputManager.Tick();

            ProcessPromptInput();
            
            if (_gameState.ActivePrompt == ActivePrompt.None)
            {
                TickRound();
            }
            
            _renderer.Render(_gameState, _roundManager.State);
            
            long frameEndTime = Environment.TickCount64;
            float sleepTime = frameTime - (frameEndTime - frameStartTime);
            if (sleepTime > 0)
            {
                Thread.Sleep((int)sleepTime);
            }
        }
    }
    
    public async Task RunAsync()
    {
        float frameTime = 1000f / _settings.FrameRate;
        
        while (!_quitRequested)
        {
            long frameStartTime = Environment.TickCount64;
            
            _inputManager.Tick();

            ProcessPromptInput();
            
            if (_gameState.ActivePrompt == ActivePrompt.None)
            {
                TickRound();
            }
            
            _renderer.Render(_gameState, _roundManager.State);
            
            long frameEndTime = Environment.TickCount64;
            float sleepTime = frameTime - (frameEndTime - frameStartTime);
            if (sleepTime > 0)
            {
                await Task.Delay((int)sleepTime);
            }
        }
    }

    private void ProcessPromptInput()
    {
        if (_gameState.ActivePrompt == ActivePrompt.None)
        {
            if (_inputManager.IsRestartPressed())
            {
                _gameState.ActivePrompt = ActivePrompt.Restart;
            }
            
            if (_inputManager.IsQuitPressed())
            {
                _gameState.ActivePrompt = ActivePrompt.Quit;
            }
        }

        if (_gameState.ActivePrompt == ActivePrompt.None)
        {
            return;
        }
        
        switch (_gameState.ActivePrompt) // ~Poor man's FSM~
        {
            case ActivePrompt.Restart:
            case ActivePrompt.GameOverRestart:
                // Allowing [No] on GameOverRestart to be able to see the final board state
                if (_inputManager.IsNoPressed())
                {
                    _gameState.ActivePrompt = ActivePrompt.None;
                }
                    
                if (_inputManager.IsYesPressed())
                {
                    _gameState.ActivePrompt = ActivePrompt.None;
                    Restart();
                }
                break;
            case ActivePrompt.Quit:
                if (_inputManager.IsNoPressed())
                {
                    _gameState.ActivePrompt = ActivePrompt.None;
                }

                if (_inputManager.IsYesPressed())
                {
                    _quitRequested = true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Restart()
    {
        _roundManager.Restart();
    }

    private void TickRound()
    {
        bool wasGameOver = _roundManager.State.IsGameOver;
        _roundManager.Tick();
        if (!wasGameOver && _roundManager.State.IsGameOver)
        {
            _gameState.ActivePrompt = ActivePrompt.GameOverRestart;
        }

        if (_roundManager.State.Score > _gameState.HighScore)
        {
            _gameState.HighScore = _roundManager.State.Score;
            _userDataStorage.SetHighScore(_gameState.HighScore);
        }
    }
}