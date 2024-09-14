using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snake.Components.Snake;
using System;
using System.Linq;

namespace Snake.Components;

/// <summary>
/// The map component where the game takes place.
/// <br/>
/// It manages the game's boundaries, the <see cref="SnakeComponent"/> and the <see cref="AppleComponent"/>.
/// </summary>
public sealed class MapComponent : DrawableGameComponent
{
    /// <summary>
    /// The texture size.
    /// </summary>
    /// <remarks>
    /// The original textures are 32x32 pixels, but they are scaled down to 16x16 pixels.
    /// </remarks>
    private readonly System.Drawing.Size _textureSize = new(16, 16);

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
    /// The score.
    /// </summary>
    private readonly ScoreComponent _score;

    /// <summary>
    /// The game over component.
    /// </summary>
    private readonly GameOverComponent _gameOver;

    /// <summary>
    /// The grss texture.
    /// </summary>
    private Texture2D? _grassTexture;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="width">The map's width.</param>
    /// <param name="height">The map's height.</param>
    public MapComponent(Game game, int width, int height) : base(game)
    {
        _size = new System.Drawing.Size(width, height);
        _apple = new AppleComponent(game, new Point(48, 48));
        _snake = new SnakeComponent(game, new Point(SnakeComponent.TextureSize.Width * 2, 0), Direction.Right);
        _score = new ScoreComponent(game, new Vector2(6, height - 30)); // The sprite font size is 48 but we scale the score at 50%.
        _gameOver = new GameOverComponent(game);

        UpdateOrder = 0;
        DrawOrder = 0;
    }

    /// <summary>
    /// Initializes the map.
    /// </summary>
    public override void Initialize()
    {
        Game.Components.Add(_apple);
        Game.Components.Add(_snake);
        Game.Components.Add(_score);
        Game.Components.Add(_gameOver);

        base.Initialize();
    }

    /// <summary>
    /// Loads the map's content.
    /// </summary>
    protected override void LoadContent()
    {
        _grassTexture = Game.Content.Load<Texture2D>("Grass");

        base.LoadContent();
    }

    /// <summary>
    /// Unloads the map's content.
    /// </summary>
    protected override void UnloadContent()
    {
        _grassTexture?.Dispose();
        _grassTexture = null;

        base.UnloadContent();
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
    /// Draws the map.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        for (var i = 0; i < _size.Height; i += _textureSize.Height)
        {
            for (var j = 0; j < _size.Width; j += _textureSize.Width)
            {
                SnakeGame.SpriteBatch.Draw(_grassTexture, new Vector2(j, i), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
        }

        base.Draw(gameTime);
    }

    /// <summary>
    /// Handles the snake's collision with the wall, itself and the apple.
    /// </summary>
    private void HandleSnakeCollision()
    {
        if (_snake.IsCollidingWithItself() || IsSnakeCollidingWithWall())
        {
            // TODO: Handle game over differently?

            // Visible needs to be set before Enabled otherwise the game over won't be drawn?
            _gameOver.Visible = true;
            _gameOver.Enabled = true;

            _score.Score = 0;

            // Visible needs to be set before Enabled otherwise the snake won't be drawn?
            _snake.Visible = false; // Stops drawing the snake.
            _snake.Enabled = false; // Stops updating the snake.

            // Resets the apple to its initial position because it could be in the snake's body (snake's initial position).
            // Visible needs to be set before Enabled otherwise the apple won't be drawn?
            _apple.Visible = false; // Stops drawing the apple.
            _apple.Enabled = false; // Stops updating the apple.
        }
        else if (CanSnakeEat())
        {
            _score.Score++;
            _snake.Grow();
            SetRandomApplePosition();
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

    /// <summary>
    /// Sets a new random position for the apple.
    /// </summary>
    /// <remarks>
    /// The apple's position is set to a random position within the map's boundaries and can't be on the snake.
    /// </remarks>
    private void SetRandomApplePosition()
    {
        // TODO: Optimize?

        var random = new Random();
        var snakePositions = _snake.Parts.Select(part => part.Position).ToHashSet();

        // Divide by texture size and multiply by texture size to ensure the apple is placed in a multiple of texture size.
        var maxAbscissa = (_size.Width - AppleComponent.TextureSize.Width) / AppleComponent.TextureSize.Width;
        var maxOrdinate = (_size.Height - AppleComponent.TextureSize.Height) / AppleComponent.TextureSize.Height;

        do
        {
            _apple.Position = new Point(random.Next(maxAbscissa) * AppleComponent.TextureSize.Width, random.Next(maxOrdinate) * AppleComponent.TextureSize.Height);
        } while (snakePositions.Contains(_apple.Position));
    }
}
