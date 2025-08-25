# 钓鱼游戏 (Fishing Game)

## 项目介绍

这是一个基于Unity开发的休闲钓鱼游戏，玩家可以通过钓鱼赚取金币，升级装备提高效率。游戏支持横屏模式，并针对不同Android设备进行了分辨率适配。

## 系统要求

- Unity 2021.3 或更高版本
- Android SDK 21 (Android 5.0) 或更高版本
- 支持的平台：Android


## 项目结构

- `Assets/Scripts/` - 游戏脚本
  - `Managers/` - 游戏管理器脚本
  - `Fish/` - 鱼类相关脚本
  - `Hook/` - 钓鱼钩相关脚本
- `Assets/Graphics/` - 游戏图形资源
- `Assets/Animation/` - 动画文件
- `Assets/prefab/` - 预制体


## 注意事项

- 游戏默认设置为横屏模式
- 已针对不同Android设备进行了分辨率适配
- 使用`MobileOptimizer.cs`脚本自动调整游戏性能设置

