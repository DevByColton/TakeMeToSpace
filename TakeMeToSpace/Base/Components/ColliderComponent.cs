using System.Collections.Generic;

namespace TakeMeToSpace.Base.Components;

public enum ColliderGroupDirection
{
    Vertical,
    Horizontal,
    Box,
    None
}

public class ColliderComponent
{
    // I want a list? of tiles to know what tiles this collider will make up
    // Sort the tiles by position
    // I need a direction property to know which axis to check

    public PositionComponent PositionComponent;
    public BoundingPolygonComponent BoundingPolygonComponent;
    public ColliderGroupDirection ColliderGroupDirection;
    public List<Tile> Tiles;

    public ColliderComponent(Tile tile)
    {
        Tiles = new List<Tile> { tile };
        ColliderGroupDirection = tile.ColliderGroupDirection;
    }
}