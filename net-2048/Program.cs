﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameLogic;
using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Render;
using GameLogic.UserData;
using Serilog;

namespace Net2048;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("gameSettings.json", optional: false, reloadOnChange: false)
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
                    .AddGameServices()
                    .AddSingleton<IInputHandler, ConsoleInputHandler>()
                    .AddSingleton<IUserDataStorage, UserDataStorage>()
                    .AddSingleton<IConsoleWrapper, ConsoleWrapper>();
            })
            .UseSerilog() 
            .Build();

        var gameManager = host.Services.GetRequiredService<IGameManager>();
        gameManager.Run();
    }
}