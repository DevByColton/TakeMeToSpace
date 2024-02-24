using Microsoft.Xna.Framework;

namespace TakeMeToSpace.Base.Components;

public class VertexComponent
{
    public static readonly Color[] Colors = { Color.Green, Color.Pink, Color.White, Color.Orange, Color.Aqua, Color.Coral, Color.Purple };
    
    public Vector2 RotatedTranslatedVertex;
    public Vector2 TranslatedVertex;
    public Color EdgeColor;
    public bool IsCollidingColorOverride;
    
    public Color GetEdgeColor() => IsCollidingColorOverride ? Color.Red : EdgeColor;
}