# Endless Pain — Unity Car Endless Runner

A fast, stylized endless driving game built with Unity. You control a car dodging traffic on an infinite road while the world streams in front of you. Simple controls, clean look (toon-like shading), and a quick reset loop.

## Overview
- Objective: drive as far as possible without crashing.
- Core loop: auto-accelerate forward, steer between lanes, avoid AI cars, survive to increase your distance score.
- Game feel: light, readable visuals with toon shading; quick restarts for a snappy arcade feel.

## Gameplay
- Endless level streaming in front of the player.
- AI cars spawn ahead on random lanes and speeds.
- Colliding hard ends the run and shows a Game Over overlay with your distance.
- From the main menu you can preview/select a player car before starting the stage.

## Controls
- Movement: `A`/`D` or `Left`/`Right` to steer between lanes.
- Throttle/Brake: `W`/`S` or `Up`/`Down` to add boost or brake (auto-accelerate is always active unless braking).
- Restart (in-game): `R` to reload the current scene.
- Quit: `Esc`.

## Scenes
- `Assets/Scenes/Main menu.unity`
  - Car selection preview (rotating car), start game button.
- `Assets/Scenes/Stage.unity`
  - Main gameplay scene: endless sections, AI traffic, HUD (distance), Game Over UI.

## Systems & Architecture
- Player Car
  - Spawning: `PlayerCarSpawner` instantiates the selected prefab. In menu, it creates a non-interactive rotating preview; in game, it spawns a controllable car with the required components.
  - Control & Physics: `CarHandler` reads input (via `InputHandler` on the Player) and applies forces:
    - Auto-acceleration up to a max speed.
    - Optional manual boost (vertical > 0) and brake (vertical < 0).
    - Lateral steering (no yaw rotation to keep the endless-runner feel).
  - Audio: engine and skid sounds adjust to speed/braking.

- Endless World
  - `EndlessLevelHandler` manages a pool of road/environment sections and repositions them ahead as the player advances.
  - Configurable `sectionLength` and pool sizes.

- AI Traffic
  - `AICarSpawner` pools AI car prefabs and activates them ahead in random lanes, with anti-overlap checks.
  - `AIHandler` provides simple lane following and speed variation, and disables itself on the Player.

- Collisions & Game Over
  - `CarCollisionHandler` detects heavy impacts (speed threshold and approach filter) and invokes an event.
  - `DeathController` listens for heavy impacts and triggers Game Over immediately (configurable). It freezes time, disables player control, and shows the Game Over overlay, also passing the final distance.

- UI
  - HUD Distance: `UIHandler` shows `DistanceTraveled` from `CarHandler` (zero-padded). `ProgressCounter` is also available for text-based meters if needed.
  - Game Over: `GameOverUI` overlay with Restart/Quit buttons; auto-stretches to full screen.
  - Optional post-process overlay: `GameOverEffectToggle` can toggle a URP Renderer Feature when the overlay appears.

- Camera
  - Uses Cinemachine (`CinemachineCamera`) following the player (assigned by `PlayerCarSpawner`).

- Shaders (Visual Style)
  - Toon: `Assets/Materials/Toonshader.shader` with `ToonMaterial.mat`, used by the Police car prefab variant.
  - B/W Toon variant: `Assets/Shaders/ToonshaderBW.shader` (shader name `Unlit/ToonShaderBW`), with `_Threshold`, `_Feather`, `_Brightness` to control a soft two-tone look.

## Project Structure (selected)
- `Assets/Scenes/` — Main menu and Stage scenes.
- `Assets/Prefabs/` — Player/AI cars, road sections, UI.
- `Assets/Scripts/` — Car, AI, Endless world, UI, Utils.
- `Assets/Materials/` — Materials including toon variants.
- `Assets/Shaders/` — Custom shaders (toon, overlays).

## How To Run (Editor)
1. Open the project in Unity (URP-capable version recommended; 2021.3+ or newer).
2. Open `Assets/Scenes/Main menu.unity` and press Play.
3. Use the UI to pick a car and Start. The game loads `Stage` and starts the run.

## Build Instructions
1. Open Build Settings and add both scenes:
   - Main menu
   - Stage
2. Set Main menu as the first scene in Build (index 0).
3. Build for your target platform.

## Tuning & Configuration
- Player
  - `CarHandler`: `autoAcceleration`, `maxAutoSpeed`, `acceleration`, `brakeForce`, `steerForce`.
  - `InputHandler`: attached only on the Player (auto-destroyed on AI).
- Endless
  - `EndlessLevelHandler`: assign `sectionPrefabs`, tweak `sectionLength`.
- AI
  - `AICarSpawner`: assign `carAIPrefabs`, adjust pool size, layer masks, spawn timing.
  - `AIHandler`: lane logic and detection layer masks.
- Death/Game Over
  - `CarCollisionHandler`: `impactSpeedThreshold`, `requireApproachAlongNormal`.
  - `DeathController`: `dieOnAnyHeavyImpact` (immediate), `dropFractionThreshold`, `stopSpeedThreshold`, UI reference.
- Shaders
  - `ToonMaterial` (Police): color banding via `_Detail`, strength and brightness.
  - `ToonShaderBW`: `_Threshold` (cut), `_Feather` (softness), `_Brightness` (lift blacks).

## Screenshots
- Main Menu preview
  - [PLACEHOLDER: add image here]
- In-Game (HUD + traffic)
  - [PLACEHOLDER: add image here]
- Game Over overlay
  - [PLACEHOLDER: add image here]

## Demo Video
- Watch a short gameplay demo:
  - [PLACEHOLDER: paste video link here]

## Credits / Assets
- Environment and vehicles use assets under `Assets/Models/KayKit_City_Builder_Bits_1.0_FREE` by KayKit. Please ensure proper attribution per their license in your final release.
- Fonts/UI: TextMesh Pro.
- Camera: Cinemachine.

## Troubleshooting
- Player not moving: ensure `PlayerCarSpawner` is present and spawns a car with tag `Player`, and that `CarHandler` and `InputHandler` are attached.
- No Game Over UI: verify the Game Over Screen prefab exists in the scene, or let `DeathController` auto-resolve it by name (`Game Over Screen`).
- Very dark visuals: increase scene light intensity or raise `_Brightness` in toon shaders.

