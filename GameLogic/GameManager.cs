using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Prompts;
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
    private readonly IGamePromptHandler _promptHandler;
    
    private readonly GameConfiguration _settings;
    private readonly GameState _gameState = new();
    private bool _quitRequested = false;
    
    public GameManager(
        IRoundManager roundManager,
        IInputManager inputManager, 
        IRenderer renderer,
        IUserDataStorage userDataStorage,
        IGamePromptHandler promptHandler,
        IOptions<GameConfiguration> settings)
    {
        _roundManager = roundManager;
        _inputManager = inputManager;
        _renderer = renderer;
        _userDataStorage = userDataStorage;
        
        _promptHandler = promptHandler;
        _promptHandler.OnRestart += Restart;
        _promptHandler.OnQuit += () => _quitRequested = true;
        
        _settings = settings.Value;

        _gameState.HighScore = _userDataStorage.GetHighScore();
    }
    
    public void Run()
    {
        RunInternal(async: false).GetAwaiter().GetResult();
    }
    
    public async Task RunAsync()
    {
        await RunInternal(async: true);
    }

    private async Task RunInternal(bool async)
    {
        float frameTime = 1000f / _settings.FrameRate;
        
        while (!_quitRequested)
        {
            long frameStartTime = Environment.TickCount64;
            
            _inputManager.Tick();

            if (_promptHandler.HandlePrompts(_gameState))
            {
                continue;
            }
            
            if (_gameState.ActivePrompt == ActivePrompt.None)
            {
                TickRound();
            }
            
            _renderer.Render(_gameState, _roundManager.State);
            
            long frameEndTime = Environment.TickCount64;
            float sleepTime = frameTime - (frameEndTime - frameStartTime);
            if (sleepTime > 0)
            {
                if (async)
                {
                    await Task.Delay((int)sleepTime);
                }
                else
                {
                    Thread.Sleep((int)sleepTime);
                }
            }
        }
    }
    
    private void Restart()
    {
        _gameState.ActivePrompt = ActivePrompt.None;
        _roundManager.Restart();
        _renderer.Clear();
    }

    private void TickRound()
    {
        bool wasGameOver = _roundManager.State.IsGameOver;
        _roundManager.Tick();
        
        if (!wasGameOver && _roundManager.State.IsGameOver)
        {
            _gameState.ActivePrompt = ActivePrompt.GameOverRestart;
        }

        UpdateHighScore();
    }
    
    private void UpdateHighScore()
    {
        if (_roundManager.State.Score > _gameState.HighScore)
        {
            _gameState.HighScore = _roundManager.State.Score;
            _userDataStorage.SetHighScore(_gameState.HighScore);
        }
    }
}