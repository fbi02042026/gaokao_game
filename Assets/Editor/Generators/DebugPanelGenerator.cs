using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class DebugPanelGenerator : EditorWindow
{
    [MenuItem("Tools/生成调试面板")]
    public static void GenerateDebugPanel()
    {
        GameObject canvasGO = FindOrCreateCanvas();
        Canvas canvas = canvasGO.GetComponent<Canvas>();

        GameObject panelRoot = new GameObject("DebugPanel", typeof(RectTransform), typeof(Image));
        panelRoot.transform.SetParent(canvasGO.transform, false);
        RectTransform rootRect = panelRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.85f, 0.0f);
        rootRect.anchorMax = new Vector2(1.0f, 1.0f);
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        Image rootImg = panelRoot.GetComponent<Image>();
        rootImg.color = new Color(0, 0, 0, 0.8f);

        VerticalLayoutGroup layout = panelRoot.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 4;
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;

        ContentSizeFitter fitter = panelRoot.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = panelRoot.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        GameObject scrollView = new GameObject("ScrollView", typeof(RectTransform));
        scrollView.transform.SetParent(panelRoot.transform, false);
        RectTransform svRect = scrollView.GetComponent<RectTransform>();
        svRect.anchorMin = Vector2.zero;
        svRect.anchorMax = Vector2.one;
        svRect.offsetMin = Vector2.zero;
        svRect.offsetMax = Vector2.zero;

        VerticalLayoutGroup svLayout = scrollView.AddComponent<VerticalLayoutGroup>();
        svLayout.spacing = 4;
        svLayout.padding = new RectOffset(4, 4, 4, 4);
        svLayout.childForceExpandWidth = true;
        svLayout.childForceExpandHeight = false;

        ContentSizeFitter svFitter = scrollView.AddComponent<ContentSizeFitter>();
        svFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content = svRect;

        Text title = CreateSmallText(scrollView, "Title", "调试面板 (F1)", 16, Color.white);
        Text statusText = CreateSmallText(scrollView, "StatusText", "状态: 等待初始化", 12, Color.gray);
        Text userIdText = CreateSmallText(scrollView, "UserIdText", "UID: --", 12, Color.gray);

        Button guestBtn = CreateSmallButton(scrollView, "GuestLoginBtn", "游客登录", new Color(0.3f, 0.5f, 0.7f));
        Button wechatBtn = CreateSmallButton(scrollView, "WeChatLoginBtn", "微信登录", new Color(0.2f, 0.7f, 0.3f));
        Button douyinBtn = CreateSmallButton(scrollView, "DouyinLoginBtn", "抖音登录", new Color(0.6f, 0.3f, 0.7f));
        Button kuaishouBtn = CreateSmallButton(scrollView, "KuaishouLoginBtn", "快手登录", new Color(0.9f, 0.5f, 0.2f));
        Button taptapBtn = CreateSmallButton(scrollView, "TapTapLoginBtn", "TapTap登录", new Color(0.3f, 0.7f, 0.9f));
        Button logoutBtn = CreateSmallButton(scrollView, "LogoutBtn", "注销", new Color(0.6f, 0.3f, 0.3f));

        Button rewardedBtn = CreateSmallButton(scrollView, "RewardedAdBtn", "激励广告", new Color(0.7f, 0.6f, 0.2f));
        Button interstitialBtn = CreateSmallButton(scrollView, "InterstitialAdBtn", "插屏广告", new Color(0.6f, 0.5f, 0.2f));
        Button bannerBtn = CreateSmallButton(scrollView, "BannerAdBtn", "显示Banner", new Color(0.5f, 0.5f, 0.6f));
        Button hideBannerBtn = CreateSmallButton(scrollView, "HideBannerBtn", "隐藏Banner", new Color(0.5f, 0.5f, 0.5f));

        Button shareBtn = CreateSmallButton(scrollView, "ShareBtn", "分享", new Color(0.3f, 0.8f, 0.5f));
        Button sidebarBtn = CreateSmallButton(scrollView, "SidebarBtn", "抖音侧边栏", new Color(0.2f, 0.5f, 0.3f));
        Button recordStartBtn = CreateSmallButton(scrollView, "RecordStartBtn", "开始录屏", new Color(0.8f, 0.3f, 0.3f));
        Button recordStopBtn = CreateSmallButton(scrollView, "RecordStopBtn", "停止录屏", new Color(0.6f, 0.2f, 0.2f));
        Button antiAddictionBtn = CreateSmallButton(scrollView, "AntiAddictionBtn", "合规认证", new Color(0.3f, 0.4f, 0.8f));
        Button commonUseBtn = CreateSmallButton(scrollView, "CommonUseBtn", "设为常用", new Color(0.8f, 0.5f, 0.2f));
        Button vibrateShortBtn = CreateSmallButton(scrollView, "VibrateShortBtn", "短振动", new Color(0.5f, 0.5f, 0.5f));
        Button vibrateLongBtn = CreateSmallButton(scrollView, "VibrateLongBtn", "长振动", new Color(0.4f, 0.4f, 0.4f));

        DebugPanel debugPanel = panelRoot.AddComponent<DebugPanel>();
        debugPanel.panelRoot = panelRoot;
        debugPanel.guestLoginButton = guestBtn;
        debugPanel.wechatLoginButton = wechatBtn;
        debugPanel.douyinLoginButton = douyinBtn;
        debugPanel.kuaishouLoginButton = kuaishouBtn;
        debugPanel.taptapLoginButton = taptapBtn;
        debugPanel.logoutButton = logoutBtn;
        debugPanel.rewardedAdButton = rewardedBtn;
        debugPanel.interstitialAdButton = interstitialBtn;
        debugPanel.bannerAdButton = bannerBtn;
        debugPanel.hideBannerButton = hideBannerBtn;
        debugPanel.shareButton = shareBtn;
        debugPanel.sidebarButton = sidebarBtn;
        debugPanel.recordStartButton = recordStartBtn;
        debugPanel.recordStopButton = recordStopBtn;
        debugPanel.antiAddictionButton = antiAddictionBtn;
        debugPanel.commonUseButton = commonUseBtn;
        debugPanel.vibrateShortButton = vibrateShortBtn;
        debugPanel.vibrateLongButton = vibrateLongBtn;
        debugPanel.statusText = statusText;
        debugPanel.userIdText = userIdText;

        Selection.activeGameObject = panelRoot;
        Debug.Log("[DebugPanelGenerator] 调试面板已生成，按 F1 切换显示/隐藏");
    }

    private static GameObject FindOrCreateCanvas()
    {
        Canvas existing = Object.FindObjectOfType<Canvas>();
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject canvasGO = new GameObject("Canvas", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(750, 1334);
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        return canvasGO;
    }

    private static Text CreateSmallText(GameObject parent, string name, string content, int fontSize, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, fontSize + 6);

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleLeft;
        text.raycastTarget = false;

        return text;
    }

    private static Button CreateSmallButton(GameObject parent, string name, string label, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);

        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor = color * 0.7f;
        btn.colors = cb;

        GameObject labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(go.transform, false);
        RectTransform labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = labelGo.AddComponent<Text>();
        text.text = label;
        text.fontSize = 12;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        return btn;
    }
}