namespace GameLogic.Input;

public interface IInputHandler
{
    void StartListening(Action<ConsoleKey> keyPressedAction);
    void AddKey(ConsoleKey consoleKey);
}