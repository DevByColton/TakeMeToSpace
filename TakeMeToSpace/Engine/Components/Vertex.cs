using Microsoft.Xna.Framework;

namespace TakeMeToSpace.Engine.Components;

public class Vertex
{
    public static readonly Color[] Colors = { Color.Green, Color.Pink, Color.White, Color.Orange, Color.Aqua, Color.Coral, Color.Purple };
    
    public Vector2 RotatedTranslated;
    public Vector2 Translated;
    public Color EdgeColor;
    public bool IsCollidingColorOverride;
    
    public Color GetEdgeColor() => IsCollidingColorOverride ? Color.Red : EdgeColor;
}