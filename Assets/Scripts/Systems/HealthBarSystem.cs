using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // we are using camera forward to set the health bar rotation always towards the camera
        Vector3 cameraForward = Vector3.zero;
        if(Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }
        
        foreach ((RefRW<LocalTransform> localTransform, RefRO<HealthBar> healthBar) in 
                 SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
        {
            // set bar rotation towards the camera
            LocalTransform parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
            localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            
            // get health bar value
            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);
            float healthNormalized = (float)health.healthAmount / health.healthAmountMax;

            localTransform.ValueRW.Scale = healthNormalized == 1f ? 0f : 1f;
            
            // NOTE : we cant use `RefRW<LocalTransform>` to set the scale, because scale is float instead of float3, which will set the scale uniformly in all directions.
            // in the health bar, we want to set only in x direction to set the bar
            //RefRW<LocalTransform> barVisualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barVisualEntity);
            //barVisualLocalTransform.ValueRW.Scale = healthNormalized;

            // we are using `PostTransformMatrix` to set the scale of the health bar
            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1f, 1f);
        }    
    }
}
