using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake.Components;

/// <summary>
/// The apple component.
/// </summary>
/// <remarks>
/// The original textures are 40x40 pixels, but they are scaled down to 16x16 pixels.
/// </remarks>
public sealed class AppleComponent : DrawableGameComponent
{
    /// <summary>
    /// The square texture size.
    /// </summary>
    /// <remarks>
    /// The original textures are 32x32 pixels, but they are scaled down to 16x16 pixels.
    /// </remarks>
    public static readonly System.Drawing.Size TextureSize = new(16, 16);

    /// <summary>
    /// The initial position of the apple.
    /// </summary>
    private readonly Point _initialPosition;

    /// <summary>
    /// The apple texture.
    /// </summary>
    private Texture2D _appleTexture;

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
        _initialPosition = initialPosition;
        Position = _initialPosition;
        UpdateOrder = 1;
        DrawOrder = 1;
    }

    /// <summary>
    /// Loads the apple's content.
    /// </summary>
    protected override void LoadContent()
    {
        _appleTexture = Game.Content.Load<Texture2D>("Apple");

        base.LoadContent();
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void UnloadContent()
    {
        _appleTexture.Dispose();

        base.UnloadContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        if (!Enabled)
        {
            Position = _initialPosition;

            // Visible needs to be set before Enabled otherwise the snake won't be drawn?
            Visible = true; // Starts drawing the snake again.
            Enabled = true; // Starts updating the snake again.
        }
        base.OnEnabledChanged(sender, args);
    }

    /// <summary>
    /// Draws the apple.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        SnakeGame.SpriteBatch.Draw(_appleTexture, Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);

        base.Draw(gameTime);
    }
}
