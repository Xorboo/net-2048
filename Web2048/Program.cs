using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GameLogic;
using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Render;
using GameLogic.Round;
using GameLogic.UserData;
using Web2048;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

var appSettings = await httpClient.GetFromJsonAsync<Dictionary<string, object>>("appsettings.json");
var gameSettings = await httpClient.GetFromJsonAsync<Dictionary<string, object>>("gameSettings.json");

var configuration = new ConfigurationBuilder()
    .AddJsonStream(new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(appSettings)))
    .AddJsonStream(new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(gameSettings)))
    .Build();

builder.Configuration.AddConfiguration(configuration);

builder.Logging
    .AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services
    .Configure<GameConfiguration>(builder.Configuration.GetSection("GameConfiguration"))
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddSingleton<IGameManager, GameManager>()
    .AddSingleton<IInputManager, InputManager>()
    .AddSingleton<IInputHandler, WebInputHandler>()
    .AddSingleton<IUserDataStorage, TempUserDataStorage>()
    .AddTransient<IRoundManager, RoundManager>()
    .AddTransient<ITileSpawner, TileSpawner>()
    .AddTransient<IMerger, Merger>()
    .AddSingleton<IRenderer, WebRenderer>()
    .AddSingleton<IConsoleWrapper, ConsoleRedirection>();

await builder.Build().RunAsync();
