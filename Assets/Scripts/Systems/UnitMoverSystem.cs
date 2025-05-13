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
        UnitMoverJob unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        unitMoverJob.ScheduleParallel();

        // foreach ((RefRW<LocalTransform> localTransform, RefRO<UnitMover> unitMover, RefRW<PhysicsVelocity> physicsVelocity) in 
        //          SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>())
        // {
        //     float3 targetPosition = unitMover.ValueRO.targetPosition;
        //     float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
        //     moveDirection = math.normalize(moveDirection);
        //
        //     float rotationSpeed = SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed;
        //     localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, 
        //         quaternion.LookRotation(moveDirection, math.up()), rotationSpeed);
        //     physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
        //     physicsVelocity.ValueRW.Angular = float3.zero;
        // }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}

/// <summary>
/// we are using UnitMoverJob to move the unit in the world.
/// instead of using `UnitMoverSystem`, we can use `UnitMoverJob` to move the unit.
/// UnitMoverSystem is a system that runs on the main thread.
/// UnitMoverJob is a job that runs on the worker thread. and more efficient, and faster.
/// </summary>
[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float deltaTime;
    
    /// <summary>
    /// ref is similar like RefRW, but DOTS recommends to use ref instead of RefRW.
    /// in is similar like RefRO, but DOTS recommends to use in instead of RefRO.
    /// </summary>
    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        if(unitMover.targetPosition.Equals(float3.zero))
        {
            return;
        }
        
        float3 targetPosition = unitMover.targetPosition;
        float3 moveDirection = targetPosition - localTransform.Position;
        
        // stop the unit, when moves near to target position.
        const float reachedTargetDistanceSq = 2.0f;
        if (math.lengthsq(moveDirection) < reachedTargetDistanceSq)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        
        moveDirection = math.normalize(moveDirection);

        float rotationSpeed = deltaTime * unitMover.rotationSpeed;
        localTransform.Rotation = math.slerp(localTransform.Rotation, 
            quaternion.LookRotation(moveDirection, math.up()), rotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
