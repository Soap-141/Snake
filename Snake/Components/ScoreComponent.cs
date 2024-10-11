using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snake.Components;

/// <summary>
/// The score component.
/// </summary>
public sealed class ScoreComponent : DrawableGameComponent
{
    /// <summary>
    /// The position of the score.
    /// </summary>
    private readonly Vector2 _position;

    /// <summary>
    /// The sprite font used to draw the score.
    /// </summary>
    private SpriteFont? _spriteFont;

    /// <summary>
    /// The score (number of apples eaten).
    /// </summary>
    public uint Score { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScoreComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public ScoreComponent(Game game, Vector2 position) : base(game)
    {
        _position = position;
        UpdateOrder = 3;
        DrawOrder = 3;
    }

    /// <summary>
    /// Loads the map's content.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteFont = Game.Content.Load<SpriteFont>("Pixels");

        base.LoadContent();
    }

    /// <summary>
    /// Unloads the map's content.
    /// </summary>
    protected override void UnloadContent()
    {
        _spriteFont = null;

        base.UnloadContent();
    }

    /// <summary>
    /// Draws the score.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        SnakeGame.SpriteBatch.DrawString(_spriteFont, string.Concat("Score: ", Score), _position, Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

        base.Draw(gameTime);
    }
}
