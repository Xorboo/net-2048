using GameLogic.Configuration;
using GameLogic.Input;
using GameLogic.Round;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests.UnitTests;

public class MergerTests
{
    private readonly Mock<IOptions<GameConfiguration>> _mockSettings;
    
    public MergerTests()
    {
        _mockSettings = new();
        _mockSettings.Setup(s => s.Value).Returns(new GameConfiguration { BoardSize = 4 });
    }

    [Theory]
    [MemberData(nameof(PairShiftTestCases))]
    public void Should_MergePairs_When_TryShift(
        int[,] board, BoardCommand command, int[,] expectedBoard, int expectedScore)
    {
        // Arrange
        var merger = new Merger(_mockSettings.Object);

        // Act
        var result = merger.TryShift(board, command);

        // Assert
        Assert.Equal(expectedBoard, board);
        Assert.Equal(expectedScore, result.Score);
    }

    // TODO More tests would go there ¯\_(ツ)_/¯
    
    public static IEnumerable<object[]> PairShiftTestCases =>
        new List<object[]>
        {
            new object[]
            {
                new[,] { { 0, 0, 0, 0 }, { 2, 2, 2, 2 }, { 0, 0, 0, 0 }, { 2, 2, 2, 2 } },
                BoardCommand.Left,
                new[,] { { 0, 0, 0, 0 }, { 4, 4, 0, 0 }, { 0, 0, 0, 0 }, { 4, 4, 0, 0 } },
                16
            },
            new object[]
            {
                new[,] { { 0, 0, 0, 0 }, { 2, 2, 2, 2 }, { 0, 0, 0, 0 }, { 2, 2, 2, 2 } },
                BoardCommand.Right,
                new[,] { { 0, 0, 0, 0 }, { 0, 0, 4, 4 }, { 0, 0, 0, 0 }, { 0, 0, 4, 4 } },
                16
            },
            new object[]
            {
                new[,] { { 2, 0, 2, 0 }, { 2, 0, 2, 0 }, { 2, 0, 2, 0 }, { 2, 0, 2, 0 } },
                BoardCommand.Up,
                new[,] { { 4, 0, 4, 0 }, { 4, 0, 4, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } },
                16
            },
            new object[]
            {
                new[,] { { 2, 0, 2, 0 }, { 2, 0, 2, 0 }, { 2, 0, 2, 0 }, { 2, 0, 2, 0 } },
                BoardCommand.Down,
                new[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 4, 0, 4, 0 }, { 4, 0, 4, 0 } },
                16
            }
        };
}