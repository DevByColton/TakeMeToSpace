using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Services;
using TakeMeToSpace.HomeArea.Components;

namespace TakeMeToSpace.HomeArea;

public class HomeAreaManager
{
    public HomeTileSet HomeTileSet;

    public HomeAreaManager(ContentManager content)
    {
        HomeTileSet = new HomeTileSet(content);
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        HomeTileSet.Draw(spriteBatch, primitiveDrawingService, font);
    }
}