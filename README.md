# Adaptive AI Tower Defense

This is a Unity-based tower defense game developed as a group project.

## Project Overview
- Built using **Unity**
- Features a **Main Menu**, **Loading Screen**, and **MVP gameplay scene**
- Designed to be portable across platforms, including **iOS**

## Scenes
- **MainMenuScene** – Main menu UI (Play, Settings, Credits, Quit)
- **LoadingScreen** – Transitional loading screen
- **MVP** – Core gameplay scene

## How to Run the Project (Unity)
1. Open the project in **Unity Hub**
2. Load `MainMenuScene`
3. Press Play

## iOS Build Instructions
Due to Apple signing requirements:

- The iOS build **cannot be run directly** without signing
- To run on an iPhone:
  1. Open the project in Unity
  2. Switch build target to **iOS**
  3. Build the project
  4. Open the generated Xcode project
  5. Sign with an Apple ID
  6. Run on a physical iOS device

This is standard for all iOS Unity projects.

## Repository Notes
- Generated build files are ignored via `.gitignore`
- Only source assets, scenes, scripts, and project settings are tracked

