using System;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Base.Components;

namespace TakeMeToSpace.Base.Services;

public class CollisionService
{
    private void FindMinMaxProjection(Vertex[] vertices, Vector2 edgeNormal, out float min, out float max)
    {
        // Reset min and max projections to their respective opposites to ensure they are set on the first pass
        min = float.MaxValue;
        max = float.MinValue;
        
        foreach (Vertex vertex in vertices)
        {
            float projection = Vector2.Dot(vertex.RotatedTranslated, edgeNormal);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }

    public bool DetectCollision(Vertex[] vertices1, Vertex[] vertices2, out Vector2 collisionDirection, out float smallestCollisionDepth)
    {
        // Reset the resolution values
        collisionDirection = Vector2.Zero;
        smallestCollisionDepth = float.MaxValue;
        
        // Project the first polygon onto each edge normal
        for (int i = 0; i < vertices1.Length; i++)
        {
            // Get the current edge to check
            Vector2 edge;
            if (i == vertices1.Length - 1)
                edge = vertices1[0].RotatedTranslated - vertices1[i].RotatedTranslated;
            else
                edge = vertices1[i + 1].RotatedTranslated - vertices1[i].RotatedTranslated;
            
            // Determine the perpendicular to the edge and normalize
            Vector2 edgeNormal = new Vector2(edge.Y, -edge.X);
            edgeNormal.Normalize();
            
            // Project the vertices to find the minimum and maximum projections
            FindMinMaxProjection(vertices1, edgeNormal, out float min1, out float max1);
            FindMinMaxProjection(vertices2, edgeNormal, out float min2, out float max2);

            // Separation found, not colliding
            if (max1 <= min2 || max2 <= min1)
                return false;

            // Separation not found, update the collision depth for resolution
            float currentCollisionDepth = Math.Min(max1 - min2, max2 - min1);
            if (currentCollisionDepth < smallestCollisionDepth)
            {
                smallestCollisionDepth = currentCollisionDepth;
                collisionDirection = edgeNormal;
            }
        }
        
        // No separating axis found on the first polygon edge normals, check the second polygon
        for (int i = 0; i < vertices2.Length; i++)
        {
            // Get the current edge to check
            Vector2 edge;
            if (i == vertices2.Length - 1)
                edge = vertices2[0].RotatedTranslated - vertices2[i].RotatedTranslated;
            else
                edge = vertices2[i + 1].RotatedTranslated - vertices2[i].RotatedTranslated;
            
            // Determine the perpendicular to the edge and normalize
            Vector2 edgeNormal = new Vector2(edge.Y, -edge.X);
            edgeNormal.Normalize();
            
            // Project the vertices to find the minimum and maximum projections
            FindMinMaxProjection(vertices1, edgeNormal, out float min1, out float max1);
            FindMinMaxProjection(vertices2, edgeNormal, out float min2, out float max2);

            // Separation found, not colliding
            if (max1 <= min2 || max2 <= min1)
                return false;

            // Separation not found, update the collision depth for resolution
            float currentCollisionDepth = Math.Min(max1 - min2, max2 - min1);
            if (currentCollisionDepth < smallestCollisionDepth)
            {
                smallestCollisionDepth = currentCollisionDepth;
                collisionDirection = edgeNormal;
            }
        }

        return true;
    }
}