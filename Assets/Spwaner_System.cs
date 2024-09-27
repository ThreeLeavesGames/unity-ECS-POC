using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public partial class SpawnerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        // Get the EndSimulationEntityCommandBufferSystem to handle structural changes
        _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // Get the EntityCommandBuffer to queue structural changes
        var ecb = _ecbSystem.CreateCommandBuffer();

        // Capture the delta time for use in the lambda
        float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

        Entities.ForEach((ref Spwaner_Component spawner) =>
        {
            // Calculate the time interval to spawn 4 entities per second (every 0.25 seconds)
            float spawnInterval = 0.25f;

            // Check if it's time to spawn new entities
            if (spawner.NextSpawnTime < elapsedTime)
            {
                for (int i = 0; i < 4; i++) // Spawn 4 entities
                {
                    // Instantiate a new entity using the command buffer
                    Entity newEntity = ecb.Instantiate(spawner.Prefab);

                    // Set the initial position of the entity using the command buffer
                    ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

                    // Add a movement component to move the entity towards (0,0,-30)
                    ecb.AddComponent(newEntity, new MoveToTarget_Component
                    {
                        TargetPosition = new float3(0, 0, -30),
                        Speed = 10f // Adjust speed as needed
                    });
                }

                // Reset the next spawn time to spawn again after the interval (0.25 seconds)
                spawner.NextSpawnTime = elapsedTime + spawnInterval;
            }

        }).Schedule(); // Schedule the job to run asynchronously

        // Ensure that the command buffer system knows about this job
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
