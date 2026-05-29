using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class UIPrefabGenerator : EditorWindow
{
    private const string PREFAB_DIR = "Assets/Resources/Prefabs";
    private const string CHINESE_FONT_PATH = "Fonts/HanYiChaoCuYuanJian-1";
    private static Font _defaultFont;

    private static Font GetDefaultFont()
    {
        if (_defaultFont == null)
        {
            _defaultFont = Resources.Load<Font>(CHINESE_FONT_PATH);
            if (_defaultFont == null)
            {
                _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                Debug.LogWarning("[UIPrefabGenerator] 未找到自定义字体，使用系统默认字体");
            }
            else
            {
                Debug.Log($"[UIPrefabGenerator] 使用自定义字体: {CHINESE_FONT_PATH}");
            }
        }
        return _defaultFont;
    }

    [MenuItem("Tools/生成UI预制体")]
    public static void GenerateAllPrefabs()
    {
        if (!Directory.Exists(PREFAB_DIR))
            Directory.CreateDirectory(PREFAB_DIR);

        CreateCanvasPrefab();
        CreateLoginPanel();
        CreateHomePanel();
        CreateTalentSelectPanel();
        CreateHighSchoolPanel();
        CreateGaokaoPanel();
        CreateZhiyuanPanel();
        CreateCollegePanel();
        CreateLifePanel();
        CreateResultPanel();
        CreateSettingsPanel();

        AssetDatabase.Refresh();
        Debug.Log($"[UIPrefabGenerator] 所有UI预制体已生成到 {PREFAB_DIR}");
    }

    private static void CreateLoginPanel()
    {
        GameObject panel = CreateBasePanel("LoginPanel", new Color(0.98f, 0.96f, 0.93f));

        CreateText(panel, "TitleText", "我的高考志愿模拟器", 40, new Color(0.2f, 0.3f, 0.6f), new Vector2(0, 450), new Vector2(500, 60));
        CreateText(panel, "VersionText", "V1.0", 20, Color.gray, new Vector2(0, 400), new Vector2(200, 30));

        GameObject loginButtons = CreateBasePanel("LoginButtonsPanel", new Color(0, 0, 0, 0));
        loginButtons.transform.SetParent(panel.transform, false);

        CreateButton(loginButtons, "GuestBtn", "游客登录", new Vector2(0, 150), new Vector2(350, 60), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(loginButtons, "TapTapBtn", "TapTap 登录", new Vector2(0, 80), new Vector2(350, 60), new Color(0.7f, 0.6f, 0.3f));
        CreateButton(loginButtons, "WeChatBtn", "微信登录", new Vector2(0, 10), new Vector2(350, 60), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(loginButtons, "DouyinBtn", "抖音登录", new Vector2(0, -60), new Vector2(350, 60), new Color(0.6f, 0.3f, 0.7f));

        GameObject startButtonPanel = CreateBasePanel("StartButtonPanel", new Color(0, 0, 0, 0));
        startButtonPanel.transform.SetParent(panel.transform, false);

        CreateButton(startButtonPanel, "StartGameBtn", "开始游戏", new Vector2(0, 0), new Vector2(400, 70), new Color(0.9f, 0.4f, 0.3f));
        startButtonPanel.SetActive(false);

        GameObject agreePanel = CreateBasePanel("AgreePanel", new Color(0.95f, 0.95f, 0.95f));
        agreePanel.transform.SetParent(panel.transform, false);
        RectTransform agreeRect = agreePanel.GetComponent<RectTransform>();
        agreeRect.anchoredPosition = new Vector2(0, 0);
        agreeRect.sizeDelta = new Vector2(600, 400);

        CreateText(agreePanel, "AgreeTitle", "用户协议", 28, Color.black, new Vector2(0, 120), new Vector2(400, 40));
        CreateText(agreePanel, "AgreeText", "请阅读并同意用户协议和隐私政策", 18, Color.gray, new Vector2(0, 60), new Vector2(500, 60));
        CreateButton(agreePanel, "AgreeBtn", "同意", new Vector2(-100, -80), new Vector2(180, 50), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(agreePanel, "DisagreeBtn", "不同意", new Vector2(100, -80), new Vector2(180, 50), new Color(0.7f, 0.3f, 0.3f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "LoginPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static GameObject CreateBasePanel(string panelName, Color bgColor)
    {
        GameObject panel = new GameObject(panelName, typeof(RectTransform));
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = bgColor;
        img.raycastTarget = true;

        return panel;
    }

    private static Text CreateText(GameObject parent, string name, string content, int fontSize, Color color, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = GetDefaultFont();

        return text;
    }

    private static Button CreateButton(GameObject parent, string name, string label, Vector2 anchoredPos, Vector2 sizeDelta, Color normalColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Image img = go.AddComponent<Image>();
        img.color = normalColor;
        img.raycastTarget = true;
        img.type = Image.Type.Sliced;

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.ColorTint;

        GameObject labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(go.transform, false);
        RectTransform labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = labelGo.AddComponent<Text>();
        text.text = label;
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = GetDefaultFont();

        return btn;
    }

    private static Slider CreateSlider(GameObject parent, string name, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 50;
        slider.transition = Selectable.Transition.ColorTint;
        slider.direction = Slider.Direction.LeftToRight;

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0);
        fillAreaRect.anchorMax = new Vector2(1, 1);
        fillAreaRect.offsetMin = new Vector2(10, 0);
        fillAreaRect.offsetMax = new Vector2(-10, 0);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.7f, 1.0f);

        GameObject handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleSlideArea.transform.SetParent(go.transform, false);
        RectTransform handleSlideRect = handleSlideArea.GetComponent<RectTransform>();
        handleSlideRect.anchorMin = new Vector2(0, 0);
        handleSlideRect.anchorMax = new Vector2(1, 1);
        handleSlideRect.offsetMin = new Vector2(10, 0);
        handleSlideRect.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleSlideArea.transform, false);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;

        return slider;
    }

    private static void CreateCanvasPrefab()
    {
        GameObject canvasGO = new GameObject("UIRoot", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(750, 1334);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject bg = new GameObject("Background", typeof(RectTransform));
        bg.transform.SetParent(canvasGO.transform, false);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.95f, 0.95f, 0.98f);

        PrefabUtility.SaveAsPrefabAsset(canvasGO, Path.Combine(PREFAB_DIR, "UIRoot.prefab"));
        DestroyImmediate(canvasGO);
    }

    private static void CreateHomePanel()
    {
        GameObject panel = CreateBasePanel("HomePanel", new Color(0, 0, 0, 0));

        GameObject mainMenu = CreateBasePanel("MainMenu", new Color(0.95f, 0.95f, 0.98f));
        mainMenu.transform.SetParent(panel.transform, false);

        CreateText(mainMenu, "TitleText", "我的高考志愿模拟器", 36, Color.black, new Vector2(0, 300), new Vector2(500, 60));
        CreateText(mainMenu, "VersionText", "V1.0", 18, Color.gray, new Vector2(0, 260), new Vector2(200, 30));
        CreateButton(mainMenu, "StartBtn", "开始新游戏", new Vector2(0, 80), new Vector2(300, 60), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(mainMenu, "ContinueBtn", "继续游戏", new Vector2(0, 0), new Vector2(300, 60), new Color(0.2f, 0.5f, 0.7f));
        CreateButton(mainMenu, "SettingsBtn", "设置", new Vector2(0, -80), new Vector2(300, 60), new Color(0.4f, 0.4f, 0.4f));

        GameObject genderSelect = CreateBasePanel("GenderSelect", new Color(0.9f, 0.92f, 0.95f));
        genderSelect.transform.SetParent(panel.transform, false);
        CreateText(genderSelect, "TitleText", "选择你的性别", 32, Color.black, new Vector2(0, 200), new Vector2(400, 50));
        CreateButton(genderSelect, "BoyBtn", "男", new Vector2(-120, 0), new Vector2(160, 200), new Color(0.4f, 0.6f, 1.0f));
        CreateButton(genderSelect, "GirlBtn", "女", new Vector2(120, 0), new Vector2(160, 200), new Color(1.0f, 0.5f, 0.7f));

        GameObject provinceSelect = CreateBasePanel("ProvinceSelect", new Color(0.95f, 0.95f, 0.98f));
        provinceSelect.transform.SetParent(panel.transform, false);
        CreateText(provinceSelect, "ProvinceTitle", "选择省份", 28, Color.black, new Vector2(0, 300), new Vector2(300, 40));
        CreateText(provinceSelect, "SelectedProvinceText", "", 20, Color.blue, new Vector2(0, 260), new Vector2(300, 30));
        CreateButton(provinceSelect, "ToggleProvinceBtn", "选择省份", new Vector2(0, 180), new Vector2(200, 50), new Color(0.3f, 0.6f, 0.9f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "HomePanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateTalentSelectPanel()
    {
        GameObject panel = CreateBasePanel("TalentSelectPanel", new Color(0.95f, 0.93f, 0.98f));

        CreateText(panel, "TitleText", "选择你的天赋", 32, Color.black, new Vector2(0, 400), new Vector2(400, 50));

        GameObject container = new GameObject("TalentCardContainer", typeof(RectTransform));
        container.transform.SetParent(panel.transform, false);
        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 100);
        containerRect.sizeDelta = new Vector2(600, 300);

        CreateButton(panel, "ConfirmBtn", "确认选择", new Vector2(-100, -300), new Vector2(200, 55), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(panel, "RefreshBtn", "刷新 (看广告)", new Vector2(100, -300), new Vector2(200, 55), new Color(0.7f, 0.5f, 0.3f));

        GameObject memoryTag = new GameObject("MemoryTalentTag", typeof(RectTransform));
        memoryTag.transform.SetParent(panel.transform, false);
        RectTransform memoryRect = memoryTag.GetComponent<RectTransform>();
        memoryRect.anchoredPosition = new Vector2(0, 450);
        memoryRect.sizeDelta = new Vector2(300, 40);
        Text memoryText = memoryTag.AddComponent<Text>();
        memoryText.text = "前世记忆: -";
        memoryText.fontSize = 18;
        memoryText.color = new Color(0.6f, 0.4f, 0.1f);
        memoryText.alignment = TextAnchor.MiddleCenter;
        memoryText.font = GetDefaultFont();
        memoryText.raycastTarget = false;

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "TalentSelectPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateHighSchoolPanel()
    {
        GameObject panel = CreateBasePanel("HighSchoolPanel", new Color(0.95f, 0.97f, 1.0f));

        CreateText(panel, "GradeText", "高三上学期", 24, Color.black, new Vector2(0, 500), new Vector2(300, 35));

        string[] statNames = { "IntellectBar", "MentalBar", "SocialBar", "HealthBar" };
        Color[] statColors = { new Color(0.3f, 0.7f, 1.0f), new Color(0.3f, 1.0f, 0.5f), new Color(1.0f, 0.6f, 0.3f), new Color(1.0f, 0.3f, 0.3f) };
        for (int i = 0; i < 4; i++)
        {
            int yPos = 450 - i * 35;
            CreateText(panel, statNames[i].Replace("Bar", "Label"), statNames[i].Replace("Bar", ""), 18, Color.black, new Vector2(-200, yPos), new Vector2(80, 30));
            CreateSlider(panel, statNames[i], new Vector2(50, yPos), new Vector2(300, 20));
        }

        GameObject timeline = CreateBasePanel("TimelineContainer", new Color(0, 0, 0, 0));
        timeline.transform.SetParent(panel.transform, false);
        RectTransform tlRect = timeline.GetComponent<RectTransform>();
        tlRect.anchoredPosition = new Vector2(0, 320);
        tlRect.sizeDelta = new Vector2(500, 30);

        GameObject eventCard = CreateBasePanel("EventCardPanel", Color.white);
        eventCard.transform.SetParent(panel.transform, false);
        RectTransform ecRect = eventCard.GetComponent<RectTransform>();
        ecRect.anchoredPosition = new Vector2(0, 50);
        ecRect.sizeDelta = new Vector2(650, 450);

        CreateText(eventCard, "EventNameText", "事件名称", 24, Color.black, new Vector2(0, 180), new Vector2(400, 35));

        GameObject interactionArea = new GameObject("InteractionArea", typeof(RectTransform));
        interactionArea.transform.SetParent(eventCard.transform, false);
        RectTransform iaRect = interactionArea.GetComponent<RectTransform>();
        iaRect.anchoredPosition = new Vector2(0, 20);
        iaRect.sizeDelta = new Vector2(600, 120);

        CreateButton(eventCard, "AdvanceTimeBtn", "推进时间", new Vector2(200, -140), new Vector2(180, 45), new Color(0.5f, 0.5f, 0.8f));

        GameObject dejaVuBubble = new GameObject("DejaVuBubble", typeof(RectTransform));
        dejaVuBubble.transform.SetParent(panel.transform, false);
        RectTransform dvRect = dejaVuBubble.GetComponent<RectTransform>();
        dvRect.anchoredPosition = new Vector2(0, -300);
        dvRect.sizeDelta = new Vector2(400, 40);
        Text dvText = dejaVuBubble.AddComponent<Text>();
        dvText.text = "";
        dvText.fontSize = 16;
        dvText.color = new Color(0.2f, 0.3f, 0.6f);
        dvText.alignment = TextAnchor.MiddleCenter;
        dvText.font = GetDefaultFont();
        dvText.raycastTarget = false;

        GameObject resultPanel = CreateBasePanel("ResultPanel", new Color(0.95f, 0.95f, 0.95f));
        resultPanel.transform.SetParent(panel.transform, false);
        RectTransform rpRect = resultPanel.GetComponent<RectTransform>();
        rpRect.anchoredPosition = new Vector2(0, 0);
        rpRect.sizeDelta = new Vector2(600, 400);
        CreateText(resultPanel, "NarrativeText", "事件结果描述", 22, Color.black, new Vector2(0, 80), new Vector2(500, 100));
        CreateText(resultPanel, "StatChangesText", "属性变化", 20, Color.black, new Vector2(0, -30), new Vector2(400, 30));
        CreateButton(resultPanel, "ContinueBtn", "继续", new Vector2(0, -130), new Vector2(200, 50), new Color(0.3f, 0.7f, 0.4f));
        resultPanel.SetActive(false);

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "HighSchoolPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateGaokaoPanel()
    {
        GameObject panel = CreateBasePanel("GaokaoPanel", new Color(0.98f, 0.95f, 0.9f));

        CreateText(panel, "TitleText", "高考成绩", 36, new Color(0.8f, 0.2f, 0.2f), new Vector2(0, 400), new Vector2(400, 50));
        CreateText(panel, "ProvinceText", "省份", 20, Color.gray, new Vector2(0, 350), new Vector2(200, 30));

        Text scoreText = CreateText(panel, "ScoreText", "0", 72, new Color(0.8f, 0.2f, 0.2f), new Vector2(0, 200), new Vector2(300, 80));
        scoreText.fontStyle = FontStyle.Bold;

        string[] subjects = { "语文", "数学", "英语", "综合" };
        string[] subKeys = { "ChineseText", "MathText", "EnglishText", "ComprehensiveText" };
        for (int i = 0; i < 4; i++)
        {
            CreateText(panel, subKeys[i], $"{subjects[i]}: --", 22, Color.black, new Vector2(-120 + i * 80, 50), new Vector2(140, 30));
        }

        CreateButton(panel, "ConfirmBtn", "确认成绩，开始填报", new Vector2(0, -200), new Vector2(300, 55), new Color(0.3f, 0.6f, 0.9f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "GaokaoPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateZhiyuanPanel()
    {
        GameObject panel = CreateBasePanel("ZhiyuanPanel", new Color(0.95f, 0.97f, 0.95f));

        CreateText(panel, "ScoreText", "你的分数: 0", 24, Color.black, new Vector2(-180, 450), new Vector2(250, 30));
        CreateText(panel, "ProvinceText", "省份", 20, Color.gray, new Vector2(180, 450), new Vector2(150, 30));

        CreateButton(panel, "FirstLayerBtn", "冲一冲", new Vector2(-200, 400), new Vector2(120, 40), new Color(0.9f, 0.4f, 0.3f));
        CreateButton(panel, "SecondLayerBtn", "稳一稳", new Vector2(0, 400), new Vector2(120, 40), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(panel, "ThirdLayerBtn", "保一保", new Vector2(200, 400), new Vector2(120, 40), new Color(0.3f, 0.6f, 0.9f));

        GameObject collegeList = new GameObject("CollegeListContainer", typeof(RectTransform));
        collegeList.transform.SetParent(panel.transform, false);
        RectTransform clRect = collegeList.GetComponent<RectTransform>();
        clRect.anchoredPosition = new Vector2(0, 150);
        clRect.sizeDelta = new Vector2(350, 250);

        GameObject zhiyuanList = new GameObject("ZhiyuanContainer", typeof(RectTransform));
        zhiyuanList.transform.SetParent(panel.transform, false);
        RectTransform zlRect = zhiyuanList.GetComponent<RectTransform>();
        zlRect.anchoredPosition = new Vector2(200, 150);
        zlRect.sizeDelta = new Vector2(300, 250);

        GameObject strategyPanel = CreateBasePanel("StrategyPanel", new Color(0.98f, 0.98f, 0.95f));
        strategyPanel.transform.SetParent(panel.transform, false);
        RectTransform spRect = strategyPanel.GetComponent<RectTransform>();
        spRect.anchoredPosition = new Vector2(0, -100);
        spRect.sizeDelta = new Vector2(650, 100);
        CreateText(strategyPanel, "StrategyScoreText", "方案评分: --", 20, Color.black, new Vector2(-150, 20), new Vector2(200, 30));
        CreateText(strategyPanel, "StrategyRiskText", "风险: --", 20, Color.black, new Vector2(0, 20), new Vector2(200, 30));
        CreateText(strategyPanel, "StrategyAdviceText", "建议: 请先选择志愿", 18, Color.gray, new Vector2(150, 20), new Vector2(250, 30));

        CreateButton(panel, "ConfirmBtn", "提交志愿", new Vector2(0, -300), new Vector2(300, 55), new Color(0.3f, 0.7f, 0.4f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "ZhiyuanPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateCollegePanel()
    {
        GameObject panel = CreateBasePanel("CollegePanel", new Color(0.95f, 0.95f, 1.0f));

        CreateText(panel, "CollegeNameText", "大学名称", 28, Color.black, new Vector2(0, 450), new Vector2(400, 35));
        CreateText(panel, "MajorNameText", "专业名称", 22, Color.gray, new Vector2(0, 410), new Vector2(300, 30));
        CreateText(panel, "YearText", "大一", 24, Color.black, new Vector2(0, 350), new Vector2(150, 30));
        CreateText(panel, "GPAText", "GPA: 3.0", 22, Color.black, new Vector2(0, 310), new Vector2(200, 30));

        CreateButton(panel, "EventBtn", "参加活动", new Vector2(-120, 100), new Vector2(180, 55), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(panel, "StudyBtn", "学习", new Vector2(120, 100), new Vector2(180, 55), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(panel, "SocialBtn", "社交", new Vector2(-120, 30), new Vector2(180, 55), new Color(0.9f, 0.6f, 0.3f));
        CreateButton(panel, "GraduateBtn", "毕业选择", new Vector2(120, 30), new Vector2(180, 55), new Color(0.7f, 0.3f, 0.6f));
        CreateButton(panel, "AdvanceYearBtn", "进入下学期", new Vector2(0, -120), new Vector2(300, 50), new Color(0.5f, 0.5f, 0.8f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "CollegePanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateLifePanel()
    {
        GameObject panel = CreateBasePanel("LifePanel", new Color(0.98f, 0.95f, 0.93f));

        CreateText(panel, "AgeText", "22岁", 28, Color.black, new Vector2(-180, 450), new Vector2(150, 35));
        CreateText(panel, "CareerText", "职业", 22, Color.gray, new Vector2(0, 450), new Vector2(250, 35));
        CreateText(panel, "IncomeText", "月收入: 0元", 22, new Color(0.2f, 0.6f, 0.2f), new Vector2(180, 450), new Vector2(200, 35));
        CreateText(panel, "SatisfactionText", "满意度: 3/5", 22, Color.black, new Vector2(0, 400), new Vector2(200, 30));

        CreateButton(panel, "EventBtn", "经历事件", new Vector2(-120, 150), new Vector2(180, 55), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(panel, "WorkBtn", "工作", new Vector2(120, 150), new Vector2(180, 55), new Color(0.3f, 0.7f, 0.4f));
        CreateButton(panel, "FamilyBtn", "家庭", new Vector2(-120, 80), new Vector2(180, 55), new Color(0.9f, 0.6f, 0.3f));
        CreateButton(panel, "RestBtn", "休息", new Vector2(120, 80), new Vector2(180, 55), new Color(0.5f, 0.5f, 0.8f));
        CreateButton(panel, "AdvanceYearBtn", "进入下一年", new Vector2(0, -80), new Vector2(300, 50), new Color(0.5f, 0.5f, 0.8f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "LifePanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateResultPanel()
    {
        GameObject panel = CreateBasePanel("ResultPanel", new Color(0.98f, 0.95f, 0.93f));

        CreateText(panel, "SummaryCareerText", "职业: --", 24, Color.black, new Vector2(-150, 400), new Vector2(200, 30));
        CreateText(panel, "SummaryIncomeText", "月收入: 0k", 22, new Color(0.2f, 0.6f, 0.2f), new Vector2(0, 400), new Vector2(200, 30));
        CreateText(panel, "SummaryFamilyText", "家庭: --", 22, Color.black, new Vector2(150, 400), new Vector2(200, 30));

        GameObject starsContainer = new GameObject("StarsContainer", typeof(RectTransform));
        starsContainer.transform.SetParent(panel.transform, false);
        RectTransform scRect = starsContainer.GetComponent<RectTransform>();
        scRect.anchoredPosition = new Vector2(0, 350);
        scRect.sizeDelta = new Vector2(300, 40);

        Text personalityName = CreateText(panel, "PersonalityNameText", "人格名称", 28, Color.black, new Vector2(0, 250), new Vector2(300, 35));
        personalityName.fontStyle = FontStyle.Bold;
        CreateText(panel, "PersonalityTaglineText", "人格标语", 20, Color.gray, new Vector2(0, 220), new Vector2(400, 30));
        CreateText(panel, "PersonalityIconText", "🔬", 48, Color.black, new Vector2(0, 280), new Vector2(60, 60));

        CreateText(panel, "RationalValueText", "0", 20, Color.black, new Vector2(-150, 170), new Vector2(60, 25));
        CreateSlider(panel, "RationalBar", new Vector2(-50, 170), new Vector2(200, 15));
        CreateText(panel, "ImpulsiveValueText", "0", 20, Color.black, new Vector2(-150, 140), new Vector2(60, 25));
        CreateSlider(panel, "ImpulsiveBar", new Vector2(-50, 140), new Vector2(200, 15));
        CreateText(panel, "AdventurousValueText", "0", 20, Color.black, new Vector2(-150, 110), new Vector2(60, 25));
        CreateSlider(panel, "AdventurousBar", new Vector2(-50, 110), new Vector2(200, 15));

        CreateButton(panel, "SharePersonalityBtn", "分享人格卡", new Vector2(-160, 20), new Vector2(140, 40), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(panel, "ShareEndingBtn", "分享结局卡", new Vector2(0, 20), new Vector2(140, 40), new Color(0.9f, 0.6f, 0.3f));
        CreateButton(panel, "ShareKnowledgeBtn", "分享冷知识", new Vector2(160, 20), new Vector2(140, 40), new Color(0.3f, 0.7f, 0.4f));

        CreateButton(panel, "PlayAgainBtn", "再来一局", new Vector2(-100, -80), new Vector2(180, 50), new Color(0.3f, 0.6f, 0.9f));
        CreateButton(panel, "StartNewGenBtn", "传承下一代", new Vector2(100, -80), new Vector2(180, 50), new Color(0.7f, 0.3f, 0.6f));

        GameObject inheritPanel = CreateBasePanel("InheritPanel", new Color(0.95f, 0.93f, 0.98f));
        inheritPanel.transform.SetParent(panel.transform, false);
        RectTransform ipRect = inheritPanel.GetComponent<RectTransform>();
        ipRect.anchoredPosition = new Vector2(0, -200);
        ipRect.sizeDelta = new Vector2(650, 150);
        CreateText(inheritPanel, "InheritTitleText", "传承数据", 24, new Color(0.5f, 0.2f, 0.7f), new Vector2(0, 40), new Vector2(500, 30));
        CreateText(inheritPanel, "InheritStatPreviewText", "属性加成: 无", 18, Color.black, new Vector2(-150, 0), new Vector2(250, 25));
        CreateText(inheritPanel, "InheritMemoryText", "记忆天赋: 无", 18, Color.black, new Vector2(150, 0), new Vector2(250, 25));
        CreateText(inheritPanel, "InheritAlumniText", "校友院校: 无", 18, Color.black, new Vector2(0, -30), new Vector2(400, 25));
        inheritPanel.SetActive(false);

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "ResultPanel.prefab"));
        DestroyImmediate(panel);
    }

    private static void CreateSettingsPanel()
    {
        GameObject panel = CreateBasePanel("SettingsPanel", new Color(0.9f, 0.92f, 0.95f));

        CreateText(panel, "TitleText", "设置", 32, Color.black, new Vector2(0, 400), new Vector2(200, 45));

        CreateText(panel, "MusicLabel", "音乐音量", 22, Color.black, new Vector2(-200, 250), new Vector2(120, 30));
        CreateSlider(panel, "MusicSlider", new Vector2(50, 250), new Vector2(300, 20));

        CreateText(panel, "SfxLabel", "音效音量", 22, Color.black, new Vector2(-200, 180), new Vector2(120, 30));
        CreateSlider(panel, "SfxSlider", new Vector2(50, 180), new Vector2(300, 20));

        GameObject toggleGo = new GameObject("NotificationToggle", typeof(RectTransform));
        toggleGo.transform.SetParent(panel.transform, false);
        RectTransform toggleRect = toggleGo.GetComponent<RectTransform>();
        toggleRect.anchoredPosition = new Vector2(50, 110);
        toggleRect.sizeDelta = new Vector2(120, 30);
        Toggle toggle = toggleGo.AddComponent<Toggle>();
        toggle.isOn = true;

        CreateButton(panel, "ResetBtn", "重置游戏进度", new Vector2(0, -50), new Vector2(300, 50), new Color(0.8f, 0.3f, 0.3f));
        CreateButton(panel, "CloseBtn", "关闭", new Vector2(0, -150), new Vector2(200, 50), new Color(0.4f, 0.4f, 0.4f));

        PrefabUtility.SaveAsPrefabAsset(panel, Path.Combine(PREFAB_DIR, "SettingsPanel.prefab"));
        DestroyImmediate(panel);
    }

    [MenuItem("Tools/生成全部资源 (占位图+预制体)")]
    public static void GenerateAll()
    {
        PlaceholderGenerator.GenerateAllPlaceholders();
        GenerateAllPrefabs();
    }
}
