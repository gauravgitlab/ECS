using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthDeadTestSystem : ISystem
{
    // destroying the entity using `EntityCommandBuffer`
    // entityCommandBuffer.DestroyEntity(entity); will not destroying the entity when we call this, but it will just add in the queue buffer
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach ((RefRO<Health> health, Entity entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
        {
            if (health.ValueRO.healthAmount <= 0)
            {
                UnityEngine.Debug.Log($"Entity {entity} is dead");
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
    
    // NOTE : this code is without using the `EntityCommandBuffer`
    // second, `WithEntityAccess` is used to get the entity reference from the query of the component, 
    // like in this case we are using `RefRO<Health>` to get the entity reference
    // third, to destroy the entity, we need to use `state.EntityManager.DestroyEntity(entity)` to destroy the entity
    // but if we use `state.EntityManager.DestroyEntity(entity)` inside the `OnUpdate` method, it will throw an error
    // InvalidOperationException: System.InvalidOperationException: Structural changes are not allowed while iterating over entities. Please use EntityCommandBuffer instead.
    // [BurstCompile]
    // public void OnUpdate(ref SystemState state)
    // {
    //     foreach ((RefRO<Health> health, Entity entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
    //     {
    //         if (health.ValueRO.healthAmount <= 0)
    //         {
    //             UnityEngine.Debug.Log($"Entity {entity} is dead");
    //             state.EntityManager.DestroyEntity(entity);
    //         }
    //     }   
    // }
}
