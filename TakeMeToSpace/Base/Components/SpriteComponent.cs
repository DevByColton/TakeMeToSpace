using Microsoft.Xna.Framework.Graphics;

namespace TakeMeToSpace.Base.Components;

public class SpriteComponent
{
    public Texture2D Texture;

    public SpriteComponent(Texture2D texture)
    {
        Texture = texture;
    }

    public int Width()
    {
        return Texture.Width;
    }

    public int Height()
    {
        return Texture.Height;
    }
}