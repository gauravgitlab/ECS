using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<RandomWalking> randomWalking, RefRO<LocalTransform> localTransform, RefRW<UnitMover> unitMover) in 
                 SystemAPI.Query<RefRW<RandomWalking>, RefRO<LocalTransform>, RefRW<UnitMover>>())
        {
            if(math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition) < UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
            {
                //reached random target, now Generate a new random target position within a certain range
                Random random = randomWalking.ValueRO.random;
                float3 randomDirection = new float3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                randomDirection = math.normalize(randomDirection);
                
                randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + 
                    randomDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
                randomWalking.ValueRW.random = random; // update the random state
            }
            else
            {
                // too far, move closer
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
            }
        }
    }
}
