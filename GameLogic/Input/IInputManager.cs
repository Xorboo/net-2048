namespace GameLogic.Input;

public interface IInputManager: ITickable
{
    bool IsRestartPressed();
    bool IsQuitPressed();
    PromptResponse? GetPromptResponse();
    BoardCommand GetNextCommand();
    void AddKey(ConsoleKey key);
}