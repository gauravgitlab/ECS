using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
         
         // get all entities with the `Selected` component and make every entity not selected first
         EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
         EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
         NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
         NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);
         
         for (int i = 0; i < entityArray.Length; i++)
         {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            Selected selected = selectedArray[i];
            selected.onDeselected = true;
            selectedArray[i] = selected;
            entityManager.SetComponentData(entityArray[i], selected);
         }
         
         // get the selection area rect, and check if the selection area is a multiple selection or not, for selecting multiple units or single unit
         Rect selectionAreaRect = GetSelectionAreaRect();
         float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
         float multipleSelectionSizeMin = 40f;
         bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;
         
         // get all entities with the `Unit` component and check if the entity is inside the selection area
         if (isMultipleSelection)
         {
            SelectMultipleEntities(entityManager, selectionAreaRect);
         }
         else
         {
            // single selection unit
            SelectSingleUnit(entityManager);
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
      
      NativeArray<float3> movePositionArray = GenerateMovePositionArray(mousePosition, entityArray.Length);

      for (int i = 0; i < unitMoverArray.Length; i++)
      {
         UnitMover unitMover = unitMoverArray[i];
         unitMover.targetPosition = movePositionArray[i];
         unitMoverArray[i] = unitMover;
      }
      entityQuery.CopyFromComponentDataArray(unitMoverArray);
   }

   /// <summary>
   /// select multiple units using the selection area rect.
   /// </summary>
   private void SelectMultipleEntities(EntityManager entityManager, Rect selectionAreaRect)
   {
      EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).
         WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);

      NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
      NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
      for (int i = 0; i < localTransformArray.Length; i++)
      {
         LocalTransform localTransform = localTransformArray[i];
         Vector2 screenPosition = Camera.main.WorldToScreenPoint(localTransform.Position);
         if (selectionAreaRect.Contains(screenPosition))
         {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
            Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
            selected.onSelected = true;
            entityManager.SetComponentData(entityArray[i], selected);
         }
      }
   }

   /// <summary>
   /// select single unit on click, using DOTS Physics raycast.
   /// if we getting the selected unit or entity, using physics raycast
   /// </summary>
   private static void SelectSingleUnit(EntityManager entityManager)
   {
      var entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
      PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
      CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
      UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
      int unitLayer = 6;
      RaycastInput raycastInput = new RaycastInput
      {
         Start = cameraRay.GetPoint(0f),
         End = cameraRay.GetPoint(9999f),
         Filter = new CollisionFilter
         {
            BelongsTo = ~0u,
            CollidesWith = 1u << unitLayer,
            GroupIndex = 0
         }
      };
      if(collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
      {
         Entity entity = raycastHit.Entity;
         if (entityManager.HasComponent<Unit>(entity))
         {
            // hit a unit
            entityManager.SetComponentEnabled<Selected>(entity, true);
            Selected selected = entityManager.GetComponentData<Selected>(entity);
            selected.onSelected = true;
            entityManager.SetComponentData(entity, selected);
         }
      }
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

   private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
   {
      NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
      if (positionCount == 0)
      {
         return positionArray;
      }

      positionArray[0] = targetPosition;
      if (positionCount == 1)
      {
         return positionArray;
      }

      float ringSize = 2.2f;
      int ring = 0;
      int positionIndex = 1;
      
      while(positionIndex < positionCount)
      {
         int ringPositionCount = 3 + ring * 2;

         for (int i = 0; i < ringPositionCount; i++)
         {
            float angle = i * (math.PI2 / ringPositionCount);
            float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ringSize + 1), 0, 0));
            float3 ringPosition = targetPosition + ringVector;
            
            positionArray[positionIndex] = ringPosition;
            positionIndex++;
            
            if (positionIndex >= positionCount)
               break;
         }
         ring++;
      }

      return positionArray;
   }
}
