using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.Base.Components;

public class Tile
{
    public SpriteComponent SpriteComponent;
    public PositionComponent PositionComponent;
    public BoundingPolygonComponent BoundingPolygonComponent;
    public bool HasCollider = false;
    public bool IsColliderGrouped = false;
    public ColliderGroupDirection ColliderGroupDirection = ColliderGroupDirection.None;
    public int Row;
    public int Column;

    public void SetBoundingPolygonComponent()
    {
        if (HasCollider)
        {
            Vector2[] vertices = 
            {
                new(PositionComponent.Position.X - PositionComponent.Origin.X, PositionComponent.Position.Y - PositionComponent.Origin.Y),
                new(PositionComponent.Position.X + PositionComponent.Origin.X, PositionComponent.Position.Y - PositionComponent.Origin.Y),
                new(PositionComponent.Position.X + PositionComponent.Origin.X, PositionComponent.Position.Y + PositionComponent.Origin.Y),
                new(PositionComponent.Position.X - PositionComponent.Origin.X, PositionComponent.Position.Y + PositionComponent.Origin.Y)
            };
            BoundingPolygonComponent = new BoundingPolygonComponent(vertices);
        }
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        spriteBatch.Draw(
            texture: SpriteComponent.Texture,
            position: PositionComponent.Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: PositionComponent.Rotation,
            origin: PositionComponent.Origin,
            scale: Vector2.One,
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
        
        if (HasCollider)
            BoundingPolygonComponent?.Draw(primitiveDrawingService);
    }
}