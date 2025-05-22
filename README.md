# ECS Tutorial

## Lesson - 01, Add ECS Packages
- (Branch - https://github.com/gauravgitlab/ECS/tree/01_add_ecs_packages)
- (PR - https://github.com/gauravgitlab/ECS/pull/1)
### Description
- Add givn Packages from Package Manager    
    - Entities
    - Entities Graphics
    - Unity Physics

- In the Project settings --> Editor, we changed the
we also set the "Enter Play Mode Settings" to - Do not reload Domain or Scene

![Screenshot 2025-04-29 220019](https://github.com/user-attachments/assets/572e24a5-c29b-4294-9f71-28d97f88aa60)

## Lesson - 02, Import Assets into Games
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/02_add_basic_assets)
- (PR - https://github.com/gauravgitlab/ECS/pull/2)
### Description
- Add or Import Assets like Soldier and Zombies Meshes and Textures

![Screenshot 2025-05-01 064420](https://github.com/user-attachments/assets/fde816cf-5d8b-4dc1-a916-02d39088b9cc)

- Create a new Scene (GamaScene.scene) and add the ground, add the Player and Zombie
- Removed assets which comes default with unity Projects

<img width="354" alt="image" src="https://github.com/user-attachments/assets/67968259-4403-41cf-9b9d-f5b3cb1e0713" />

## Lesson - 03, Post Processing and Lighting settings
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/03_post_processing_lighting)
- (PR - https://github.com/gauravgitlab/ECS/pull/3)
### Description
- The changes are mostly setup the post processing and lighting setting in scene
- There were `Global Volume` Gameobject in the scene, we rename the volume Profile object to GameSceneProfile, and remove component like Motion Blur and ToneMapping.
- Then we go to Windowns --> Rendering --> Ligting, and click to Environment tab .
    - we change the source of Environment Lighting --> Color and use color value (222,222,222)
- select the `PC_Renderer` from the `Settings` folder in Project, 
    - then select the `Screen Space Ambient Occlusion` from it.
    - Set `Interleaved Gradient` from the Method.
    - Set Intensity to 10
    - Set Radius to 0.6
    - set Direct Lighting Streangth to 1

## Lesson - 04, Subscene Baking
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/04_dots_subscene_baking)
- (PR - https://github.com/gauravgitlab/ECS/pull/4)
- Learn basic understanding of Baking
- change Entities settings in Preference
    -- Set `Scene View Mode` to `RunTime Data`

<img width="520" alt="image" src="https://github.com/user-attachments/assets/64199b6d-5f22-4675-9c6f-8ebc102ec11c" />

## Lesson - 05, Create component and Unit Setup
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/05_create_component_and_unit_setup)
- (PR - https://github.com/gauravgitlab/ECS/pull/5)
### Description
- Add Unit with Component and System with following steps
- Add the new empty gameobject in Entity SubScene.
- Name it Unit, This gameobject is only holding data component.
- create another empty gameobject as a child inside Unit gameobject and named it as Mesh
- Create new Monobehavior class, called MoveSpeedAuthoring
- Attach this script with Unit gameobject
- Create new Script inherit from ISytem.

![image](https://github.com/user-attachments/assets/09441a7a-ad71-43b4-b292-afebfc6eb51c)

## Lesson - 06, Move Unit using Physics and Mouse World Position
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/06_unit_move_physics)
- (PR - https://github.com/gauravgitlab/ECS/pull/6)
### Description
- Move Unit using Physics
- In `UnitMoverSystem.cs`, we use `PhysicsVelocity` to move the unit linearly.
- we applied the `capsule collider` and `Rigidbody` to the unit.

![image](https://github.com/user-attachments/assets/ad264990-f898-4867-85b7-bcb9c3a8b0b6)
![image](https://github.com/user-attachments/assets/047a7f18-e765-455d-9c5b-b7096e4b40bf)


Note - the ECS physics works completely different to Normal Gameobject Physics, so it won't interact with each other.
  
### Lesson - 6.1, Mouse World Position
- we add the Monobehavoiur scrpit `MouseWorldPosition.cs` and make it singleton which returns the Mouse Position
- Create Empty gameobject and attach the scrpit `MouseWorldPosition.cs`
- Use the MouseWorldPosition in `UnitMoveSystem.cs` to move and rotate the unit.

![image](https://github.com/user-attachments/assets/31e07fb5-98e3-4cb9-85b4-12223f300e07)


## Lesson - 7, Move Unit baesd on Mouse click position
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/07_click_to_move)
- (PR - https://github.com/gauravgitlab/ECS/pull/7)
### Description
- first rename `MoveSpeedAuthoring` class to `UnitMoverAuthoring`
- Created the new script UnitSelectionManager.cs which run on update method if user mouse button clicked or not. If clicked we run the query for getting all the units containing UnitMover Component to get , and set the target position based on mouse button click position.
- In the given screenshots, we temporary created few Units which are moving towards the mouse clicked position.

![image](https://github.com/user-attachments/assets/00ac339a-493c-4bc7-85eb-55f7e83405ba)

## Lesson - 8, Move Job system
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/08_move_job_system)
- (PR - https://github.com/gauravgitlab/ECS/pull/8)
### Description
- we are create a job to move the unit instead of using System update.
- Job is useful to perform better and efficient.
- for demo purpose we created 10,000 units and move the units using `System` (which is running on main thread), using `job` (which is running on worker thread) and using `Job + Burst`
- we got the following output
  - System = 9.5ms,
  - Job = 0.290ms,
  - Job+Burst = 0.035ms
    
![image](https://github.com/user-attachments/assets/86fecc5d-7576-4410-9d7b-46fc0888e919)
![image](https://github.com/user-attachments/assets/17950972-8dfa-44a3-a821-d68009d85bd1)


## Lesson - 9, Unit select Single
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/09_unit_selection_single)
- (PR - https://github.com/gauravgitlab/ECS/pull/9)
### Description
- We added the Unit as Prefab.
- We also added selected child gameobject inside Unit prefab, This selected gameobject have Mesh Renderer GameObject.
- We added the SelectedAuthoring component to Unit Gameobject.
### NOTE
- Dots does not support Sprint Rendering, so for showing the selected unit, we are adding a `Quad` gameobject with mesh attached to it.
- Dots are adding the `Companion Link` and `Companion Reference` components to those which entity which are not able to baked by dots.
- In this case `Selected Gameobject` with `Sprite Renderer` attached to it.
- so we are removing the `SpriteRenderer` component from `Selected` gameobject and attached the `Mesh Renderer` to it.
### Screenshots
A Unit Gameobject with Selected Component
If the Component does not have any properties, it will come inside Tags
![image](https://github.com/user-attachments/assets/70273c74-ce30-40cd-8758-e2859815abd0)

If Selected Gameobject contains SpriteRenderer component, Dots added Companion Link and Companion Reference components to entities.
That's why we are not using SpriteRenderer but MeshRenderer
![image](https://github.com/user-attachments/assets/a8208a0e-8899-4446-b689-e6eabecdc4b7)
![image](https://github.com/user-attachments/assets/2750ef45-a0cc-4b1b-9b1a-4eba2041de7c)
![image](https://github.com/user-attachments/assets/87a87ede-8d32-432e-889e-1a3d5e16ddc4)


## Lesson - 10, Unit Selection Multiple, UI Setup
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/10_unit_selection_mutiple)
- (PR - https://github.com/gauravgitlab/ECS/pull/10)
### Description
- we are adding the UI for represting the `selection area`,
- we are selecting the `Unit` for moving on `target` position by Input Mouse position button 0.

![image](https://github.com/user-attachments/assets/2737194d-1089-4472-977c-92660a716869)


## Lesson - 11, DOTS Physics, Raycast
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/11_dots_physics_raycast)
- (PR - https://github.com/gauravgitlab/ECS/pull/11)
### Description
- Add Layer into `Unit` Prefab
- Use DOTS Physics, raycasting to get the `Entity` when cast a ray from `Camera`.

![image](https://github.com/user-attachments/assets/b4dccb00-cc52-4bd9-a910-2e7e216481f4)

## Lesson - 12, Generate more position for Units
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/12_generate_move_positions)
- (PR - https://github.com/gauravgitlab/ECS/pull/12)
### Description
- Remove `isTrigger` from Unit prefab.
- Write a method to generate more position based on creating position on clicked mouse position and its around ring areas.

![image](https://github.com/user-attachments/assets/a96c9a26-4c62-4157-9e5f-009db9d42dc4)

## Lesson - 13, Added the event for selecting and deselecting unit 
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/13_unit_events)
- (PR - https://github.com/gauravgitlab/ECS/pull/13)
### Description
- added the `event` for when we `select` and `deselect` the units
- Use `UpdateBefore` and `UpdateInGroup` to update the system execution order.

![image](https://github.com/user-attachments/assets/400077b1-3874-4b71-8fc4-7ad2e8223712)

## Lesson - 14, Add Enemy, and assign Find Target and Shoot Target Systems
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/14_enemies_factions)
- (PR - https://github.com/gauravgitlab/ECS/pull/14)
### Description
- Create Base Unit prefab, and create variant prefab for Soldier and Zombie
- attaching the FindTarget Component to `Soldier` and `Zombie`
- attaching the Target component to `Soldier` and `Zombie`. This component is basically to store the target when we trying to find it.
- Add the faction for both `Soldier` and `Zombie`
- Create the `FindTargetSystem` and `ShootAttackSystem`
  
![image](https://github.com/user-attachments/assets/e5b25862-49a7-44f7-9baf-6049f80f399b)

- added components in `Soldier`
![image](https://github.com/user-attachments/assets/fff9ca7f-d0f3-4412-bcb0-59204a605488)

- added components to Zombie. Zombie does not have shoot attack because it will do Melee
![image](https://github.com/user-attachments/assets/c4eade7d-a205-495d-ae2b-fe9979e94656)

## Lesson - 15, Health
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/15_health)
- (PR - https://github.com/gauravgitlab/ECS/pull/15)
### Description
- add the `heath` Component in `Base Unit` Prefab
- reducing the health of the target entity, in ShootAttackSystem
- Write `HealthDeadTestSystem` to destroy the entity when its health reduced to 0, using EntityCommandBuffer
- Write `ResetTargetSystem` to make the entity Null, when its destroyed.
- we need to attach Linked Entity Group Authoring Component to BaseUnit prefab, because when we destroy the Entity it were just destroy the entity, but not the mesh in child component.

![image](https://github.com/user-attachments/assets/8e2e5fbf-924e-4bc9-a203-543b64e11bbe)

## Lesson - 15, Bullet
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/16_bullet)
- (PR - https://github.com/gauravgitlab/ECS/pull/16)
### Description
- add the bullet authoring script for getting speed and damage
- add `bulletMover` system to move the bullet towards the target, damage the target and destroy the bullet after hit
- Add the `EntitiesReferencesAuthoring` which would be responsible for containing gameobject prefab which later to use Instantiating as Entity.
- Spawn the bullet, when Zombie comes into attack range of Soldier
### NOTE :
- when the component is not attached to any Entity, we are going to use `SystemAPI.GetSingleton`.
  - like in our case, we want to use the `EntitiesReferences` component, we need to access using `SystemAPI.GetSingleton<T>()`,
  - `EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();`
- If we want to Instantiate the Entity, we will use `state.EntityManager.Instantiate`
  - `Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);`  

![image](https://github.com/user-attachments/assets/1e0ffe5c-dc42-4737-be2b-381742c95b95)
