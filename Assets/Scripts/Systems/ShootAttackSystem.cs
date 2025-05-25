using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // NOTE : if we want to use the `EntitiesReferences` component, we need to access using `SystemAPI.GetSingleton<T>()`
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        
        foreach ((RefRW<LocalTransform> localTransform, RefRW<ShootAttack> shootAttack, RefRO<Target> target, RefRW<UnitMover> unitMover) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>, RefRW<UnitMover>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                continue;
            }
            
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            
            // if the target is too far, we need to set the target
            if(math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > shootAttack.ValueRO.attackDistance)
            {
                // too far to attack
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            }
            else
            {
                // stop moving and attack
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }
            
            // rotate towards target
            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalize(aimDirection);
            
            quaternion targetRotation = quaternion.LookRotation(aimDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            
            // shoot attack timer
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
            float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            RefRW<Target> bulletTargetComp = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTargetComp.ValueRW.targetEntity = target.ValueRO.targetEntity;

            shootAttack.ValueRW.onShoot.isTriggered = true;
            shootAttack.ValueRW.onShoot.shootFromPosition = bulletSpawnWorldPosition;

        }
    }
}
