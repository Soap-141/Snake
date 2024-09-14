using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake.Components.Snake;

/// <summary>
/// The snake component.
/// </summary>
public sealed class SnakeComponent : DrawableGameComponent
{
    /// <summary>
    /// The snake's textures.
    /// </summary>
    private enum SnakeTexture
    {
        BodyBottomLeft,
        BodyBottomRight,
        BodyHorizontal,
        BodyTopLeft,
        BodyTopRight,
        BodyVertical,
        HeadDown,
        HeadLeft,
        HeadRight,
        HeadUp,
        TailDown,
        TailLeft,
        TailRight,
        TailUp,
    }

    /// <summary>
    /// Map of the snake's textures based on the directions (previous, current) of the snake's body parts.
    /// </summary>
    private readonly Dictionary<(Direction, Direction), SnakeTexture> _directionsTextureMap = new()
    {
        { (Direction.Up, Direction.Up), SnakeTexture.BodyVertical },
        { (Direction.Down, Direction.Down), SnakeTexture.BodyVertical },
        { (Direction.Up, Direction.Left), SnakeTexture.BodyTopRight },
        { (Direction.Right, Direction.Down), SnakeTexture.BodyTopRight },
        { (Direction.Up, Direction.Right), SnakeTexture.BodyTopLeft },
        { (Direction.Left, Direction.Down), SnakeTexture.BodyTopLeft },
        { (Direction.Down, Direction.Left), SnakeTexture.BodyBottomRight },
        { (Direction.Right, Direction.Up), SnakeTexture.BodyBottomRight },
        { (Direction.Down, Direction.Right), SnakeTexture.BodyBottomLeft },
        { (Direction.Left, Direction.Up), SnakeTexture.BodyBottomLeft },
        { (Direction.Left, Direction.Left), SnakeTexture.BodyHorizontal },
        { (Direction.Right, Direction.Right), SnakeTexture.BodyHorizontal }
    };

    /// <summary>
    /// The square texture size.
    /// </summary>
    /// <remarks>
    /// The original textures are 40x40 pixels, but they are scaled down to 16x16 pixels.
    /// </remarks>
    public static readonly System.Drawing.Size TextureSize = new(16, 16);

    /// <summary>
    /// The texture scale.
    /// </summary>
    private readonly float _textureScale = 0.4f;

    /// <summary>
    /// The snake's textures.
    /// </summary>
    private readonly Dictionary<SnakeTexture, Texture2D> _snakeTextures = [];

    /// <summary>
    /// The time between each movement.
    /// </summary>
    private readonly float _moveInterval = 0.2f;

    /// <summary>
    /// The snake's initial position.
    /// </summary>
    private readonly Point _initialPosition;

    /// <summary>
    /// The snake's initial direction.
    /// </summary>
    private readonly Direction _initialDirection = Direction.Right;

    /// <summary>
    /// The time since the last move.
    /// </summary>
    private float _timeSinceLastMove;

    /// <summary>
    /// The next direction the snake will move to.
    /// </summary>
    private Direction? _nextDirection;

    /// <summary>
    /// The interpolated head position.
    /// </summary>
    private Vector2 _interpolatedHeadPosition;

    /// <summary>
    /// The interpolated tail position.
    /// </summary>
    private Vector2? _interpolatedTailPosition;

    /// <summary>
    /// The snake's parts (head, body and tail).
    /// </summary>
    public LinkedList<SnakePart> Parts { get; } = new();

    /// <summary>
    /// The snake's head.
    /// </summary>
    public ref SnakePart Head => ref Parts.First!.ValueRef;

    /// <summary>
    /// The snake's tail.
    /// </summary>
    public ref SnakePart Tail => ref Parts.Last!.ValueRef;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeComponent"/> class.
    /// </summary>
    /// <remarks>
    /// The snake starts with a lenght of 3 parts, be careful with the initial position and direction.
    /// </remarks>
    /// <param name="game">The game.</param>
    /// <param name="initialPosition">The snake's head initial position.</param>
    /// <param name="initialDirection">The snake's direction.</param>
    public SnakeComponent(Game game, Point initialPosition, Direction initialDirection) : base(game)
    {
        _initialPosition = initialPosition;
        _initialDirection = initialDirection;

        UpdateOrder = 2;
        DrawOrder = 2;

        InitializeParts();
    }

