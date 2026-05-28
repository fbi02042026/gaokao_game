# 高考人生模拟器

一款基于团结引擎（Tuanjie Engine）开发的人生模拟养成游戏，体验从高中到职场的完整人生旅程。

## 🎮 游戏特色

### 核心玩法
- **天赋系统**: 12种天赋3选1，贯穿高中→高考→选专业→大学→职场全流程
- **性别选择**: 首次登录选择性别，不同性别不同事件插图
- **人格系统**: 8种高考人格，根据选择动态演化
- **二周目模式**: 4级既视感系统，多代传承
- **多结局**: 根据选择、天赋、成绩解锁不同人生结局

### 游戏阶段
1. **高中三年**: 努力学习、发展社交、塑造人格
2. **高考**: 决定命运的大考
3. **填报志愿**: 选择学校和专业
4. **大学四年**: 计算机/法学/医学/商科方向
5. **职场人生**: 开启职业生涯
6. **结局**: 人生总结与传承

### 美术风格
- 2D Q版软萌卡通风格
- 色板：#FFF8F0 暖白 / #6B9DF7 主蓝 / #FFB5B5 柔粉

## 🛠️ 技术栈

- **引擎**: 团结引擎（Tuanjie Engine）2D URP
- **编程语言**: C# (全局命名空间)
- **目标平台**: 微信小游戏、抖音小游戏、TapTap
- **屏幕比例**: 竖版 9:16 (750×1334)

## 📦 项目结构

```
Assets/
├── Scripts/                  # C# 脚本
│   ├── Models/               # 数据模型
│   │   ├── PlayerState.cs    # 玩家状态(含性别/属性/人格)
│   │   ├── GameEvent.cs      # 事件模型(含插图映射)
│   │   ├── Talent.cs         # 天赋模型
│   │   ├── Personality.cs    # 人格模型
│   │   ├── College.cs        # 院校数据
│   │   ├── Major.cs          # 专业数据
│   │   ├── Province.cs       # 省份分数线
│   │   └── NPC.cs            # NPC数据
│   ├── Engine/               # 游戏引擎
│   │   ├── EventEngine.cs    # 事件驱动引擎
│   │   ├── TalentEngine.cs   # 天赋引擎
│   │   ├── ScoreEngine.cs    # 高考计分引擎
│   │   ├── PersonalityEngine.cs # 人格计算
│   │   ├── DejaVuEngine.cs   # 既视感系统
│   │   ├── InheritEngine.cs  # 多代传承
│   │   └── MergeEngine.cs    # 合成系统
│   ├── Managers/             # 管理器
│   │   ├── GameStateManager.cs # 游戏状态管理
│   │   ├── SaveManager.cs    # 存档(PlayerPrefs)
│   │   ├── AdManager.cs      # 广告管理
│   │   └── ShareManager.cs   # 分享管理
│   ├── Data/                 # 数据层
│   │   ├── DataLoader.cs     # JSON加载(StreamingAssets)
│   │   └── ResourceHelper.cs # 资源加载(含性别区分)
│   ├── UI/                   # 界面
│   │   ├── HomeUI.cs         # 主界面+性别选择
│   │   ├── GenderSelectUI.cs # 性别选择面板
│   │   ├── HighSchoolUI.cs   # 高中阶段
│   │   ├── GaokaoUI.cs       # 高考出分
│   │   ├── ZhiyuanUI.cs      # 志愿填报
│   │   ├── CollegeUI.cs       # 大学阶段
│   │   ├── LifeUI.cs         # 人生阶段
│   │   └── ResultUI.cs       # 结局+传承
│   └── GameManager.cs        # 总控
├── Tex/                      # 美术资源
│   ├── 事件高中/             # 高中事件插图(男女)
│   ├── 事件大学/             # 大学事件插图(男女)
│   ├── 结局/                 # 结局插画(男女)
│   ├── bg_*.png              # 场景背景
│   └── npc_*.png             # NPC角色
├── Resources/                # Resources.Load资源
├── StreamingAssets/          # 运行时JSON数据
├── 策划文档/                 # 策划设计文档
└── 高考志愿游戏设计_GameData/ # 游戏原始数据
```

## 🚀 开发指南

### 环境要求
- 团结引擎 3.x 或更高版本
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

### 运行游戏
1. 打开 `Home` 场景
2. 点击运行按钮

## 🔧 平台构建

使用菜单栏的 `BuildConfig` 工具：
- **Build/WeChat Mini Game** - 构建微信小游戏
- **Build/Douyin Mini Game** - 构建抖音小游戏 (WebGL)
- **Build/TapTap** - 构建TapTap Android包

## 📊 数据存储

- 存档: `PlayerPrefs` 本地存储
- 数据: `StreamingAssets` 加载JSON
- 云存档: 通过平台SDK实现

## 🎯 天赋列表 (12种)

| 天赋 | 稀有度 | 效果 |
|------|--------|------|
| 过目不忘 | SSR | 学习效率提升50% |
| 逻辑思维 | SSR | 理科成绩+20% |
| 情绪管理 | SR | 考试稳定性+30% |
| 社交达人 | SR | 解锁隐藏社交事件 |
| 运动天才 | R | 体育必定满分 |
| 幸运星 | SSR | 关键时刻成功率+40% |
| 速学者 | R | 理解力+30% |
| 时间操控 | SR | 每日学习时间+2h |
| 艺术天赋 | SR | 创造力+20 |
| 领导力 | SSR | 社交收益+50% |
| 坚韧不拔 | SR | 逆境属性损失减半 |
| 全面发展 | R | 所有属性均衡增长 |

## 📝 开发日志

### V1.0 (正式版)
- ✅ 性别选择系统
- ✅ 男女不同事件插图
- ✅ 12种天赋3选1
- ✅ 高中/大学/人生全阶段
- ✅ 高考计分出分
- ✅ 志愿填报44所院校
- ✅ 8种高考人格
- ✅ 二周目既视感传承
- ✅ 资源目录整理备份

## 📄 许可证

本项目仅供学习交流使用。

---

**祝所有考生金榜题名！🎓**