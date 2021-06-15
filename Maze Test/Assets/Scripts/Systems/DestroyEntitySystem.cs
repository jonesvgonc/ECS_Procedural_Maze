using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DestroyEntitySystem : SystemBase
{
    private EntityCommandBufferSystem _commandBufferSystem;
    protected override void OnCreate()
    {
        _commandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Dependency = Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref DestroyFlag destroyData) =>
            {
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            })
            .ScheduleParallel(Dependency);

        _commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
