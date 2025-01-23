namespace GameLogic.Input;

public class ConsoleInputHandler: IInputHandler
{
    private Action<ConsoleKey>? _keyPressedAction;
    
    public void StartListening(Action<ConsoleKey> keyPressedAction)
    {
        _keyPressedAction = keyPressedAction;
        Task.Run(TrackInput);
    }

    public void AddKey(ConsoleKey consoleKey)
    {
        _keyPressedAction?.Invoke(consoleKey);
    }

    private void TrackInput()
    {
        while (true)
        {
            var key = Console.ReadKey(true);
            AddKey(key.Key);
        }
    }
}