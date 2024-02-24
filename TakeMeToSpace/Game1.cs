﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TakeMeToSpace.Base.Camera;
using TakeMeToSpace.Base.Services;
using TakeMeToSpace.HomeArea;
using TakeMeToSpace.Player;

namespace TakeMeToSpace
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        
        private PrimitiveDrawingService _primitiveDrawingService;
        private GameCamera _gameCamera;
        private HomeAreaManager _homeAreaManager;
        private PlayerManager _playerManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphics.ApplyChanges();

            _primitiveDrawingService = new PrimitiveDrawingService(_graphics);
            _homeAreaManager = new HomeAreaManager(Content);
            _playerManager = new PlayerManager(Content);
            _gameCamera = new GameCamera(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, _playerManager.PlayerEntity.PositionComponent.Position);
            _gameCamera.SetMinScroll(_homeAreaManager.HomeTileSet.MapTotalWidth, _homeAreaManager.HomeTileSet.MapTotalHeight);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Fonts/Lambda");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            _playerManager.Update(gameTime, _homeAreaManager.HomeTileSet.Tiles);
            _gameCamera.Update(_playerManager.PlayerEntity.PositionComponent.Position, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin(transformMatrix: _gameCamera.TransformMatrix);
        
            _homeAreaManager.Draw(_spriteBatch, _primitiveDrawingService, _font);
            _playerManager.Draw(_spriteBatch, _primitiveDrawingService, _font);
            
            _spriteBatch.End();
        
            _primitiveDrawingService.Draw(_gameCamera.TransformMatrix);

            base.Draw(gameTime);
        }
    }
}
