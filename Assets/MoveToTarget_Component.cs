using Unity.Entities;
using Unity.Mathematics;

public struct MoveToTarget_Component : IComponentData
{
    public float3 TargetPosition; // The target position (in this case (0,0,-30))
    public float Speed; // Movement speed toward the target
}