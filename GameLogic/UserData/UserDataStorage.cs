using System.Text.Json;
using GameLogic.Configuration;
using Microsoft.Extensions.Logging;

namespace GameLogic.UserData;

public class UserDataStorage: IUserDataStorage
{
    private readonly ILogger<GameManager> _logger;
    private readonly GameConfiguration _settings;

    private UserData _userData = new();
    
    public UserDataStorage(Microsoft.Extensions.Options.IOptions<GameConfiguration> settings, ILogger<GameManager> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        LoadData();
    }

    public int GetHighScore()
    {
        return _userData.HighScore;
    }

    public void SetHighScore(int score)
    {
        if (_userData.HighScore != score)
        {
            _userData.HighScore = score;
            SaveData();
        }
    }

    private void LoadData()
    {
        if (File.Exists(_settings.SaveDataFile))
        {
            try
            {
                _logger.LogInformation("Loading user data");
                string json = File.ReadAllText(_settings.SaveDataFile);
                _userData = JsonSerializer.Deserialize<UserData>(json) ?? new UserData();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing JSON: {Message}", ex.Message);
                _userData = new UserData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred when parsing JSON: {Message}", ex.Message);
                _userData = new UserData();
            }
        }
        else
        {
            _userData = new();
        }
    }

    private void SaveData()
    {
        try
        {
            _logger.LogInformation("Saving user data");
            string json = JsonSerializer.Serialize(_userData);
            File.WriteAllText(_settings.SaveDataFile, json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error saving JSON: {Message}", ex.Message);
            _userData = new UserData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred when saving JSON: {Message}", ex.Message);
            _userData = new UserData();
        }
    }
}