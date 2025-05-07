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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<LocalTransform> localTransform, RefRO<MoveSpeed> moveSpeed, RefRW<PhysicsVelocity> physicsVelocity) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeed>, RefRW<PhysicsVelocity>>())
        {
            float3 targetPosition = MouseWorldPostion.Instance.GetPosition();
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            float rotationSpeed = SystemAPI.Time.DeltaTime * 10.0f;
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, 
                quaternion.LookRotation(moveDirection, math.up()), rotationSpeed);
            physicsVelocity.ValueRW.Linear = moveDirection * moveSpeed.ValueRO.value;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
