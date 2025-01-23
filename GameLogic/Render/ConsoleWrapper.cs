namespace GameLogic.Render;

public class ConsoleWrapper: IConsoleWrapper
{
    public void Setup()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void Write(string text)
    {
        Console.Write(text);
    }

    public void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Write(text);
        Console.ResetColor();
    }

    public void SetCursorPosition(int x, int y)
    {
        Console.SetCursorPosition(x, y);
    }
}