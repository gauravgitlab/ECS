using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct TestingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        int unitCount = 0;
        
        // NOTE: Query will fetch the entities that have component attached and enabled
        // like for example : using `RefRO<selected>` will only fetch the entities that have `Selected` component and its enabled
        // if the unit have `Selected` component but its disabled, it will not be fetched.
        // SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>(), RefRO<Selected>()
        
        // secondly, if we want to get the entities that have `Selected` component and its disabled, we can use `WithDisabled<Selected>()`
        // SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>().WithDisabled<Selected>()
        
        // thirdly, if we want to get the entities that have `Selected` component and does not matter if its enabled or disabled, we can use `WithPresent<Selected>()`
        // SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>().WithPresent<Selected>()
        
        // foreach ((RefRW<LocalTransform> localTransform, RefRO<UnitMover> unitMover, RefRW<PhysicsVelocity> physicsVelocity) in 
        //          SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>().WithPresent<Selected>())
        // {
        //     unitCount++;
        // }
        //
        // Debug.Log($"Total unit count = {unitCount}");
    }
}