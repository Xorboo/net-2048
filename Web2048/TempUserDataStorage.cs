using GameLogic.UserData;

namespace Web2048;

public class TempUserDataStorage: IUserDataStorage
{
    private int HighScore = 0;
    
    public TempUserDataStorage()
    {
    }

    public int GetHighScore()
    {
        return HighScore;
    }

    public void SetHighScore(int score)
    {
        HighScore = score;
    }
}