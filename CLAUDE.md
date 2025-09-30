# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity game project titled "PlayFabTest" that uses Unity 6000.1.5f1 (Unity 6). The project is configured for mobile development using the Universal Render Pipeline (URP).

## Project Structure

- **Client/** - Unity project root directory
  - **Assets/** - Unity assets and scripts
    - **Scenes/** - Contains Main.unity scene
    - **Settings/** - URP render pipeline settings (separate profiles for mobile and PC)
    - **Editor/** - Editor-only scripts
    - **TutorialInfo/** - Unity template tutorial files
  - **ProjectSettings/** - Unity project configuration files
  - **Packages/** - Unity package dependencies (managed via manifest.json)
  - **Library/** - Unity-generated files (excluded from git)

## Key Technologies

- **Unity Version**: 6000.1.5f1
- **Rendering**: Universal Render Pipeline (URP) 17.1.0
- **Input System**: New Unity Input System 1.14.0
- **Platform Target**: Mobile-first (with PC support via separate render settings)
- **Notable Packages**:
  - Unity Timeline 1.8.7
  - Unity Input System 1.14.0
  - Visual Scripting 1.9.7

## Development Commands

### Opening the Project
Open the `Client` folder in Unity Hub or Unity Editor. The project uses Unity 6000.1.5f1.

### Building the Project
Use Unity Editor's Build Settings (File > Build Settings) to build for target platforms.

### Input System
The project uses the new Unity Input System. Input actions are defined in `Assets/InputSystem_Actions.inputactions`.

## Architecture Notes

### Render Pipeline Configuration
- The project has separate URP renderer and pipeline assets for Mobile and PC targets
- Mobile settings: `Assets/Settings/Mobile_RPAsset.asset` and `Mobile_Renderer.asset`
- PC settings: `Assets/Settings/PC_RPAsset.asset` and `PC_Renderer.asset`
- Global URP settings: `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset`

### Scene Setup
- Main scene: `Assets/Scenes/Main.unity`
- Scene builds are configured in `ProjectSettings/EditorBuildSettings.asset`

### Company and Product Info
- Company: DefaultCompany
- Product Name: PlayFabTest

## Working with Unity Projects

- Unity generates code in .csproj files (Assembly-CSharp.csproj, Assembly-CSharp-Editor.csproj)
- The Library/ folder contains Unity's build cache and should not be modified directly
- All custom scripts should be placed in appropriate subdirectories under Assets/
- Editor scripts must be in an "Editor" folder to be excluded from builds