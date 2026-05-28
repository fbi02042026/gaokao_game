using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HighSchoolUI : MonoBehaviour
{
    [Header("顶部栏")]
    [SerializeField] private Text gradeText;
    [SerializeField] private Slider intellectBar;
    [SerializeField] private Slider mentalBar;
    [SerializeField] private Slider socialBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text intellectText;
    [SerializeField] private Text mentalText;
    [SerializeField] private Text socialText;
    [SerializeField] private Text healthText;

    [Header("时间线")]
    [SerializeField] private Transform timelineContainer;
    [SerializeField] private GameObject timelineDotPrefab;
    private List<Image> timelineDots = new List<Image>();

    [Header("事件卡片")]
    [SerializeField] private GameObject eventCardPanel;
    [SerializeField] private Image eventIllustration;
    [SerializeField] private Image categoryIcon;
    [SerializeField] private Text interactionTypeIcon;
    [SerializeField] private Text eventNameText;
    [SerializeField] private Transform interactionArea;
    [SerializeField] private Button advanceTimeBtn;

    [Header("既视感")]
    [SerializeField] private GameObject dejaVuBubble;
    [SerializeField] private Text dejaVuHintText;

    [Header("结果弹窗")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultNarrativeText;
    [SerializeField] private Text resultStatChangesText;
    [SerializeField] private Button resultContinueBtn;

    [Header("交互预制体")]
    [SerializeField] private GameObject choiceBtnPrefab;
    [SerializeField] private GameObject placeholderTextPrefab;

    private PlayerState playerState;
    private GameEvent currentEvent;
    private EventEngine eventEngine;
    private DejaVuEngine dejaVuEngine;
    private InheritEngine inheritEngine;
    private TalentEngine talentEngine;

    private Color dotActive = ThemeColors.TimelineActive;
    private Color dotPast = ThemeColors.TimelinePast;
    private Color dotFuture = ThemeColors.TimelineFuture;

    void Start()
    {
        eventEngine = FindObjectOfType<EventEngine>();
        dejaVuEngine = FindObjectOfType<DejaVuEngine>();
        inheritEngine = FindObjectOfType<InheritEngine>();
        talentEngine = FindObjectOfType<TalentEngine>();

        playerState = GameStateManager.Instance?.GetPlayerState();
        if (playerState == null)
        {
            playerState = new PlayerState();
            GameStateManager.Instance?.SetPlayerState(playerState);
        }

        if (eventEngine != null)
        {
            eventEngine.SetPlayerState(playerState);
            StartCoroutine(LoadAndBegin());
        }

        advanceTimeBtn?.onClick.AddListener(OnAdvanceTime);

        if (dejaVuBubble != null) dejaVuBubble.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        BuildTimeline();
        RefreshTopBar();
    }

    IEnumerator LoadAndBegin()
    {
        yield return StartCoroutine(eventEngine.LoadEventsFromStreamingAssets("highschool"));
        ShowNextEvent();
    }

    void BuildTimeline()
    {
        if (timelineContainer == null || timelineDotPrefab == null) return;

        foreach (Transform child in timelineContainer)
            Destroy(child.gameObject);

        timelineDots.Clear();
        for (int i = 0; i < 6; i++)
        {
            var dot = Instantiate(timelineDotPrefab, timelineContainer);
            var img = dot.GetComponent<Image>();
            if (img != null) timelineDots.Add(img);
        }

        RefreshTimeline();
    }

    void RefreshTimeline()
    {
        int currentSemester = playerState.grade;
        for (int i = 0; i < timelineDots.Count; i++)
        {
            if (i < currentSemester - 1)
                timelineDots[i].color = dotPast;
            else if (i == currentSemester - 1)
                timelineDots[i].color = dotActive;
            else
                timelineDots[i].color = dotFuture;
        }
    }

    void RefreshTopBar()
    {
        if (gradeText != null)
            gradeText.text = $"高{((playerState.grade - 1) / 2) + 1}{(playerState.grade % 2 == 1 ? "上" : "下")}学期";

        SetSlider(intellectBar, intellectText, playerState.intellect);
        SetSlider(mentalBar, mentalText, playerState.mental);
        SetSlider(socialBar, socialText, playerState.social);
        SetSlider(healthBar, healthBar != null ? healthBar.GetComponentInChildren<Text>() : null, playerState.health);
        if (healthBar != null) { healthBar.value = playerState.health / 100f; }
        if (healthText != null) healthText.text = playerState.health.ToString();
    }

    void SetSlider(Slider slider, Text text, int value)
    {
        if (slider != null) slider.value = value / 100f;
        if (text != null) text.text = value.ToString();
    }

    void ShowNextEvent()
    {
        if (eventEngine == null) return;

        currentEvent = eventEngine.GetNextEvent();

        if (currentEvent == null)
        {
            Debug.Log("[HighSchoolUI] 没有更多事件");
            if (eventCardPanel != null) eventCardPanel.SetActive(false);
            return;
        }

        if (eventCardPanel != null) eventCardPanel.SetActive(true);

        if (eventNameText != null)
            eventNameText.text = currentEvent.name;

        if (interactionTypeIcon != null)
            interactionTypeIcon.text = GetInteractionIcon(currentEvent.interactionType);

        LoadEventIllustration();

        ClearInteractionArea();
        BuildInteractionArea();

        CheckDejaVu();
    }

    string GetInteractionIcon(string type)
    {
        return type switch
        {
            "choice" => "🔘",
            "merge" => "🧩",
            "sort" => "📊",
            "slider" => "🎚️",
            "timing" => "⏱️",
            "dialog" => "💬",
            _ => "❓"
        };
    }

    void ClearInteractionArea()
    {
        if (interactionArea == null) return;
        foreach (Transform child in interactionArea)
            Destroy(child.gameObject);
    }

    void BuildInteractionArea()
    {
        if (interactionArea == null || currentEvent == null) return;

        switch (currentEvent.interactionType)
        {
            case "choice":
                BuildChoiceArea();
                break;
            case "merge":
                AddPlaceholder("合成模式 (待实现)");
                break;
            case "sort":
                AddPlaceholder("排序模式 (待实现)");
                break;
            case "slider":
                BuildSliderArea();
                break;
            case "timing":
                AddPlaceholder("限时模式 (待实现)");
                break;
            case "dialog":
                BuildDialogArea();
                break;
            default:
                AddPlaceholder("未知交互");
                break;
        }
    }

    void BuildChoiceArea()
    {
        var choices = currentEvent.choiceContent?.choices;
        if (choices == null) return;

        foreach (var choice in choices)
        {
            var btnGo = Instantiate(choiceBtnPrefab, interactionArea);
            var btnText = btnGo.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = $"{choice.icon} {choice.text}";

            var btn = btnGo.GetComponent<Button>();
            string cid = choice.id;
            if (btn != null)
                btn.onClick.AddListener(() => OnChoiceSelected(cid));
        }
    }

    void BuildSliderArea()
    {
        var content = currentEvent.sliderContent;
        if (content == null) return;

        foreach (var cat in content.categories)
        {
            var row = new GameObject($"Slider_{cat.name}", typeof(RectTransform));
            row.transform.SetParent(interactionArea);
            var rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(600, 50);

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(row.transform);
            var label = labelGo.GetComponent<Text>();
            label.text = $"{cat.icon} {cat.name}";
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 18;
            label.alignment = TextAnchor.MiddleLeft;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(120, 50);
            labelRect.anchoredPosition = new Vector2(0, 0);

            var sliderGo = new GameObject("Slider", typeof(Slider));
            sliderGo.transform.SetParent(row.transform);
            var slider = sliderGo.GetComponent<Slider>();
            var sliderRect = sliderGo.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(400, 30);
            sliderRect.anchoredPosition = new Vector2(300, 0);
        }
    }

    void BuildDialogArea()
    {
        var content = currentEvent.dialogContent;
        if (content?.lines == null) return;

        foreach (var line in content.lines)
        {
            var go = Instantiate(placeholderTextPrefab, interactionArea);
            var txt = go.GetComponent<Text>();
            if (txt != null) txt.text = line;
        }
    }

    void AddPlaceholder(string text)
    {
        var go = Instantiate(placeholderTextPrefab, interactionArea);
        var txt = go.GetComponent<Text>();
        if (txt != null) txt.text = text;
    }

    void CheckDejaVu()
    {
        if (dejaVuEngine == null || inheritEngine == null)
        {
            if (dejaVuBubble != null) dejaVuBubble.SetActive(false);
            return;
        }

        int maxLevel = inheritEngine.GetMaxDejaVuLevel();
        if (maxLevel <= 0)
        {
            if (dejaVuBubble != null) dejaVuBubble.SetActive(false);
            return;
        }

        var result = dejaVuEngine.CheckDejaVu(currentEvent.id, maxLevel);
        if (result == null)
        {
            if (dejaVuBubble != null) dejaVuBubble.SetActive(false);
            return;
        }

        if (dejaVuBubble != null)
        {
            dejaVuBubble.SetActive(true);
            if (dejaVuHintText != null)
            {
                if (!string.IsNullOrEmpty(result.hint))
                    dejaVuHintText.text = $"🌀 {result.hint}";
                else
                    dejaVuHintText.text = "🌀 直觉闪现...";
            }
        }

        if (!string.IsNullOrEmpty(result.highlightChoiceId))
        {
            foreach (Transform child in interactionArea)
            {
                var btn = child.GetComponent<Button>();
                if (btn != null)
                {
                    var btnText = btn.GetComponentInChildren<Text>();
                    if (btnText != null && btnText.text.Contains(result.highlightChoiceId))
                    {
                        var glow = child.gameObject.AddComponent<DejaVuGlow>();
                        glow.StartGlow();
                    }
                }
            }
        }
    }

    void OnChoiceSelected(string choiceId)
    {
        Debug.Log($"[HighSchoolUI] 选择: {choiceId}");

        EventOutcome outcome = eventEngine.ResolveEvent(currentEvent.id, choiceId);
        if (outcome == null) return;

        RefreshTopBar();
        ShowResult(outcome);
    }

    void ShowResult(EventOutcome outcome)
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        if (resultNarrativeText != null)
            resultNarrativeText.text = outcome.narrative;

        if (resultStatChangesText != null)
        {
            var changes = outcome.GetStatChanges();
            var parts = new List<string>();
            foreach (var kv in changes)
            {
                string sign = kv.Value >= 0 ? "+" : "";
                parts.Add($"{kv.Key}: {sign}{kv.Value}");
            }
            resultStatChangesText.text = string.Join("  ", parts);
        }

        if (resultContinueBtn != null)
        {
            resultContinueBtn.onClick.RemoveAllListeners();
            resultContinueBtn.onClick.AddListener(OnContinueAfterResult);
        }
    }

    void OnContinueAfterResult()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (dejaVuBubble != null) dejaVuBubble.SetActive(false);

        GameStateManager.Instance?.QuickSave();
        ShowNextEvent();
    }

    void OnAdvanceTime()
    {
        playerState.grade = Mathf.Min(playerState.grade + 1, 6);
        RefreshTopBar();
        RefreshTimeline();

        if (playerState.grade >= 5 || playerState.completedEvents.Count > 3)
        {
            Debug.Log("[HighSchoolUI] 即将进入高考阶段");
        }

        GameStateManager.Instance?.QuickSave();
        ShowNextEvent();
    }

    void LoadEventIllustration()
    {
        if (eventIllustration == null || currentEvent == null) return;

        Gender gender = GameStateManager.Instance?.GetPlayerGender() ?? Gender.Male;
        string illustrationId = ResourceHelper.GetIllustrationIdForEvent(currentEvent.id, currentEvent.stage);
        var sprite = ResourceHelper.LoadEventIllustration(illustrationId, gender, currentEvent.stage);
        if (sprite != null) eventIllustration.sprite = sprite;
    }
}