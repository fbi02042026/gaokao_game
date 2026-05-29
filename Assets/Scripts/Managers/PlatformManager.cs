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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DetectPlatform();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GuestLogin()
    {
        Debug.Log("[PlatformManager] 游客登录");
        OnLoginSuccess?.Invoke();
    }

    public void TapTapLogin()
    {
        Debug.Log("[PlatformManager] TapTap登录");
        if (!IsMiniGame && CurrentPlatform == GamePlatform.Native)
        {
            // TODO: 接入TapTap SDK登录
            OnLoginSuccess?.Invoke();
        }
        else
        {
            OnLoginSuccess?.Invoke();
        }
    }

    public void WechatLogin()
    {
        Debug.Log("[PlatformManager] 微信登录");
        if (CurrentPlatform == GamePlatform.WeChat)
        {
            // TODO: 接入微信小程序SDK登录
            OnLoginSuccess?.Invoke();
        }
        else
        {
            OnLoginSuccess?.Invoke();
        }
    }

    public void DouyinLogin()
    {
        Debug.Log("[PlatformManager] 抖音登录");
        if (CurrentPlatform == GamePlatform.Douyin)
        {
            // TODO: 接入抖音小程序SDK登录
            OnLoginSuccess?.Invoke();
        }
        else
        {
            OnLoginSuccess?.Invoke();
        }
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
#endif
                break;
            case GamePlatform.Douyin:
#if UNITY_DOUYINMINIGAME
                ShowDouyinRewardedAd(placementId, callback);
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

    public void ShowInterstitialAd(string placementId, Action<bool> callback)
    {
        if (!IsMiniGame)
        {
            callback?.Invoke(true);
            return;
        }

#if UNITY_WEIXINMINIGAME
        try
        {
            var ad = WeChatWASM.WX.CreateInterstitialAd(
                new WeChatWASM.WXCreateInterstitialAdParam { adUnitId = placementId });
            ad.OnClose(() => callback?.Invoke(true));
            ad.OnError((res) => callback?.Invoke(false));
            ad.Show();
        }
        catch (Exception e)
        {
            Debug.LogError($"[PlatformManager] 微信插屏广告异常: {e.Message}");
            callback?.Invoke(false);
        }
#else
        callback?.Invoke(true);
#endif
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
#endif
                break;
            default:
                callback?.Invoke(true);
                break;
        }
    }

    public void VibrateShort()
    {
#if UNITY_WEIXINMINIGAME
        try { WeChatWASM.WX.VibrateShort(new WeChatWASM.WXVibrateShortParam { type = "light" }); }
        catch { Handheld.Vibrate(); }
#elif UNITY_DOUYINMINIGAME
        try { TTSDK.TT.VibrateShort(new TTSDK.VibrateShortParam { type = "light" }); }
        catch { Handheld.Vibrate(); }
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