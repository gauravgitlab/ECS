using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
   
   /// <summary>
   /// We can not use System.API inside MonoBehaviour class. Its only used inside the ECS system.
   /// that's why, EntityManager and EntityQuery are used to query and update entities in the ECS world.
   /// using that we are getting the system and components data into the MonoBehaviour class.
   /// Note : UnitMover is a struct, that's why change in that struct will not be reflected in the system.
   /// we need to use `SetComponentData` to update the system. first option we can use this two following lines - 
   /// unitMover.targetPosition = mousePosition;
   /// entityManager.SetComponentData(entityArray[i], unitMover);
   /// but this would be a bit slow, because of setComponentData for each UnitMover entity.
   /// we are using the second option, which is `CopyFromComponentDataArray` to update the system.
   /// we just need to make sure the `unitMoverArray` is updated with the value of `unitMover`.
   /// </summary>
   private void Update()
   {
      if (Input.GetMouseButtonDown(1))
      {
         Vector3 mousePosition = MouseWorldPosition.Instance.GetPosition();
         
         EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
         EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).
            WithAll<UnitMover, Selected>().Build(entityManager);
         
         NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
         NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
         for (int i = 0; i < unitMoverArray.Length; i++)
         {
            UnitMover unitMover = unitMoverArray[i];
            unitMover.targetPosition = mousePosition;
            unitMoverArray[i] = unitMover;
         }
         entityQuery.CopyFromComponentDataArray(unitMoverArray);
      }
   }
}
