using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Engine.Services;
using TakeMeToSpace.Engine.Utilities;

namespace TakeMeToSpace.Engine.Components;

public class BoundingPolygon
{
    // Todo: Make sure once destroying objects is implemented, to clean up ids that are being destroyed
    private static readonly HashSet<int> UniqueIds = new();
    private static readonly int MaxIdNumber = 1001;

    public readonly int Id;
    public readonly HashSet<int> CollidingWithIds = new();
    public Vertex[] Vertices;

    private bool _isColliding;
    public bool IsColliding
    {
        get => _isColliding;
        private set
        {
            _isColliding = value;
            foreach (Vertex vertex in Vertices)
                vertex.IsCollidingColorOverride = value;
        }
    }

    public BoundingPolygon(Vector2[] vertices)
    {
        // Generate the id and add it to the static unique ids list
        Id = GenerateId();
        UniqueIds.Add(Id);

        Vertices = new Vertex[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
           Vertices[i] = new Vertex
           {
               EdgeColor = Vertex.Colors[i],
               RotatedTranslated = vertices[i],
               Translated = vertices[i]
           };
        }
    }
    
    private static int GenerateId(int previousId = 0)
    {
        int nextId = Randomizer.Next(1, MaxIdNumber);
        return previousId != nextId && !UniqueIds.Contains(nextId) ? nextId : GenerateId(nextId);
    }
    
    private void UpdateIsColliding()
    {
        IsColliding = CollidingWithIds.Count != 0;
    }

    public void AddCollidingWith(int id)
    {
        CollidingWithIds.Add(id);
        UpdateIsColliding();
    }

    public void RemoveCollidingWith(int id)
    {
        CollidingWithIds.Remove(id);
        UpdateIsColliding();
    }

    public void Transform(Vector2 position, Vector2 translation, float rotation)
    {
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);
        
        foreach (Vertex vertex in Vertices)
        {   
            // Translate
            vertex.Translated += translation;
            
            // Rotate by the un-rotated translation vector
            Vector2 rotationalOrigin = vertex.Translated - position;
            Vector2 rotated = new Vector2(
                rotationalOrigin.X * cos - rotationalOrigin.Y * sin,
                rotationalOrigin.X * sin + rotationalOrigin.Y * cos
            );
            
            // Translate the rotated point back to the point
            vertex.RotatedTranslated = rotated + position;
        }
    }

    public Vertex[] TransformCopyVertices(Vector2 position, Vector2 translation, float rotation)
    {
        Vertex[] verticesCopy = new Vertex[Vertices.Length];
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);

        for (int i = 0; i < Vertices.Length; i++)
        {
            // Create the new vertex component and do translation
            verticesCopy[i] = new Vertex
            {
                Translated = new Vector2(
                    Vertices[i].Translated.X + translation.X,
                    Vertices[i].Translated.Y + translation.Y
                )
            };

            // Rotate by the un-rotated translation vector
            Vector2 rotationalOrigin = verticesCopy[i].Translated - position;
            Vector2 rotated = new Vector2(
                rotationalOrigin.X * cos - rotationalOrigin.Y * sin,
                rotationalOrigin.X * sin + rotationalOrigin.Y * cos
            );

            // Translate the rotated point back to the point
            verticesCopy[i].RotatedTranslated = new Vector2(rotated.X + position.X, rotated.Y + position.Y);
        }
        
        return verticesCopy;
    }

    public void Draw(PrimitiveDrawingService primitiveDrawingService)
    {
        primitiveDrawingService.DrawShapeOutline(Vertices);
    }
}