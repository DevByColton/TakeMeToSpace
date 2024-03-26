using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Engine.Components;
using TakeMeToSpace.Engine.Services;

namespace TakeMeToSpace.HomeArea;

public class HomeAreaManager
{
    private TileMapper _tileMapper = new();

    public HomeAreaManager(ContentManager content)
    {
        _tileMapper.MapFromJson(content, @"Data\TileMapData\HomeTileMap.json");
        _tileMapper.CreateColliders();
    }

    public Tile[,] Tiles()
    {
        return _tileMapper.Tiles;
    }
    
    public List<Collider> Colliders()
    {
        return _tileMapper.Colliders;
    }

    public float TotalWidth()
    {
        return _tileMapper.MapTotalWidth();
    }
    
    public float TotalHeight()
    {
        return _tileMapper.MapTotalHeight();
    }

    public void Update(Vector2 mousePosition)
    {
        foreach (Tile tile in _tileMapper.Tiles)
            tile.Update(mousePosition);
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        _tileMapper.Colliders.ForEach(c => c.Draw(primitiveDrawingService));
        
        foreach (Tile tile in _tileMapper.Tiles)
            tile.Draw(spriteBatch, primitiveDrawingService, font);
    }
}