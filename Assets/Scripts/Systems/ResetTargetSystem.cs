using Unity.Burst;
using Unity.Entities;

/// <summary>
/// We need this `ResetTargetSystem` because when we destroy the entity, we are getting the following error:
/// ArgumentException: The entity does not exist. Entities Journaling may be able to help determine more information. Please enable Entities Journaling for a more helpful error message.
/// because in `ShootAttackSystem`, we are still trying to reduce the health from target entity, which is no longer exist anymore
/// so for the solution, we need to reset the target entity to `Entity.Null` after every health system check
/// that's why we are using `UpdateInGroup(typeof(LateSimulationSystemGroup))` so it will run after `HealthDeadTestSystem`
/// </summary>

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct ResetTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>())
        {
            if (!SystemAPI.Exists(target.ValueRO.targetEntity))
            {
                target.ValueRW.targetEntity = Entity.Null;    
            }
        }
    }
}
