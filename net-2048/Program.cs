using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameLogic;
using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Render;
using GameLogic.Round;
using GameLogic.UserData;
using Serilog;
using Serilog.Events;

namespace Net2048;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddConfiguration(configuration);
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .Configure<GameConfiguration>(context.Configuration.GetSection("GameConfiguration"))
                    .AddSingleton<IGameManager, GameManager>()
                    .AddSingleton<IInputManager, InputManager>()
                    .AddSingleton<IUserDataStorage, UserDataStorage>()
                    .AddTransient<IRoundManager, RoundManager>()
                    .AddTransient<ITileSpawner, TileSpawner>()
                    .AddTransient<IMerger, Merger>()
                    .AddSingleton<IRenderer, Renderer>();
            })
            .UseSerilog() 
            .Build();

        var gameManager = host.Services.GetRequiredService<IGameManager>();
        gameManager.Run();
    }
}