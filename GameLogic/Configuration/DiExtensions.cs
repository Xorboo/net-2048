using GameLogic.Input;
using GameLogic.Prompts;
using GameLogic.Render;
using GameLogic.Round;
using Microsoft.Extensions.DependencyInjection;

namespace GameLogic.Configuration;

public static class DiExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IGameManager, GameManager>()
            .AddTransient<IGamePromptHandler, GamePromptHandler>()
            .AddSingleton<IInputManager, InputManager>()
            .AddTransient<IRoundManager, RoundManager>()
            .AddTransient<ITileSpawner, TileSpawner>()
            .AddTransient<IMerger, Merger>()
            .AddSingleton<IRenderer, Renderer>();
    }
}