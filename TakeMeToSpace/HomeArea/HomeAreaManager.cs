using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Components;
using TakeMeToSpace.Base.Reader;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.HomeArea;

public class HomeAreaManager
{
    private TileMapper _tileMapper = new();

    public HomeAreaManager(ContentManager content)
    {
        // Read and map the home tiles from json data
        string path = Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, 
            @"Content\TileMapData\HomeTileMap.json"
        );

        _tileMapper.MapFromJson(content, path);
        _tileMapper.CreateColliders();
    }
    
    public List<ColliderComponent> Colliders()
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