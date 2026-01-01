# PaintVR-Multiplayer

A collaborative virtual reality painting experience built with Unity. Create art together with friends in immersive 3D environments with full multiplayer support, voice chat, and hand tracking.

![Unity Version](https://img.shields.io/badge/Unity-6000.0.48f1-blue)
![Platform](https://img.shields.io/badge/Platform-VR-green)

## ✨ Features

- **VR Painting System**: Draw and paint in 3D space using motion controllers or hand tracking
- **Multiplayer Support**: Real-time collaborative painting with other players using Netcode for GameObjects
- **Voice Chat**: Communicate with other players using integrated Vivox voice chat
- **Hand Tracking**: Full hand tracking support via XR Hands for natural interactions
- **Drawing Zones**: Designated areas for creating art with automatic replication
- **Undo Support**: Undo your brush strokes while painting
- **Multiple Environments**: Various scenes including city environments to paint in
- **Mini-Games**: Fun activities including Climber, Slingshot, and WhackAPig
- **Player Customization**: Customize your avatar's appearance
- **Cross-Platform VR**: OpenXR support for compatibility with multiple VR headsets

## 📋 Requirements

### Hardware
- VR headset compatible with OpenXR (Meta Quest, Valve Index, HTC Vive, etc.)
- VR controllers or hand tracking capable device

### Software
- **Unity 6000.0.48f1** (Unity 6 LTS)
- Git LFS (for large asset files)

## 🚀 Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/fernandoramirez1337/PaintVR-Multiplayer.git
   cd PaintVR-Multiplayer
   ```

2. **Open in Unity Hub**
   - Open Unity Hub
   - Click "Add" and select the cloned project folder
   - Ensure you have Unity 6000.0.48f1 installed
   - Open the project

3. **Configure Unity Services** (for multiplayer features)
   - Go to `Edit > Project Settings > Services`
   - Link to your Unity project
   - Enable Authentication, Multiplayer, and Vivox services as needed

### Running the Project

1. Open one of the main scenes:
   - `Assets/Scenes/MainMenu.unity` - Main menu and lobby
   - `Assets/Scenes/BasicScene.unity` - Basic painting environment
   - `Assets/Scenes/BigCityScene.unity` - City environment

2. Configure your XR settings:
   - Go to `Edit > Project Settings > XR Plug-in Management`
   - Enable OpenXR for your target platform

3. Press Play or build for your target platform

## 📁 Project Structure

```
PaintVR-Multiplayer/
├── Assets/
│   ├── Scenes/              # Game scenes (MainMenu, BasicScene, BigCityScene, etc.)
│   ├── VRMPAssets/          # VR Multiplayer template assets
│   │   ├── Scripts/         # Core scripts
│   │   │   ├── Gameplay/    # Gameplay mechanics (Drawing, MessageBoard, etc.)
│   │   │   ├── Network/     # Networking code (Managers, Player sync)
│   │   │   ├── Player/      # Player-related scripts
│   │   │   └── UI/          # User interface scripts
│   │   ├── Prefabs/         # Prefab assets
│   │   └── MiniGames/       # Mini-game assets and scripts
│   ├── fer/                 # Custom painting and VR brush system
│   │   ├── scripts/         # Paint VR scripts
│   │   │   └── paintvr/     # Core painting functionality
│   │   └── vr-brush/        # VR brush and drawing system
│   ├── XR/                  # XR configuration
│   ├── XRI/                 # XR Interaction Toolkit settings
│   ├── Plugins/             # Third-party plugins
│   └── SyntyStudios/        # Synty asset packages
├── Packages/                # Unity package dependencies
├── ProjectSettings/         # Unity project settings
└── UIElementsSchema/        # UI Toolkit schemas
```

## 📦 Key Dependencies

| Package | Version | Description |
|---------|---------|-------------|
| XR Interaction Toolkit | 3.0.8 | VR interaction framework |
| Netcode for GameObjects | 2.4.0 | Multiplayer networking |
| XR Hands | 1.5.1 | Hand tracking support |
| OpenXR | 1.14.0 | Cross-platform VR runtime |
| Unity Transport | 2.5.1 | Network transport layer |
| Vivox | 16.6.0 | Voice chat integration |
| Universal Render Pipeline | 17.0.4 | Graphics rendering |
| Unity Services Authentication | 3.4.1 | User authentication |
| Unity Services Multiplayer | 1.1.3 | Multiplayer services |

## 🎮 Controls

### VR Controllers
- **Grip**: Grab objects and paint brushes
- **Trigger**: Draw/paint when holding a brush
- **Thumbstick**: Locomotion and teleportation

### Hand Tracking
- **Pinch Gesture**: Activate drawing when holding a brush
- **Grab Gesture**: Pick up objects and tools

## 🔧 Building

### Standalone (Windows)
1. Go to `File > Build Settings`
2. Select "Windows, Mac, Linux" platform
3. Click "Build" or "Build and Run"

### Android (Quest)
1. Go to `File > Build Settings`
2. Switch to "Android" platform
3. Configure Player Settings for Quest
4. Click "Build" or "Build and Run"

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project uses various open-source components. See individual license files in the respective directories.

## 🙏 Acknowledgments

- [Unity VR Multiplayer Template](https://docs.unity3d.com/Packages/com.unity.template.vr-multiplayer@1.0/manual/index.html) - Foundation for multiplayer VR functionality
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html) - VR interaction framework by Unity
- [Synty Studios](https://www.syntystudios.com/) - 3D art assets and environments
- [EasyCurvedLine](https://github.com/gpvigano/EasyCurvedLine) - Line rendering utilities
