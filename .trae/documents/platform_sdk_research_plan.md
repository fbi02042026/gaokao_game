# 各平台SDK搜索与功能整理计划

## 项目背景
- **项目名称**: 我的高考志愿模拟器 V1.0
- **引擎**: 团结引擎 (Tuanjie Engine) 2D URP
- **目标平台**: 微信小游戏、抖音小游戏、快手小游戏、TapTap、Web、Native(Android/iOS)

---

## 研究任务清单

### 1. 微信小游戏 SDK (WeChat Mini Game SDK)
**SDK名称**: `WeChatWASM` (已在代码中使用)
**官方文档入口**: 微信开放文档 → 小游戏

需要整理的功能模块：
- [ ] 登录/授权 (wx.login, wx.getUserInfo, 头像昵称获取)
- [ ] 激励视频广告 (wx.createRewardedVideoAd) — 已有代码
- [ ] 插屏广告 (wx.createInterstitialAd) — 已有代码
- [ ] Banner广告 (wx.createBannerAd)
- [ ] 分享 (wx.shareAppMessage) — 已有代码
- [ ] 支付/虚拟支付 (wx.requestMidasPayment)
- [ ] 云存储/存档 (wx.setUserCloudStorage / wx.getUserCloudStorage)
- [ ] 开放数据域 (排行榜)
- [ ] 订阅消息 / 客服消息
- [ ] 振动反馈 — 已有代码
- [ ] 系统信息获取
- [ ] 短代/激励视频广告组件
- [ ] Unity小游戏SDK转换插件特殊API

### 2. 抖音小游戏 SDK (Douyin Mini Game SDK)
**SDK名称**: `TTSDK` (已在代码中使用)
**官方文档入口**: 抖音开放平台 → 小游戏

需要整理的功能模块：
- [ ] 登录/授权 (tt.login, tt.getUserInfo)
- [ ] 激励视频广告 (tt.createRewardedVideoAd) — 已有代码
- [ ] 插屏广告
- [ ] Banner广告
- [ ] 分享 (tt.shareAppMessage) — 已有代码
- [ ] 支付/虚拟支付
- [ ] 云存储
- [ ] 排行榜/关系链数据
- [ ] 订阅消息
- [ ] 振动反馈 — 已有代码
- [ ] 系统信息
- [ ] 录屏/复玩功能
- [ ] 侧边栏复访
- [ ] Unity小游戏转换插件API

### 3. 快手小游戏 SDK (KuaiShou Mini Game SDK)
**SDK名称**: `KS` (待确认)
**官方文档入口**: 快手开放平台 → 小游戏

需要整理的功能模块：
- [ ] 登录/授权
- [ ] 激励视频广告
- [ ] 插屏广告
- [ ] Banner广告
- [ ] 分享
- [ ] 支付
- [ ] 云存储
- [ ] 排行榜
- [ ] 振动/系统API
- [ ] Unity转换插件特有API

### 4. TapTap SDK
**SDK名称**: `TapTapSDK` (待确认)
**官方文档入口**: TapTap开发者中心

需要整理的功能模块：
- [ ] TapTap登录 (TapTap Login / TDS Auth) — 已有TODO
- [ ] TapTap成就系统
- [ ] TapTap排行榜
- [ ] TapTap数据存储 (TDS)
- [ ] 内嵌动态/社区功能
- [ ] 防沉迷
- [ ] Unity SDK接入方式

### 5. Unity WebGL 平台
需要整理的功能：
- [ ] 网页端登录方式 (游客/JWT等)
- [ ] 网页端广告方案 (Google AdSense / 第三方)
- [ ] 网页端分享 (Web Share API)
- [ ] IndexedDB 本地存储
- [ ] 浏览器兼容性注意事项

---

## 输出格式要求

对于每个平台，输出结构化的功能对比表格，包含以下列：
| 功能类别 | 功能名称 | SDK API | 参数说明 | 回调说明 | 备注 |
|---------|---------|---------|---------|---------|------|

并最终生成一份**跨平台功能对照总表**，方便对比各平台SDK能力差异。

---

## 执行步骤

1. **Step 1**: 搜索微信小游戏SDK文档，整理完整功能清单
2. **Step 2**: 搜索抖音小游戏SDK文档，整理完整功能清单
3. **Step 3**: 搜索快手小游戏SDK文档，整理完整功能清单
4. **Step 4**: 搜索TapTap SDK文档，整理完整功能清单
5. **Step 5**: 整理Unity WebGL平台方案
6. **Step 6**: 生成跨平台功能对照总表
7. **Step 7**: 分析当前PlatformManager.cs的完成度，标注缺失功能