using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public float attackDistance;
    public Transform bulletSpawnTransform;
    
    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                attackDistance = authoring.attackDistance,
                bulletSpawnLocalPosition = authoring.bulletSpawnTransform.localPosition
            });
        }
    }
}

public struct ShootAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public float attackDistance;
    public float3 bulletSpawnLocalPosition;
}
