﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake.Components;

/// <summary>
/// The game over component.
/// </summary>
public sealed class GameOverComponent : DrawableGameComponent
{
    /// <summary>
    /// The game over title text.
    /// </summary>
    private readonly string _titleText = "GAME OVER";

    /// <summary>
    /// The game over sub title text.
    /// </summary>
    private readonly string _subTitleText = "Press ENTER to continue";

    /// <summary>
    /// The sub title scaling.
    /// </summary>
    private readonly Vector2 _subTitleScale = new(0.5f);

    /// <summary>
    /// The sprite font used to draw the game over text.
    /// </summary>
    private SpriteFont? _spriteFont;

    /// <summary>
    /// The background texture.
    /// </summary>
    private Texture? _backgroundTexture;

    /// <summary>
    /// The game over title text's position.
    /// </summary>
    private Vector2 _titlePosition;

    /// <summary>
    /// The game over sub title text's position.
    /// </summary>
    private Vector2 _subTitlePosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameOverComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public GameOverComponent(Game game) : base(game)
    {
        UpdateOrder = 4;
        DrawOrder = 4;
        Enabled = false;
        Visible = false;
    }

    /// <summary>
    /// Loads the game over's content.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteFont = Game.Content.Load<SpriteFont>("Pixels");
        _backgroundTexture = Game.Content.Load<Texture2D>("WhiteBackground");

        var titleTextSize = _spriteFont.MeasureString(_titleText);
        var subTitleTextSize = _spriteFont.MeasureString(_subTitleText) * _subTitleScale;
        var windowWidth = Game.Window.ClientBounds.Width;
        var windowHeight = Game.Window.ClientBounds.Height;

        _titlePosition = new Vector2(
            windowWidth / 2 - titleTextSize.X / 2,
            windowHeight / 2 - titleTextSize.Y / 2 - 24 // The sprite font size is 48.
        );

        _subTitlePosition = new Vector2(
            windowWidth / 2 - subTitleTextSize.X / 2,
            windowHeight / 2 - subTitleTextSize.Y / 2 + 24 // The sprite font size is 48.
        );

        base.LoadContent();
    }

    /// <summary>
    /// Unloads the game over's content.
    /// </summary>
    protected override void UnloadContent()
    {
        _spriteFont = null;

        base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
        HandleInput();

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the game over.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        DrawBackground();

        SnakeGame.SpriteBatch.DrawString(_spriteFont, _titleText, _titlePosition, Color.Black);
        SnakeGame.SpriteBatch.DrawString(_spriteFont, _subTitleText, _subTitlePosition, Color.Black, 0f, Vector2.Zero, _subTitleScale, SpriteEffects.None, 0f);

        base.Draw(gameTime);
    }

    private void DrawBackground()
    {
        SnakeGame.SpriteBatch.Draw((Texture2D)_backgroundTexture!, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Handles the snake's input.
    /// </summary>
    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            Enabled = false;
            Visible = false;
        }
    }
}
