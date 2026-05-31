# PlatformManager.cs 全面接入实施计划

> 项目：我的高考志愿模拟器 V1.0 | 基于 [SDK研究文档](./platform_sdk_research_plan.md) 的调查结果

***

## 一、当前状态诊断

### 1.1 SDK安装状态

| SDK        |   状态  | 说明                                    |
| ---------- | :---: | ------------------------------------- |
| WeChatWASM |  ✅ 可用 | 团结引擎内置，导出微信小游戏时自动链接                   |
| TTSDK (抖音) |  ✅ 可用 | 团结引擎内置，导出抖音小游戏时自动链接                   |
| KS (快手)    | ❌ 未安装 | 需下载unitypackage: `com.kwai.mini.game` |
| TapTap SDK | ❌ 未安装 | 需通过UPM或手动导入                           |

### 1.2 当前架构

```
PlatformManager.cs (324行)
├── 平台检测 ✅ (7个平台)
├── 登录 (TODO x3)
├── 激励视频广告 ✅ (微信+抖音)
├── 插屏广告 ⚠️ (仅微信)
├── Banner广告 ❌
├── 分享 ✅ (微信+抖音主动分享)
├── 振动 ✅ (微信+抖音)
└── 退出 ✅

AdManager.cs → 调用 PlatformManager.ShowRewardedAd()
ShareManager.cs → 调用 PlatformManager.Share()
```

### 1.3 核心约束

* **无后端服务器** — 登录只能做客户端层面，获取平台openid即可

* **纯广告变现** — 支付功能不需要（也无版号）

* **团结引擎** — 微信/抖音SDK通过条件编译自动可用

***

## 二、实施总览

### 目标：将 PlatformManager 完成度从 35% → 90%

| 指标            |   当前  |   目标  |
| ------------- | :---: | :---: |
| 功能覆盖          | \~10项 | \~25项 |
| 平台覆盖（代码中有效实现） |  2/7  |  5/7  |
| 已解决TODO       |   0   |   6   |

***

## 三、实施步骤（5个阶段，共15个任务）

### 🔴 阶段一：SDK安装 + 基础设施 (计划先行)

#### Task 1.1 — 安装快手SDK

