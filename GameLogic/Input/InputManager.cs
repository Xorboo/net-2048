﻿using System.Collections.Concurrent;

namespace GameLogic.Input;

public class InputManager: IInputManager
{
    private readonly List<BoardCommand> _commands = new();
    private bool _isRestartPressed;
    private bool _isQuitPressed;
    private bool _isYesPressed;
    private bool _isNoPressed;

    private readonly ConcurrentQueue<ConsoleKey> _inputBuffer = new();

    public InputManager(IInputHandler inputHandler)
    {
        inputHandler.StartListening(key => _inputBuffer.Enqueue(key));
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

    public PromptResponse? GetPromptResponse()
    {
        if (_isYesPressed)
        {
            return PromptResponse.Yes;
        }

        if (_isNoPressed)
        {
            return PromptResponse.No;
        }

        return null;
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

    public void AddKey(ConsoleKey key)
    {
        _inputBuffer.Enqueue(key);
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
}