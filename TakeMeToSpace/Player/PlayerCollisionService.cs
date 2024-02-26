using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Engine.Components;
using TakeMeToSpace.Engine.Services;

namespace TakeMeToSpace.Player;

public class PlayerCollisionService
{
    private readonly CollisionService _collisionService = new();

    public Vector2 GetAllowedMovement(Vector2 potentialDirection, PlayerEntity playerEntity, List<Collider> colliders)
    {
        // Get the potential position by adding the potential direction to the current position
        Vector2 potentialPosition = playerEntity.PositionComponent.Position + potentialDirection;
        
        // Get the potential new vertices components
        Vertex[] potentialVertices = playerEntity.BoundingPolygon.TransformCopyVertices(
            potentialPosition,
            potentialDirection,
            playerEntity.PositionComponent.Rotation
        );
        
        // Check the collision of the player against all boundaries
        Vector2 collisionResolution = Vector2.Zero;
        collisionResolution += CheckColliders(potentialVertices, playerEntity, colliders);
        
        // Return the allowed movement amount based off direction and collision resolution
        return potentialDirection + collisionResolution;
    }

    private Vector2 CheckColliders(Vertex[] potentialVcs, PlayerEntity playerEntity, List<Collider> colliders)
    {
        Vector2 collisionResolution = Vector2.Zero;
        
        colliders.ForEach(collider =>
        {
            bool isColliding = _collisionService.DetectCollision(
                potentialVcs,
                collider.BoundingPolygon.Vertices,
                out Vector2 collisionDirection,
                out float collisionDepth
            );
            
            // If there is collision, check the collision direction
            if (isColliding)
            {
                // Make sure the direction of the collisionDirection is facing the right way
                // if the dot product is negative, then its facing the opposite direction, flip it
                if (Vector2.Dot(playerEntity.PositionComponent.Position - collider.PositionComponent.Position, collisionDirection) < 0f)
                    collisionDirection = -collisionDirection;

                // Aggregate the collision resolutions
                collisionResolution += collisionDirection * collisionDepth;
                
                playerEntity.BoundingPolygon.AddCollidingWith(collider.BoundingPolygon.Id);
                collider.BoundingPolygon.AddCollidingWith(playerEntity.BoundingPolygon.Id);
            }
            else
            {
                playerEntity.BoundingPolygon.RemoveCollidingWith(collider.BoundingPolygon.Id);
                collider.BoundingPolygon.RemoveCollidingWith(playerEntity.BoundingPolygon.Id);
            }
        });

        return collisionResolution;
    }
}