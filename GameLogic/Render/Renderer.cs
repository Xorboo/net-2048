﻿using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Round;
using GameLogic.State;

namespace GameLogic.Render;

public class Renderer: IRenderer
{
    // UI settings can be moved to Config if needed
    private const int CellWidth = 4;
    private readonly Coordinate _boardPosition = new(2, 4);
    private readonly Coordinate _hihScorePosition = new(5, 1);
    private readonly Coordinate _scorePosition = new(5, 2);
    private readonly Coordinate _promptCenterPosition = new(12, 8);

    private readonly ConsoleColor _fallbackNumberColor = ConsoleColor.DarkGray;
    private readonly Dictionary<int, ConsoleColor> _numberColors = new()
    {
        { 2, ConsoleColor.DarkGray },
        { 4, ConsoleColor.Gray },
        { 8, ConsoleColor.DarkCyan },
        { 16, ConsoleColor.Cyan },
        { 32, ConsoleColor.DarkMagenta },
        { 64, ConsoleColor.Magenta },
        { 128, ConsoleColor.DarkBlue },
        { 256, ConsoleColor.Blue },
        { 512, ConsoleColor.DarkRed },
        { 1024, ConsoleColor.Red },
        { 2048, ConsoleColor.Yellow }
    };

    private readonly string _boardHorizontalSeparatorTop;
    private readonly string _boardHorizontalSeparatorMid;
    private readonly string _boardHorizontalSeparatorBot;
    private readonly string _emptyCell = new(' ', CellWidth);

    private const int ScoreAnimationDuration = 15;
    private int _tickScore = 0;
    private int _tickScoreFramesLeft = 0;

    private const int ShiftAnimationDuration = 7;
    private BoardCommand? _tickCommand = null;
    private int _tickCommandFramesLeft = 0;

    private ActivePrompt _lastActivePrompt = ActivePrompt.None;

    private readonly GameConfiguration _settings;
    
    
    public Renderer(Microsoft.Extensions.Options.IOptions<GameConfiguration> settings)
    {
        _settings = settings.Value;
        int n = _settings.BoardSize;
        string cellLine = new string('─', CellWidth);
        _boardHorizontalSeparatorTop = "┌" + string.Join("┬", Enumerable.Repeat(cellLine, n)) + "┐";
        _boardHorizontalSeparatorMid = "├" + string.Join("┼", Enumerable.Repeat(cellLine, n)) + "┤";
        _boardHorizontalSeparatorBot = "└" + string.Join("┴", Enumerable.Repeat(cellLine, n)) + "┘";
        
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();
    }

    public void Render(GameState gameState, RoundState roundState)
    {
        if (_lastActivePrompt != gameState.ActivePrompt)
        {
            Console.Clear();
        }
            
        RenderRoundState(roundState);
        DrawGameState(gameState);
    }

    private void DrawGameState(GameState gameState)
    {
        DrawHighScore(gameState.HighScore);
        
        _lastActivePrompt = gameState.ActivePrompt;
        if (_lastActivePrompt != ActivePrompt.None)
        {
            DrawPrompt(_lastActivePrompt);
        }
    }

    private void RenderRoundState(RoundState roundState)
    {
        DrawBoard(roundState.Board, roundState.LastMoveCommand);
        DrawScore(roundState.Score, roundState.LastTickScore);
    }

    private void DrawBoard(int[,] board, BoardCommand? lastTickCommand)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            SetPosition(_boardPosition.X, _boardPosition.Y + y * 2);
            Console.Write(y == 0 ? _boardHorizontalSeparatorTop : _boardHorizontalSeparatorMid);

