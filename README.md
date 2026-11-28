# AIsemble

**AIsemble** is a 2D sliding puzzle game built with Unity. The game challenges players to arrange numbered puzzle pieces into their correct positions by moving tiles into an empty space.

## 🎮 Game Overview

AIsemble features a classic 4×4 sliding puzzle mechanic. Players must rearrange shuffled pieces and restore the original arrangement to complete the puzzle.

## ✨ Features

- **4×4 Sliding Puzzle:** A board containing 15 movable pieces and one empty space.
- **Puzzle Shuffling:** Randomized puzzle arrangements using valid tile movements.
- **Smooth Movement:** Animated piece movement between adjacent positions.
- **Victory Detection:** Automatically detects when all pieces return to their original positions.
- **Game UI:** Includes start instructions, completion feedback, and a Coming Soon panel.
- **Pause & Exit Menu:** Provides an exit confirmation interface.
- **Main Menu:** Includes navigation, About section, and loading interface.
- **Mobile Orientation:** Supports landscape orientation.

## 🕹️ Gameplay

1. Launch the game and enter the puzzle scene.
2. Tap or click the screen to start.
3. The puzzle pieces will be shuffled.
4. Select a piece adjacent to the empty space to move it.
5. Continue rearranging the pieces until the original pattern is restored.
6. Complete the puzzle to trigger the victory screen.

## 🎯 Controls

| Action | Control |
|---|---|
| Start puzzle | Tap / Click |
| Move a piece | Tap / Click an adjacent piece |
| Restart after completion | Tap / Click |
| Open exit menu | Escape key |

## 🛠️ Built With

- **Game Engine:** Unity
- **Unity Version:** 2022.3.39f1
- **Programming Language:** C#
- **UI:** Unity UI
- **Physics:** Unity 2D Physics

## 📂 Project Structure

```text
AIsemble/
├── Assets/
│   ├── Scenes/
│   │   ├── GameScene.unity
│   │   └── MenuScene.unity
│   ├── Scripts/
│   │   ├── Constants.cs
│   │   ├── Game.cs
│   │   ├── GameManager.cs
│   │   ├── Piece.cs
│   │   ├── ScreenOrientationSetter.cs
│   │   └── MainMenu/
│   │       └── MainMenuManager.cs
│   └── ...
└── README.md
```

## ⚙️ Getting Started

### Prerequisites

- Unity Hub
- Unity Editor 2022.3.39f1 or compatible version
- Basic knowledge of Unity

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Ristwak/AIsemble.git
   ```

2. Open Unity Hub.
3. Select **Add Project from Disk**.
4. Navigate to the cloned AIsemble folder.
5. Open the project using Unity 2022.3.39f1.
6. Open `MenuScene.unity` from the Scenes folder.
7. Press Play to run the game.

## 📜 License & Attribution

This project is distributed under the MIT License.

The original project credits and copyright notices are retained. This project is based on the original work credited in the repository README, including Dimitris-Ilias Gkanatsios and the PuzzleGameUnity project.

Please refer to the [LICENSE](LICENSE) file for complete license terms.

---

**Developed with Unity and C#.**
