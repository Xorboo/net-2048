using GameLogic.Input;

namespace Web2048;

public class WebInputHandler: IInputHandler
{
    private Action<ConsoleKey>? _keyPressedAction;
    
    public void StartListening(Action<ConsoleKey> keyPressedAction)
    {
        _keyPressedAction = keyPressedAction;
    }
    
    public void AddKey(ConsoleKey consoleKey)
    {
        _keyPressedAction?.Invoke(consoleKey);
    }
}