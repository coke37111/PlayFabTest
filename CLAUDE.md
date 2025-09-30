# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity game project titled "PlayFabTest" - a standalone test project for integrating and testing PlayFab SDK and Firebase functionality. Built with Unity 6000.1.5f1 (Unity 6), it's configured for mobile development using the Universal Render Pipeline (URP).

**Primary Goal**: Test and implement social login-based account linking between PlayFab and Firebase, where Firebase Guest ID is synchronized with PlayFab through social authentication (Google, etc.).

## Project Structure

- **Client/** - Unity project root directory
  - **Assets/** - Unity assets and scripts
    - **Scripts/** - Custom game scripts
      - `PlayFabLogin.cs` - Main authentication service with Google and Guest login support
    - **PlayFabSDK/** - Complete PlayFab SDK with all API modules (Client, Server, Admin, Authentication, Economy, etc.)
    - **PlayFabEditorExtensions/** - Unity Editor extensions for PlayFab integration and configuration
    - **Scenes/** - Contains Main.unity scene
    - **Settings/** - URP render pipeline settings (separate profiles for mobile and PC)
    - **Editor/** - Editor-only scripts and mobile notifications configuration
    - **TutorialInfo/** - Unity template tutorial files
  - **ProjectSettings/** - Unity project configuration files
  - **Packages/** - Unity package dependencies (managed via manifest.json)
  - **Library/** - Unity-generated files (excluded from git)

## Key Technologies

- **Unity Version**: 6000.1.5f1
- **PlayFab SDK**: Complete SDK installed locally in Assets/PlayFabSDK
- **Firebase SDK**: Planned integration for authentication and data synchronization
- **Rendering**: Universal Render Pipeline (URP) 17.1.0
- **Input System**: New Unity Input System 1.14.0
- **Platform Target**: Mobile-first (Android) with PC/Editor support
- **Notable Packages**:
  - Unity Timeline 1.8.7
  - Unity Input System 1.14.0
  - Visual Scripting 1.9.7
  - Unity Mobile Notifications (configured in Assets/Editor)

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

## PlayFab Integration

### Authentication System
The project implements a comprehensive authentication system in `Assets/Scripts/PlayFabLogin.cs`:

- **Editor Mode**: Uses CustomID login with fixed ID "GettingStartedGuide"
- **Build Mode (Android)**: Uses Android Device ID for automatic login
- **Google Login**: Async method `LoginWithGoogleAsync()` with fallback to guest login
- **Guest Login**: Async method `LoginAsGuestAsync()` using device unique identifier
- **Firebase Integration**: Includes `SyncFirebaseUserIdOnAccountLink()` method for syncing Firebase UserID with PlayFab (implementation in progress)

### PlayFab Configuration
- **TitleId Setup**: Must be configured in PlayFabSettings (accessed via Unity menu: Window > PlayFab > Editor Extensions)
- **API Modules Available**: Client, Server, Admin, Authentication, Economy, Events, Groups, Matchmaker, Multiplayer, Profiles, and more
- **Editor Extensions**: Use Window > PlayFab menu in Unity Editor for configuration and SDK management

### Key PlayFab Features in Use
- Account creation on first login
- Player profile information retrieval
- Social authentication (Google) with guest fallback
- Cross-platform login (CustomID for testing, DeviceID for production)

## Firebase Integration (Planned)

### Architecture Overview
The project is designed to use Firebase and PlayFab together with the following architecture:

1. **Firebase Guest Authentication**: User starts with Firebase Anonymous/Guest login
2. **Social Login Upgrade**: User upgrades to Google/Apple login through social providers
3. **PlayFab Account Linking**: Social login credentials are used to authenticate with PlayFab
4. **Firebase UserID Sync**: Firebase UserID is stored in PlayFab's user data for cross-reference

### Account Linking Flow
```
Firebase Guest ID → Social Login (Google) → PlayFab Authentication → Store Firebase UserID in PlayFab
```

### Key Components (To Be Implemented)
- Firebase Authentication SDK
- Firebase UserID storage in PlayFab's PlayerData
- Social authentication providers (Google Play Games, Apple Sign-In)
- Account recovery mechanism using Firebase UserID
- Progress synchronization between Firebase and PlayFab

### Benefits of This Architecture
- **Firebase**: Handles guest sessions and provides Firebase-specific services (Analytics, Remote Config, etc.)
- **PlayFab**: Manages player data, economy, leaderboards, and cross-platform progression
- **Social Login**: Provides seamless account upgrade path from guest to persistent account
- **UserID Sync**: Enables account recovery and cross-service data consistency

## Working with Unity Projects

- Unity generates code in .csproj files (Assembly-CSharp.csproj, Assembly-CSharp-Editor.csproj)
- The Library/ folder contains Unity's build cache and should not be modified directly
- All custom scripts should be placed in appropriate subdirectories under Assets/
- Editor scripts must be in an "Editor" folder to be excluded from builds

## Development Workflow

### Testing PlayFab Features
1. Configure PlayFab TitleId via Window > PlayFab > Editor Extensions
2. In Editor: Uses CustomID authentication automatically
3. On Device: Uses Android Device ID for persistent authentication
4. Check Unity Console for login success/failure messages

### Adding New PlayFab Features
- PlayFab APIs are available through `PlayFabClientAPI`, `PlayFabServerAPI`, etc.
- Use callback pattern or async/await with TaskCompletionSource (as shown in PlayFabLogin.cs)
- All API requests require prior authentication (login)
- Error handling via `PlayFabError.GenerateErrorReport()`

### Implementing Firebase Integration
1. Add Firebase Unity SDK via Package Manager or manual installation
2. Configure `google-services.json` (Android) and `GoogleService-Info.plist` (iOS)
3. Implement Firebase Anonymous Authentication for guest sessions
4. Store Firebase UserID in PlayFab using `UpdateUserData` API
5. Retrieve Firebase UserID from PlayFab using `GetUserData` API for account recovery

### Social Login Integration Strategy
- Use social provider tokens (Google, Apple) to authenticate with both Firebase and PlayFab
- Firebase: `FirebaseAuth.SignInWithCredential()`
- PlayFab: `LoginWithGoogleAccount()` or `LoginWithApple()`
- After successful authentication on both platforms, sync user identifiers between services