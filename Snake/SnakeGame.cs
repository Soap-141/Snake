using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake;

/// <summary>
/// The snake game loop.
/// </summary>
public sealed class SnakeGame : Game
{
    /// <summary>
    /// Used to initialize and control the presentation of the graphics device.
    /// </summary>
    private readonly GraphicsDeviceManager _graphics;

    /// <summary>
    /// The map.
    /// </summary>
    private readonly MapComponent _map;

    /// <summary>
    /// The sprite batch used to draw textures.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeGame"/> class.
    /// </summary>
    public SnakeGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 400,
            PreferredBackBufferHeight = 400,
            SynchronizeWithVerticalRetrace = true
        };
        _map = new MapComponent(this, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// Initializes the game.
    /// </summary>
    protected override void Initialize()
    {
        Components.Add(_map);

        base.Initialize();
    }

    /// <summary>
    /// Loads the game content.
    /// </summary>
    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    /// <summary>
    /// Updates the game.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    protected override void Update(GameTime gameTime)
    {
        // The game is paused when the window is not active (e.g. lost focus).
        if (!IsActive)
            return;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the game.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin();
        base.Draw(gameTime);
        SpriteBatch.End();
    }
}
