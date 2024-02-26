using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Engine.Services;

namespace TakeMeToSpace.Engine.Components;

public class Tile
{
    public Sprite Sprite;
    public PositionComponent PositionComponent;
    public Rectangle TileRectangle;
    public bool HasCollider = false;
    public bool IsColliderGrouped = false;
    public ColliderType ColliderType = ColliderType.None;
    public int Row;
    public int Column;
    

    public void SetTileRectangle()
    {
        TileRectangle = new Rectangle(
            (int) PositionComponent.Offset().X,
            (int) PositionComponent.Offset().Y,
            Sprite.Width(),
            Sprite.Height()
       );
    }

    public void Update(Vector2 mousePosition)
    {
        Sprite.Color = TileRectangle.Contains(mousePosition) ? Color.Red : Color.White;
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        spriteBatch.Draw(
            texture: Sprite.Texture,
            position: PositionComponent.Position,
            sourceRectangle: null,
            color: Sprite.Color,
            rotation: PositionComponent.Rotation,
            origin: PositionComponent.Origin,
            scale: Vector2.One,
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
    }
}