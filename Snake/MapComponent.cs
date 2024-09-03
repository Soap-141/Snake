using Microsoft.Xna.Framework;
using System;

namespace Snake;

/// <summary>
/// The map component where the game takes place.
/// <br/>
/// It manages the game's boundaries, the <see cref="SnakeComponent"/> and the <see cref="AppleComponent"/>.
/// </summary>
public sealed class MapComponent : GameComponent
{
    /// <summary>
    /// The map's size.
    /// </summary>
    private readonly System.Drawing.Size _size;

    /// <summary>
    /// The apple.
    /// </summary>
    private readonly AppleComponent _apple;

    /// <summary>
    /// The snake.
    /// </summary>
    private readonly SnakeComponent _snake;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="width">The map's width.</param>
    /// <param name="height">The map's height.</param>
    public MapComponent(Game game, int width, int height) : base(game)
    {
        _size = new System.Drawing.Size(width, height);
        _apple = new AppleComponent(game, new Point(24, 24));
        _snake = new SnakeComponent(game, new Point(0, 0));
    }

    /// <summary>
    /// Initializes the map.
    /// </summary>
    public override void Initialize()
    {
        Game.Components.Add(_apple);
        Game.Components.Add(_snake);

        base.Initialize();
    }

    /// <summary>
    /// Updates the map.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Update(GameTime gameTime)
    {
        HandleSnakeCollision();

        base.Update(gameTime);
    }

    /// <summary>
    /// Handles the snake's collision with the wall, itself and the apple.
    /// </summary>
    private void HandleSnakeCollision()
    {
        if (_snake.IsCollidingWithItself() || IsSnakeCollidingWithWall())
        {
            _snake.Enabled = false;

            // TODO: Handle game over differently?
        }
        else if (CanSnakeEat())
        {
            _snake.Grow();

            var random = new Random();

            // Divide by texture size and multiply by texture size to ensure the apple is placed in a multiple of texture size.
            var maxAbscissa = (_size.Width - AppleComponent.TextureSize.Width) / AppleComponent.TextureSize.Width;
            var maxOrdinate = (_size.Height - AppleComponent.TextureSize.Height) / AppleComponent.TextureSize.Height;
            _apple.Position = new Point(random.Next(maxAbscissa) * AppleComponent.TextureSize.Width, random.Next(maxOrdinate) * AppleComponent.TextureSize.Height);
        }
    }

    /// <summary>
    /// Returns whether the snake is colliding with the wall.
    /// </summary>
    /// <returns>Whether the snake is colliding with the wall.</returns>
    private bool IsSnakeCollidingWithWall()
    {
        // TODO: Use the snake's interpolated head position instead of the actual head position for more accurate collision detection?
        var snakeHeadPosition = _snake.Head;

        return snakeHeadPosition.Position.X < 0 || snakeHeadPosition.Position.X + SnakeComponent.TextureSize.Width > _size.Width
            || snakeHeadPosition.Position.Y < 0 || snakeHeadPosition.Position.Y + SnakeComponent.TextureSize.Height > _size.Height;
    }

    /// <summary>
    /// Returns whether the snake can eat the apple.
    /// </summary>
    /// <returns>whether the snake can eat the apple.</returns>
    private bool CanSnakeEat()
    {
        var snakeHeadPosition = _snake.Head;
        var applePosition = _apple.Position;

        return snakeHeadPosition.Position == applePosition;
    }
}
