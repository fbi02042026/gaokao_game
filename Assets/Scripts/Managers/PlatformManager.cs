using System;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }

    public enum GamePlatform
    {
        WeChat,
        Douyin,
        KuaiShou,
        MiniHost,
        TapTap,
        Web,
        Native
    }

    public GamePlatform CurrentPlatform { get; private set; }
    public bool IsMiniGame => CurrentPlatform == GamePlatform.WeChat || CurrentPlatform == GamePlatform.Douyin || CurrentPlatform == GamePlatform.KuaiShou || CurrentPlatform == GamePlatform.MiniHost;

    public Action OnLoginSuccess;
    public Action OnLogout;
    public Action OnGameResume;
    public Action OnGamePause;

    public string UserId { get; private set; }
    public string PlatformUserId { get; private set; }
    public string NickName { get; private set; }
    public string AvatarUrl { get; private set; }
    public bool IsLoggedIn { get; private set; }

    private const string TAPTAP_CLIENT_ID = "YOUR_TAPTAP_CLIENT_ID";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DetectPlatform();
            RegisterLifecycleEvents();
            AutoLogin();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetPlatformName()
    {
        return CurrentPlatform switch
        {
            GamePlatform.WeChat => "wechat",
            GamePlatform.Douyin => "douyin",
            GamePlatform.KuaiShou => "kuaishou",
            GamePlatform.TapTap => "taptap",
            GamePlatform.Web => "web",
            _ => "native"
        };
    }

    void DetectPlatform()
    {
#if UNITY_WEIXINMINIGAME
        CurrentPlatform = GamePlatform.WeChat;
#elif UNITY_DOUYINMINIGAME
        CurrentPlatform = GamePlatform.Douyin;
#elif UNITY_KUAISHOUMINIGAME
        CurrentPlatform = GamePlatform.KuaiShou;
#elif UNITY_MINIHOST
        CurrentPlatform = GamePlatform.MiniHost;
#elif UNITY_WEBGL
        CurrentPlatform = GamePlatform.Web;
#elif UNITY_ANDROID || UNITY_IOS
        CurrentPlatform = GamePlatform.Native;
#else
        CurrentPlatform = GamePlatform.Native;
#endif
        Debug.Log($"[PlatformManager] 检测到平台: {CurrentPlatform}, 小游戏模式: {IsMiniGame}");
    }

    void RegisterLifecycleEvents()
    {
        try
        {
#if UNITY_WEIXINMINIGAME
            WeChatWASM.WX.OnShow(() =>
            {
                Debug.Log("[PlatformManager] 游戏回到前台");
                OnGameResume?.Invoke();
            });
            WeChatWASM.WX.OnHide(() =>
            {
                Debug.Log("[PlatformManager] 游戏进入后台");
                OnGamePause?.Invoke();
            });
#elif UNITY_DOUYINMINIGAME
            TTSDK.TT.OnShow(() =>
            {
                Debug.Log("[PlatformManager] 游戏回到前台");
                OnGameResume?.Invoke();
            });
            TTSDK.TT.OnHide(() =>
            {
                Debug.Log("[PlatformManager] 游戏进入后台");
                OnGamePause?.Invoke();
            });
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 生命周期注册失败: {e.Message}");
        }
    }

    void AutoLogin()
    {
        if (IsMiniGame)
        {
            switch (CurrentPlatform)
            {
                case GamePlatform.WeChat: WechatLogin(); break;
                case GamePlatform.Douyin: DouyinLogin(); break;
                case GamePlatform.KuaiShou: KuaiShouLogin(); break;
                default: GuestLogin(); break;
            }
        }
        else
        {
            GuestLogin();
        }
    }

    public void GuestLogin()
    {
        UserId = "guest_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        IsLoggedIn = true;
        Debug.Log($"[PlatformManager] 游客登录: {UserId}");
        OnLoginSuccess?.Invoke();
    }

    public void Logout()
    {
        UserId = null;
        PlatformUserId = null;
        NickName = null;
        AvatarUrl = null;
        IsLoggedIn = false;
        Debug.Log("[PlatformManager] 已注销");
        OnLogout?.Invoke();
    }

    public void WechatLogin()
    {
        Debug.Log("[PlatformManager] 微信登录");
#if UNITY_WEIXINMINIGAME
        try
        {
            WeChatWASM.WX.Login(new WeChatWASM.WXLoginOption
            {
                success = (res) =>
                {
                    PlatformUserId = res.code;
                    UserId = "wx_" + Math.Abs(res.code.GetHashCode()).ToString("X8");
                    IsLoggedIn = true;
                    Debug.Log($"[PlatformManager] 微信登录成功: {UserId}");
                    FetchWeChatUserInfo();
                    OnLoginSuccess?.Invoke();
                },
                fail = (res) =>
                {
                    Debug.LogWarning($"[PlatformManager] 微信登录失败: {res.errMsg}, 降级游客");
                    GuestLogin();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 微信登录异常: {e.Message}, 降级游客");
            GuestLogin();
        }
#else
        GuestLogin();
#endif
    }

#if UNITY_WEIXINMINIGAME
    private void FetchWeChatUserInfo()
    {
        try
        {
            WeChatWASM.WX.GetUserInfo(new WeChatWASM.WXGetUserInfoOption
            {
                success = (res) =>
                {
                    NickName = res.userInfo.nickName;
                    AvatarUrl = res.userInfo.avatarUrl;
                    Debug.Log($"[PlatformManager] 微信用户信息: {NickName}");
                },
                fail = (res) =>
                {
                    Debug.LogWarning($"[PlatformManager] 获取微信用户信息失败: {res.errMsg}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 获取微信用户信息异常: {e.Message}");
        }
    }
#endif

    public void DouyinLogin()
    {
        Debug.Log("[PlatformManager] 抖音登录");
#if UNITY_DOUYINMINIGAME
        try
        {
            TTSDK.TT.Login(new TTSDK.TTLoginOption
            {
                success = (res) =>
                {
                    PlatformUserId = res.code;
                    UserId = "dy_" + Math.Abs(res.code.GetHashCode()).ToString("X8");
                    IsLoggedIn = true;
                    Debug.Log($"[PlatformManager] 抖音登录成功: {UserId}");
                    FetchDouyinUserInfo();
                    OnLoginSuccess?.Invoke();
                },
                fail = (res) =>
                {
                    Debug.LogWarning($"[PlatformManager] 抖音登录失败: {res.errMsg}, 降级游客");
                    GuestLogin();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 抖音登录异常: {e.Message}, 降级游客");
            GuestLogin();
        }
#else
        GuestLogin();
#endif
    }

#if UNITY_DOUYINMINIGAME
    private void FetchDouyinUserInfo()
    {
        try
        {
            TTSDK.TT.GetUserInfo(new TTSDK.TTGetUserInfoOption
            {
                success = (res) =>
                {
                    NickName = res.userInfo.nickName;
                    AvatarUrl = res.userInfo.avatarUrl;
                    Debug.Log($"[PlatformManager] 抖音用户信息: {NickName}");
                },
                fail = (res) =>
                {
                    Debug.LogWarning($"[PlatformManager] 获取抖音用户信息失败: {res.errMsg}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 获取抖音用户信息异常: {e.Message}");
        }
    }
#endif

    public void KuaiShouLogin()
    {
        Debug.Log("[PlatformManager] 快手登录");
#if UNITY_KUAISHOUMINIGAME
        try
        {
            KS.Login((result) =>
            {
                if (result != null)
                {
                    PlatformUserId = result.code;
                    UserId = "ks_" + Math.Abs(result.code.GetHashCode()).ToString("X8");
                    IsLoggedIn = true;
                    Debug.Log($"[PlatformManager] 快手登录成功: {UserId}");
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    Debug.LogWarning("[PlatformManager] 快手登录失败, 降级游客");
                    GuestLogin();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 快手登录异常: {e.Message}, 降级游客");
            GuestLogin();
        }
#else
        GuestLogin();
#endif
    }

    public void TapTapLogin()
    {
        Debug.Log("[PlatformManager] TapTap登录");
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            TapTap.Login.TapLogin.Init(TAPTAP_CLIENT_ID);
            TapTap.Login.TapLogin.Login().ContinueWith(task =>
            {
                if (task.IsFaulted || task.Result == null)
                {
                    Debug.LogWarning($"[PlatformManager] TapTap登录失败, 降级游客");
                    GuestLogin();
                    return;
                }
                var token = task.Result;
                TapTap.Login.TapLogin.FetchProfile().ContinueWith(profileTask =>
                {
                    if (profileTask.IsFaulted || profileTask.Result == null)
                    {
                        PlatformUserId = token.kid;
                        UserId = "tt_" + (token.kid?.Substring(0, Math.Min(8, token.kid?.Length ?? 0)) ?? "guest");
                        NickName = "TapTap玩家";
                    }
                    else
                    {
                        var profile = profileTask.Result;
                        PlatformUserId = profile.openid;
                        UserId = "tt_" + (profile.openid?.Substring(0, Math.Min(8, profile.openid?.Length ?? 0)) ?? "guest");
                        NickName = profile.name ?? "TapTap玩家";
                        AvatarUrl = profile.avatar;
                    }
                    IsLoggedIn = true;
                    Debug.Log($"[PlatformManager] TapTap登录成功: {UserId}");
                    OnLoginSuccess?.Invoke();
                });
            });
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] TapTap登录异常: {e.Message}, 降级游客");
            GuestLogin();
        }
#else
        GuestLogin();
#endif
    }

    public void ShowRewardedAd(string placementId, Action<bool> callback)
    {
        if (!IsMiniGame)
        {
            Debug.Log($"[PlatformManager] 编辑器模拟广告完成: {placementId}");
            callback?.Invoke(true);
            return;
        }

        switch (CurrentPlatform)
        {
            case GamePlatform.WeChat:
#if UNITY_WEIXINMINIGAME
                ShowWeChatRewardedAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.Douyin:
#if UNITY_DOUYINMINIGAME
                ShowDouyinRewardedAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.KuaiShou:
#if UNITY_KUAISHOUMINIGAME
                ShowKuaiShouRewardedAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            default:
                callback?.Invoke(true);
                break;
        }
    }

#if UNITY_WEIXINMINIGAME
    private void ShowWeChatRewardedAd(string placementId, Action<bool> callback)
    {
        try
        {
            var ad = WeChatWASM.WX.CreateRewardedVideoAd(
                new WeChatWASM.WXCreateRewardedVideoAdParam { adUnitId = placementId });
            ad.OnClose((res) =>
            {
                callback?.Invoke(res.isEnded);
                Debug.Log($"[PlatformManager] 微信广告关闭, 完整观看={res.isEnded}");
            });
            ad.OnError((res) =>
            {
                Debug.LogError($"[PlatformManager] 微信广告错误: {res.errMsg}");
                callback?.Invoke(false);
            });
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 微信广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_DOUYINMINIGAME
    private void ShowDouyinRewardedAd(string placementId, Action<bool> callback)
    {
        try
        {
            var param = new TTSDK.CreateRewardedVideoAdParam { AdUnitId = placementId };
            var ad = TTSDK.TT.CreateRewardedVideoAd(param);
            ad.OnClose += (ended, count) =>
            {
                callback?.Invoke(ended);
                Debug.Log($"[PlatformManager] 抖音广告关闭, 完整观看={ended}, 第{count}次");
            };
            ad.OnError += (errorCode, errorMessage) =>
            {
                Debug.LogError($"[PlatformManager] 抖音广告错误: {errorCode} - {errorMessage}");
                callback?.Invoke(false);
            };
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 抖音广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_KUAISHOUMINIGAME
    private void ShowKuaiShouRewardedAd(string placementId, Action<bool> callback)
    {
        try
        {
            var ad = KS.CreateRewardedVideoAd(placementId);
            ad.OnClose((res) =>
            {
                callback?.Invoke(res != null && res.isEnded);
                Debug.Log($"[PlatformManager] 快手广告关闭, 完整观看={res?.isEnded}");
            });
            ad.OnError((code, msg) =>
            {
                Debug.LogError($"[PlatformManager] 快手广告错误: {code} - {msg}");
                callback?.Invoke(false);
            });
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 快手广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

    public void ShowInterstitialAd(string placementId, Action<bool> callback)
    {
        if (!IsMiniGame)
        {
            callback?.Invoke(true);
            return;
        }

        switch (CurrentPlatform)
        {
            case GamePlatform.WeChat:
#if UNITY_WEIXINMINIGAME
                ShowWeChatInterstitialAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.Douyin:
#if UNITY_DOUYINMINIGAME
                ShowDouyinInterstitialAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.KuaiShou:
#if UNITY_KUAISHOUMINIGAME
                ShowKuaiShouInterstitialAd(placementId, callback);
#else
                callback?.Invoke(true);
#endif
                break;
            default:
                callback?.Invoke(true);
                break;
        }
    }

#if UNITY_WEIXINMINIGAME
    private void ShowWeChatInterstitialAd(string placementId, Action<bool> callback)
    {
        try
        {
            var ad = WeChatWASM.WX.CreateInterstitialAd(
                new WeChatWASM.WXCreateInterstitialAdParam { adUnitId = placementId });
            ad.OnClose(() => callback?.Invoke(true));
            ad.OnError((res) =>
            {
                Debug.LogError($"[PlatformManager] 微信插屏广告错误: {res.errMsg}");
                callback?.Invoke(false);
            });
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 微信插屏广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_DOUYINMINIGAME
    private void ShowDouyinInterstitialAd(string placementId, Action<bool> callback)
    {
        try
        {
            var param = new TTSDK.CreateInterstitialAdParam { AdUnitId = placementId };
            var ad = TTSDK.TT.CreateInterstitialAd(param);
            ad.OnClose += () =>
            {
                callback?.Invoke(true);
                Debug.Log("[PlatformManager] 抖音插屏广告关闭");
            };
            ad.OnError += (errorCode, errorMessage) =>
            {
                Debug.LogError($"[PlatformManager] 抖音插屏广告错误: {errorCode} - {errorMessage}");
                callback?.Invoke(false);
            };
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 抖音插屏广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_KUAISHOUMINIGAME
    private void ShowKuaiShouInterstitialAd(string placementId, Action<bool> callback)
    {
        try
        {
            var ad = KS.CreateInterstitialAd(placementId);
            ad.OnClose(() =>
            {
                callback?.Invoke(true);
                Debug.Log("[PlatformManager] 快手插屏广告关闭");
            });
            ad.OnError((err) =>
            {
                Debug.LogError($"[PlatformManager] 快手插屏广告错误: {err}");
                callback?.Invoke(false);
            });
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 快手插屏广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

    public void ShowBannerAd(string placementId, string position, Action<bool> callback)
    {
        if (!IsMiniGame)
        {
            Debug.Log($"[PlatformManager] 编辑器模拟Banner广告: {placementId}");
            callback?.Invoke(true);
            return;
        }

        switch (CurrentPlatform)
        {
            case GamePlatform.WeChat:
#if UNITY_WEIXINMINIGAME
                ShowWeChatBannerAd(placementId, position, callback);
#else
                callback?.Invoke(false);
#endif
                break;
            case GamePlatform.Douyin:
#if UNITY_DOUYINMINIGAME
                ShowDouyinBannerAd(placementId, position, callback);
#else
                callback?.Invoke(false);
#endif
                break;
            case GamePlatform.KuaiShou:
#if UNITY_KUAISHOUMINIGAME
                ShowKuaiShouBannerAd(placementId, position, callback);
#else
                callback?.Invoke(false);
#endif
                break;
            default:
                callback?.Invoke(false);
                break;
        }
    }

#if UNITY_WEIXINMINIGAME
    private WeChatWASM.WXBannerAd _wxBannerAd;
    private void ShowWeChatBannerAd(string placementId, string position, Action<bool> callback)
    {
        try
        {
            _wxBannerAd = WeChatWASM.WX.CreateBannerAd(
                new WeChatWASM.WXCreateBannerAdParam
                {
                    adUnitId = placementId,
                    adIntervals = 30,
                    style = new WeChatWASM.WXStyle { left = 0, top = 0, width = 320 }
                });
            _wxBannerAd.OnLoad(() =>
            {
                Debug.Log("[PlatformManager] 微信Banner广告加载成功");
                callback?.Invoke(true);
            });
            _wxBannerAd.OnError((res) =>
            {
                Debug.LogError($"[PlatformManager] 微信Banner广告错误: {res.errMsg}");
                callback?.Invoke(false);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 微信Banner广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_DOUYINMINIGAME
    private object _dyBannerAd;
    private void ShowDouyinBannerAd(string placementId, string position, Action<bool> callback)
    {
        try
        {
            var param = new TTSDK.CreateBannerAdParam { AdUnitId = placementId };
            _dyBannerAd = TTSDK.TT.CreateBannerAd(param);
            callback?.Invoke(true);
            Debug.Log("[PlatformManager] 抖音Banner广告创建成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 抖音Banner广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

#if UNITY_KUAISHOUMINIGAME
    private object _ksBannerAd;
    private void ShowKuaiShouBannerAd(string placementId, string position, Action<bool> callback)
    {
        try
        {
            _ksBannerAd = KS.CreateBannerAd(placementId);
            callback?.Invoke(true);
            Debug.Log("[PlatformManager] 快手Banner广告创建成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 快手Banner广告异常: {e.Message}");
            callback?.Invoke(false);
        }
    }
#endif

    public void HideBannerAd()
    {
        try
        {
#if UNITY_WEIXINMINIGAME
            _wxBannerAd?.Hide();
#elif UNITY_DOUYINMINIGAME
            (_dyBannerAd as dynamic)?.Hide();
#elif UNITY_KUAISHOUMINIGAME
            (_ksBannerAd as dynamic)?.Hide();
#endif
            Debug.Log("[PlatformManager] Banner广告已隐藏");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 隐藏Banner广告失败: {e.Message}");
        }
    }

    public void Share(string title, string imageUrl, Action<bool> callback)
    {
        if (!IsMiniGame)
        {
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = title;
#endif
            Debug.Log($"[PlatformManager] 编辑器模拟分享: {title}");
            callback?.Invoke(true);
            return;
        }

        switch (CurrentPlatform)
        {
            case GamePlatform.WeChat:
#if UNITY_WEIXINMINIGAME
                try
                {
                    WeChatWASM.WX.ShareAppMessage(new WeChatWASM.WXShareAppMessageParam
                    {
                        title = title,
                        imageUrl = imageUrl
                    });
                    callback?.Invoke(true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PlatformManager] 微信分享异常: {e.Message}");
                    callback?.Invoke(false);
                }
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.Douyin:
#if UNITY_DOUYINMINIGAME
                try
                {
                    TTSDK.TT.ShareAppMessage(new TTSDK.ShareAppMessageParam
                    {
                        title = title,
                        imageUrl = imageUrl
                    });
                    callback?.Invoke(true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PlatformManager] 抖音分享异常: {e.Message}");
                    callback?.Invoke(false);
                }
#else
                callback?.Invoke(true);
#endif
                break;
            case GamePlatform.KuaiShou:
#if UNITY_KUAISHOUMINIGAME
                try
                {
                    KS.ShareAppMessage(new { title = title, imageUrl = imageUrl });
                    callback?.Invoke(true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PlatformManager] 快手分享异常: {e.Message}");
                    callback?.Invoke(false);
                }
#else
                callback?.Invoke(true);
#endif
                break;
            default:
                callback?.Invoke(true);
                break;
        }
    }

    public void RegisterPassiveShare(string title, string imageUrl)
    {
        try
        {
#if UNITY_WEIXINMINIGAME
            WeChatWASM.WX.ShowShareMenu(new WeChatWASM.WXShowShareMenuParam
            {
                withShareTicket = true,
                menus = new string[] { "shareAppMessage", "shareTimeline" }
            });
            WeChatWASM.WX.OnShareAppMessage(() =>
            {
                return new WeChatWASM.WXShareAppMessageParam
                {
                    title = title,
                    imageUrl = imageUrl
                };
            });
            WeChatWASM.WX.OnShareTimeline(() =>
            {
                return new WeChatWASM.WXShareTimelineParam
                {
                    title = title,
                    imageUrl = imageUrl
                };
            });
            Debug.Log("[PlatformManager] 微信被动分享已注册");
#elif UNITY_DOUYINMINIGAME
            TTSDK.TT.OnShareAppMessage(() =>
            {
                return new TTSDK.ShareAppMessageParam
                {
                    title = title,
                    imageUrl = imageUrl
                };
            });
            Debug.Log("[PlatformManager] 抖音被动分享已注册");
#elif UNITY_KUAISHOUMINIGAME
            KS.OnShareAppMessage(() =>
            {
                return new { title = title, imageUrl = imageUrl };
            });
            Debug.Log("[PlatformManager] 快手被动分享已注册");
#endif
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] 注册被动分享失败: {e.Message}");
        }
    }

    public void NavigateToSidebar()
    {
#if UNITY_DOUYINMINIGAME
        try
        {
            TTSDK.TT.NavigateToScene(new TTSDK.NavigateToSceneParam
            {
                scene = "sidebar"
            });
            Debug.Log("[PlatformManager] 跳转抖音侧边栏");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 跳转侧边栏失败: {e.Message}");
        }
#else
        Debug.Log("[PlatformManager] 侧边栏复访仅支持抖音平台");
#endif
    }

    public void StartRecord()
    {
#if UNITY_DOUYINMINIGAME
        try
        {
            TTSDK.TT.StartRecord(new TTSDK.StartRecordParam());
            Debug.Log("[PlatformManager] 开始录屏");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 开始录屏失败: {e.Message}");
        }
#else
        Debug.Log("[PlatformManager] 录屏仅支持抖音平台");
#endif
    }

    public void StopRecord(Action<bool> callback)
    {
#if UNITY_DOUYINMINIGAME
        try
        {
            TTSDK.TT.StopRecord(new TTSDK.StopRecordParam
            {
                success = () =>
                {
                    Debug.Log("[PlatformManager] 录屏完成");
                    callback?.Invoke(true);
                },
                fail = (err) =>
                {
                    Debug.LogError($"[PlatformManager] 录屏失败: {err}");
                    callback?.Invoke(false);
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 停止录屏异常: {e.Message}");
            callback?.Invoke(false);
        }
#else
        callback?.Invoke(false);
#endif
    }

    public void StartAntiAddictionCheck(Action<bool> callback)
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            var config = new TapTap.AntiAddiction.Model.AntiAddictionConfig
            {
                gameId = TAPTAP_CLIENT_ID,
                showSwitchAccount = true
            };
            TapTap.AntiAddiction.AntiAddictionUIKit.Init(config);
            TapTap.AntiAddiction.AntiAddictionUIKit.SetAntiAddictionCallback((code, msg) =>
            {
                bool passed = code == 500;
                Debug.Log($"[PlatformManager] 合规认证结果: code={code}, passed={passed}, msg={msg}");
                callback?.Invoke(passed);
            });
            TapTap.AntiAddiction.AntiAddictionUIKit.Startup(PlatformUserId ?? UserId);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PlatformManager] TapTap合规认证异常: {e.Message}");
            callback?.Invoke(true);
        }
#else
        callback?.Invoke(true);
#endif
    }

    public void AddToCommonUse()
    {
#if UNITY_KUAISHOUMINIGAME
        try
        {
            KS.AddCommonUse((result) =>
            {
                Debug.Log("[PlatformManager] 已添加到常用");
            }, (code, msg) =>
            {
                Debug.LogWarning($"[PlatformManager] 添加常用失败: {code} - {msg}");
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 设为常用异常: {e.Message}");
        }
#else
        Debug.Log("[PlatformManager] 设为常用仅支持快手平台");
#endif
    }

    public void CheckCommonUse(Action<bool> callback)
    {
#if UNITY_KUAISHOUMINIGAME
        try
        {
            KS.CheckCommonUse((result) =>
            {
                bool isCommon = result != null && result.isCommonUse;
                Debug.Log($"[PlatformManager] 检查常用结果: {isCommon}");
                callback?.Invoke(isCommon);
            }, (code, msg) =>
            {
                Debug.LogWarning($"[PlatformManager] 检查常用失败: {code} - {msg}");
                callback?.Invoke(false);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 检查常用异常: {e.Message}");
            callback?.Invoke(false);
        }
#else
        callback?.Invoke(false);
#endif
    }

    public void AddShortcut(Action<bool> callback)
    {
#if UNITY_KUAISHOUMINIGAME
        try
        {
            KS.AddShortcut((result) =>
            {
                Debug.Log("[PlatformManager] 添加桌面快捷方式请求已发送");
                callback?.Invoke(result != null && result.success);
            }, (code, msg) =>
            {
                Debug.LogWarning($"[PlatformManager] 添加桌面快捷方式失败: {code} - {msg}");
                callback?.Invoke(false);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 添加桌面异常: {e.Message}");
            callback?.Invoke(false);
        }
#else
        callback?.Invoke(false);
#endif
    }

    public void VibrateShort()
    {
#if UNITY_WEIXINMINIGAME
        try { WeChatWASM.WX.VibrateShort(new WeChatWASM.WXVibrateShortParam { type = "light" }); }
        catch { Handheld.Vibrate(); }
#elif UNITY_DOUYINMINIGAME
        try { TTSDK.TT.VibrateShort(new TTSDK.VibrateShortParam { type = "light" }); }
        catch { Handheld.Vibrate(); }
#elif UNITY_KUAISHOUMINIGAME
        try { Handheld.Vibrate(); }
        catch { }
#else
        Handheld.Vibrate();
#endif
    }

    public void VibrateLong()
    {
#if UNITY_WEIXINMINIGAME
        try { WeChatWASM.WX.VibrateLong(new WeChatWASM.WXVibrateLongParam()); }
        catch { }
#elif UNITY_DOUYINMINIGAME
        try { TTSDK.TT.VibrateLong(new TTSDK.VibrateLongParam()); }
        catch { }
#elif UNITY_KUAISHOUMINIGAME
        try { Handheld.Vibrate(); }
        catch { }
#endif
    }

    public void QuitGame()
    {
        if (IsMiniGame)
        {
            Debug.Log("[PlatformManager] 小游戏平台无退出概念，返回首页");
            if (GameManager.Instance != null)
                GameManager.Instance.ChangePhase(GamePhase.Home);
            return;
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}