    /// <summary>
    /// Initializes the snake's parts.
    /// </summary>
    private void InitializeParts()
    {
        // Head.
        var part = Parts.AddFirst(new SnakePart(_initialPosition, _initialDirection));
        _interpolatedHeadPosition = part.ValueRef.Position.ToVector2();

        // Body.
        part = Parts.AddLast(new SnakePart(new Point(part.ValueRef.Position.X - TextureSize.Width, part.ValueRef.Position.Y), _initialDirection));

        // Tail.
        part = Parts.AddLast(new SnakePart(new Point(part.ValueRef.Position.X - TextureSize.Width, part.ValueRef.Position.Y), _initialDirection));
        _interpolatedTailPosition = part.ValueRef.Position.ToVector2();
    }

    /// <summary>
    /// On enabled changed event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    protected override void OnEnabledChanged(object sender, EventArgs e)
    {
        if (!Enabled)
        {
            Parts.Clear();
            InitializeParts();

            _nextDirection = null;
            _timeSinceLastMove = 0f;

            // Visible needs to be set before Enabled otherwise the snake won't be drawn?
            Visible = true; // Starts drawing the snake again.
            Enabled = true; // Starts updating the snake again.
        }
        base.OnEnabledChanged(sender, e);
    }

    /// <summary>
    /// Loads the snake's content.
    /// </summary>
    protected override void LoadContent()
    {
        var gameContent = Game.Content;

        _snakeTextures.Add(SnakeTexture.BodyBottomLeft, gameContent.Load<Texture2D>("BodyBottomLeft"));
        _snakeTextures.Add(SnakeTexture.BodyBottomRight, gameContent.Load<Texture2D>("BodyBottomRight"));
        _snakeTextures.Add(SnakeTexture.BodyHorizontal, gameContent.Load<Texture2D>("BodyHorizontal"));
        _snakeTextures.Add(SnakeTexture.BodyTopLeft, gameContent.Load<Texture2D>("BodyTopLeft"));
        _snakeTextures.Add(SnakeTexture.BodyTopRight, gameContent.Load<Texture2D>("BodyTopRight"));
        _snakeTextures.Add(SnakeTexture.BodyVertical, gameContent.Load<Texture2D>("BodyVertical"));
        _snakeTextures.Add(SnakeTexture.HeadDown, gameContent.Load<Texture2D>("HeadDown"));
        _snakeTextures.Add(SnakeTexture.HeadLeft, gameContent.Load<Texture2D>("HeadLeft"));
        _snakeTextures.Add(SnakeTexture.HeadRight, gameContent.Load<Texture2D>("HeadRight"));
        _snakeTextures.Add(SnakeTexture.HeadUp, gameContent.Load<Texture2D>("HeadUp"));
        _snakeTextures.Add(SnakeTexture.TailDown, gameContent.Load<Texture2D>("TailDown"));
        _snakeTextures.Add(SnakeTexture.TailLeft, gameContent.Load<Texture2D>("TailLeft"));
        _snakeTextures.Add(SnakeTexture.TailRight, gameContent.Load<Texture2D>("TailRight"));
        _snakeTextures.Add(SnakeTexture.TailUp, gameContent.Load<Texture2D>("TailUp"));
    }

    /// <summary>
    /// Unloads the snake's content.
    /// </summary>
    protected override void UnloadContent()
    {
        foreach (var texture in _snakeTextures.Values)
        {
            texture.Dispose();
        }

        _snakeTextures.Clear();

        base.UnloadContent();
    }

