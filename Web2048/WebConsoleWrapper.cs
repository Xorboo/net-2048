using GameLogic.Render;
using Microsoft.JSInterop;

namespace Web2048;

public class WebConsoleWrapper: IConsoleWrapper
{
    private readonly IJSRuntime _jsRuntime;
    
    public WebConsoleWrapper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void Setup()
    {
    }

    public void Clear()
    {
        _jsRuntime.InvokeVoidAsync("consoleEmulator.clear");
    }

    public void Write(string text)
    {
        _jsRuntime.InvokeVoidAsync("consoleEmulator.write", text);
    }

    public void Write(string text, ConsoleColor color)
    {
        _jsRuntime.InvokeVoidAsync("consoleEmulator.writeColor", text, (int)color);
    }

    void IConsoleWrapper.SetCursorPosition(int x, int y)
    {
        _jsRuntime.InvokeVoidAsync("consoleEmulator.setCursor", x, y);
    }
}
