using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Spawner: IComponentData
{
    public Entity Prefab;
    public int Nums;
}

public class CardSpawner : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private GameObject card;
    [SerializeField] private RectTransform container;
    [SerializeField] private int nums = 12;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(card);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new Spawner
        {
            Prefab = conversionSystem.GetPrimaryEntity(card),
            Nums = nums
        };

        dstManager.AddComponentData(entity, spawnerData);
    }
}
