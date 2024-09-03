using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake;

/// <summary>
/// The apple component.
/// </summary>
public sealed class AppleComponent : DrawableGameComponent
{
    /// <summary>
    /// The square texture size.
    /// </summary>
    public static readonly System.Drawing.Size TextureSize = new(8, 8);

    /// <summary>
    /// The red square texture for the snake's food (apple).
    /// </summary>
    private Texture2D _redSquareTexture;

    /// <summary>
    /// The position of the apple.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="initialPosition">The apple's initial position.</param>
    public AppleComponent(Game game, Point initialPosition) : base(game)
    {
        Position = initialPosition;
        DrawOrder = 0;
    }

    /// <summary>
    /// Loads the apple's content.
    /// </summary>
    protected override void LoadContent()
    {
        _redSquareTexture = Game.Content.Load<Texture2D>("redSquare");
    }

    /// <summary>
    /// Draws the apple.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        SnakeGame.SpriteBatch.Draw(_redSquareTexture, Position.ToVector2(), Color.White);
    }
}
