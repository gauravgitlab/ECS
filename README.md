# ECS Tutorial

Lesson - 01, Add ECS Packages
==
- (Branch - https://github.com/gauravgitlab/ECS/tree/01_add_ecs_packages)
- (PR - https://github.com/gauravgitlab/ECS/pull/1)
- Add givn Packages from Package Manager    
    - Entities
    - Entities Graphics
    - Unity Physics

- In the Project settings --> Editor, we changed the
we also set the "Enter Play Mode Settings" to - Do not reload Domain or Scene

![Screenshot 2025-04-29 220019](https://github.com/user-attachments/assets/572e24a5-c29b-4294-9f71-28d97f88aa60)

Lesson - 02, Import Assets into Games
==================================================================
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/02_add_basic_assets)
- (PR - https://github.com/gauravgitlab/ECS/pull/2)
- Add or Import Assets like Soldier and Zombies Meshes and Textures

![Screenshot 2025-05-01 064420](https://github.com/user-attachments/assets/fde816cf-5d8b-4dc1-a916-02d39088b9cc)

- Create a new Scene (GamaScene.scene) and add the ground, add the Player and Zombie

- Removed assets which comes default with unity Projects
<img width="354" alt="image" src="https://github.com/user-attachments/assets/67968259-4403-41cf-9b9d-f5b3cb1e0713" />

Lesson - 03, Post Processing and Lighting settings
==================================================================
- (Branch - https://github.com/gauravgitlab/ECS/tree/features/03_post_processing_lighting)
- (PR - https://github.com/gauravgitlab/ECS/pull/3)
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

Lesson - 04, Subscene Baking
==================================================================
- Learn basic understanding of Baking
- change Entities settings in Preference
    -- Set `Scene View Mode` to `RunTime Data`
<img width="520" alt="image" src="https://github.com/user-attachments/assets/64199b6d-5f22-4675-9c6f-8ebc102ec11c" />
