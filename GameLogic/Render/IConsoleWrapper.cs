namespace GameLogic.Render;

public interface IConsoleWrapper
{
    void Setup();
    void Clear();
    void Write(string text);
    void Write(string text, ConsoleColor color);
    void SetCursorPosition(int x, int y);
}