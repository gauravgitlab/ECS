using UnityEngine;
using Unity.Entities;

public class SelectedAuthoring : MonoBehaviour
{
    public GameObject visualGameObject;
    public float showScale = 2.0f;
    
    public class Baker : Baker<SelectedAuthoring>
    {
        public override void Bake(SelectedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Selected
            {
                visualEntity = GetEntity(authoring.visualGameObject, TransformUsageFlags.Dynamic),
                showScale = authoring.showScale
            });
            SetComponentEnabled<Selected>(entity, false);   // Disable the component by default
        }
    }
}

// IEnableableComponent is used to provide tick box in the inspector when select the entity with component
public struct Selected : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float showScale;

    public bool onSelected;
    public bool onDeselected;
}
