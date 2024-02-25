using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TakeMeToSpace.Base.Components;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.Player;

public class PlayerEntity
{
    private SpriteComponent _spriteComponent;
    private PlayerCollisionService _playerCollisionService = new();
    private float _linearVelocity = 300f;
    
    public BoundingPolygonComponent BoundingPolygonComponent;
    public PositionComponent PositionComponent;
    
    public PlayerEntity(Texture2D squareTexture, Vector2 position)
    {
        _spriteComponent = new SpriteComponent(squareTexture);
        PositionComponent = new PositionComponent
        {
            Position = position,
            Origin = new Vector2(squareTexture.Width / 2f, squareTexture.Height / 2f),
            Rotation = 0f
        };

        BoundingPolygonComponent = new BoundingPolygonComponent(new[]
        {
            new Vector2(PositionComponent.Offset().X, PositionComponent.Offset().Y),
            new Vector2(PositionComponent.Offset().X + _spriteComponent.Texture.Width, PositionComponent.Offset().Y),
            new Vector2(PositionComponent.Offset().X + _spriteComponent.Texture.Width, PositionComponent.Offset().Y + _spriteComponent.Texture.Height),
            new Vector2(PositionComponent.Offset().X, PositionComponent.Offset().Y + _spriteComponent.Texture.Height)
        });
    }

    public void Update(GameTime gameTime, List<ColliderComponent> colliders)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState currentKeyBoardState = Keyboard.GetState();
        Vector2 direction = Vector2.Zero;
        
        if (currentKeyBoardState.IsKeyDown(Keys.W)) direction.Y = -1;
        if (currentKeyBoardState.IsKeyDown(Keys.A)) direction.X = -1;
        if (currentKeyBoardState.IsKeyDown(Keys.S)) direction.Y = 1;
        if (currentKeyBoardState.IsKeyDown(Keys.D)) direction.X = 1;
        if (direction.X != 0 || direction.Y != 0) direction.Normalize();

        if (currentKeyBoardState.IsKeyDown(Keys.Up)) PositionComponent.Rotation += MathHelper.Pi / 90;
        if (currentKeyBoardState.IsKeyDown(Keys.Down)) PositionComponent.Rotation -= MathHelper.Pi / 90;
        if (currentKeyBoardState.IsKeyDown(Keys.Right)) PositionComponent.Rotation = 0;

        direction *= _linearVelocity * deltaTime;
        Vector2 allowedMovement = _playerCollisionService.GetAllowedMovement(direction, this, colliders);
        PositionComponent.Position += allowedMovement;
        
        BoundingPolygonComponent.Transform(
            PositionComponent.Position, 
            allowedMovement,
            PositionComponent.Rotation
        );
    }
    
    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        spriteBatch.Draw(
            texture: _spriteComponent.Texture,
            position: PositionComponent.Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: PositionComponent.Rotation,
            origin: PositionComponent.Origin,
            scale: Vector2.One,
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
        BoundingPolygonComponent.Draw(primitiveDrawingService);

        // spriteBatch.DrawString(font, $"ID: {BoundingPolygonComponent.Id} \n", WorldPositionOriginOffset(), Color.White);
        //spriteBatch.DrawString(font, $"Position: {PositionComponent.Position}", Vector2.Zero, Color.White);
        //
        // string collidingWithIdsText = BoundingPolygonComponent.CollidingWithIds.Aggregate("", (current, id) => current + (id + "\n"));
        // spriteBatch.DrawString(font, $"collidingWithIdsText: \n {collidingWithIdsText}", new Vector2(WorldPositionOriginOffset().X, WorldPositionOriginOffset().Y + 60), Color.White);
        //
        // spriteBatch.DrawString(font, $"IsColliding: {BoundingPolygonComponent.IsColliding}", new Vector2(WorldPositionOriginOffset().X, WorldPositionOriginOffset().Y + 260), Color.White);
    }
}