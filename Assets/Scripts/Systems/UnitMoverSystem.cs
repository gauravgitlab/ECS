using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct UnitMoverSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }
    
    /// <summary>
    /// NOTE
    /// first, SystemAPI.Query is a new API that allows you to query entities in the world.
    /// its only inside the ISystem and can't be access inside other gameObject classes like MonoBehaviour.
    /// second, MouseWorldPosition is a singleton class that allows you to get the mouse position in the world.
    /// and using this class, we are learning how can we access the gameObject class inside the ECS system.
    /// </summary>
    /// <param name="state"></param>

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<UnitMover> unitMover, RefRW<PhysicsVelocity> physicsVelocity) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
        {
            float3 targetPosition = unitMover.ValueRO.targetPosition;
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            float rotationSpeed = SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed;
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, 
                quaternion.LookRotation(moveDirection, math.up()), rotationSpeed);
            physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
