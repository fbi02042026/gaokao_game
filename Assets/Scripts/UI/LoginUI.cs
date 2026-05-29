using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header("UI元素")]
    [SerializeField] private Image bgImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Button guestLoginBtn;
    [SerializeField] private Button taptapLoginBtn;
    [SerializeField] private Button wechatLoginBtn;
    [SerializeField] private Button douyinLoginBtn;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private Button agreeBtn;
    [SerializeField] private Button disagreeBtn;
    [SerializeField] private Text agreeText;
    [SerializeField] private GameObject agreePanel;
    [SerializeField] private GameObject agreementPanel;
    [SerializeField] private Text agreementContentText;
    [SerializeField] private GameObject loginButtonsPanel;
    [SerializeField] private GameObject startButtonPanel;

    private bool isAgreed = false;
    private bool isLoggedIn = false;

    void Start()
    {
        LoadBackground();
        SetupButtons();
        UpdateButtonVisibility();

        // 游戏启动时显示协议面板
        if (agreePanel != null) agreePanel.SetActive(true);
    }

    private void LoadBackground()
    {
        if (bgImage != null)
        {
            var sprite = ResourceHelper.LoadTexLoginBg();
            if (sprite == null)
                sprite = ResourceHelper.LoadTexBg("bg_home");
            if (sprite != null)
                bgImage.sprite = sprite;
        }
    }

    private void SetupButtons()
    {
        if (guestLoginBtn != null)
            guestLoginBtn.onClick.AddListener(OnGuestLogin);
        if (taptapLoginBtn != null)
            taptapLoginBtn.onClick.AddListener(OnTapTapLogin);
        if (wechatLoginBtn != null)
            wechatLoginBtn.onClick.AddListener(OnWechatLogin);
        if (douyinLoginBtn != null)
            douyinLoginBtn.onClick.AddListener(OnDouyinLogin);
        if (startGameBtn != null)
            startGameBtn.onClick.AddListener(OnStartGame);
        if (agreeBtn != null)
            agreeBtn.onClick.AddListener(OnAgree);
        if (disagreeBtn != null)
            disagreeBtn.onClick.AddListener(OnDisagree);
    }

    private void UpdateButtonVisibility()
    {
        if (loginButtonsPanel != null)
            loginButtonsPanel.SetActive(!isLoggedIn);
        if (startButtonPanel != null)
            startButtonPanel.SetActive(isLoggedIn);
    }

    private void OnGuestLogin()
    {
        if (!isAgreed)
        {
            if (agreePanel != null) agreePanel.SetActive(true);
            return;
        }
        Debug.Log("[LoginUI] 游客登录");
        PlatformManager.Instance?.GuestLogin();
        OnLoginSuccess();
    }

    private void OnTapTapLogin()
    {
        if (!isAgreed)
        {
            if (agreePanel != null) agreePanel.SetActive(true);
            return;
        }
        Debug.Log("[LoginUI] TapTap登录");
        PlatformManager.Instance?.TapTapLogin();
        OnLoginSuccess();
    }

    private void OnWechatLogin()
    {
        if (!isAgreed)
        {
            if (agreePanel != null) agreePanel.SetActive(true);
            return;
        }
        Debug.Log("[LoginUI] 微信登录");
        PlatformManager.Instance?.WechatLogin();
        OnLoginSuccess();
    }

    private void OnDouyinLogin()
    {
        if (!isAgreed)
        {
            if (agreePanel != null) agreePanel.SetActive(true);
            return;
        }
        Debug.Log("[LoginUI] 抖音登录");
        PlatformManager.Instance?.DouyinLogin();
        OnLoginSuccess();
    }

    private void OnAgree()
    {
        isAgreed = true;
        if (agreePanel != null) agreePanel.SetActive(false);
    }

    private void OnDisagree()
    {
        Debug.Log("[LoginUI] 拒绝协议，退出游戏");
        PlatformManager.Instance?.QuitGame();
    }

    private void OnLoginSuccess()
    {
        Debug.Log("[LoginUI] 登录成功，显示开始游戏按钮");
        isLoggedIn = true;
        UpdateButtonVisibility();
    }

    private void OnStartGame()
    {
        Debug.Log("[LoginUI] 点击开始游戏，进入主界面");
        GameManager.Instance?.ChangePhase(GamePhase.Home);
    }

    public void ShowAgreement()
    {
        if (agreementPanel != null)
        {
            agreementPanel.SetActive(true);
            LoadAgreementText();
        }
    }

    private void LoadAgreementText()
    {
        if (agreementContentText == null) return;

        string platformAgreement = PlatformManager.Instance?.GetPlatformName() switch
        {
            "wechat" => "agreement_wechat",
            "douyin" => "agreement_douyin",
            "taptap" => "agreement_taptap",
            _ => "agreement_taptap"
        };

        var textAsset = Resources.Load<TextAsset>($"Agreements/{platformAgreement}");
        if (textAsset != null)
        {
            agreementContentText.text = textAsset.text;
        }
        else
        {
            agreementContentText.text = "协议文件加载失败。";
        }
    }

    public void HideAgreement()
    {
        if (agreementPanel != null)
            agreementPanel.SetActive(false);
    }
}
