using Unity.Entities;using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject bulletPrefab;
    
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity bulletPrefabEntity;
}
