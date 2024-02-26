using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TakeMeToSpace.Engine.Components;

public class Sprite
{
    public Texture2D Texture;
    public Color Color = Color.White;

    public Sprite(Texture2D texture)
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