# 各平台SDK功能整理与对照

> 项目：我的高考志愿模拟器 V1.0 | 引擎：团结引擎 2D URP | 更新：2026-05-31

---

## 一、微信小游戏 SDK (WeChatWASM)

**官方文档**: [微信小游戏开发文档](https://developers.weixin.qq.com/minigame/dev/api/)  
**Unity桥接SDK**: `WeChatWASM` 命名空间（团结引擎内置 / 微信Unity插件）  
**编译宏**: `UNITY_WEIXINMINIGAME`  
**文档入口**: [Unity微信小游戏转换插件](https://game.weixin.qq.com/cgi-bin/gamewxagwasmsplitwap/getunityplugininfo?download=1)

### 1.1 登录与用户信息

| 功能 | JS API | Unity C# 调用 | 说明 |
|------|--------|--------------|------|
| 微信登录 | `wx.login()` | `WX.Login( callback )` | 获取临时code，需服务端换openid |
| 检查登录态 | `wx.checkSession()` | `WX.CheckSession( callback )` | 验证session_key是否过期 |
| 获取用户信息 | `wx.getUserInfo()` | `WX.GetUserInfo( callback )` | 需用户授权；新版需通过按钮方式 |
| 创建用户信息按钮 | `wx.createUserInfoButton()` | `WX.CreateUserInfoButton( param )` | 推荐方式获取头像昵称 |
| 用户授权设置 | `wx.getSetting()` | `WX.GetSetting( callback )` | 查询已授权权限 |
| 发起授权请求 | `wx.authorize()` | `WX.Authorize( param )` | 提前向用户请求特定权限 |

### 1.2 广告

| 功能 | JS API | Unity C# 调用 | 说明 | 代码已完成 |
|------|--------|--------------|------|----------|
| 激励视频广告 | `wx.createRewardedVideoAd()` | `WX.CreateRewardedVideoAd( param )` | 全局单例，通过isEnded判断是否发奖励 | ✅ |
| 插屏广告 | `wx.createInterstitialAd()` | `WX.CreateInterstitialAd( param )` | 全屏插屏，有频次限制 | ✅ |
| Banner广告 | `wx.createBannerAd()` | `WX.CreateBannerAd( param )` | 横幅广告，可控制显示/隐藏 | ❌ |
| 原生模板广告 | `wx.createCustomAd()` | 待确认 | 自定义广告组件 | ❌ |
| 格子广告 | `wx.createGridAd()` | 待确认 | 多格子广告 | ❌ |

### 1.3 分享与社交

| 功能 | JS API | Unity C# 调用 | 说明 | 代码已完成 |
|------|--------|--------------|------|----------|
| 主动分享 | `wx.shareAppMessage()` | `WX.ShareAppMessage( param )` | 游戏内主动拉起分享 | ✅ |
| 被动分享 | `wx.onShareAppMessage()` | `WX.OnShareAppMessage( callback )` | 右上角菜单分享 | ❌ |
| 分享到朋友圈 | `wx.onShareTimeline()` | `WX.OnShareTimeline( callback )` | 朋友圈分享 | ❌ |
| 显示/隐藏分享菜单 | `wx.showShareMenu()` / `wx.hideShareMenu()` | 对应方法 | 控制右上角分享按钮 | ❌ |
| 分享图片 | `wx.showShareImageMenu()` | 待确认 | 长按图片分享 | ❌ |

### 1.4 支付

| 功能 | JS API | Unity C# 调用 | 说明 |
|------|--------|--------------|------|
| 虚拟支付 | `wx.requestMidasPayment()` | `WX.RequestMidasPayment( param )` | 需要开通虚拟支付，配置道具ID |
| 米大师支付 | Midas SDK | 通过WX封装 | 支持道具直购、游戏币 |

### 1.5 数据存储

| 功能 | JS API | Unity C# 调用 | 说明 |
|------|--------|--------------|------|
| 云存储 | `wx.setUserCloudStorage()` | `WX.SetUserCloudStorage( param )` | KV存储，可用于排行榜数据 |
| 获取云存储 | `wx.getUserCloudStorage()` | `WX.GetUserCloudStorage( param )` | 获取自己和好友的云存储数据 |
| 本地文件系统 | `wx.getFileSystemManager()` | `WX.GetFileSystemManager()` | 替代System.IO，读写用户文件 |
| 用户数据路径 | `wx.env.USER_DATA_PATH` | `WX.env.USER_DATA_PATH` | 可读写的用户目录 |

### 1.6 开放数据域（排行榜）

| 功能 | JS API | Unity C# 调用 | 说明 |
|------|--------|--------------|------|
| 获取开放数据域 | `wx.getOpenDataContext()` | `WX.GetOpenDataContext()` | 用于排行榜 |
| 向开放域发消息 | `openDataContext.postMessage()` | 对应方法 | 主域→开放域通信 |

### 1.7 其他核心功能

| 功能 | JS API | Unity C# 调用 | 说明 | 代码已完成 |
|------|--------|--------------|------|----------|
| 振动（短） | `wx.vibrateShort()` | `WX.VibrateShort( param )` | type: light/medium/heavy | ✅ |
| 振动（长） | `wx.vibrateLong()` | `WX.VibrateLong()` | 400ms长振动 | ✅ |
| 系统信息 | `wx.getSystemInfoSync()` | `WX.GetSystemInfoSync()` | 设备/窗口/微信版本等 | ❌ |
| 生命周期 | `wx.onShow()` / `wx.onHide()` | `WX.OnShow()` / `WX.OnHide()` | 前台/后台切换监听 | ❌ |
| 版本更新 | `wx.getUpdateManager()` | `WX.GetUpdateManager()` | 小游戏热更新管理 | ❌ |
| 复制到剪贴板 | `wx.setClipboardData()` | `WX.SetClipboardData()` | 复制文字 | ❌ |
| 订阅消息 | `wx.requestSubscribeMessage()` | `WX.RequestSubscribeMessage()` | 模板消息订阅 | ❌ |
| 客服消息 | `wx.openCustomerServiceConversation()` | 对应方法 | 打开客服会话 | ❌ |
| 收藏 | `wx.addFavorite()` | 待确认 | 添加到收藏 | ❌ |
| 跳转其他小程序 | `wx.navigateToMiniProgram()` | `WX.NavigateToMiniProgram()` | 跳转其他微信小程序 | ❌ |

---

## 二、抖音小游戏 SDK (TTSDK)

**官方文档**: [抖音小游戏开发文档](https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/bytedance-mini-game)  
**Unity C# SDK**: `TTSDK` 命名空间（TT类）  
**编译宏**: `UNITY_DOUYINMINIGAME`  
**特别提醒**: 旧版StarkSDK已废弃，新项目统一使用TTSDK

### 2.1 登录与用户信息

| 功能 | JS API | Unity C# (TTSDK) 调用 | 说明 |
|------|--------|----------------------|------|
| 抖音登录 | `tt.login()` | `TT.Login( callback )` | 获取临时code |
| 检查登录态 | `tt.checkSession()` | `TT.CheckSession( callback )` | 验证session是否过期 |
| 获取用户信息 | `tt.getUserInfo()` | `TT.GetUserInfo( callback )` | 获取头像昵称 |
| 用户授权设置 | `tt.getSetting()` | `TT.GetSetting( callback )` | 已授权权限查询 |
| 发起授权 | `tt.authorize()` | `TT.Authorize( param )` | 提前请求权限 |

### 2.2 广告

| 功能 | JS API | Unity C# (TTSDK) 调用 | 说明 | 代码已完成 |
|------|--------|----------------------|------|----------|
| 激励视频广告 | `tt.createRewardedVideoAd()` | `TT.CreateRewardedVideoAd( param )` | 支持再得广告模式(multiton) | ✅ |
| 插屏广告 | `tt.createInterstitialAd()` | 待确认 | 全屏插屏广告 | ❌ |
| Banner广告 | `tt.createBannerAd()` | 待确认 | 横幅广告 | ❌ |
| 原生模板广告 | `tt.createCustomAd()` | 待确认 | 自定义广告 | ❌ |

### 2.3 分享与社交

| 功能 | JS API | Unity C# (TTSDK) 调用 | 说明 | 代码已完成 |
|------|--------|----------------------|------|----------|
| 主动分享 | `tt.shareAppMessage()` | `TT.ShareAppMessage( param )` | 支持channel参数（好友/邀请等） | ✅ |
| 被动分享 | `tt.onShareAppMessage()` | `TT.OnShareAppMessage( callback )` | 右上角分享 | ❌ |
| 分享视频 | `tt.shareVideo()` | 待确认 | 视频分享 | ❌ |

### 2.4 支付

| 功能 | JS API | Unity C# (TTSDK) 调用 | 说明 |
|------|--------|----------------------|------|
| 虚拟支付 | `tt.requestGamePayment()` | `TT.RequestGamePayment( param )` | 抖音担保支付，**需要版号** |
| 支付查询 | 服务端API | 服务端调用 | 订单状态查询 |

### 2.5 平台特色功能（抖音必接！）

| 功能 | JS API | Unity C# 调用 | 说明 | 强制性 |
|------|--------|--------------|------|--------|
| **侧边栏复访** | `tt.navigateToScene()` | `TT.NavigateToScene( param )` | 抖音必接！引导用户从侧边栏回访 | ✅ 必接 |
| **录屏功能** | `tt.startRecord()` / `tt.stopRecord()` | `TT.StartRecord()` / `TT.StopRecord()` | Android必接，UCG分发需要 | ✅ Android必接 |
| 录屏分享 | `tt.shareAppMessage()` 配合录屏 | 录制后分享 | 录屏+分享一体 | 推荐 |

### 2.6 数据存储

| 功能 | JS API | Unity C# 调用 | 说明 |
|------|--------|--------------|------|
| 云存储 | `tt.setUserCloudStorage()` | `TT.SetUserCloudStorage()` | 排行榜数据存储 |
| 本地缓存 | `tt.setStorageSync()` / `tt.getStorageSync()` | `TT.SetStorageSync()` / `TT.GetStorageSync()` | 同步本地KV存储 |

### 2.7 其他核心功能

| 功能 | JS API | Unity C# (TTSDK) 调用 | 说明 | 代码已完成 |
|------|--------|----------------------|------|----------|
| 初始化SDK | 自动 | `TT.InitSDK( callback )` | 小游戏启动时自动初始化 | ❌ |
| 系统信息 | `tt.getSystemInfoSync()` | `TT.GetSystemInfoSync()` | 设备/宿主信息 | ❌ |
| 振动短 | `tt.vibrateShort()` | `TT.VibrateShort( param )` | light/medium/heavy | ✅ |
| 振动长 | `tt.vibrateLong()` | `TT.VibrateLong()` | 400ms | ✅ |
| 生命周期 | `tt.onShow()` / `tt.onHide()` | `TT.OnShow()` / `TT.OnHide()` | 需手动监听 | ❌ |
| 启动参数 | `tt.getLaunchOptionsSync()` | `TT.GetLaunchOptionsSync()` | 获取启动场景参数 | ❌ |
| 订阅消息 | `tt.requestSubscribeMessage()` | 待确认 | 模板消息 | ❌ |
| 剪贴板 | `tt.setClipboardData()` | `TT.SetClipboardData()` | 复制文字 | ❌ |
| 跳转抖音号 | 待确认 | `TT.FollowDouyinUserProfile()` | 关注抖音号 | ❌ |
| 抖音云 | 抖音云服务 | 抖音云C# SDK | 云端数据/云函数 | ❌ |

---

## 三、快手小游戏 SDK (KS)

**官方文档**: [快手小游戏开发文档](https://ks-game-docs.kuaishou.com/minigame/start/start.html)  
**Unity IG (Instant Game) SDK**: 快手平台专用 Unity 插件包 (`com.kwai.mini.game`)  
**编译宏**: `UNITY_KUAISHOUMINIGAME`  
**注意**: 快手Unity小游戏目前主要支持 **Android** 平台

### 3.1 登录与用户信息

| 功能 | JS API | Unity/KS API | 说明 |
|------|--------|-------------|------|
| 快手登录 | `ks.login()` | `KS.Login( callback )` | 获取code，服务端换openid |
| 检查登录态 | `ks.checkSession()` | `KS.CheckSession( callback )` | 验证session有效性 |
| 用户信息授权 | `ks.authorize()` + `ks.getUserInfo()` | `KS.AuthorizeGetUserInfo( callback )` | 授权后获取用户信息 |

### 3.2 广告

| 功能 | JS API | Unity/KS API | 说明 | 代码已完成 |
|------|--------|-------------|------|----------|
| 激励视频广告 | `ks.createRewardedVideoAd()` | `KS.CreateRewardedVideoAd( adUnitId )` | 全局单例模式 | ❌ |
| 插屏广告 | `ks.createInterstitialAd()` | `KS.CreateInterstitialAd( adUnitId )` | 30秒后才能首次展示 | ❌ |
| Banner广告 | `ks.createBannerAd()` | 待确认 | 横幅广告 | ❌ |

### 3.3 分享

| 功能 | JS API | Unity/KS API | 说明 | 代码已完成 |
|------|--------|-------------|------|----------|
| 主动分享 | `ks.shareAppMessage()` | `KS.ShareAppMessage( param )` | 游戏内拉起分享 | ❌ |
| 被动分享 | `ks.onShareAppMessage()` | 待确认 | 右上角分享 | ❌ |

### 3.4 支付

| 功能 | JS API | Unity/KS API | 说明 |
|------|--------|-------------|------|
| 虚拟支付 | `ks.requestGamePayment()` | `KS.RequestGamePayment( param )` | 快手小游戏支付 |

### 3.5 平台特色功能

| 功能 | JS API | Unity/KS API | 说明 | 重要性 |
|------|--------|-------------|------|--------|
| 设为常用 | `ks.addCommonUse()` | `KS.AddCommonUse( callback )` | 引导用户添加常用 | 推荐 |
| 检查常用 | `ks.checkCommonUse()` | `KS.CheckCommonUse( callback )` | 检查是否已设常用 | 推荐 |
| 添加桌面快捷方式 | `ks.addShortcut()` / `ks.checkShortcut()` | `KS.AddShortcut()` / `KS.CheckShortcut()` | Android支持 | 推荐 |
| 侧边栏复访 | `ks.navigateToScene()` | `KS.CheckSliderBarIsAvailable()` / 跳转 | 已备案游戏推荐接入 | 推荐 |

### 3.6 其他功能

| 功能 | JS API | Unity/KS API | 说明 |
|------|--------|-------------|------|
| 系统信息 | `ks.getSystemInfoSync()` | `KS.GetSystemInfoSync()` | 设备和平台信息 |
| 生命周期 | `ks.onShow()` / `ks.onHide()` | 对应方法 | 前台/后台切换 |
| 退出小游戏 | `ks.exitMiniProgram()` | `KS.ExitMiniProgram()` | 退出当前小游戏 |
| 振动 | 待确认 | 待确认 | 设备振动反馈 |
| 剪贴板 | `ks.setClipboardData()` | 待确认 | 复制文字 |
| 启动参数 | `ks.getLaunchOptionsSync()` | `KS.GetLaunchOptionsSync()` | 冷/热启动参数 |

---

## 四、TapTap SDK

**官方文档**: [TapSDK 开发指南 (v4)](https://developer.taptap.io/docs/zh-Hans/sdk/access/quickstart/)  
**Unity包名**: `com.taptap.tds.*` (通过UPM/NPMJS导入)  
**版本**: 当前最新 4.10.2 (v4) / v3系列 (3.30.3)  
**推荐使用**: TapSDK v3 系列 (内建账户体系更完善)

### 4.1 登录与账户

| 功能 | 模块 | Unity API | 说明 | 代码已完成 |
|------|------|-----------|------|----------|
| TapTap登录 | `com.taptap.tds.login` | `TapLogin.Login()` | OAuth授权登录 | ❌ (TODO) |
| 内建账户 | `com.taptap.tds.bootstrap` | TDSUser 体系 | 统一账户系统，支持绑定多平台 | ❌ |
| 登出 | 同上 | `TapLogin.Logout()` | 退出登录 | ❌ |

### 4.2 合规认证（防沉迷）

| 功能 | 模块 | Unity API | 说明 | 重要性 |
|------|------|-----------|------|--------|
| 合规认证 | `com.tapsdk.antiaddiction` | `AntiAddiction.StartUp( clientId )` | 实名认证+防沉迷+时长限制 | ✅ 中国大陆必接 |
| 年龄限制 | 同上 | 回调code=1100处理 | 可配置最低年龄 | 可选 |

**回调code说明**:
- `500`: 玩家未受限制，正常进入
- `1000`: 认证凭证无效
- `1001`: 触发时长限制，点击切换账号
- `1100`: 触发年龄限制
- `1200`: 数据请求失败
- `9002`: 实名认证窗口被关闭

### 4.3 成就系统

| 功能 | 模块 | Unity API | 说明 |
|------|------|-----------|------|
| 初始化成就 | `com.taptap.tds.achievement` | `TapAchievement.Init()` | 依赖内建账户 |
| 解锁成就 | 同上 | `TapAchievement.Grow()` / `Reach()` / `Unlock()` | 三种解锁方式 |
| 显示成就面板 | 同上 | `TapAchievement.ShowAchievementPage()` | 游戏内展示成就页 |

### 4.4 排行榜

| 功能 | 模块 | Unity API | 说明 |
|------|------|-----------|------|
| 排行榜 | `com.taptap.tds.leaderboard` | `TapLeaderboard` 相关API | 基于内建账户的好友/全服排行 |
| 提交分数 | 同上 | 提交分数接口 | 更新排行数据 |
| 查询排行 | 同上 | 查询排行数据 | 获取Top N列表 |

### 4.5 其他服务

| 功能 | 模块 | 说明 |
|------|------|------|
| 内嵌动态(Moments) | `com.taptap.tds.moment` | 游戏内嵌社区动态，图文视频分享 |
| 好友系统 | `com.taptap.tds.friends` | 基于内建账户的好友关系 |
| TapDB数据分析 | `com.taptap.tds.tapdb` | 用户行为数据分析 |
| 云存档 | LeanCloud Storage | 基于LeanCloud的云端存档 |

---

## 五、WebGL / Web 平台

**平台特征**: 无原生SDK支持，纯Web环境  
**编译宏**: `UNITY_WEBGL`

### 5.1 功能方案

| 功能 | 方案 | 实现方式 | 说明 |
|------|------|---------|------|
| 登录 | 游客登录 / JWT | 本地生成UUID + 可选后端绑定 | 无需第三方SDK |
| 广告 | Google AdSense / 第三方广告SDK | JS桥接调用 | 需对接网页广告商 |
| 分享 | Web Share API | `navigator.share()` | 浏览器原生分享，兼容性有限 |
| 本地存储 | IndexedDB / PlayerPrefs | Unity内置PlayerPrefs | 替代System.IO |
| 支付 | Stripe / PayPal / 支付宝网页 | JS桥接 | 需接入支付网关 |
| 社交分享 | 复制链接 + 分享文案 | 剪贴板复制 + 引导 | 通用降级方案 |

### 5.2 限制与注意事项

- 不能使用System.IO，用PlayerPrefs/IndexedDB替代
- 网络请求需注意CORS跨域策略
- 微信/抖音等平台的纯Web版本功能远弱于小游戏版本
- 建议Web平台作为**辅助展示版本**，不追求完整功能

---

## 六、跨平台功能对照总表

| 功能 | 微信小游戏 | 抖音小游戏 | 快手小游戏 | TapTap | Web | Native |
|------|:---------:|:---------:|:---------:|:-----:|:---:|:------:|
| **登录** | ✅ wx.login | ✅ tt.login | ✅ ks.login | ✅ TapLogin | ⚠️ 游客 | ✅ 平台SDK |
| **激励视频广告** | ✅ 已实现 | ✅ 已实现 | ❌ 待接入 | ❌ | ❌ | ❌ |
| **插屏广告** | ✅ 已实现 | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ❌ |
| **Banner广告** | ❌ 待接入 | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ❌ |
| **主动分享** | ✅ 已实现 | ✅ 已实现 | ❌ 待接入 | ❌ | ⚠️ Web API | ❌ |
| **被动分享(右上角)** | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ❌ | ❌ |
| **虚拟支付** | ❌ 待接入 | ❌ 需版号 | ❌ 待接入 | ❌ | ❌ | ⚠️ |
| **云存储/存档** | ❌ 待接入 | ❌ 待接入 | ❌ | ✅ TDS | ⚠️ IndexedDB | ❌ |
| **排行榜** | ❌ 待接入 | ❌ 待接入 | ❌ | ✅ | ❌ | ❌ |
| **振动反馈** | ✅ 已实现 | ✅ 已实现 | ❌ | ❌ | ❌ | ✅ |
| **合规认证(防沉迷)** | ✅ 平台自带 | ✅ 平台自带 | ✅ 平台自带 | ✅ AntiAddiction | ❌ | ⚠️ |
| **侧边栏复访** | ❌ | ✅ 抖音必接 | ⚠️ 推荐 | ❌ | ❌ | ❌ |
| **录屏** | ❌ | ✅ Android必接 | ❌ | ❌ | ❌ | ❌ |
| **添加到桌面** | ❌ | ❌ | ✅ KS | ❌ | ❌ | ❌ |
| **设为常用** | ❌ | ❌ | ✅ KS | ❌ | ❌ | ❌ |
| **成就系统** | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| **好友系统** | ❌ | ❌ | ❌ | ✅ TDS | ❌ | ❌ |
| **订阅消息** | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ❌ | ✅ |
| **生命周期监听** | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ❌ | ✅ |
| **系统信息** | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ✅ | ✅ |
| **剪贴板** | ❌ 待接入 | ❌ 待接入 | ❌ | ❌ | ✅ | ✅ |
| **版本更新管理** | ❌ 待接入 | ❌ | ❌ | ❌ | ❌ | ❌ |

> ✅ 已实现 | ⚠️ 部分实现/有条件 | ❌ 未实现/待接入

---

## 七、PlatformManager.cs 完成度分析

### 7.1 已完成功能

| 功能 | 代码位置 | 覆盖平台 |
|------|---------|---------|
| 平台检测 | `DetectPlatform()` L99-118 | 全部7个平台 |
| 激励视频广告 | `ShowRewardedAd()` L120-145 | WeChat, Douyin |
| 插屏广告 | `ShowInterstitialAd()` L201-226 | WeChat only |
| 主动分享 | `Share()` L228-282 | WeChat, Douyin |
| 短振动 | `VibrateShort()` L284-295 | WeChat, Douyin, Native |
| 长振动 | `VibrateLong()` L297-306 | WeChat, Douyin |
| 退出游戏 | `QuitGame()` L308-323 | MiniGame/Editor/Native |
| 游客登录 | `GuestLogin()` L51-55 | 全部 |
| 平台名称 | `GetPlatformName()` L24-35 | 全部 |

### 7.2 标记TODO但未实现

| 功能 | 代码位置 | 说明 |
|------|---------|------|
| TapTap登录 | `TapTapLogin()` L61-68 | `// TODO: 接入TapTap SDK登录` |
| 微信登录 | `WechatLogin()` L75-77 | `// TODO: 接入微信小程序SDK登录` |
| 抖音登录 | `DouyinLogin()` L89-91 | `// TODO: 接入抖音小程序SDK登录` |

### 7.3 平台覆盖缺口（优先级排序）

**🔴 高优先级（核心收益/平台准入）**:
1. **抖音侧边栏复访** — 抖音必接，不通则无法上架
2. **抖音录屏功能** — Android必接
3. **微信/抖音登录** — 影响用户留存和数据追踪
4. **快手激励视频广告** — 变现基础
5. **TapTap合规认证** — 国内上架TapTap的合规要求

**🟡 中优先级（用户体验/变现增强）**:
6. **快手分享** — 裂变获客
7. **各平台Banner广告** — 补充变现
8. **微信/抖音被动分享(右上角)** — 提高分享率
9. **各平台生命周期监听** — 正确处理前后台切换
10. **TapTap成就/排行榜** — TapTap平台特色，加分项

**🟢 低优先级（锦上添花）**:
11. **快手侧边栏/添加桌面/设为常用** — 提升留存
12. **订阅消息** — 用户召回
13. **版本更新管理** — 微信有热更新需求时
14. **云存储/存档** — 多设备同步
15. **支付系统** — 需版号，且当前无内购设计

### 7.4 快手平台完全空白

当前PlatformManager中快手平台仅完成了**平台检测**（#elif UNITY_KUAISHOUMINIGAME），以下功能全部缺失：
- 登录 (KS.Login)
- 激励视频广告 (KS.CreateRewardedVideoAd)
- 分享 (KS.ShareAppMessage)
- 插屏广告
- 所有其他平台功能

---

## 八、建议接入顺序（按阶段）

### 阶段一：平台准入必备（立即启动）
1. **抖音侧边栏复访** → `TT.NavigateToScene()`
2. **抖音录屏** → `TT.StartRecord()` / `TT.StopRecord()`
3. **TapTap合规认证** → 接入AntiAddiction模块

### 阶段二：用户体系完善
4. **微信登录** → `WX.Login()` + 服务端code2session
5. **抖音登录** → `TT.Login()` + 服务端code2session
6. **TapTap登录** → `TapLogin.Login()` + TDSUser

### 阶段三：变现能力补齐
7. **快手激励视频广告** → `KS.CreateRewardedVideoAd()`
8. **各平台Banner广告接入**
9. **抖音插屏广告**

### 阶段四：社交裂变增强
10. **快手分享** → `KS.ShareAppMessage()`
11. **被动分享(右上角菜单)** 各平台
12. **快手设为常用/添加桌面**

### 阶段五：平台特色与长期运营
13. **TapTap成就/排行榜** → 游戏内成就与排行
14. **云存储/存档** → 跨设备同步
15. **订阅消息** → 用户召回
16. **支付** → 如需内购

---

## 九、官方文档链接汇总

| 平台 | 文档地址 |
|------|---------|
| 微信小游戏 | https://developers.weixin.qq.com/minigame/dev/api/ |
| 微信Unity插件 | https://game.weixin.qq.com/cgi-bin/gamewxagwasmsplitwap/getunityplugininfo?download=1 |
| 抖音小游戏(JS API) | https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/bytedance-mini-game |
| 抖音Unity C# API | https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/game-engine/rd-to-SCgame/c-api/api-overview |
| 抖音TTSDK迁移 | https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/guide/game-engine/rd-to-SCgame/unity-game-access/starksdkmigrationguide/starksdk-migration-guide |
| 快手小游戏 | https://ks-game-docs.kuaishou.com/minigame/start/start.html |
| 快手Unity SDK | https://docs.qingque.cn/d/home/eZQBKPPXInAfG7EKhBVmCzYhC |
| TapTap SDK | https://developer.taptap.io/docs/zh-Hans/sdk/access/quickstart/ |
| TapTap合规认证 | https://developer.taptap.cn/docs/v3/sdk/anti-addiction/practice/ |
| TapTap内建账户 | https://developer.taptap.cn/docs/v3/sdk/authentication/guide/ |
| TapTap成就 | https://developer.taptap.cn/docs/v3/sdk/achievement/guide/ |