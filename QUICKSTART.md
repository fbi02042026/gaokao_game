# 高考人生模拟器 - 快速开始指南

## 🎮 项目已创建完成！

我已经为您创建了完整的高考人生模拟器游戏框架，使用**团结引擎（Tuanjie Engine）**开发。

## 📁 项目结构

```
gaokao/
├── assets/
│   ├── data/                      # 游戏数据
│   │   ├── game_phases.json       # 游戏阶段配置
│   │   └── talents.json           # 天赋配置
│   ├── scenes/                    # 场景脚本
│   │   └── MainMenuScene.ts       # 主菜单场景
│   ├── scripts/                   # TypeScript脚本
│   │   ├── core/
│   │   │   └── GameConfig.ts      # 游戏配置
│   │   ├── systems/
│   │   │   ├── TalentSystem.ts    # 天赋系统
│   │   │   ├── TimeSystem.ts      # 时间系统
│   │   │   └── SaveSystem.ts      # 存档系统
│   │   ├── ui/
│   │   │   └── UIManager.ts       # UI管理器
│   │   └── platform/
│   │       └── PlatformSDK.ts     # 平台SDK
│   └── resources/                 # 资源配置
├── game_config.json               # 游戏配置
├── README.md                      # 项目说明
└── push-to-github.ps1            # GitHub推送脚本
```

## 🚀 下一步操作

### 1. 在团结引擎中打开项目
1. 启动团结引擎编辑器
2. 点击"打开项目"
3. 选择 `e:\unity_project\project\gaokao` 目录

### 2. 查看现有代码
- **核心配置**: [GameConfig.ts](file:///e:/unity_project/project/gaokao/assets/scripts/core/GameConfig.ts)
- **天赋系统**: [TalentSystem.ts](file:///e:/unity_project/project/gaokao/assets/scripts/systems/TalentSystem.ts)
- **时间系统**: [TimeSystem.ts](file:///e:/unity_project/project/gaokao/assets/scripts/systems/TimeSystem.ts)
- **存档系统**: [SaveSystem.ts](file:///e:/unity_project/project/gaokao/assets/scripts/systems/SaveSystem.ts)

### 3. 配置平台SDK
在 [PlatformSDK.ts](file:///e:/unity_project/project/gaokao/assets/scripts/platform/PlatformSDK.ts) 中配置您的AppID：
- 微信: `wx_your_app_id`
- 抖音: `tt_your_app_id`
- TapTap: `tap_your_app_id`

### 4. 运行游戏
1. 在团结引擎中打开 `MainMenuScene` 场景
2. 按 `Ctrl+P` 或点击运行按钮

## 🎯 核心功能

### 天赋系统
- 10种天赋（前世记忆、过目不忘、情绪管理等）
- 天赋贯穿游戏全程（高中→高考→选专业→大学→职场）
- 二周目解锁"前世记忆"天赋

### 时间系统
- 游戏时间自动推进
- 每阶段2-3天真实时间
- 支持时间跳跃功能

### 存档系统
- 本地JSON存储
- 云存档支持（通过平台SDK）
- 自动保存功能

## 📝 开发计划

### v1.0.0 - 基础框架 ✅
- [x] 核心游戏架构
- [x] 天赋系统
- [x] 时间推进系统
- [x] 存档系统
- [x] UI框架
- [x] 平台SDK集成

### v1.1.0 - 主菜单和设置
- [ ] 完善主菜单UI
- [ ] 音乐和音效系统
- [ ] 设置面板
- [ ] 语言切换

### v1.2.0 - 游戏核心循环
- [ ] 日/周活动系统
- [ ] 随机事件系统
- [ ] 学习/锻炼/社交活动
- [ ] 属性值计算

### v1.3.0 - 高中阶段
- [ ] 高一剧情
- [ ] 分班考试
- [ ] 文理分科选择
- [ ] 高二、高三剧情

### v1.4.0 - 高考系统
- [ ] 高考模拟
- [ ] 成绩计算
- [ ] 志愿填报
- [ ] 大学录取

### v1.5.0 - 大学阶段
- [ ] 大学生活模拟
- [ ] 专业学习
- [ ] 社交发展
- [ ] 毕业事件

### v1.6.0 - 职场阶段
- [ ] 职业选择
- [ ] 职场发展
- [ ] 人生结局

## 🔧 技术特性

### 支持的平台
- ✅ 微信小游戏
- ✅ 抖音小游戏
- ✅ TapTap
- ✅ iOS App
- ✅ Android App
- ✅ Windows/Mac (团结引擎)

### 屏幕适配
- 竖版 9:16 布局
- 自动适配各种分辨率

### 数据存储
- 本地: `localStorage`
- 云端: 平台SDK云存档API

## 📚 学习资源

### 团结引擎文档
- 官网: https://www.tuanjie.com/
- 文档: https://docs.tuanjie.com/

### 相关技术
- TypeScript: https://www.typescriptlang.org/
- 微信小游戏: https://developers.weixin.qq.com/minigame/
- 抖音小游戏: https://developer.toutiao.com/

## 🤝 如何贡献

1. Fork 项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📞 遇到问题？

- 提交 GitHub Issue: https://github.com/fbi02042026/gaokao_game/issues
- 查看已有问题: https://github.com/fbi02042026/gaokao_game/issues?q=

## 🎓 许可证

本项目仅供学习交流使用。

---

**祝开发顺利！如有需要随时联系我！** 🚀
