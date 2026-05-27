# 高考人生模拟器

一款基于团结引擎（Tuanjie Engine）开发的人生模拟养成游戏，体验从高中到职场的完整人生旅程。

## 🎮 游戏特色

### 核心玩法
- **天赋系统**: 天赋贯穿高中→高考→选专业→大学→职场全流程
- **时间推进**: 游戏内2-3天为一周真实时间循环
- **二周目模式**: 获得"前世记忆"天赋，解锁全新体验
- **多结局**: 根据你的选择和天赋解锁不同人生结局

### 游戏阶段
1. **高中三年**: 努力学习、发展社交、塑造人格
2. **高考**: 决定命运的大考
3. **填报志愿**: 选择学校和专业
4. **大学四年**: 继续成长和发展
5. **职场人生**: 开启职业生涯

### 美术风格
- 2D Q版软萌卡通风格
- 参考：肥鹅健身房、恐龙多多

## 🛠️ 技术栈

- **引擎**: 团结引擎（Tuanjie Engine）
- **编程语言**: TypeScript
- **目标平台**: 微信小游戏、抖音小游戏、TapTap、原生App
- **屏幕比例**: 竖版 9:16

## 📦 项目结构

```
gaokao/
├── assets/
│   ├── data/              # 游戏数据配置
│   │   ├── game_phases.json    # 游戏阶段配置
│   │   └── talents.json        # 天赋系统配置
│   ├── scenes/            # 游戏场景
│   │   └── MainMenuScene.ts
│   ├── scripts/           # TypeScript脚本
│   │   ├── core/          # 核心配置
│   │   │   └── GameConfig.ts
│   │   ├── systems/      # 游戏系统
│   │   │   ├── TalentSystem.ts    # 天赋系统
│   │   │   ├── TimeSystem.ts      # 时间系统
│   │   │   └── SaveSystem.ts      # 存档系统
│   │   ├── ui/           # UI管理
│   │   │   └── UIManager.ts
│   │   └── platform/     # 平台SDK
│   │       └── PlatformSDK.ts
│   └── resources/        # 资源配置
├── game_config.json       # 游戏配置文件
└── README.md
```

## 🚀 开发指南

### 环境要求
- 团结引擎 3.x 或更高版本
- Node.js 18+
- Git

### 安装步骤
1. 克隆仓库
```bash
git clone https://github.com/fbi02042026/gaokao_game.git
cd gaokao_game
```

2. 使用团结引擎打开项目
   - 启动团结引擎
   - 选择"打开项目"
   - 选择项目根目录

3. 配置平台SDK
   - 微信: 在 `PlatformSDK.ts` 中配置 `wx_your_app_id`
   - 抖音: 在 `PlatformSDK.ts` 中配置 `tt_your_app_id`
   - TapTap: 在 `PlatformSDK.ts` 中配置 `tap_your_app_id`

### 运行游戏
1. 在团结引擎中打开 `MainMenuScene` 场景
2. 点击运行按钮（或按 Ctrl+P）

### 构建发布
1. 选择目标平台（微信/抖音/TapTap/iOS/Android）
2. 配置构建选项
3. 点击构建

## 🎯 天赋列表

| 天赋名称 | 稀有度 | 效果 |
|---------|--------|------|
| 前世记忆 | 传说 | 二周目专属，记得所有选择 |
| 过目不忘 | 稀有 | 学习效率+50% |
| 情绪管理 | 普通 | 考试稳定性+30% |
| 时间操控 | 史诗 | 每日学习时间+2小时 |
| 社交达人 | 稀有 | 解锁隐藏社交事件 |
| 幸运星 | 史诗 | 关键时刻成功率+40% |
| 速学者 | 普通 | 理解力+30% |
| 运动天才 | 普通 | 体育必定满分 |

## 📊 数据存储

### 本地存储
- 存档位置: `localStorage.game_save.json`
- 设置存储: `localStorage.game_settings`

### 云存档
通过平台SDK实现云端同步：
- 微信: `wx.getFriendCloudStorage()`
- 抖音: `tt.getFriendCloudStorage()`
- TapTap: TapTap SDK云存档API

## 🔧 平台特性

### 微信小游戏
- 分享功能
- 好友排行榜
- 云存档
- 广告（Banner、激励视频、插屏）

### 抖音小游戏
- 分享功能
- 好友排行榜
- 云存档
- 广告

### TapTap
- 分享功能
- 排行榜
- 云存档
- 支付

## 📝 开发日志

### v1.0.0 (开发中)
- ✅ 核心游戏架构
- ✅ 天赋系统
- ✅ 时间推进系统
- ✅ 存档系统
- ✅ UI框架
- ✅ 平台SDK集成
- ⏳ 主菜单场景
- ⏳ 游戏核心循环
- ⏳ 各阶段剧情

## 🤝 贡献指南

欢迎提交Issue和Pull Request！

## 📄 许可证

本项目仅供学习交流使用。

## 📞 联系方式

- GitHub: https://github.com/fbi02042026/gaokao_game
- 问题反馈: 请提交GitHub Issue

---

**祝所有考生金榜题名！🎓**
