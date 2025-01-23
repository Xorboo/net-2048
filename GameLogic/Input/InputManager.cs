using System.Collections.Concurrent;

namespace GameLogic.Input;

public class InputManager: IInputManager
{
    private readonly List<BoardCommand> _commands = new();
    private bool _isRestartPressed;
    private bool _isQuitPressed;
    private bool _isYesPressed;
    private bool _isNoPressed;
    
    
    private readonly ConcurrentQueue<ConsoleKey> _inputBuffer = new();

    public InputManager()
    {
        Task.Run(TrackInput);
    }
    
    public void Tick()
    {
        _isRestartPressed = false;
        _isQuitPressed = false;
        _isYesPressed = false;
        _isNoPressed = false;
        _commands.Clear();
        
        while (!_inputBuffer.IsEmpty)
        {
            while (_inputBuffer.TryDequeue(out var key))
            {
                switch (key)
                {
                    case ConsoleKey.R:
                        _isRestartPressed = true;
                        break;
                    case ConsoleKey.Q:
                        _isQuitPressed = true;
                        break;
                    case ConsoleKey.Y:
                        _isYesPressed = true;
                        break;
                    case ConsoleKey.N:
                        _isNoPressed = true;
                        break;
                    default:
                        var command = ParseDirectionCommand(key);
                        if (command != null)
                        {
                            _commands.Add(command.Value);
                        }
                        break;
                }
            }
        }
    }

    public bool IsRestartPressed()
    {
        return _isRestartPressed;
    }

    public bool IsQuitPressed()
    {
        return _isQuitPressed;
    }

    public bool IsYesPressed()
    {
        return _isYesPressed;
    }

    public bool IsNoPressed()
    {
        return _isNoPressed;
    }

    public BoardCommand GetNextCommand()
    {
        if (_commands.Count == 0)
        {
            return BoardCommand.None;
        }

        var command = _commands[0];
        _commands.RemoveAt(0);
        return command;
    }

    private BoardCommand? ParseDirectionCommand(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.W:
                return BoardCommand.Up;
            case ConsoleKey.A:
                return BoardCommand.Left;
            case ConsoleKey.S:
                return BoardCommand.Down;
            case ConsoleKey.D:
                return BoardCommand.Right;
        }

        return null;
    }

    private void TrackInput()
    {
        while (true)
        {
            var key = Console.ReadKey(true);
            _inputBuffer.Enqueue(key.Key);
        }
    }
}