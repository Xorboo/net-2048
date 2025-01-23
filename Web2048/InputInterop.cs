using GameLogic.Input;
using Microsoft.JSInterop;

namespace Web2048;

public static class InputInterop
{
    private static InputManager _inputManager = new();

    [JSInvokable]
    public static void EnqueueKey(string key)
    {
        var keyInfo = new ConsoleKeyInfo(key[0], ConsoleKey.NoName, false, false, false);
        // TODO _inputManager.AddInput(keyInfo);
    }

    public static InputManager GetInputManager()
    {
        return _inputManager;
    }
}
