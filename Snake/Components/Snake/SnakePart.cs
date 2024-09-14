using Microsoft.Xna.Framework;

namespace Snake.Components.Snake;

/// <summary>
/// A part of the snake.
/// </summary>
public struct SnakePart
{
    /// <summary>
    /// The position.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// The direction.
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakePart"/> class.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="direction">The direction.</param>
    public SnakePart(Point position, Direction direction)
    {
        Position = position;
        Direction = direction;
    }
}
