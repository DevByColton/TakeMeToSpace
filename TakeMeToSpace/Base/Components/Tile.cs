using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.Base.Components;

public class Tile
{
    public SpriteComponent SpriteComponent;
    public PositionComponent PositionComponent;
    public Rectangle TileRectangle;
    public bool HasCollider = false;
    public bool IsColliderGrouped = false;
    public ColliderGroupDirection ColliderGroupDirection = ColliderGroupDirection.None;
    public int Row;
    public int Column;
    

    public void SetTileRectangle()
    {
        TileRectangle = new Rectangle(
            (int) PositionComponent.Offset().X,
            (int) PositionComponent.Offset().Y,
            SpriteComponent.Width(),
            SpriteComponent.Height()
       );
    }

    public void Update(Vector2 mousePosition)
    {
        SpriteComponent.Color = TileRectangle.Contains(mousePosition) ? Color.Red : Color.White;
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        spriteBatch.Draw(
            texture: SpriteComponent.Texture,
            position: PositionComponent.Position,
            sourceRectangle: null,
            color: SpriteComponent.Color,
            rotation: PositionComponent.Rotation,
            origin: PositionComponent.Origin,
            scale: Vector2.One,
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
    }
}