using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
public partial struct MoveToTarget_System : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, moveToTarget) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveToTarget_Component>>())
        {
            // Get current position
            float3 currentPosition = transform.ValueRO.Position;

            // Calculate the direction to the target
            float3 direction = math.normalize(moveToTarget.ValueRO.TargetPosition - currentPosition);

            // Move the entity toward the target
            float3 newPosition = currentPosition + direction * moveToTarget.ValueRO.Speed * SystemAPI.Time.DeltaTime;

            // Set the new position
            transform.ValueRW.Position = newPosition;

            // Optionally, stop the entity if it reaches the target position
            // if (math.distance(newPosition, moveToTarget.ValueRO.TargetPosition) < 0.1f)
            // {
            //     // Stop movement by setting speed to 0 or removing the MoveToTargetComponent
            //     state.EntityManager.RemoveComponent<MoveToTarget_Component>(moveToTarget.ValueRO.e);
            // }
        }
    }
}
