using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

// public class RandomWalkingAuthoring : MonoBehaviour
// {
//     public class Baker : Baker<RandomWalkingAuthoring>
//     {
//         public override void Bake(RandomWalkingAuthoring authoring)
//         {
//             Entity entity = GetEntity(TransformUsageFlags.Dynamic);
//             AddComponent(entity, new RandomWalking());
//         }
//     }
// }

public struct RandomWalking : IComponentData
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public Random random;
}
