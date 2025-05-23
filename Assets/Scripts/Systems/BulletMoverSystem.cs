using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BulletMoverSystem : ISystem
{
    /// <summary>
    /// NOTE : we are calculating distanceBeforeSq and distanceAfterSq to check if the bullet is overshooting the target.
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRW<LocalTransform> localTransform, RefRO<Bullet> bullet, RefRO<Target> target, Entity entity) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>, RefRO<Target>>().WithEntityAccess())
        {
            if (target.ValueRO.targetEntity == Entity.Null)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }
            
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);
            float3 targetPosition = targetLocalTransform.TransformPoint(shootVictim.hitLocalPosition);
            
            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);
            
            // move bullet towards target
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += bullet.ValueRO.speed * SystemAPI.Time.DeltaTime * moveDirection;

            // check if bullet overshooting the target
            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);
            if (distanceAfterSq > distanceBeforeSq)
            {
                // overshot
                localTransform.ValueRW.Position = targetPosition;
            }

            // damage the target if the bullet is close enough and destroy the bullet
            float destroyDistanceSq = 0.2f;
            if(math.distancesq(localTransform.ValueRO.Position, targetPosition) < destroyDistanceSq)
            {
                // add damage to the target
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                
                // destroy the bullet
                entityCommandBuffer.DestroyEntity(entity);
            }
            
        }
    }
}