            Console.SetCursorPosition(_boardPosition.X, _boardPosition.Y + y * 2 + 1);
            Console.Write("│");
            for (int x = 0; x < board.GetLength(1); x++)
            {
                int value = board[y, x];
                if (value == 0)
                {
                    Console.Write(_emptyCell);
                }
                else
                {
                    if (!_numberColors.TryGetValue(value, out var color))
                    {
                        color = _fallbackNumberColor;
                    }
                    Console.ForegroundColor = color;
                    Console.Write(value.ToString().PadLeft(CellWidth));
                    Console.ResetColor();
                }
                
                Console.Write("│");
            }
        }

        SetPosition(_boardPosition.X, _boardPosition.Y + board.GetLength(0) * 2);
        Console.Write(_boardHorizontalSeparatorBot);
        
        
        if (lastTickCommand != null)
        {
            if (_tickCommand != null)
            {
                DrawShiftCommand(true);
            }
            
            _tickCommand = lastTickCommand;
            _tickCommandFramesLeft = ShiftAnimationDuration;
        }

        if (_tickCommand != null)
        {
            if (_tickCommandFramesLeft > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                DrawShiftCommand();
                Console.ResetColor();
                _tickCommandFramesLeft--;
            }
            else
            {
                DrawShiftCommand(true);
                _tickCommand = null;
            }
        }
    }

    private void DrawShiftCommand(bool clear = false)
    {
        var pos = _boardPosition;
        int boardWidth = _settings.BoardSize * (CellWidth + 1) + 1;
        int boardHeight = _settings.BoardSize * 2 + 1;
        
        switch (_tickCommand)
        {
            case BoardCommand.Up:
                pos.Y -= 1;
                SetPosition(pos);
                Console.Write(new string(clear ? ' ' : '^', boardWidth));
                break;
            case BoardCommand.Down:
                pos.Y += boardHeight;
                SetPosition(pos);
                Console.Write(new string(clear ? ' ' : 'V', boardWidth));
                break;
            case BoardCommand.Left:
                pos.X -= 1;
                for (int y = 0; y < boardHeight; y++)
                {
                    SetPosition(pos);
                    Console.Write(clear ? ' ' : '<');
                    pos.Y++;
                }
                break;
            case BoardCommand.Right:
                pos.X += boardWidth;
                for (int y = 0; y < boardHeight; y++)
                {
                    SetPosition(pos);
                    Console.Write(clear ? ' ' : '>');
                    pos.Y++;
                }
                break;
        }
    }

    private void DrawScore(int score, int lastTickScore)
    {
        SetPosition(_scorePosition);
        Console.Write($"Score: {score}");

        if (lastTickScore != 0)
        {
            _tickScore = lastTickScore;
            _tickScoreFramesLeft = ScoreAnimationDuration;
        }

        if (_tickScore != 0)
        {
            if (_tickScoreFramesLeft > 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($" (+{_tickScore})".PadRight(8));
                Console.ResetColor();
                _tickScoreFramesLeft--;
            }
            else
            {
                _tickScore = 0;
                Console.Write("".PadRight(8));
            }
        }
    }

    private void DrawHighScore(int highScore)
    {
        SetPosition(_hihScorePosition);
        Console.Write($"HighScore: {highScore}".PadRight(20));
    }

    private void DrawPrompt(ActivePrompt prompt)
    {
        string text;
        switch (prompt)
        {
            case ActivePrompt.Restart:
                text = "Restart? Y/N";
                break;
            case ActivePrompt.GameOverRestart:
                text = "Game Over. Restart? Y/N";
                break;
            case ActivePrompt.Quit:
                text = "Quit? Y/N";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(prompt), prompt, null);
        }

        const int padding = 1;
        string top = "╔" + new string('═', text.Length + padding * 2) + "╗";
        string center = "║" + new string(' ', padding) + text + new string(' ', padding) + "║";
        string bottom = "╚" + new string('═', text.Length + padding * 2) + "╝";

        int width = center.Length;
        var corner = _promptCenterPosition;
        corner.Y -= 1;
        corner.X -= width / 2;
        SetPosition(corner.X, corner.Y);
        Console.Write(top);
        SetPosition(corner.X, corner.Y + 1);
        Console.Write(center);
        SetPosition(corner.X, corner.Y + 2);
        Console.Write(bottom);
    }

    private void SetPosition(Coordinate coordinate)
    {
        SetPosition(coordinate.X, coordinate.Y);
    }

    private void SetPosition(int x, int y)
    {
        Console.SetCursorPosition(Math.Max(0, x), Math.Max(0, y));
    }
}