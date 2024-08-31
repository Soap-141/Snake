using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake;

/// <summary>
/// The snake component.
/// </summary>
public sealed class SnakeComponent : DrawableGameComponent
{
    /// <summary>
    /// The square texture size.
    /// </summary>
    public static readonly System.Drawing.Size TextureSize = new(8, 8);

    /// <summary>
    /// The positions of the snake's body parts.
    /// </summary>
    private readonly LinkedList<SnakePart> _positions = new();

    /// <summary>
    /// The time between each movement.
    /// </summary>
    private readonly float _moveInterval = 0.15f;

    /// <summary>
    /// The snake's initial position.
    /// </summary>
    private readonly Point _initialPosition;

    /// <summary>
    /// The green square texture.
    /// </summary>
    private Texture2D _greenSquareTexture;

    /// <summary>
    /// The time since the last move.
    /// </summary>
    private float _timeSinceLastMove;

    /// <summary>
    /// The interpolated head position.
    /// </summary>
    private Vector2 _interpolatedHeadPosition;

    /// <summary>
    /// The interpolated tail position.
    /// </summary>
    private Vector2 _interpolatedTailPosition;

    /// <summary>
    /// The snake's head.
    /// </summary>
    public ref SnakePart Head => ref _positions.First.ValueRef;

    /// <summary>
    /// The snake's tail.
    /// </summary>
    public ref SnakePart Tail => ref _positions.Last.ValueRef;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeComponent"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="initialPosition">The snake's initial position.</param>
    public SnakeComponent(Game game, Point initialPosition) : base(game)
    {
        _positions.AddFirst(new SnakePart(initialPosition, null));
        _interpolatedHeadPosition = initialPosition.ToVector2();
        _interpolatedTailPosition = _interpolatedHeadPosition;
        DrawOrder = 1;
    }

    protected override void OnEnabledChanged(object sender, EventArgs e)
    {
        if (!Enabled)
        {
            _positions.Clear();
            _positions.AddFirst(new SnakePart(_initialPosition, null));
            _interpolatedHeadPosition = _initialPosition.ToVector2();
            _interpolatedTailPosition = _interpolatedHeadPosition;
            _timeSinceLastMove = 0f;
            Enabled = true;
        }
    }

    /// <summary>
    /// Loads the snake's content.
    /// </summary>
    protected override void LoadContent()
    {
        _greenSquareTexture = Game.Content.Load<Texture2D>("greenSquare");
    }

    /// <summary>
    /// Unloads the snake's content.
    /// </summary>
    protected override void UnloadContent()
    {
        _greenSquareTexture.Dispose();
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
            MoveInterpolatedParts();
        }
    }

    /// <summary>
    /// Draws the snake.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    public override void Draw(GameTime gameTime)
    {
        foreach (var position in _positions.SkipLast(1))
        {
            SnakeGame.SpriteBatch.Draw(_greenSquareTexture, position.Position.ToVector2(), null, Color.White, 0f /* MathF.PI Rotation */, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }
        
        // Draw the interpolated head and tail positions.
        SnakeGame.SpriteBatch.Draw(_greenSquareTexture, _interpolatedHeadPosition, Color.White);
        SnakeGame.SpriteBatch.Draw(_greenSquareTexture, _interpolatedTailPosition, Color.White);
    }

    /// <summary>
    /// Returns whether the snake is colliding with itself.
    /// </summary>
    /// <returns>Whether the snake is colliding with itself.</returns>
    public bool IsCollidingWithItself()
    {
        // Skip the head and check if the head's position is equal to any other position.
        return _positions.Skip(1).Any(position => position.Position == Head.Position);
    }

    /// <summary>
    /// Grows the snake.
    /// </summary>
    public void Grow()
    {
        var tail = _positions.Last.ValueRef;
        var newTailPosition = tail.Direction switch
        {
            Direction.Up => new Point(tail.Position.X, tail.Position.Y + TextureSize.Height),
            Direction.Down => new Point(tail.Position.X, tail.Position.Y - TextureSize.Height),
            Direction.Left => new Point(tail.Position.X + TextureSize.Width, tail.Position.Y),
            Direction.Right => new Point(tail.Position.X - TextureSize.Width, tail.Position.Y),
            _ => Point.Zero,
        };
        _positions.AddLast(new SnakePart(newTailPosition, tail.Direction));
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
    /// Sets the snake's direction.
    /// </summary>
    /// <remarks>
    /// The snake can't move in the opposite direction it's currently moving.
    /// </remarks>
    /// <param name="direction">The snake's direction.</param>
    private void SetDirection(Direction direction)
    {
        if (Head.Direction == Direction.Up && direction == Direction.Down ||
            Head.Direction == Direction.Down && direction == Direction.Up ||
            Head.Direction == Direction.Left && direction == Direction.Right ||
            Head.Direction == Direction.Right && direction == Direction.Left)
        {
            return;
        }

        Head.Direction = direction;
    }

    /// <summary>
    /// Moves the snake.
    /// </summary>
    private void Move()
    {
        var head = Head;
        var newHeadPosition = head.Direction switch
        {
            Direction.Up => new Point(head.Position.X, head.Position.Y - TextureSize.Height),
            Direction.Down => new Point(head.Position.X, head.Position.Y + TextureSize.Height),
            Direction.Left => new Point(head.Position.X - TextureSize.Width, head.Position.Y),
            Direction.Right => new Point(head.Position.X + TextureSize.Width, head.Position.Y),
            _ => Head.Position
        };

        // Add the new head and remove the tail.
        _positions.AddFirst(new SnakePart(newHeadPosition, head.Direction));
        _positions.RemoveLast();
    }

    /// <summary>
    /// Moves the interpolated snake parts (head and tail).
    /// </summary>
    private void MoveInterpolatedParts()
    {
        var head = Head;
        var tail = Tail;
        var newHeadPosition = GetNewInterpolatedPosition(head);
        var newTailPosition = GetNewInterpolatedPosition(tail);
        var interpolationFactor = _timeSinceLastMove / _moveInterval;

        _interpolatedHeadPosition = Vector2.Lerp(head.Position.ToVector2(), newHeadPosition, interpolationFactor);
        _interpolatedTailPosition = Vector2.Lerp(tail.Position.ToVector2(), newTailPosition, interpolationFactor);
    }

    /// <summary>
    /// Returns the new interpolated position of a snake part.
    /// </summary>
    /// <param name="snakePart">The snake part.</param>
    /// <returns>The new interpolated position.</returns>
    private static Vector2 GetNewInterpolatedPosition(SnakePart snakePart)
    {
        var position = snakePart.Position.ToVector2();
        return snakePart.Direction switch
        {
            Direction.Up => new Vector2(position.X, position.Y - TextureSize.Height),
            Direction.Down => new Vector2(position.X, position.Y + TextureSize.Height),
            Direction.Left => new Vector2(position.X - TextureSize.Width, position.Y),
            Direction.Right => new Vector2(position.X + TextureSize.Width, position.Y),
            _ => Vector2.Zero
        };
    }
}
