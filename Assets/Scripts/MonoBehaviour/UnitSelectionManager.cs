using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
   public static UnitSelectionManager Instance { get; private set; }
   
   private Vector2 _selectionStartMousePosition;
   
   public event EventHandler OnSelectionAreaStart;
   public event EventHandler OnSelectionAreaEnd;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }
   }

   private void Update()
   {
      if (Input.GetMouseButtonDown(0))
      {
         _selectionStartMousePosition = Input.mousePosition;
         OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
      }

      if (Input.GetMouseButtonUp(0))
      {
         Vector2 selectionEndMousePosition = Input.mousePosition;
         
         // get all entities with the `Selected` component and make every entity not selected
         EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
         EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
         NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
         for (int i = 0; i < entityArray.Length; i++)
         {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
         }
         
         // get all entities with the `Unit` component and check if the entity is inside the selection area
         entityQuery = new EntityQueryBuilder(Allocator.Temp).
            WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
         
         Rect selectionAreaRect = GetSelectionAreaRect();
         
         entityArray = entityQuery.ToEntityArray(Allocator.Temp);
         NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
         for (int i = 0; i < localTransformArray.Length; i++)
         {
            LocalTransform localTransform = localTransformArray[i];
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
            if (selectionAreaRect.Contains(screenPosition))
            {
               entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
            }
         }
         
         OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
      }
      
      if (Input.GetMouseButtonDown(1))
      {
         SetUnitsTargetPosition();
      }
   }

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
   private void SetUnitsTargetPosition()
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

   public Rect GetSelectionAreaRect()
   {
      var selectionEndMousePosition = Input.mousePosition;
      var lowerLeftCorner = new Vector2(
         Mathf.Min(_selectionStartMousePosition.x, selectionEndMousePosition.x),
         Mathf.Min(_selectionStartMousePosition.y, selectionEndMousePosition.y));
      
      var upperRightCorner = new Vector2(
         Mathf.Max(_selectionStartMousePosition.x, selectionEndMousePosition.x),
         Mathf.Max(_selectionStartMousePosition.y, selectionEndMousePosition.y));

      return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, 
         upperRightCorner.x - lowerLeftCorner.x, 
         upperRightCorner.y - lowerLeftCorner.y);
   }
}
