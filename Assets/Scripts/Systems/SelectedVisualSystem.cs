using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateBefore(typeof(ResetEventsSystem))]
public partial struct SelectedVisualSystem : ISystem
{
    /// <summary>
    /// NOTE:
    /// onSelected and onDeselected are the events that are triggered when the unit is selected or deselected.
    /// these are properties but used as events.
    /// because we added another component called `ResetEventsSystem` which will reset the events every end of frame.
    /// that's why we use `UpdateBefore(typeof(ResetEventsSystem))` to make sure that this system will run before the `ResetEventsSystem`.
    /// </summary>
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
        {
            if (selected.ValueRO.onSelected)
            {
                //UnityEngine.Debug.Log($"Selected entity: {selected.ValueRO.visualEntity}");
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
            }

            if (selected.ValueRO.onDeselected)
            {
                //UnityEngine.Debug.Log($"Deselected entity: {selected.ValueRO.visualEntity}");
                RefRW<LocalTransform> visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = 0;
            }
        }
    }
}