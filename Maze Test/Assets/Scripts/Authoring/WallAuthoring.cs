using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class WallAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Scale>(entity);
        dstManager.AddComponent<Translation>(entity);
        dstManager.AddComponent<RotationEulerXYZ>(entity);
    }
}
