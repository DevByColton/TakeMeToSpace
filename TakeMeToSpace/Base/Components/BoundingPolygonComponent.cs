using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Base.Services;
using TakeMeToSpace.Base.Utilities;

namespace TakeMeToSpace.Base.Components;

public class BoundingPolygonComponent
{
    // Todo: Make sure once destroying objects is implemented, to clean up ids that are being destoryed
    private static readonly HashSet<int> UniqueIds = new();
    private static readonly int MaxIdNumber = 1001;

    public readonly int Id;
    public readonly HashSet<int> CollidingWithIds = new();
    public VertexComponent[] VertexComponents;

    private bool _isColliding;
    public bool IsColliding
    {
        get => _isColliding;
        private set
        {
            _isColliding = value;
            foreach (VertexComponent vertexComponent in VertexComponents)
                vertexComponent.IsCollidingColorOverride = value;
        }
    }

    public BoundingPolygonComponent(Vector2[] vertices)
    {
        // Generate the id and add it to the static unique ids list
        Id = GenerateId();
        UniqueIds.Add(Id);

        VertexComponents = new VertexComponent[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
           VertexComponents[i] = new VertexComponent
           {
               EdgeColor = VertexComponent.Colors[i],
               RotatedTranslatedVertex = vertices[i],
               TranslatedVertex = vertices[i]
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
        
        foreach (VertexComponent vertexComponent in VertexComponents)
        {   
            // Translate
            vertexComponent.TranslatedVertex += translation;
            
            // Rotate by the un-rotated translation vector
            Vector2 rotationalOrigin = vertexComponent.TranslatedVertex - position;
            Vector2 rotated = new Vector2(
                rotationalOrigin.X * cos - rotationalOrigin.Y * sin,
                rotationalOrigin.X * sin + rotationalOrigin.Y * cos
            );
            
            // Translate the rotated point back to the point
            vertexComponent.RotatedTranslatedVertex = rotated + position;
        }
    }

    public VertexComponent[] TransformCopyVertexComponents(Vector2 position, Vector2 translation, float rotation)
    {
        VertexComponent[] vcsCopy = new VertexComponent[VertexComponents.Length];
        float cos = (float)Math.Cos(rotation);
        float sin = (float)Math.Sin(rotation);

        for (int i = 0; i < VertexComponents.Length; i++)
        {
            // Create the new vertex component and do translation
            vcsCopy[i] = new VertexComponent
            {
                TranslatedVertex = new Vector2(
                    VertexComponents[i].TranslatedVertex.X + translation.X,
                    VertexComponents[i].TranslatedVertex.Y + translation.Y
                )
            };

            // Rotate by the un-rotated translation vector
            Vector2 rotationalOrigin = vcsCopy[i].TranslatedVertex - position;
            Vector2 rotated = new Vector2(
                rotationalOrigin.X * cos - rotationalOrigin.Y * sin,
                rotationalOrigin.X * sin + rotationalOrigin.Y * cos
            );

            // Translate the rotated point back to the point
            vcsCopy[i].RotatedTranslatedVertex = new Vector2(rotated.X + position.X, rotated.Y + position.Y);
        }
        
        return vcsCopy;
    }

    public void Draw(PrimitiveDrawingService primitiveDrawingService)
    {
        primitiveDrawingService.DrawShapeOutline(VertexComponents);
    }
}