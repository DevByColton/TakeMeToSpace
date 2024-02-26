using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Engine.Services;

namespace TakeMeToSpace.Engine.Components;

public enum ColliderType
{
    Vertical,
    Horizontal,
    Box,
    None
}

public class Collider
{
    public PositionComponent PositionComponent;
    public BoundingPolygon BoundingPolygon;
    public ColliderType ColliderType;
    public List<Tile> Tiles;

    public Collider(Tile tile)
    {
        Tiles = new List<Tile> { tile };
        ColliderType = tile.ColliderType;
    }

    public void CreateBoundingPolygon()
    {
        switch (ColliderType)
        {
            case ColliderType.Horizontal:
                SetHorizontalBoundingPolygon();
                break;
            case ColliderType.Vertical:
                SetVerticalBoundingPolygon();
                break;
            case ColliderType.Box:
                SetBoxBoundingPolygon();
                break;
            default:
                throw new Exception("Collider should have collider type");
        }

        Vector2[] vertices = 
        {
            new(PositionComponent.Position.X - PositionComponent.Origin.X, PositionComponent.Position.Y - PositionComponent.Origin.Y),
            new(PositionComponent.Position.X + PositionComponent.Origin.X, PositionComponent.Position.Y - PositionComponent.Origin.Y),
            new(PositionComponent.Position.X + PositionComponent.Origin.X, PositionComponent.Position.Y + PositionComponent.Origin.Y),
            new(PositionComponent.Position.X - PositionComponent.Origin.X, PositionComponent.Position.Y + PositionComponent.Origin.Y)
        };
        BoundingPolygon = new BoundingPolygon(vertices);
    }

    /// <summary>
    /// Sets a horizontal bounding polygon by getting the midway point between the
    /// minimum and maximum x values: minX + ((maxX - minX) / 2)
    /// The y value can be any y because the tiles are all on the same row horizontally
    /// </summary>
    private void SetHorizontalBoundingPolygon()
    {
        float minX = Tiles.Min(t => t.PositionComponent.Position.X);
        float maxX = Tiles.Max(t => t.PositionComponent.Position.X);
        
        PositionComponent = new PositionComponent
        {
            Position = new Vector2(minX + (maxX - minX) / 2, Tiles[0].PositionComponent.Position.Y),
            Origin = new Vector2(
                Tiles[0].Sprite.Width() * Tiles.Count / 2f, 
                Tiles[0].Sprite.Height() / 2f
            ),
            Rotation = 0f
        };
    }

    /// <summary>
    /// Sets a horizontal bounding polygon by getting the midway point between the
    /// minimum and maximum y values: minY + (maxY - minY) / 2
    /// The x value can be any x because the tiles are all on the same row vertically
    /// </summary>
    private void SetVerticalBoundingPolygon()
    {
        float minY = Tiles.Min(t => t.PositionComponent.Position.Y);
        float maxY = Tiles.Max(t => t.PositionComponent.Position.Y);
        
        PositionComponent = new PositionComponent
        {
            Position = new Vector2(Tiles[0].PositionComponent.Position.X, minY + (maxY - minY) / 2),
            Origin = new Vector2(
                Tiles[0].Sprite.Width() / 2f, 
                Tiles[0].Sprite.Height() * Tiles.Count / 2f
            ),
            Rotation = 0f
        };
    }

    /// <summary>
    /// Sets a box bounding polygon by getting the midway point between the
    /// minimum and maximum x values: minX + ((maxX - minX) / 2)
    /// and getting the midway point between the
    /// minimum and maximum y values: minY + (maxY - minY) / 2
    ///
    /// Also calculates the number of tiles vertically and horizontally to set the origin
    /// </summary>
    private void SetBoxBoundingPolygon()
    {
        // x position = minX + ((maxX - minX) / 2)
        // y position = minY + ((maxY - minY) / 2)
        float minX = Tiles.Min(t => t.PositionComponent.Position.X);
        float maxX = Tiles.Max(t => t.PositionComponent.Position.X);
        
        float minY = Tiles.Min(t => t.PositionComponent.Position.Y);
        float maxY = Tiles.Max(t => t.PositionComponent.Position.Y);

        float columnCount = Tiles.Max(t => t.Column) - Tiles.Min(t => t.Column) + 1;
        float rowsCount = Tiles.Max(t => t.Row) - Tiles.Min(t => t.Row) + 1;
        
        PositionComponent = new PositionComponent
        {
            Position = new Vector2(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2),
            Origin = new Vector2(
                Tiles[0].Sprite.Width() * columnCount / 2f, 
                Tiles[0].Sprite.Height() * rowsCount / 2f
            ),
            Rotation = 0f
        };
    }
    
    public void Draw(PrimitiveDrawingService primitiveDrawingService)
    {
        BoundingPolygon?.Draw(primitiveDrawingService);
    }
}