﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Engine.Components;
using TakeMeToSpace.Engine.Services;

namespace TakeMeToSpace.Player;

public class PlayerManager
{
    public PlayerEntity PlayerEntity;

    public PlayerManager(ContentManager content, Vector2 startingPosition)
    {
        Texture2D entityTexture = content.Load<Texture2D>("Entity");
        PlayerEntity = new PlayerEntity(entityTexture, startingPosition);
    }

    public void Update(GameTime gameTime, List<Collider> colliders)
    {
        PlayerEntity.Update(gameTime, colliders);
    }

    public void Draw(SpriteBatch spriteBatch, PrimitiveDrawingService primitiveDrawingService, SpriteFont font)
    {
        PlayerEntity.Draw(spriteBatch, primitiveDrawingService, font);
    }
}