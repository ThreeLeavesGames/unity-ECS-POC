using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public partial class SpawnerSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;
    private TargetPositionProvider _targetPositionProvider;
    protected override void OnCreate()
    {
        // Get the EndSimulationEntityCommandBufferSystem to handle structural changes
        _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();

       // Get the TargetPositionProvider from the scene (assuming it's on a GameObject tagged "TargetProvider")
    // GameObject targetProviderObject = GameObject.FindWithTag("TargetProvider");
    
    // if (targetProviderObject == null)
    // {
    //     Debug.LogError("No GameObject with the tag 'TargetProvider' found!");
    // }
    // else
    // {
    //     Debug.Log("Found TargetProvider GameObject: " + targetProviderObject.name);
    //     _targetPositionProvider = targetProviderObject.GetComponent<TargetPositionProvider>();

    //     if (_targetPositionProvider == null)
    //     {
    //         Debug.LogError("TargetPositionProvider component not found on the GameObject.");
    //     }
    // }
    }

    protected override void OnUpdate()
    {
        if (_targetPositionProvider == null)
        {
            GameObject targetProviderObject = GameObject.FindWithTag("TargetProvider");
            if (targetProviderObject == null)
            {
                return;
            }
            else
            {
                _targetPositionProvider = targetProviderObject.GetComponent<TargetPositionProvider>();
            }
        }

        // If the target provider is missing, skip the update
        if (_targetPositionProvider == null) return;

        // Get the target position from the GameObject
        Vector3 targetPosition = _targetPositionProvider.GetTargetPosition();

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
                        TargetPosition =  new float3(targetPosition.x, targetPosition.y, targetPosition.z),
                        Speed = -1f, // Adjust speed as needed
                        Acceleration=1f,
                        DestinationOffset=UnityEngine.Random.Range(1,3),
                        SpawnOffset=UnityEngine.Random.Range(1,3)
                    });
                }

                // Reset the next spawn time to spawn again after the interval (0.25 seconds)
                spawner.NextSpawnTime = elapsedTime + spawnInterval;
            }

        }).Run(); // Schedule the job to run asynchronously

        // Ensure that the command buffer system knows about this job
        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}



//TODO--> works good
// using Unity.Entities;
// using Unity.Transforms;
// using Unity.Burst;
// using Unity.Mathematics;
// using UnityEngine;

// public partial class Spawner_System : SystemBase
// {
//     private EndSimulationEntityCommandBufferSystem _ecbSystem;

//     protected override void OnCreate()
//     {
//         // Get the EndSimulationEntityCommandBufferSystem to handle structural changes
//         _ecbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
//     }

//     protected override void OnUpdate()
//     {
//         // Get the EntityCommandBuffer to queue structural changes
//         var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();

//         // Capture the elapsed time for use in the job
//         float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

//         // Schedule the burst-compiled job
//         var spawnJob = new SpawnerJob
//         {
//             ecb = ecb,
//             elapsedTime = elapsedTime
//         };

//         // Schedule the job and assign the Dependency
//         Dependency = spawnJob.ScheduleParallel(Dependency);

//         // Ensure that the command buffer system knows about this job
//         _ecbSystem.AddJobHandleForProducer(Dependency);
//     }

//     // Define the job to handle entity spawning
//     [BurstCompile]
//     public partial struct SpawnerJob : IJobEntity
//     {
//         public EntityCommandBuffer.ParallelWriter ecb;
//         public float elapsedTime;

//         // The method that will be called for each entity in the query
//         public void Execute([ChunkIndexInQuery] int chunkIndex, ref Spwaner_Component spawner)
//         {
//             // Calculate the time interval to spawn 4 entities per second (every 0.25 seconds)
//             float spawnInterval = 0.25f;

//             // Check if it's time to spawn new entities
//             if (spawner.NextSpawnTime < elapsedTime)
//             {
//                 for (int i = 0; i < 4; i++) // Spawn 4 entities
//                 {
//                     // Instantiate a new entity using the command buffer
//                     Entity newEntity = ecb.Instantiate(chunkIndex, spawner.Prefab);

//                     // Set the initial position of the entity using the command buffer
//                     ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(spawner.SpawnPosition));

//                     // Add a movement component to move the entity towards (0, 0, -30)
//                     ecb.AddComponent(chunkIndex, newEntity, new MoveToTarget_Component
//                     {
//                         TargetPosition = new float3(0, 0, -30),
//                         Speed = 10f // Adjust speed as needed
//                     });
//                 }

//                 // Reset the next spawn time to spawn again after the interval (0.25 seconds)
//                 spawner.NextSpawnTime = elapsedTime + spawnInterval;
//             }
//         }
//     }
// }


