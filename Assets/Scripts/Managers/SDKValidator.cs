using UnityEngine;

public class SDKValidator : MonoBehaviour
{
    public static SDKValidator Instance { get; private set; }

    public bool WeChatAvailable { get; private set; }
    public bool DouyinAvailable { get; private set; }
    public bool KuaiShouAvailable { get; private set; }
    public bool TapTapAvailable { get; private set; }
    public bool AllMiniGameSDKsReady { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ValidateAllSDKs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ValidateAllSDKs()
    {
        CheckWeChatSDK();
        CheckDouyinSDK();
        CheckKuaiShouSDK();
        CheckTapTapSDK();

        AllMiniGameSDKsReady = WeChatAvailable && DouyinAvailable && KuaiShouAvailable;

        LogStatus();
    }

    private void CheckWeChatSDK()
    {
#if UNITY_WEIXINMINIGAME
        try
        {
            WeChatWASM.WX.GetSystemInfoSync();
            WeChatAvailable = true;
        }
        catch
        {
            WeChatAvailable = false;
        }
#else
        WeChatAvailable = false;
#endif
    }

    private void CheckDouyinSDK()
    {
#if UNITY_DOUYINMINIGAME
        try
        {
            TTSDK.TT.GetSystemInfoSync();
            DouyinAvailable = true;
        }
        catch
        {
            DouyinAvailable = false;
        }
#else
        DouyinAvailable = false;
#endif
    }

    private void CheckKuaiShouSDK()
    {
#if UNITY_KUAISHOUMINIGAME
        try
        {
            KS.GetSystemInfoSync();
            KuaiShouAvailable = true;
        }
        catch
        {
            KuaiShouAvailable = false;
        }
#else
        KuaiShouAvailable = false;
#endif
    }

    private void CheckTapTapSDK()
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            var tapType = typeof(TapTap.Login.TapLogin);
            TapTapAvailable = tapType != null;
        }
        catch
        {
            TapTapAvailable = false;
        }
#else
        TapTapAvailable = false;
#endif
    }

    private void LogStatus()
    {
        Debug.Log($"[SDKValidator] 微信SDK={(WeChatAvailable ? "就绪" : "未安装")} | " +
                  $"抖音SDK={(DouyinAvailable ? "就绪" : "未安装")} | " +
                  $"快手SDK={(KuaiShouAvailable ? "就绪" : "未安装")} | " +
                  $"TapTapSDK={(TapTapAvailable ? "就绪" : "未安装")} | " +
                  $"小游戏全部就绪={(AllMiniGameSDKsReady ? "是" : "否")}");
    }

    public string GetStatusReport()
    {
        return $"微信SDK: {(WeChatAvailable ? "就绪" : "未安装")}\n" +
               $"抖音SDK: {(DouyinAvailable ? "就绪" : "未安装")}\n" +
               $"快手SDK: {(KuaiShouAvailable ? "就绪" : "未安装")}\n" +
               $"TapTapSDK: {(TapTapAvailable ? "就绪" : "未安装")}\n" +
               $"小游戏全部就绪: {(AllMiniGameSDKsReady ? "是" : "否")}\n" +
               $"当前平台: {(PlatformManager.Instance != null ? PlatformManager.Instance.GetPlatformName() : "未知")}";
    }
}