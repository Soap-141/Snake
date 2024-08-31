using Microsoft.Xna.Framework;

namespace Snake;

/// <summary>
/// The directions the snake can move.
/// </summary>
public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

/// <summary>
/// A part of the snake.
/// </summary>
public struct SnakePart
{
    public Point Position { get; }

    public Direction? Direction { get; set; }

    public SnakePart(Point position, Direction? direction)
    {
        Position = position;
        Direction = direction;
    }
}
