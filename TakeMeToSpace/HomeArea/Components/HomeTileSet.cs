using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Components;
using TakeMeToSpace.Base.Reader;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.HomeArea.Components;

public class HomeTileSet
{
    private TileMapReader _tileMapReader = new();

    public Tile[,] Tiles;
    public float TileWidth;
    public float TileHeight;
    public float MapTotalWidth;
    public float MapTotalHeight;
    public List<ColliderComponent> Colliders = new();

    public HomeTileSet(ContentManager content)
    {
        MapTilesFromJson(content);
        SetCollidersFromTiles();

        // Set the map dimension properties
        TileWidth = Tiles[0,0].SpriteComponent.Texture.Width;
        TileHeight = Tiles[0,0].SpriteComponent.Texture.Height;
        MapTotalWidth = TileWidth * Tiles.GetLength(1);
        MapTotalHeight = TileHeight * Tiles.GetLength(0);
    }

    private void MapTilesFromJson(ContentManager content)
    {
        // Read the home tile map data
        string path = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, 
            @"Content\TileMapData\HomeTileMap.json"
        );
        
        List<TileRow> tileRows = _tileMapReader.ReadTileRows(path);
        Tiles = new Tile[tileRows.Count, tileRows[0].TileColumns.Count];
        
        // Set the tiles by the read data
        for (int row = 0; row < tileRows.Count; row++)
        {
            for (int col = 0; col < tileRows[row].TileColumns.Count; col++)
            {
                TileColumn tileColumn = tileRows[row].TileColumns[col];
                Texture2D texture = content.Load<Texture2D>($"Home/Tiles/{tileColumn.TextureName}");
                
                // Create the new tile
                Tiles[row, col] = new Tile
                {
                    SpriteComponent = new SpriteComponent(texture),
                    PositionComponent = new PositionComponent
                    {
                        Position = new Vector2(texture.Width * col, texture.Height * row),
                        Origin = new Vector2(texture.Width / 2f, texture.Height / 2f),
                        Rotation = 0f
                    },
                    HasCollider = tileColumn.HasCollider,
                    ColliderGroupDirection = tileColumn.ColliderGroupDirection,
                    Row = row,
                    Column = col
                };
                
                //TODO: can probably remove this once the colliders are set with their bounding polys, Set the bounding polygon component
                Tiles[row, col].SetBoundingPolygonComponent();
            }
        }
    }

    /// <summary>
    /// Go through the mapped tiles and group tiles that are world colliders
    /// This helps with not having so many bounding polygons for collision detection
    /// </summary>
    private void SetCollidersFromTiles()
    {
        Colliders = new List<ColliderComponent>();
        
        for (int row = 0; row < Tiles.GetLength(0); row++)
        {
            for (int col = 0; col < Tiles.GetLength(1); col++)
            {
                Tile sourceTile = Tiles[row, col];

                if (sourceTile.HasCollider && sourceTile.ColliderGroupDirection != ColliderGroupDirection.None && !sourceTile.IsColliderGrouped)
                {
                    // Create the new collider with the first tile as the starting point, marking the first tile as grouped
                    ColliderComponent collider = new ColliderComponent(sourceTile);
                    sourceTile.IsColliderGrouped = true;
                    
                    // Recursively map the rest of the tiles in the group by direction
                    switch (sourceTile.ColliderGroupDirection)
                    {
                        case ColliderGroupDirection.Horizontal:
                            // Get the horizontal tiles in the group, start at the next col
                            List<Tile> horizontalTileGroup = new List<Tile>();
                            MapHorizontalTileGroup(horizontalTileGroup, row, col + 1);
                            collider.Tiles.AddRange(horizontalTileGroup);
                            break;
                        case ColliderGroupDirection.Vertical:
                            // Get the vertical tiles in the group, start at the next row
                            List<Tile> verticalTileGroup = new List<Tile>();
                            MapVerticalTileGroup(verticalTileGroup, row + 1, col);
                            collider.Tiles.AddRange(verticalTileGroup);
                            break;
                        case ColliderGroupDirection.Box:
                            // Get the box tiles in the group, start at the next col first
                            List<Tile> boxTileGroup = new List<Tile>();
                            MapBoxTileGroup(boxTileGroup, row, col);
                            collider.Tiles.AddRange(boxTileGroup);
                            break;
                        case ColliderGroupDirection.None:
                        default: continue;
                    }
                    
                    Colliders.Add(collider);
                }
            }
        }
    }

    /// <summary>
    /// Recursive function to map all of the tiles in a horizontal group
    /// Each recursive iteration will increment the col value until the next tile
    /// is not a part of the group. If the tile is apart of the group, add it to the
    /// group and mark it as grouped
    /// </summary>
    /// <param name="tileGroup"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void MapHorizontalTileGroup(List<Tile> tileGroup, int row, int col)
    {
        // Make sure the next col value is still in bounds of the array
        if (col < Tiles.GetLength(1))
        {
            Tile nextTile = Tiles[row, col];
            if (!nextTile.IsColliderGrouped && nextTile.ColliderGroupDirection == ColliderGroupDirection.Horizontal)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapHorizontalTileGroup(tileGroup, row, col + 1);
            }
        }
    }

    /// <summary>
    /// Recursive function to map all of the tiles in a vertical group
    /// Each recursive iteration will increment the row value until the next tile
    /// is not a part of the group. If the tile is apart of the group, add it to the
    /// group and mark it as grouped
    /// </summary>
    /// <param name="tileGroup"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void MapVerticalTileGroup(List<Tile> tileGroup, int row, int col)
    {
        // Make sure the next row value is still in bounds of the array
        if (row < Tiles.GetLength(0))
        {
            Tile nextTile = Tiles[row, col];
            if (!nextTile.IsColliderGrouped && nextTile.ColliderGroupDirection == ColliderGroupDirection.Vertical)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapVerticalTileGroup(tileGroup, row + 1, col);
            }
        }
    }

    private void MapBoxTileGroup(List<Tile> tileGroup, int row, int col)
    {
        // Todo: this will start at the second row on each, need to figure how to start at the 
        // todo: first row each time (besides the very first pass) instead
        do
        {
            MapBoxTileRowGroup(tileGroup, row + 1, col);
            ++col;
        } while (col < Tiles.GetLength(1) && 
                 !Tiles[row, col].IsColliderGrouped && 
                 Tiles[row, col].ColliderGroupDirection == ColliderGroupDirection.Box);
    }

    private void MapBoxTileRowGroup(List<Tile> tileGroup, int row, int col)
    {
        // Make sure the next row value is still in bounds of the array
        if (row < Tiles.GetLength(0))
        {
            Tile nextTile = Tiles[row, col];
            if (!nextTile.IsColliderGrouped && nextTile.ColliderGroupDirection == ColliderGroupDirection.Box)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapBoxTileRowGroup(tileGroup, row + 1, col);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        foreach (Tile tile in Tiles)
            tile.Draw(spriteBatch, primitiveDrawingService, font);
    }
}