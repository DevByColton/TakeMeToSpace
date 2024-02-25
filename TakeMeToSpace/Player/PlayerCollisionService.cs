using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TakeMeToSpace.Base.Components;
using TakeMeToSpace.Base.Services;

namespace TakeMeToSpace.Player;

public class PlayerCollisionService
{
    private readonly CollisionService _collisionService = new();

    public Vector2 GetAllowedMovement(Vector2 potentialDirection, PlayerEntity playerEntity, List<ColliderComponent> colliders)
    {
        // Get the potential position by adding the potential direction to the current position
        Vector2 potentialPosition = playerEntity.PositionComponent.Position + potentialDirection;
        
        // Get the potential new vertices components
        VertexComponent[] potentialVcs = playerEntity.BoundingPolygonComponent.TransformCopyVertexComponents(
            potentialPosition,
            potentialDirection,
            playerEntity.PositionComponent.Rotation
        );
        
        // Check the collision of the player against all boundaries
        Vector2 collisionResolution = Vector2.Zero;
        collisionResolution += CheckColliders(potentialVcs, playerEntity, colliders);
        
        // Return the allowed movement amount based off direction and collision resolution
        return potentialDirection + collisionResolution;
    }

    private Vector2 CheckColliders(VertexComponent[] potentialVcs, PlayerEntity playerEntity, List<ColliderComponent> colliders)
    {
        Vector2 collisionResolution = Vector2.Zero;
        
        colliders.ForEach(collider =>
        {
            bool isColliding = _collisionService.DetectCollision(
                potentialVcs,
                collider.BoundingPolygonComponent.VertexComponents,
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
                
                playerEntity.BoundingPolygonComponent.AddCollidingWith(collider.BoundingPolygonComponent.Id);
                collider.BoundingPolygonComponent.AddCollidingWith(playerEntity.BoundingPolygonComponent.Id);
            }
            else
            {
                playerEntity.BoundingPolygonComponent.RemoveCollidingWith(collider.BoundingPolygonComponent.Id);
                collider.BoundingPolygonComponent.RemoveCollidingWith(playerEntity.BoundingPolygonComponent.Id);
            }
        });

        return collisionResolution;
    }
}