    /// <summary>
    /// Updates the snake.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Update(GameTime gameTime)
    {
        _timeSinceLastMove += (float)gameTime.ElapsedGameTime.TotalSeconds;

        HandleInput();

        if (_timeSinceLastMove >= _moveInterval)
        {
            Move();
            _timeSinceLastMove = 0f;
        }
        else
        {
            MoveInterpolatedHead();
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the snake.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        DrawNeck();
        DrawBody();
        DrawInterpolatedHead();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Draws the snake's neck.
    /// </summary>
    private void DrawNeck()
    {
        Texture2D? neckTexture;
        Rectangle? destinationRectangle;

        switch (Head.Direction)
        {
            case Direction.Up:
                neckTexture = _snakeTextures[SnakeTexture.BodyVertical];
                destinationRectangle = new Rectangle(Head.Position.X, (int)_interpolatedHeadPosition.Y + TextureSize.Height, TextureSize.Width, Head.Position.Y - (int)_interpolatedHeadPosition.Y);
                break;
            case Direction.Down:
                neckTexture = _snakeTextures[SnakeTexture.BodyVertical];
                destinationRectangle = new Rectangle(Head.Position.X, Head.Position.Y, TextureSize.Width, (int)MathF.Round(_interpolatedHeadPosition.Y) - Head.Position.Y);
                break;
            case Direction.Left:
                neckTexture = _snakeTextures[SnakeTexture.BodyHorizontal];
                destinationRectangle = new Rectangle((int)_interpolatedHeadPosition.X + TextureSize.Width, Head.Position.Y, Head.Position.X - (int)_interpolatedHeadPosition.X, TextureSize.Height);
                break;
            case Direction.Right:
                neckTexture = _snakeTextures[SnakeTexture.BodyHorizontal];
                destinationRectangle = new Rectangle(Head.Position.X, Head.Position.Y, (int)MathF.Round(_interpolatedHeadPosition.X) - Head.Position.X, TextureSize.Height);
                break;
            default:
                return;
        }

        SnakeGame.SpriteBatch.Draw(
            texture: neckTexture,
            destinationRectangle: (Rectangle)destinationRectangle,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0f,
            origin: Vector2.Zero,
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
    }

    private void DrawBody()
    {
        // Skips the head and tail, and draws the body parts.
        var part = Parts.First!.Next;
        while (part?.Next is not null)
        {
            // Previous is always on the head side.
            var previousDirection = part!.Previous!.ValueRef.Direction;

            // Current is always on the tail side.
            var currentDirection = part!.ValueRef.Direction!;

            if (_directionsTextureMap.TryGetValue((previousDirection, currentDirection), out var bodyTexture))
            {
                SnakeGame.SpriteBatch.Draw(_snakeTextures[bodyTexture], part.ValueRef.Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, _textureScale, SpriteEffects.None, 0f);
            }

            part = part.Next;
        };

        if (_interpolatedTailPosition is not null)
        {
            var tailTexture = Tail.Direction switch
            {
                Direction.Up => _snakeTextures[SnakeTexture.TailDown],
                Direction.Down => _snakeTextures[SnakeTexture.TailUp],
                Direction.Left => _snakeTextures[SnakeTexture.TailRight],
                Direction.Right => _snakeTextures[SnakeTexture.TailLeft],
                _ => _snakeTextures[SnakeTexture.TailLeft]
            };
            SnakeGame.SpriteBatch.Draw(tailTexture, Tail.Position.ToVector2(), null, Color.White, 0f, Vector2.Zero, _textureScale, SpriteEffects.None, 0f);
        }
    }

    /// <summary>
    /// Returns whether the snake is colliding with itself.
    /// </summary>
    /// <returns>Whether the snake is colliding with itself.</returns>
    public bool IsCollidingWithItself()
    {
        // Skip the head and check if the head's position is equal to any other position.
        return Parts.Skip(1).Any(position => position.Position == Head.Position);
    }

    /// <summary>
    /// Grows the snake.
    /// </summary>
    public void Grow()
    {
        var tail = Tail;
        var nextTailPosition = tail.Direction switch
        {
            Direction.Up => new Point(tail.Position.X, tail.Position.Y + TextureSize.Height),
            Direction.Down => new Point(tail.Position.X, tail.Position.Y - TextureSize.Height),
            Direction.Left => new Point(tail.Position.X + TextureSize.Width, tail.Position.Y),
            Direction.Right => new Point(tail.Position.X - TextureSize.Width, tail.Position.Y),
            _ => Point.Zero,
        };
        Parts.AddLast(new SnakePart(nextTailPosition, tail.Direction));
    }

    /// <summary>
    /// Handles the snake's input.
    /// </summary>
    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
        {
            SetDirection(Direction.Up);
        }
        else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
        {
            SetDirection(Direction.Down);
        }
        else if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
        {
            SetDirection(Direction.Left);
        }
        else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
        {
            SetDirection(Direction.Right);
        }
    }

    /// <summary>
    /// Sets the next snake's direction.
    /// </summary>
    /// <remarks>
    /// The snake can't move in the opposite direction it's currently moving.
    /// </remarks>
    /// <param name="direction">The snake's direction.</param>
    private void SetDirection(Direction direction)
    {
        if (_nextDirection == direction)
        {
            return;
        }

        var headDirection = Head.Direction;

        if (headDirection == Direction.Up && direction == Direction.Down ||
            headDirection == Direction.Down && direction == Direction.Up ||
            headDirection == Direction.Left && direction == Direction.Right ||
            headDirection == Direction.Right && direction == Direction.Left)
        {
            return;
        }

        _nextDirection = direction;
    }

    /// <summary>
    /// Moves the snake if the next direction is not null.
    /// </summary>
    private void Move()
    {
        if (_nextDirection is null)
        {
            return;
        }

        var head = Head;
        var nextHeadPosition = GetNextPosition(head.Position.ToVector2(), _nextDirection).ToPoint();

        // Add the new head and remove the tail.
        Parts.AddFirst(new SnakePart(nextHeadPosition, (Direction)_nextDirection!));
        Parts.RemoveLast();

        // Update the tail's direction (for the right texture).
        if (_interpolatedTailPosition is not null)
        {
            Tail.Direction = Parts.Last!.Previous!.ValueRef.Direction;
        }
    }

    /// <summary>
    /// Moves the interpolated snake head if the next direction is not null.
    /// </summary>
    private void MoveInterpolatedHead()
    {
        if (_nextDirection is null)
        {
            return;
        }

        var head = Head;
        var interpolationFactor = _timeSinceLastMove / _moveInterval;
        var nextHeadPosition = GetNextPosition(head.Position.ToVector2(), head.Direction);
        _interpolatedHeadPosition = Vector2.Lerp(head.Position.ToVector2(), nextHeadPosition, interpolationFactor);
    }

    /// <summary>
    /// Returns the next position of a snake position based on the provided direction.
    /// </summary>
    /// <remarks>
    /// If the snake part direction is null, it will return the same position.
    /// </remarks>
    /// <param name="position">The snake part.</param>
    /// <param name="direction">The direction.</param>
    /// <returns>The next position.</returns>
    private static Vector2 GetNextPosition(Vector2 position, Direction? direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2(position.X, position.Y - TextureSize.Height),
            Direction.Down => new Vector2(position.X, position.Y + TextureSize.Height),
            Direction.Left => new Vector2(position.X - TextureSize.Width, position.Y),
            Direction.Right => new Vector2(position.X + TextureSize.Width, position.Y),
            _ => position,
        };
    }

    /// <summary>
    /// Draws the interpolated head.
    /// </summary>
    private void DrawInterpolatedHead()
    {
        var headTexture = Head.Direction switch
        {
            Direction.Up => _snakeTextures[SnakeTexture.HeadUp],
            Direction.Down => _snakeTextures[SnakeTexture.HeadDown],
            Direction.Left => _snakeTextures[SnakeTexture.HeadLeft],
            Direction.Right => _snakeTextures[SnakeTexture.HeadRight],
            _ => _snakeTextures[SnakeTexture.HeadRight],
        };
        SnakeGame.SpriteBatch.Draw(headTexture, _interpolatedHeadPosition, null, Color.White, 0f, Vector2.Zero, _textureScale, SpriteEffects.None, 0f);
    }
}
