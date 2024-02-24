using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Components;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.Player;

public class PlayerManager
{
    public PlayerEntity PlayerEntity;

    public PlayerManager(ContentManager content)
    {
        Texture2D entityTexture = content.Load<Texture2D>("Entity");
        PlayerEntity = new PlayerEntity(entityTexture, new Vector2(1440, 650));
    }

    public void Update(GameTime gameTime, Tile[,] tiles)
    {
        PlayerEntity.Update(gameTime, tiles);
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        PlayerEntity.Draw(spriteBatch, primitiveDrawingService, font);
    }
}