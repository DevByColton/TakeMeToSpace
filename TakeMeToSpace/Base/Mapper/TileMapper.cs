using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Components;

namespace TakeMeToSpace.Base.Mapper;

public class TileMapper
{
    public Tile[,] Tiles;
    public List<ColliderComponent> Colliders = new();
    
    private List<TileRow> ReadJsonData(string path)
    {
        string fileContents = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<TileRow>>(fileContents);
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
            if (!nextTile.IsColliderGrouped && nextTile.ColliderType == ColliderType.Horizontal)
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
            if (!nextTile.IsColliderGrouped && nextTile.ColliderType == ColliderType.Vertical)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapVerticalTileGroup(tileGroup, row + 1, col);
            }
        }
    }

    /// <summary>
    /// Map the first row of tiles, when the tiles are in a group I want to start at the
    /// very first tile row and map those tiles. On first call the first tile will
    /// already be marked as grouped, so this will group the remaining tiles in the first row
    /// </summary>
    /// <param name="tileGroup"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void MapFirstBoxTileRow(List<Tile> tileGroup, int row, int col)
    {
        // Make sure the next col value is still in bounds of the array
        if (col < Tiles.GetLength(1))
        {
            Tile nextTile = Tiles[row, col];
            if (!nextTile.IsColliderGrouped && nextTile.ColliderType == ColliderType.Box)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapFirstBoxTileRow(tileGroup, row, col + 1);
            }
        }
    }

    /// <summary>
    /// Map the remaining rows after the first row has already been marked as group.
    /// The first pass of the do loop gets the first row, then iterates through each
    /// tile horizontally and vertically
    /// </summary>
    /// <param name="tileGroup"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void MapRemainingBoxTileRows(List<Tile> tileGroup, int row, int col)
    {
        do
        {
            MapBoxTileRow(tileGroup, row, col);
            ++col;
        } while (col < Tiles.GetLength(1) && 
                 !Tiles[row, col].IsColliderGrouped && 
                 Tiles[row, col].ColliderType == ColliderType.Box);
    }

    /// <summary>
    /// Recursive loop to get each row of tiles to be grouped in a box collider
    /// </summary>
    /// <param name="tileGroup"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void MapBoxTileRow(List<Tile> tileGroup, int row, int col)
    {
        // Make sure the next row value is still in bounds of the array
        if (row < Tiles.GetLength(0))
        {
            Tile nextTile = Tiles[row, col];
            if (!nextTile.IsColliderGrouped && nextTile.ColliderType == ColliderType.Box)
            {
                nextTile.IsColliderGrouped = true;
                tileGroup.Add(nextTile);
                MapBoxTileRow(tileGroup, row + 1, col);
            }
        }
    }

    public float MapTotalWidth()
    {
        return Tiles[0,0].SpriteComponent.Width() * Tiles.GetLength(1);
    }

    public float MapTotalHeight()
    {
        return Tiles[0,0].SpriteComponent.Height() * Tiles.GetLength(0);
    }
    
    public void MapFromJson(ContentManager content, string path)
    {
        List<TileRow> tileRows = ReadJsonData(path);
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
                    ColliderType = tileColumn.ColliderType,
                    Row = row,
                    Column = col
                };
                
                Tiles[row, col].SetTileRectangle();
            }
        }
    }
    
    /// <summary>
    /// Go through the tiles and group tiles by data into collider groups
    /// </summary>
    /// <returns></returns>
    public void CreateColliders()
    {
        for (int row = 0; row < Tiles.GetLength(0); row++)
        {
            for (int col = 0; col < Tiles.GetLength(1); col++)
            {
                Tile startTile = Tiles[row, col];

                if (startTile.HasCollider && startTile.ColliderType != ColliderType.None && !startTile.IsColliderGrouped)
                {
                    // Create the new collider with the first tile as the starting point, marking the startTile as grouped
                    startTile.IsColliderGrouped = true;
                    ColliderComponent collider = new ColliderComponent(startTile);
                    
                    // Recursively map the rest of the tiles in the group by direction
                    switch (startTile.ColliderType)
                    {
                        case ColliderType.Horizontal:
                            // Get the horizontal tiles in the group, start at the next col
                            List<Tile> horizontalTileGroup = new List<Tile>();
                            MapHorizontalTileGroup(horizontalTileGroup, row, col + 1);
                            collider.Tiles.AddRange(horizontalTileGroup);
                            break;
                        case ColliderType.Vertical:
                            // Get the vertical tiles in the group, start at the next row
                            List<Tile> verticalTileGroup = new List<Tile>();
                            MapVerticalTileGroup(verticalTileGroup, row + 1, col);
                            collider.Tiles.AddRange(verticalTileGroup);
                            break;
                        case ColliderType.Box:
                            // Get the box tiles in the group, start at the next col first
                            List<Tile> boxTileGroup = new List<Tile>();
                            MapFirstBoxTileRow(boxTileGroup, row, col + 1);
                            MapRemainingBoxTileRows(boxTileGroup, row + 1, col);
                            collider.Tiles.AddRange(boxTileGroup);
                            break;
                        default: continue;
                    }
                    
                    Colliders.Add(collider);
                }
            }
        }
        
        Colliders.ForEach(collider => collider.CreateBoundingPolygon());
    }
}