* 下载快手Unity IG SDK包: [下载链接](https://static.yximgs.com/udata/pkg/KS-GAME/minigame/com.kwai.mini.game-sdk-1.1.0.unitypackage)

* 导入到 `Assets/Plugins/` 目录

* 验证 KS 命名空间可用

#### Task 1.2 — 安装TapTap SDK

* 修改 `Packages/manifest.json`，添加 TapSDK 依赖:

  ```json
  "com.taptap.tds.bootstrap": "3.30.3",
  "com.taptap.tds.login": "3.30.3", 
  "com.taptap.tds.common": "3.30.3",
  "com.tapsdk.antiaddiction": "3.30.3"
  ```

* 添加 scopedRegistries 到 manifest.json

* 添加 `link.xml` 防止IL2CPP剪裁

* 添加 External Dependency Manager 依赖

#### Task 1.3 — 创建环境检查工具

* 新建 `SDKValidator.cs` 工具脚本

* 运行时检查各平台SDK可用性

* Editor下提供可视化状态面板

* 输出: 每个平台的SDK是否就绪

***

### 🔴 阶段二：登录系统 (客户端层面)

#### Task 2.1 — PlatformManager 登录重构

**设计思路**: 因为是纯广告小游戏无后端，登录流程简化为：

1. 调用平台login → 获取基础用户信息
2. 本地生成唯一用户ID
3. 测试阶段提供游客登录入口

```csharp
// 新增字段
public string UserId { get; private set; }     // 本地唯一ID
public string PlatformUserId { get; private set; } // 平台openid
public string NickName { get; private set; }    // 昵称
public string AvatarUrl { get; private set; }   // 头像
public bool IsLoggedIn { get; private set; }    // 登录状态
public Action OnLogout;                          // 登出回调
```

#### Task 2.2 — 实现微信登录

```csharp
public void WechatLogin() {
#if UNITY_WEIXINMINIGAME
    WX.Login(new WXLoginOption {
        success = (res) => {
            PlatformUserId = res.code; // 客户端拿code做标识
            UserId = "wx_" + res.code.GetHashCode().ToString("X8");
            IsLoggedIn = true;
            OnLoginSuccess?.Invoke();
        },
        fail = (res) => { /* 降级游客 */ }
    });
#endif
}
```

#### Task 2.3 — 实现抖音登录

```csharp
public void DouyinLogin() {
#if UNITY_DOUYINMINIGAME
    TT.Login(new TTLoginOption {
        success = (res) => {
            PlatformUserId = res.code;
            UserId = "dy_" + res.code.GetHashCode().ToString("X8");
            IsLoggedIn = true;
            OnLoginSuccess?.Invoke();
        },
        fail = (res) => { /* 降级游客 */ }
    });
#endif
}
```

#### Task 2.4 — 实现快手登录

```csharp
public void KuaiShouLogin() {
#if UNITY_KUAISHOUMINIGAME
    KS.Login(callback: (result) => {
        PlatformUserId = result.code;
        UserId = "ks_" + result.code.GetHashCode().ToString("X8");
        IsLoggedIn = true;
        OnLoginSuccess?.Invoke();
    });
#endif
}
```

#### Task 2.5 — 实现TapTap登录

```csharp
public void TapTapLogin() {
#if UNITY_ANDROID || UNITY_IOS
    TapLogin.Login(); // TapTap OAuth，异步回调
    // 在回调中获取TDSUser信息
#endif
}
```

#### Task 2.6 — 游客登录增强 + 注销功能

```csharp
public void GuestLogin() {
    UserId = "guest_" + System.Guid.NewGuid().ToString("N")[..8];
    IsLoggedIn = true;
    OnLoginSuccess?.Invoke();
}

public void Logout() {
    UserId = null;
    PlatformUserId = null;
    IsLoggedIn = false;
    OnLogout?.Invoke();
}
```

***

### 🟡 阶段三：广告体系补齐

#### Task 3.1 — 快手激励视频广告

```csharp
#if UNITY_KUAISHOUMINIGAME
private void ShowKuaiShouRewardedAd(string placementId, Action<bool> callback) {
    var ad = KS.CreateRewardedVideoAd(placementId);
    ad.OnClose((res) => callback?.Invoke(res?.isEnded ?? true));
    ad.OnError((code, msg) => callback?.Invoke(false));
    ad.Show();
}
#endif
```

#### Task 3.2 — 快手插屏广告

```csharp
#if UNITY_KUAISHOUMINIGAME
private void ShowKuaiShouInterstitialAd(string placementId, Action<bool> callback) {
    var ad = KS.CreateInterstitialAd(placementId);
    ad.OnClose(() => callback?.Invoke(true));
    ad.OnError((err) => callback?.Invoke(false));
    ad.Show();
}
#endif
```

#### Task 3.3 — Banner广告接口（微信+抖音+快手）

* 添加 `ShowBannerAd(placementId, position, callback)` 方法

* 添加 `HideBannerAd()` 方法

* 微信: `WX.CreateBannerAd()` / 抖音: `TT.CreateBannerAd()` / 快手: `KS.CreateBannerAd()`

#### Task 3.4 — 抖音插屏广告

* 补充 `ShowDouyinInterstitialAd()` 方法

***

### 🟡 阶段四：社交分享增强

#### Task 4.1 — 被动分享（右上角菜单）

* 微信: `WX.OnShareAppMessage()` + `WX.ShowShareMenu()`

* 抖音: `TT.OnShareAppMessage()`

* 快手: `KS.OnShareAppMessage()`

#### Task 4.2 — 快手主动分享

```csharp
#if UNITY_KUAISHOUMINIGAME
private void KuaiShouShare(string title, string imageUrl, Action<bool> callback) {
    KS.ShareAppMessage(new { title, imageUrl });
    callback?.Invoke(true);
}
#endif
```

#### Task 4.3 — 分享回调优化

* 让 `Share()` 方法能通过回调通知分享结果

* 支持自定义分享参数（query参数用于邀请追踪）

***

### 🟢 阶段五：平台特色功能 + 测试工具

#### Task 5.1 — 抖音必接功能

```csharp
// 侧边栏复访
public void NavigateToSidebar() {
#if UNITY_DOUYINMINIGAME
    TT.NavigateToScene(new TTNavigateToSceneParam { scene = "sidebar" });
#endif
}

// 录屏
public void StartRecord() {
#if UNITY_DOUYINMINIGAME
    TT.StartRecord(new TTStartRecordParam());
#endif
}
public void StopRecord(Action<bool> callback) {
#if UNITY_DOUYINMINIGAME
    TT.StopRecord(new TTStopRecordParam {
        success = () => callback?.Invoke(true),
        fail = () => callback?.Invoke(false)
    });
#endif
}
```

#### Task 5.2 — TapTap合规认证

```csharp
public void StartAntiAddictionCheck(Action<bool> callback) {
#if UNITY_ANDROID || UNITY_IOS
    AntiAddiction.StartUp(clientId, (code, msg) => {
        callback?.Invoke(code == 500); // 500=通过
    });
#else
    callback?.Invoke(true);
#endif
}
```

#### Task 5.3 — 快手特色功能

```csharp
public void AddToCommonUse()  // 设为常用
public void CheckCommonUse()  // 检查常用
public void AddShortcut()     // 添加桌面快捷方式
```

#### Task 5.4 — 测试调试UI

创建 `Assets/Scripts/Debug/DebugPanel.cs`:

* 游客登录按钮

* 注销按钮

* 平台切换模拟（Editor下模拟不同平台行为）

* SDK状态显示面板

* 各功能手动测试按钮（激励广告、插屏、分享、振动等）

#### Task 5.5 — 生命周期监听

```csharp
private void RegisterLifecycleEvents() {
#if UNITY_WEIXINMINIGAME
    WX.OnShow(() => OnGameResume?.Invoke());
    WX.OnHide(() => OnGamePause?.Invoke());
#elif UNITY_DOUYINMINIGAME
    TT.OnShow(() => OnGameResume?.Invoke());
    TT.OnHide(() => OnGamePause?.Invoke());
#endif
}
```

***

## 四、不受影响的功能（明确不实施）

| 功能           | 原因                   |
| ------------ | -------------------- |
| 虚拟支付         | 无版号，纯广告变现            |
| 云存储/存档       | 无后端，用本地PlayerPrefs即可 |
| 排行榜          | 无后端，不做服务器排行          |
| 订阅消息         | 召回场景当前不需要            |
| 好友系统         | 单人游戏，无社交需求           |
| TapTap成就/排行榜 | 等登录稳定后再考虑            |

***

## 五、PlatformManager.cs 目标结构

```
PlatformManager.cs (预计 500+ 行)
├── 枚举: GamePlatform (不变)
├── 属性: CurrentPlatform, IsMiniGame (不变)
├── 事件: OnLoginSuccess, OnLogout, OnGameResume, OnGamePause (新增)
├── 用户信息: UserId, PlatformUserId, NickName, AvatarUrl, IsLoggedIn (新增)
│
├── 初始化: Awake → DetectPlatform → RegisterLifecycleEvents (增强)
│
├── 登录 ── WechatLogin, DouyinLogin, KuaiShouLogin, TapTapLogin (实现TODO)
├── 游客 ── GuestLogin, Logout (增强)
│
├── 激励广告 ── ShowRewardedAd → WeChat/Douyin/KS (补齐KS)
├── 插屏广告 ── ShowInterstitialAd → WeChat/Douyin/KS (补齐)  
├── Banner广告 ── ShowBannerAd, HideBannerAd (新增)
│
├── 主动分享 ── Share → WeChat/Douyin/KS (补齐KS)
├── 被动分享 ── RegisterPassiveShare (新增)
│
├── 振动 ── VibrateShort, VibrateLong (已实现)
│
├── 抖音特色 ── NavigateToSidebar, StartRecord, StopRecord (新增)
├── 快手特色 ── AddToCommonUse, CheckCommonUse, AddShortcut (新增)
├── TapTap合规 ── StartAntiAddictionCheck (新增)
│
├── 生命周期 ── RegisterLifecycleEvents (新增)
│
└── 工具 ── GetPlatformName, QuitGame (不变)
```

***

## 六、测试计划

### 6.1 Editor模拟测试

* DebugPanel.cs 提供全部功能手动测试按钮

* 模拟不同平台的SDK调用结果

* 游客登录/注销流程验证

### 6.2 真机测试对照表

| 测试项    |  微信 |  抖音 |  快手 | TapTap |
| ------ | :-: | :-: | :-: | :----: |
| 平台检测   |  ✅  |  ✅  |  ✅  |    ✅   |
| 平台登录   | 需导出 | 需导出 | 需导出 |   需导出  |
| 激励广告   | 需导出 | 需导出 | 需导出 |   N/A  |
| 插屏广告   | 需导出 | 需导出 | 需导出 |   N/A  |
| 分享     | 需导出 | 需导出 | 需导出 |   N/A  |
| 振动     | 需导出 | 需导出 | 需导出 |    ✅   |
| 侧边栏/录屏 | N/A | 需导出 | N/A |   N/A  |

***

## 七、执行顺序

```
Day 1:  Task 1.1(KS安装) → Task 1.2(Tap安装) → Task 1.3(验证工具)
Day 2:  Task 2.1(重构) → Task 2.2(微信登录) → Task 2.3(抖音登录)
Day 3:  Task 2.4(快手登录) → Task 2.5(Tap登录) → Task 2.6(游客+注销)
Day 4:  Task 3.1(KS广告) → Task 3.2(KS插屏) → Task 3.3(Banner) → Task 3.4(抖音插屏)
Day 5:  Task 4.1(被动分享) → Task 4.2(KS分享) → Task 4.3(分享回调)
Day 6:  Task 5.1(抖音必接) → Task 5.2(Tap合规) → Task 5.3(KS特色)
Day 7:  Task 5.4(调试面板) → Task 5.5(生命周期) → 最终集成测试
```

