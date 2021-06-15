using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class MazeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Height;
    public int Width;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MazeData() { Width = Width, Height = Height });
    }
}
