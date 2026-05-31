using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    public static DebugPanel Instance { get; private set; }

    [Header("UI引用")]
    public GameObject panelRoot;
    public Button toggleButton;
    public Button guestLoginButton;
    public Button logoutButton;
    public Button wechatLoginButton;
    public Button douyinLoginButton;
    public Button kuaishouLoginButton;
    public Button taptapLoginButton;
    public Button rewardedAdButton;
    public Button interstitialAdButton;
    public Button bannerAdButton;
    public Button hideBannerButton;
    public Button shareButton;
    public Button sidebarButton;
    public Button recordStartButton;
    public Button recordStopButton;
    public Button antiAddictionButton;
    public Button commonUseButton;
    public Button vibrateShortButton;
    public Button vibrateLongButton;
    public Text statusText;
    public Text userIdText;

    private bool _isPanelVisible;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        BindButtons();
        HidePanel();
    }

    void Start()
    {
        if (PlatformManager.Instance != null)
        {
            PlatformManager.Instance.OnLoginSuccess += RefreshStatus;
            PlatformManager.Instance.OnLogout += RefreshStatus;
        }
        RefreshStatus();

        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePanel);
    }

    void OnDestroy()
    {
        if (PlatformManager.Instance != null)
        {
            PlatformManager.Instance.OnLoginSuccess -= RefreshStatus;
            PlatformManager.Instance.OnLogout -= RefreshStatus;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            TogglePanel();
    }

    private void BindButtons()
    {
        if (guestLoginButton != null) guestLoginButton.onClick.AddListener(() => PlatformManager.Instance?.GuestLogin());
        if (logoutButton != null) logoutButton.onClick.AddListener(() => PlatformManager.Instance?.Logout());
        if (wechatLoginButton != null) wechatLoginButton.onClick.AddListener(() => PlatformManager.Instance?.WechatLogin());
        if (douyinLoginButton != null) douyinLoginButton.onClick.AddListener(() => PlatformManager.Instance?.DouyinLogin());
        if (kuaishouLoginButton != null) kuaishouLoginButton.onClick.AddListener(() => PlatformManager.Instance?.KuaiShouLogin());
        if (taptapLoginButton != null) taptapLoginButton.onClick.AddListener(() => PlatformManager.Instance?.TapTapLogin());

        if (rewardedAdButton != null)
            rewardedAdButton.onClick.AddListener(() =>
                PlatformManager.Instance?.ShowRewardedAd("debug_placement", (s) =>
                    Debug.Log($"[DebugPanel] 激励广告结果: {s}")));

        if (interstitialAdButton != null)
            interstitialAdButton.onClick.AddListener(() =>
                PlatformManager.Instance?.ShowInterstitialAd("debug_placement", (s) =>
                    Debug.Log($"[DebugPanel] 插屏广告结果: {s}")));

        if (bannerAdButton != null)
            bannerAdButton.onClick.AddListener(() =>
                PlatformManager.Instance?.ShowBannerAd("debug_banner", "bottom", (s) =>
                    Debug.Log($"[DebugPanel] Banner广告结果: {s}")));

        if (hideBannerButton != null)
            hideBannerButton.onClick.AddListener(() => PlatformManager.Instance?.HideBannerAd());

        if (shareButton != null)
            shareButton.onClick.AddListener(() =>
                PlatformManager.Instance?.Share("测试分享标题", "", (s) =>
                    Debug.Log($"[DebugPanel] 分享结果: {s}")));

        if (sidebarButton != null)
            sidebarButton.onClick.AddListener(() => PlatformManager.Instance?.NavigateToSidebar());

        if (recordStartButton != null)
            recordStartButton.onClick.AddListener(() => PlatformManager.Instance?.StartRecord());

        if (recordStopButton != null)
            recordStopButton.onClick.AddListener(() =>
                PlatformManager.Instance?.StopRecord((s) =>
                    Debug.Log($"[DebugPanel] 录屏结果: {s}")));

        if (antiAddictionButton != null)
            antiAddictionButton.onClick.AddListener(() =>
                PlatformManager.Instance?.StartAntiAddictionCheck((s) =>
                    Debug.Log($"[DebugPanel] 合规认证结果: {s}")));

        if (commonUseButton != null)
            commonUseButton.onClick.AddListener(() => PlatformManager.Instance?.AddToCommonUse());

        if (vibrateShortButton != null)
            vibrateShortButton.onClick.AddListener(() => PlatformManager.Instance?.VibrateShort());

        if (vibrateLongButton != null)
            vibrateLongButton.onClick.AddListener(() => PlatformManager.Instance?.VibrateLong());
    }

    private void TogglePanel()
    {
        _isPanelVisible = !_isPanelVisible;
        if (_isPanelVisible) ShowPanel(); else HidePanel();
    }

    private void ShowPanel()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
        _isPanelVisible = true;
        RefreshStatus();
    }

    private void HidePanel()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        _isPanelVisible = false;
    }

    private void RefreshStatus()
    {
        if (PlatformManager.Instance == null) return;

        if (userIdText != null)
            userIdText.text = $"UID: {PlatformManager.Instance.UserId ?? "未登录"}";

        if (statusText != null)
        {
            string platform = PlatformManager.Instance.GetPlatformName();
            bool loggedIn = PlatformManager.Instance.IsLoggedIn;
            string sdkInfo = SDKValidator.Instance != null ? SDKValidator.Instance.GetStatusReport() : "SDK状态未知";

            statusText.text = $"平台: {platform}\n" +
                              $"登录: {(loggedIn ? "已登录" : "未登录")}\n" +
                              $"用户名: {PlatformManager.Instance.NickName ?? "无"}\n" +
                              $"\n{sdkInfo}";
        }
    }

    [ContextMenu("Force Refresh")]
    public void ForceRefresh()
    {
        RefreshStatus();
    }
}