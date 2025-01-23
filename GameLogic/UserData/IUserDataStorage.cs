namespace GameLogic.UserData;

public interface IUserDataStorage
{
    int GetHighScore();

    void SetHighScore(int score);
}