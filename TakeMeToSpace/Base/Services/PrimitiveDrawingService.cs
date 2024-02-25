using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TakeMeToSpace.Base.Components;

namespace TakeMeToSpace.Base.Services;

public class PrimitiveDrawingService
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private BasicEffect _basicEffect;
    
    private List<VertexPositionColor> _vertices = new();
    private List<int> _indices = new();
    
    private int _indexCount;
    private int _vertexCount;
    private const float LineThickness = 1.5f;
    private const float CirclePoints = 200f;

    public PrimitiveDrawingService(GraphicsDeviceManager graphicsDeviceManager)
    {
        _graphicsDeviceManager = graphicsDeviceManager;
        _basicEffect = new BasicEffect(_graphicsDeviceManager.GraphicsDevice);
        _basicEffect.VertexColorEnabled = true;
    }

    private void DrawLine(Vector2 startPoint, Vector2 endPoint, Color color)
    {
        // A line is just a filled rectangle around a normalized direction vector
        Vector2 direction = endPoint - startPoint;
        direction.Normalize();

        // Set the width of the line
        direction *= LineThickness;

        // Get the normal vector, the normal is perpendicular of the direction
        Vector2 normal = new(direction.Y, -direction.X);

        // Set each of the corners
        Vector2 topLeft = startPoint + normal - direction;
        Vector2 topRight = endPoint + normal + direction;
        Vector2 bottomRight = endPoint - normal + direction;
        Vector2 bottomLeft = startPoint - normal - direction;
        
        DrawRectangleFilled(topLeft, topRight, bottomRight, bottomLeft, color);
    }
    
    private void DrawRectangleFilled(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft, Color color)
    {
        _indices.Insert(_indexCount++, 0 + _vertexCount);
        _indices.Insert(_indexCount++, 1 + _vertexCount);
        _indices.Insert(_indexCount++, 2 + _vertexCount);
        _indices.Insert(_indexCount++, 0 + _vertexCount);
        _indices.Insert(_indexCount++, 2 + _vertexCount);
        _indices.Insert(_indexCount++, 3 + _vertexCount);

        _vertices.Insert(_vertexCount++, new VertexPositionColor(new Vector3(topLeft, 0), color));
        _vertices.Insert(_vertexCount++, new VertexPositionColor(new Vector3(topRight, 0), color));
        _vertices.Insert(_vertexCount++, new VertexPositionColor(new Vector3(bottomRight, 0), color));
        _vertices.Insert(_vertexCount++, new VertexPositionColor(new Vector3(bottomLeft, 0), color));
    }

    public void DrawCircleOutline(Vector2 center, float radius, Color color)
    {
        float segmentAngle = MathHelper.TwoPi / CirclePoints;
        float currentAngle = 0f;

        for (int i = 0; i < CirclePoints; i++)
        {
            Vector2 startPoint = new(
                center.X + (float)Math.Sin(currentAngle) * radius,
                center.Y + (float)Math.Cos(currentAngle) * radius
            );

            currentAngle += segmentAngle;

            Vector2 endPoint = new(
                center.X + (float)Math.Sin(currentAngle) * radius,
                center.Y + (float)Math.Cos(currentAngle) * radius
            );

            DrawLine(startPoint, endPoint, color);
        }
    }

    public void DrawShapeOutline(Vertex[] vertices)
    {
        // This method assumes the vertices are in clockwise order
        for (int i = 0; i < vertices.Length; i++)
        {
            DrawLine(
                vertices[i].RotatedTranslated, 
                i != vertices.Length - 1 ? vertices[i + 1].RotatedTranslated : vertices[0].RotatedTranslated, 
                vertices[i].GetEdgeColor()
            );
        }
    }

    public void Draw(Matrix transformMatrix)
    {
        if (_indexCount > 0 && _vertexCount > 0)
        {
            // Set the projection and view matrices
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0,
                _graphicsDeviceManager.PreferredBackBufferWidth,
                _graphicsDeviceManager.PreferredBackBufferHeight,
                0,
                0f,
                -1f
            );
            _basicEffect.View = transformMatrix;

            // Loop through all of the primitive shapes and draw
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDeviceManager.GraphicsDevice.DrawUserIndexedPrimitives(
                    primitiveType: PrimitiveType.TriangleList,
                    vertexData: _vertices.ToArray(),
                    vertexOffset: 0,
                    numVertices: _vertexCount,
                    indexData: _indices.ToArray(),
                    indexOffset: 0,
                    primitiveCount: _indexCount / 3
                );
            }

            // Reset the lists and counts
            _indexCount = 0;
            _vertexCount = 0;
            _vertices.Clear();
            _indices.Clear();
        }
    }
}