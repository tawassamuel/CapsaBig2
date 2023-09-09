using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CardSpawnerSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_entityCommBufferSystem;

    protected override void OnCreateManager()
    {
        m_entityCommBufferSystem = World.GetOrCreateManager<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var currentJob = new SpawnJob
        {
            commandBuffer = m_entityCommBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(this, inputDeps);

        m_entityCommBufferSystem.AddJobHandleForProducer(currentJob);
        return currentJob;
    }

    struct SpawnJob : IJobProcessComponentDataWithEntity<Spawner, LocalToWorld> 
    {
        public EntityCommandBuffer commandBuffer;
        public void Execute(Entity entity, int index, [ReadOnly] ref Spawner spawner, [ReadOnly] ref LocalToWorld location)
        {
            for (int i = 0; i < spawner.Nums; i++)
            {
                var instance = commandBuffer.Instantiate(spawner.Prefab);
                if (instance != null)
                {
                    //var pos = math.transform(location.Value, float3.zero);
                    //commandBuffer.SetComponent(instance, new Translation { Value = pos });
                }
            }
            commandBuffer.DestroyEntity(entity);
        }
    }
}
