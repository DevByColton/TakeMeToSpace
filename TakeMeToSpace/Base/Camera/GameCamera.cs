using Microsoft.Xna.Framework;

namespace TakeMeToSpace.Base.Camera;

public class GameCamera
{
    private float _viewportWidth;
    private float _viewportHeight;
    private float _lerpSpeed = 10f;
    private Vector2 _viewportOrigin;
    public bool ClampCamera = true;
    public Vector2 MinScroll;
    public Vector2 MaxScroll = Vector2.Zero;
    public Matrix TransformMatrix;

    public GameCamera(float width, float height, Vector2 startingPosition)
    {
        _viewportWidth = width;
        _viewportHeight = height;
        UpdateViewportOrigin();
        
        // Set the first TransformMatrix
        Vector2 offset = _viewportOrigin - startingPosition;
        TransformMatrix = Matrix.CreateTranslation(offset.X, offset.Y, 0);
    }

    private void UpdateViewportOrigin()
    {
        _viewportOrigin = new Vector2(_viewportWidth / 2f, _viewportHeight / 2f);
    }

    public void SetMinMaxScroll(float x, float y, Vector2 offset)
    {
        MaxScroll = -offset;
        MinScroll = new Vector2(-x + _viewportWidth, -y + _viewportHeight) - offset;
    }

    public void Update(Vector2 playerPosition, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Get the next position
        Vector2 offset = _viewportOrigin - playerPosition;
        
        // Clamp to the world bounds if set
        if (ClampCamera) offset = Vector2.Clamp(offset, MinScroll, MaxScroll);

        // Lerp the next transformation
        Matrix previousTransformMatrix = TransformMatrix;
        Matrix nextTransformMatrix = Matrix.CreateTranslation(offset.X, offset.Y, 0);
        TransformMatrix = Matrix.Lerp(previousTransformMatrix, nextTransformMatrix, _lerpSpeed * deltaTime);
    }
}