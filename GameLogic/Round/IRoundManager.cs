namespace GameLogic.Round;

public interface IRoundManager: ITickable
{
    public RoundState State { get; }
    
    void Restart();
}