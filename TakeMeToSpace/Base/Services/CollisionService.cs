using System;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Base.Components;

namespace TakeMeToSpace.Base.Services;

public class CollisionService
{
    private void FindMinMaxProjection(VertexComponent[] vcs, Vector2 edgeNormal, out float min, out float max)
    {
        // Reset min and max projections to their respective opposites to ensure they are set on the first pass
        min = float.MaxValue;
        max = float.MinValue;
        
        foreach (VertexComponent vc in vcs)
        {
            float projection = Vector2.Dot(vc.RotatedTranslatedVertex, edgeNormal);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }

    public bool DetectCollision(VertexComponent[] vcs1, VertexComponent[] vcs2, out Vector2 collisionDirection, out float smallestCollisionDepth)
    {
        // Reset the resolution values
        collisionDirection = Vector2.Zero;
        smallestCollisionDepth = float.MaxValue;
        
        // Project the first polygon onto each edge normal
        for (int i = 0; i < vcs1.Length; i++)
        {
            // Get the current edge to check
            Vector2 edge;
            if (i == vcs1.Length - 1)
                edge = vcs1[0].RotatedTranslatedVertex - vcs1[i].RotatedTranslatedVertex;
            else
                edge = vcs1[i + 1].RotatedTranslatedVertex - vcs1[i].RotatedTranslatedVertex;
            
            // Determine the perpendicular to the edge and normalize
            Vector2 edgeNormal = new Vector2(edge.Y, -edge.X);
            edgeNormal.Normalize();
            
            // Project the vertices to find the minimum and maximum projections
            FindMinMaxProjection(vcs1, edgeNormal, out float min1, out float max1);
            FindMinMaxProjection(vcs2, edgeNormal, out float min2, out float max2);

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
        for (int i = 0; i < vcs2.Length; i++)
        {
            // Get the current edge to check
            Vector2 edge;
            if (i == vcs2.Length - 1)
                edge = vcs2[0].RotatedTranslatedVertex - vcs2[i].RotatedTranslatedVertex;
            else
                edge = vcs2[i + 1].RotatedTranslatedVertex - vcs2[i].RotatedTranslatedVertex;
            
            // Determine the perpendicular to the edge and normalize
            Vector2 edgeNormal = new Vector2(edge.Y, -edge.X);
            edgeNormal.Normalize();
            
            // Project the vertices to find the minimum and maximum projections
            FindMinMaxProjection(vcs1, edgeNormal, out float min1, out float max1);
            FindMinMaxProjection(vcs2, edgeNormal, out float min2, out float max2);

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