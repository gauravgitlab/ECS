using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // NOTE : if we want to use the `EntitiesReferences` component, we need to access using `SystemAPI.GetSingleton<T>()`
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        
        foreach ((RefRO<LocalTransform> localTransform, RefRW<ShootAttack> shootAttack, RefRO<Target> target) in 
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }
            
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRW.timer > 0)
            {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;
            // UnityEngine.Debug.Log($"Shoot attack damage to {target.ValueRO.targetEntity}");
            
            // spawn a bullet,
            // NOTE : this is how we are instantiating an entity in the ECS, and 
            // then we are set bullet position to the unit position
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            RefRW<Target> bulletTargetComp = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTargetComp.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }
}
