using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GamePrefabManager : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject WallPrefab;
    public static Entity WallEntity;

    public GameObject GroundPrefab;
    public static Entity GroundEntity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        WallEntity = conversionSystem.GetPrimaryEntity(WallPrefab);
        GroundEntity = conversionSystem.GetPrimaryEntity(GroundPrefab);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(GroundPrefab);
    }
}
