# Roll-a-Ball Expanded: Time Attack Survival

**Developer:** Manav Kheni  
**Course:** CSYE 7270 - [Insert Full Course Name, e.g., Building Game Engines]  
**Assignment:** Assignment 2 - Unity Physics & Scripting  
**Engine:** Unity 6 (LTS)

---

## 📖 Project Overview

This project is an advanced iteration of the standard Unity "Roll-a-Ball" tutorial. While the core mechanic involves collecting objects, the gameplay has been transformed into a **Time Attack Survival** experience. 

Instead of passively collecting items, the player must race against the clock to set a high score while evading an intelligent AI pursuer and navigating dynamic environmental hazards.

### 🎮 Key Features
This project implements **5 significant enhancements** beyond the base tutorial:

1.  **Advanced Mobility (Jump & Dash):** * Implemented `Raycast` ground detection to allow jumping only when grounded.
    * Added a physics-based **Dash** mechanic (using `ForceMode.VelocityChange`) for sudden bursts of speed.
2.  **Enemy AI (NavMesh Pursuer):** * An enemy agent that uses Unity's **NavMesh** system to intelligently pathfind and hunt the player through the arena.
3.  **Dynamic Hazards:** * A **Kinematic Rotating Sweeper** arm that patrols the map, knocking the player back upon contact using physics interaction.
4.  **Data Persistence (High Score):** * A local save system using `PlayerPrefs` that remembers the player's **Best Time** even after the game is closed.
5.  **Game Feel & Polish:** * **Audio Manager:** Singleton pattern handling global sound effects (Jump, Collect, Win/Lose).
    * **Visuals:** Particle explosions on pickup collection and a Trail Renderer for speed feedback.

---

## 🕹️ Controls

| Action | Key / Input |
| :--- | :--- |
| **Move** | `W`, `A`, `S`, `D` or Arrow Keys |
| **Jump** | `Spacebar` |
| **Dash** | `Left Shift` or `E` |
| **Restart** | (Automatic on Win/Lose) |

---

## 🛠️ Installation & How to Run

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/Roll-a-Ball-Expanded.git](https://github.com/YOUR_USERNAME/Roll-a-Ball-Expanded.git)
    ```
2.  **Open in Unity:**
    * Launch **Unity Hub**.
    * Click **Add** and select the cloned folder.
    * Open the project (Recommended Version: Unity 6 or 2022 LTS).
3.  **Play:**
    * Open the scene: `Assets/Scenes/MiniGame.unity`.
    * Press the ▶️ **Play** button at the top.

---

## 📂 Project Structure

* `Assets/Scripts/`: Contains all C# source code (`PlayerController.cs`, `EnemyAI.cs`, `SweeperHazard.cs`, etc.).
* `Assets/Prefabs/`: Reusable game objects (Pickups, Particles, Enemies).
* `Assets/Materials/`: Custom physics materials and textures.

---

## 🏆 Credits & Attribution

* **Development:** Manav Kheni
* **3D Environment Assets:** [KayKit Forest Remastered](https://kaylousberg.itch.io/kaykit-forest) by Kay Lousberg (CC0).
* **Audio SFX:** [Insert Source, e.g., Kenney Assets] (CC0).
* **Core Logic:** Adapted from Unity Learn "Roll-a-Ball" tutorial.

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.
