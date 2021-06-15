using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MazeRendererSystem : SystemBase
{
    private EntityQuery _query;
    private EntityCommandBufferSystem _commandBufferSystem;

    protected override void OnCreate()
    {
        _query = GetEntityQuery(
             ComponentType.ReadOnly<MazeRendererData>());

        _commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        RequireForUpdate(_query);
    }

    protected override void OnUpdate()
    {
        var commandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();


        var settings = GetSingleton<MazeSettingsData>();
        var wall = GamePrefabManager.WallEntity;
        var ground = GamePrefabManager.GroundEntity;
        var instantiatedGround = EntityManager.Instantiate(ground);
        var groundScale = EntityManager.GetComponentData<Scale>(instantiatedGround);
        var groundTranslation = EntityManager.GetComponentData<Translation>(instantiatedGround);
        groundScale.Value = settings.Width / 5;
        groundTranslation.Value = new float3(-.5f, 0, -.5f);

        EntityManager.SetComponentData(instantiatedGround, groundScale);
        EntityManager.SetComponentData(instantiatedGround, groundTranslation);

        Dependency = Entities
            .ForEach((int entityInQueryIndex, Entity entity, ref MazeRendererData nodeData) =>
            {
                var position = new float3(-settings.Width / 2 + nodeData.Node.NodePosition.X, 0, -settings.Height / 2 + nodeData.Node.NodePosition.Y);

                if ((nodeData.Node.WallState & WallState.UP) != WallState.NONE)
                {
                    var topWall = commandBuffer.Instantiate(entityInQueryIndex, wall);
                    commandBuffer.SetComponent(entityInQueryIndex, topWall, new Translation() { Value = position + new float3(0, 0, 0.5f) });
                }
                if ((nodeData.Node.WallState & WallState.LEFT) != WallState.NONE)
                {
                    var leftWall = commandBuffer.Instantiate(entityInQueryIndex, wall);
                    commandBuffer.SetComponent(entityInQueryIndex, leftWall, new RotationEulerXYZ() { Value = math.radians(new float3(0, 90, 0)) });
                    commandBuffer.SetComponent(entityInQueryIndex, leftWall, new Translation() { Value = position + new float3(-0.5f, 0, 0) });
                }
                if (nodeData.Node.NodePosition.X == settings.Width - 1)
                {
                    if ((nodeData.Node.WallState & WallState.RIGHT) != WallState.NONE)
                    {
                        var rightWall = commandBuffer.Instantiate(entityInQueryIndex, wall);
                        commandBuffer.SetComponent(entityInQueryIndex, rightWall, new RotationEulerXYZ() { Value = math.radians(new float3(0, 90, 0)) });
                        commandBuffer.SetComponent(entityInQueryIndex, rightWall, new Translation() { Value = position + new float3(0.5f, 0, 0) });
                    }
                }
                if (nodeData.Node.NodePosition.Y == 0)
                {
                    if ((nodeData.Node.WallState & WallState.DOWN) != WallState.NONE)
                    {
                        var downWall = commandBuffer.Instantiate(entityInQueryIndex, wall);
                        commandBuffer.SetComponent(entityInQueryIndex, downWall, new Translation() { Value = position + new float3(0, 0, -0.5f) });
                    }
                }
                commandBuffer.AddComponent(entityInQueryIndex, entity, new DestroyFlag());
            })
            .ScheduleParallel(Dependency);

        _commandBufferSystem.AddJobHandleForProducer(Dependency);

    }
}
