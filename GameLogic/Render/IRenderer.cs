using GameLogic.Round;
using GameLogic.State;

namespace GameLogic.Render;

public interface IRenderer
{
    void Render(GameState gameState, RoundState roundState);
    void Clear();
}