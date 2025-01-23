namespace GameLogic.Input;

public interface IInputManager: ITickable
{
    bool IsRestartPressed();
    bool IsQuitPressed();
    bool IsYesPressed();
    bool IsNoPressed();
    BoardCommand GetNextCommand();
    void AddKey(ConsoleKey key);
}