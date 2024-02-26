using Microsoft.Xna.Framework;

namespace TakeMeToSpace.Engine.Components;

public class PositionComponent
{
    public Vector2 Position;
    public Vector2 Origin;
    public float Rotation;
    
    public Vector2 Offset()
    {
        return Position - Origin;
    }
}