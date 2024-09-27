using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
public partial struct MoveToTarget_System : ISystem
{
    // public void OnUpdate(ref SystemState state)
    // {
    //     var ecb = new EntityCommandBuffer(Allocator.Temp);

    //     foreach (var (transform, moveToTarget, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveToTarget_Component>>().WithEntityAccess())
    //     {
    //         // Calculate the direction to the target
    //         float3 direction = math.normalize(moveToTarget.ValueRO.TargetPosition - transform.ValueRO.Position);

    //         // Calculate the distance to the target
    //         float distanceToTarget = math.distance(transform.ValueRO.Position, moveToTarget.ValueRO.TargetPosition);

    //         // Move the entity toward the target along the calculated direction
    //         float3 newPosition = direction * moveToTarget.ValueRO.Speed * SystemAPI.Time.DeltaTime;

    //         // Update the new position
    //         transform.ValueRW.Position += newPosition;

    //         // Check if the entity has reached the target position
    //         if (distanceToTarget < 0.1f)
    //         {
    //             // Disable the entity when it reaches the target by adding the Disabled component
    //             ecb.AddComponent<Disabled>(entity);
    //         }
    //     }

    //     // Play back the structural changes (like disabling the entity)
    //     ecb.Playback(state.EntityManager);
    //     ecb.Dispose();
    // }

[BurstCompile]
public void OnUpdate(ref SystemState state)
    {
        // EntityCommandBuffer for structural changes
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // Loop over entities with LocalTransform and MoveToTarget_Component
        foreach (var (transform, moveToTarget, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MoveToTarget_Component>>().WithEntityAccess())
        {
            // Similar to the uiTarget, the target position is used as the destination
            float3 targetPosition = moveToTarget.ValueRO.TargetPosition + new float3(moveToTarget.ValueRO.DestinationOffset, moveToTarget.ValueRO.SpawnOffset, 0f);

            // Move entity towards the target position using MoveTowards logic
            transform.ValueRW.Position = math.lerp(transform.ValueRO.Position, targetPosition, moveToTarget.ValueRO.Speed * SystemAPI.Time.DeltaTime);

            // If the entity is close to the target (within a threshold), call Arrived()
            if (math.distance(transform.ValueRO.Position, targetPosition) > 0.03f)
            {
                // Increase speed and acceleration over time
                moveToTarget.ValueRW.Speed += moveToTarget.ValueRO.Acceleration * SystemAPI.Time.DeltaTime;
                moveToTarget.ValueRW.Acceleration += moveToTarget.ValueRO.Acceleration * SystemAPI.Time.DeltaTime;
            }
            else
            {
                // Call Arrived logic (disabling the entity)
                Arrived(ecb, entity);
            }
        }

        // Play back structural changes
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void Arrived(EntityCommandBuffer ecb, Entity entity)
    {
        // Here we handle what happens when the entity reaches the target (similar to the Arrived method in Dot)
        // Disable the entity or do something else (like reduce pointsInDots)

        // For example: disable the entity when it reaches its target
        ecb.AddComponent<Disabled>(entity);

        // Optionally, handle any additional logic like reducing points, playing sounds, or re-enabling the entity later
    }
}
