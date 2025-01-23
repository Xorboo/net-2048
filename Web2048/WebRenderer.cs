using GameLogic.Configuration;
using GameLogic.Render;
using GameLogic.Round;
using GameLogic.State;
using Microsoft.Extensions.Options;

namespace Web2048;

public class WebRenderer : Renderer, IRenderer
{
    private readonly IConsoleWrapper _consoleWrapper;
    
    public WebRenderer(IConsoleWrapper consoleWrapper, IOptions<GameConfiguration> settings)
        : base(consoleWrapper, settings)
    {
        _consoleWrapper = consoleWrapper;
    }

    public new void Render(GameState gameState, RoundState roundState)
    {
        // xterm doesn't clean the first line properly
        _consoleWrapper.SetCursorPosition(0, 0);
        _consoleWrapper.Write(new string(' ', 60));
            
        base.Render(gameState, roundState);
    }
}