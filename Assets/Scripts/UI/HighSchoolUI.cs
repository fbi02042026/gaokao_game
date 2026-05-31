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

    [Header("选科面板")]
    [SerializeField] private SubjectSelectUI subjectSelectPanel;

    [Header("交互预制体")]
    [SerializeField] private GameObject choiceBtnPrefab;
    [SerializeField] private GameObject placeholderTextPrefab;

    private PlayerState playerState;
    private GameEvent currentEvent;
    private EventEngine eventEngine;
    private DejaVuEngine dejaVuEngine;
    private InheritEngine inheritEngine;
    private TalentEngine talentEngine;
    private MergeEngine mergeEngine;

    private Color dotActive = ThemeColors.TimelineActive;
    private Color dotPast = ThemeColors.TimelinePast;
    private Color dotFuture = ThemeColors.TimelineFuture;

    private List<GameObject> mergeItemSlots = new List<GameObject>();
    private int selectedMergeItem1 = -1;
    private int selectedMergeItem2 = -1;
    private int mergeMoveCount = 0;
    private int mergeMaxMoves = 5;

    private List<string> sortItems = new List<string>();
    private List<string> sortCorrectOrder = new List<string>();
    private List<string> sortPlayerOrder = new List<string>();

    private float timingTimeLeft = 0f;
    private int timingScore = 0;
    private bool timingActive = false;
    private Coroutine timingCoroutine;
    private bool gaokaoEveTriggered = false;

    void Start()
    {
        eventEngine = FindObjectOfType<EventEngine>();
        dejaVuEngine = FindObjectOfType<DejaVuEngine>();
        inheritEngine = FindObjectOfType<InheritEngine>();
        talentEngine = FindObjectOfType<TalentEngine>();
        mergeEngine = FindObjectOfType<MergeEngine>();

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
        SetSlider(healthBar, healthText, playerState.health);
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
                BuildMergeArea();
                break;
            case "sort":
                BuildSortArea();
                break;
            case "slider":
                BuildSliderArea();
                break;
            case "timing":
                BuildTimingArea();
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

    void BuildMergeArea()
    {
        mergeItemSlots.Clear();
        selectedMergeItem1 = -1;
        selectedMergeItem2 = -1;
        mergeMoveCount = 0;

        var content = currentEvent.mergeContent;
        if (content == null) return;

        mergeMaxMoves = content.movesLimit > 0 ? content.movesLimit : 5;

        if (!string.IsNullOrEmpty(content.description))
        {
            var descGo = Instantiate(placeholderTextPrefab, interactionArea);
            var descTxt = descGo.GetComponent<Text>();
            if (descTxt != null) descTxt.text = $"🧩 {content.description} (剩余{mergeMaxMoves}步)";
        }

        int rows = content.gridRows > 0 ? content.gridRows : 2;
        int cols = content.gridCols > 0 ? content.gridCols : 3;

        var items = content.availableItems ?? new string[0];
        if (items.Length == 0 && MergeEngine.Instance != null)
        {
            var recipes = MergeEngine.Instance.GetAllRecipes();
            var itemSet = new HashSet<string>();
            foreach (var r in recipes)
            {
                itemSet.Add(r.item1);
                itemSet.Add(r.item2);
            }
            items = new string[itemSet.Count];
            itemSet.CopyTo(items);
        }

        for (int i = 0; i < rows * cols && i < items.Length; i++)
        {
            var slotGo = new GameObject($"MergeSlot_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            slotGo.transform.SetParent(interactionArea);

            var slotRect = slotGo.GetComponent<RectTransform>();
            int row = i / cols;
            int col = i % cols;
            slotRect.sizeDelta = new Vector2(140, 80);
            slotRect.anchoredPosition = new Vector2(col * 160 - (cols - 1) * 80, -row * 100);

            var slotImg = slotGo.GetComponent<Image>();
            slotImg.color = new Color(0.9f, 0.9f, 0.95f);

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(slotGo.transform);
            var label = labelGo.GetComponent<Text>();
            label.text = items[i].Replace("_", " ");
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 14;
            label.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(130, 70);
            labelRect.anchoredPosition = Vector2.zero;

            int idx = i;
            slotGo.GetComponent<Button>().onClick.AddListener(() => OnMergeItemClick(idx, items[idx]));

            mergeItemSlots.Add(slotGo);
        }

        var mergeBtnGo = new GameObject("MergeBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        mergeBtnGo.transform.SetParent(interactionArea);
        var mergeBtnRect = mergeBtnGo.GetComponent<RectTransform>();
        mergeBtnRect.sizeDelta = new Vector2(200, 50);
        mergeBtnRect.anchoredPosition = new Vector2(0, -rows * 100 - 20);
        mergeBtnGo.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f);

        var mergeBtnLabelGo = new GameObject("Label", typeof(Text));
        mergeBtnLabelGo.transform.SetParent(mergeBtnGo.transform);
        var mergeBtnLabel = mergeBtnLabelGo.GetComponent<Text>();
        mergeBtnLabel.text = "合成选中的物品";
        mergeBtnLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mergeBtnLabel.fontSize = 16;
        mergeBtnLabel.alignment = TextAnchor.MiddleCenter;
        var mergeBtnLabelRect = mergeBtnLabelGo.GetComponent<RectTransform>();
        mergeBtnLabelRect.sizeDelta = new Vector2(190, 40);
        mergeBtnLabelRect.anchoredPosition = Vector2.zero;

        mergeBtnGo.GetComponent<Button>().onClick.AddListener(OnMergeConfirm);
    }

    string GetItemDisplayName(string itemId)
    {
        return itemId switch
        {
            "textbook_basic" => "基础课本",
            "textbook_advanced" => "进阶教材",
            "sports_equipment" => "运动器材",
            "nutrition_plan" => "营养计划",
            "social_connections" => "社交人脉",
            "leadership_book" => "领导力书籍",
            "art_supplies" => "美术用品",
            "creative_workshop" => "创意工坊",
            "computer" => "电脑",
            "programming_books" => "编程书籍",
            "music_instrument" => "乐器",
            "practice_time" => "练习时间",
            "medical_knowledge" => "医学知识",
            "internship_experience" => "实习经验",
            "debate_skills" => "辩论技巧",
            "writing_practice" => "写作练习",
            "scholarship" => "奖学金",
            "athletic_talent" => "运动天赋",
            "leadership_skill" => "领导力",
            "artistic_talent" => "艺术天赋",
            "tech_talent" => "科技天赋",
            "musical_talent" => "音乐天赋",
            "medical_talent" => "医学天赋",
            "rhetoric_talent" => "修辞才能",
            _ => itemId
        };
    }

    void OnMergeItemClick(int index, string itemId)
    {
        if (selectedMergeItem1 == -1)
        {
            selectedMergeItem1 = index;
            mergeItemSlots[index].GetComponent<Image>().color = new Color(1f, 0.9f, 0.3f);
        }
        else if (selectedMergeItem2 == -1 && index != selectedMergeItem1)
        {
            selectedMergeItem2 = index;
            mergeItemSlots[index].GetComponent<Image>().color = new Color(1f, 0.9f, 0.3f);
        }
        else if (index == selectedMergeItem1)
        {
            mergeItemSlots[index].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.95f);
            selectedMergeItem1 = -1;
        }
        else if (index == selectedMergeItem2)
        {
            mergeItemSlots[index].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.95f);
            selectedMergeItem2 = -1;
        }
        else if (selectedMergeItem1 >= 0 && selectedMergeItem2 >= 0)
        {
            mergeItemSlots[selectedMergeItem1].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.95f);
            mergeItemSlots[selectedMergeItem2].GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.95f);
            selectedMergeItem1 = index;
            selectedMergeItem2 = -1;
            mergeItemSlots[index].GetComponent<Image>().color = new Color(1f, 0.9f, 0.3f);
        }
    }

    void OnMergeConfirm()
    {
        if (selectedMergeItem1 < 0 || selectedMergeItem2 < 0)
        {
            ClearInteractionArea();
            BuildMergeRetryArea();
            var warnGo = Instantiate(placeholderTextPrefab, interactionArea);
            var warnTxt = warnGo.GetComponent<Text>();
            if (warnTxt != null) warnTxt.text = "请先选择两个物品！";
            return;
        }

        var content = currentEvent.mergeContent;
        string item1 = content?.availableItems?[selectedMergeItem1] ?? "";
        string item2 = content?.availableItems?[selectedMergeItem2] ?? "";

        mergeMoveCount++;

        bool success = false;
        if (MergeEngine.Instance != null)
        {
            if (playerState.ownedItemIds == null)
                playerState.ownedItemIds = new List<string>();

            if (!playerState.ownedItemIds.Contains(item1))
                playerState.ownedItemIds.Add(item1);
            if (!playerState.ownedItemIds.Contains(item2))
                playerState.ownedItemIds.Add(item2);

            success = MergeEngine.Instance.TryMerge(item1, item2, playerState);
        }

        if (success)
        {
            var recipe = MergeEngine.Instance?.GetRecipe(item1, item2);
            var changes = new Dictionary<string, int> { { "intellect", 5 } };
            playerState.ApplyChanges(changes);
            RefreshTopBar();

            ShowResult(new EventOutcome
            {
                narrative = $"合成成功！{GetItemDisplayName(item1)} + {GetItemDisplayName(item2)} = {GetItemDisplayName(recipe?.result ?? "")}\n智力 +5",
                statKeys = new[] { "intellect" },
                statValues = new[] { 5 }
            });

            if (currentEvent != null)
                eventEngine?.ResolveEvent(currentEvent.id, "merge_success");
        }
        else if (mergeMoveCount >= mergeMaxMoves)
        {
            playerState.ApplyChanges(new Dictionary<string, int> { { "intellect", 2 } });
            RefreshTopBar();

            ShowResult(new EventOutcome
            {
                narrative = "步数用完了！虽然没有合成成功，但你有所收获。智力 +2",
                statKeys = new[] { "intellect" },
                statValues = new[] { 2 }
            });

            if (currentEvent != null)
                eventEngine?.ResolveEvent(currentEvent.id, "merge_fail");
        }
        else
        {
            ClearInteractionArea();
            BuildMergeRetryArea();
        }
    }

    void BuildMergeRetryArea()
    {
        mergeItemSlots.Clear();
        selectedMergeItem1 = -1;
        selectedMergeItem2 = -1;

        var content = currentEvent.mergeContent;
        if (content == null) return;

        if (!string.IsNullOrEmpty(content.description))
        {
            var descGo = Instantiate(placeholderTextPrefab, interactionArea);
            var descTxt = descGo.GetComponent<Text>();
            if (descTxt != null) descTxt.text = $"🧩 {content.description} (剩余{mergeMaxMoves - mergeMoveCount}步)";
        }

        var failGo = Instantiate(placeholderTextPrefab, interactionArea);
        var failTxt = failGo.GetComponent<Text>();
        if (failTxt != null) failTxt.text = "合成失败，请重试！选择两个能合成的物品。";

        int rows = content.gridRows > 0 ? content.gridRows : 2;
        int cols = content.gridCols > 0 ? content.gridCols : 3;

        var items = content.availableItems ?? new string[0];

        for (int i = 0; i < rows * cols && i < items.Length; i++)
        {
            var slotGo = new GameObject($"MergeSlot_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            slotGo.transform.SetParent(interactionArea);

            var slotRect = slotGo.GetComponent<RectTransform>();
            int row = i / cols;
            int col = i % cols;
            slotRect.sizeDelta = new Vector2(140, 80);
            slotRect.anchoredPosition = new Vector2(col * 160 - (cols - 1) * 80, -row * 100 - 60);

            var slotImg = slotGo.GetComponent<Image>();
            slotImg.color = new Color(0.9f, 0.9f, 0.95f);

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(slotGo.transform);
            var label = labelGo.GetComponent<Text>();
            label.text = items[i].Replace("_", " ");
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 14;
            label.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(130, 70);
            labelRect.anchoredPosition = Vector2.zero;

            int idx = i;
            slotGo.GetComponent<Button>().onClick.AddListener(() => OnMergeItemClick(idx, items[idx]));

            mergeItemSlots.Add(slotGo);
        }

        var mergeBtnGo = new GameObject("MergeBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        mergeBtnGo.transform.SetParent(interactionArea);
        var mergeBtnRect = mergeBtnGo.GetComponent<RectTransform>();
        mergeBtnRect.sizeDelta = new Vector2(200, 50);
        mergeBtnRect.anchoredPosition = new Vector2(0, -rows * 100 - 80);
        mergeBtnGo.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.3f);

        var mergeBtnLabelGo = new GameObject("Label", typeof(Text));
        mergeBtnLabelGo.transform.SetParent(mergeBtnGo.transform);
        var mergeBtnLabel = mergeBtnLabelGo.GetComponent<Text>();
        mergeBtnLabel.text = "合成选中的物品";
        mergeBtnLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mergeBtnLabel.fontSize = 16;
        mergeBtnLabel.alignment = TextAnchor.MiddleCenter;
        var mergeBtnLabelRect = mergeBtnLabelGo.GetComponent<RectTransform>();
        mergeBtnLabelRect.sizeDelta = new Vector2(190, 40);
        mergeBtnLabelRect.anchoredPosition = Vector2.zero;

        mergeBtnGo.GetComponent<Button>().onClick.AddListener(OnMergeConfirm);
    }

    void BuildSortArea()
    {
        sortItems.Clear();
        sortCorrectOrder.Clear();
        sortPlayerOrder.Clear();

        var content = currentEvent.sortContent;
        if (content == null) return;

        if (!string.IsNullOrEmpty(content.description))
        {
            var descGo = Instantiate(placeholderTextPrefab, interactionArea);
            var descTxt = descGo.GetComponent<Text>();
            if (descTxt != null) descTxt.text = $"📊 {content.description}";
        }

        var items = content.items ?? new string[0];
        sortCorrectOrder = new List<string>(items);
        sortItems = new List<string>(items);

        for (int i = sortItems.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = sortItems[i];
            sortItems[i] = sortItems[j];
            sortItems[j] = temp;
        }

        for (int i = 0; i < sortItems.Count; i++)
        {
            var btnGo = new GameObject($"SortItem_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(interactionArea);

            var btnRect = btnGo.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(500, 44);
            btnRect.anchoredPosition = new Vector2(0, -i * 52);

            btnGo.GetComponent<Image>().color = new Color(0.95f, 0.95f, 1f);

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(btnGo.transform);
            var label = labelGo.GetComponent<Text>();
            label.text = sortItems[i];
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;
            label.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(480, 34);
            labelRect.anchoredPosition = Vector2.zero;

            string itemText = sortItems[i];
            btnGo.GetComponent<Button>().onClick.AddListener(() => OnSortItemClick(itemText));
        }

        var confirmBtnGo = new GameObject("SortConfirmBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        confirmBtnGo.transform.SetParent(interactionArea);
        var confirmBtnRect = confirmBtnGo.GetComponent<RectTransform>();
        confirmBtnRect.sizeDelta = new Vector2(200, 50);
        confirmBtnRect.anchoredPosition = new Vector2(0, -sortItems.Count * 52 - 30);
        confirmBtnGo.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.9f);

        var confirmLabelGo = new GameObject("Label", typeof(Text));
        confirmLabelGo.transform.SetParent(confirmBtnGo.transform);
        var confirmLabel = confirmLabelGo.GetComponent<Text>();
        confirmLabel.text = "确认排序";
        confirmLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        confirmLabel.fontSize = 16;
        confirmLabel.alignment = TextAnchor.MiddleCenter;
        var confirmLabelRect = confirmLabelGo.GetComponent<RectTransform>();
        confirmLabelRect.sizeDelta = new Vector2(190, 40);
        confirmLabelRect.anchoredPosition = Vector2.zero;

        confirmBtnGo.GetComponent<Button>().onClick.AddListener(OnSortConfirm);
    }

    void OnSortItemClick(string itemText)
    {
        sortPlayerOrder.Add(itemText);
        sortItems.Remove(itemText);

        ClearInteractionArea();
        BuildSortResultArea();
    }

    void BuildSortResultArea()
    {
        var content = currentEvent.sortContent;
        if (content == null) return;

        if (!string.IsNullOrEmpty(content.description))
        {
            var descGo = Instantiate(placeholderTextPrefab, interactionArea);
            var descTxt = descGo.GetComponent<Text>();
            if (descTxt != null) descTxt.text = $"📊 你的排序 ({sortPlayerOrder.Count}/{sortCorrectOrder.Count})";
        }

        for (int i = 0; i < sortPlayerOrder.Count; i++)
        {
            var go = Instantiate(placeholderTextPrefab, interactionArea);
            var txt = go.GetComponent<Text>();
            if (txt != null) txt.text = $"{i + 1}. {sortPlayerOrder[i]}";
        }

        if (sortItems.Count > 0)
        {
            for (int i = 0; i < sortItems.Count; i++)
            {
                var btnGo = new GameObject($"SortRemain_{i}", typeof(RectTransform), typeof(Image), typeof(Button));
                btnGo.transform.SetParent(interactionArea);

                var btnRect = btnGo.GetComponent<RectTransform>();
                btnRect.sizeDelta = new Vector2(500, 44);
                btnRect.anchoredPosition = new Vector2(0, -(sortPlayerOrder.Count + i + 1) * 52);

                btnGo.GetComponent<Image>().color = new Color(0.95f, 0.95f, 1f);

                var labelGo = new GameObject("Label", typeof(Text));
                labelGo.transform.SetParent(btnGo.transform);
                var label = labelGo.GetComponent<Text>();
                label.text = sortItems[i];
                label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                label.fontSize = 16;
                label.alignment = TextAnchor.MiddleCenter;
                var labelRect = labelGo.GetComponent<RectTransform>();
                labelRect.sizeDelta = new Vector2(480, 34);
                labelRect.anchoredPosition = Vector2.zero;

                string itemText = sortItems[i];
                btnGo.GetComponent<Button>().onClick.AddListener(() => OnSortItemClick(itemText));
            }
        }

        if (sortPlayerOrder.Count >= sortCorrectOrder.Count)
        {
            var confirmBtnGo = new GameObject("SortFinalBtn", typeof(RectTransform), typeof(Image), typeof(Button));
            confirmBtnGo.transform.SetParent(interactionArea);
            var confirmBtnRect = confirmBtnGo.GetComponent<RectTransform>();
            confirmBtnRect.sizeDelta = new Vector2(200, 50);
            confirmBtnRect.anchoredPosition = new Vector2(0, -(sortCorrectOrder.Count + 1) * 52 - 30);
            confirmBtnGo.GetComponent<Image>().color = new Color(0.3f, 0.6f, 0.9f);

            var confirmLabelGo = new GameObject("Label", typeof(Text));
            confirmLabelGo.transform.SetParent(confirmBtnGo.transform);
            var confirmLabel = confirmLabelGo.GetComponent<Text>();
            confirmLabel.text = "确认排序";
            confirmLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            confirmLabel.fontSize = 16;
            confirmLabel.alignment = TextAnchor.MiddleCenter;
            var confirmLabelRect = confirmLabelGo.GetComponent<RectTransform>();
            confirmLabelRect.sizeDelta = new Vector2(190, 40);
            confirmLabelRect.anchoredPosition = Vector2.zero;

            confirmBtnGo.GetComponent<Button>().onClick.AddListener(OnSortConfirm);
        }
    }

    void OnSortConfirm()
    {
        if (sortPlayerOrder.Count < sortCorrectOrder.Count) return;

        int correctCount = 0;
        for (int i = 0; i < sortCorrectOrder.Count; i++)
        {
            if (i < sortPlayerOrder.Count && sortPlayerOrder[i] == sortCorrectOrder[i])
                correctCount++;
        }

        float accuracy = (float)correctCount / sortCorrectOrder.Count;
        int bonus = Mathf.RoundToInt(accuracy * 10);

        string outcomeId;
        string narrative;
        if (accuracy >= 0.8f)
        {
            outcomeId = "sort_perfect";
            narrative = $"排序基本正确！({correctCount}/{sortCorrectOrder.Count})\n智力 +{bonus}";
        }
        else if (accuracy >= 0.5f)
        {
            outcomeId = "sort_good";
            narrative = $"排序部分正确。({correctCount}/{sortCorrectOrder.Count})\n智力 +{bonus}";
        }
        else
        {
            outcomeId = "sort_poor";
            narrative = $"排序不太对...再想想？({correctCount}/{sortCorrectOrder.Count})\n智力 +{bonus}";
        }

        playerState.AddStat("intellect", bonus);
        RefreshTopBar();

        ShowResult(new EventOutcome
        {
            narrative = narrative,
            statKeys = new[] { "intellect" },
            statValues = new[] { bonus }
        });

        eventEngine?.ResolveEvent(currentEvent.id, outcomeId);
    }

    void BuildTimingArea()
    {
        timingScore = 0;
        timingActive = true;

        var content = currentEvent.timingContent;
        if (content == null) return;

        if (!string.IsNullOrEmpty(content.description))
        {
            var descGo = Instantiate(placeholderTextPrefab, interactionArea);
            var descTxt = descGo.GetComponent<Text>();
            if (descTxt != null) descTxt.text = $"⏱️ {content.description}";
        }

        timingTimeLeft = content.duration > 0 ? content.duration : 10f;

        var timerGo = new GameObject("TimerText", typeof(Text));
        timerGo.transform.SetParent(interactionArea);
        var timerText = timerGo.GetComponent<Text>();
        timerText.text = $"剩余时间: {timingTimeLeft:F1}s";
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.fontSize = 20;
        timerText.alignment = TextAnchor.MiddleCenter;
        timerText.color = Color.red;
        var timerRect = timerGo.GetComponent<RectTransform>();
        timerRect.sizeDelta = new Vector2(300, 40);
        timerRect.anchoredPosition = new Vector2(0, 0);

        var scoreGo = new GameObject("ScoreText", typeof(Text));
        scoreGo.transform.SetParent(interactionArea);
        var scoreText = scoreGo.GetComponent<Text>();
        scoreText.text = $"点击次数: {timingScore}";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 18;
        scoreText.alignment = TextAnchor.MiddleCenter;
        var scoreRect = scoreGo.GetComponent<RectTransform>();
        scoreRect.sizeDelta = new Vector2(300, 40);
        scoreRect.anchoredPosition = new Vector2(0, -40);

        var tapBtnGo = new GameObject("TapBtn", typeof(RectTransform), typeof(Image), typeof(Button));
        tapBtnGo.transform.SetParent(interactionArea);
        var tapBtnRect = tapBtnGo.GetComponent<RectTransform>();
        tapBtnRect.sizeDelta = new Vector2(200, 150);
        tapBtnRect.anchoredPosition = new Vector2(0, -130);
        tapBtnGo.GetComponent<Image>().color = new Color(1f, 0.4f, 0.4f);

        var tapLabelGo = new GameObject("Label", typeof(Text));
        tapLabelGo.transform.SetParent(tapBtnGo.transform);
        var tapLabel = tapLabelGo.GetComponent<Text>();
        tapLabel.text = "疯狂点击！";
        tapLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tapLabel.fontSize = 20;
        tapLabel.alignment = TextAnchor.MiddleCenter;
        var tapLabelRect = tapLabelGo.GetComponent<RectTransform>();
        tapLabelRect.sizeDelta = new Vector2(190, 140);
        tapLabelRect.anchoredPosition = Vector2.zero;

        var tapBtn = tapBtnGo.GetComponent<Button>();
        tapBtn.onClick.AddListener(() =>
        {
            if (!timingActive) return;
            timingScore++;

            if (scoreText != null)
                scoreText.text = $"点击次数: {timingScore}";
        });

        if (timingCoroutine != null)
            StopCoroutine(timingCoroutine);
        timingCoroutine = StartCoroutine(TimingCountdown(timerText, scoreText, tapBtnGo));
    }

    IEnumerator TimingCountdown(Text timerText, Text scoreText, GameObject tapBtn)
    {
        while (timingTimeLeft > 0 && timingActive)
        {
            timingTimeLeft -= Time.deltaTime;
            if (timerText != null)
                timerText.text = $"剩余时间: {Mathf.Max(0, timingTimeLeft):F1}s";
            yield return null;
        }

        timingActive = false;

        if (tapBtn != null)
            Destroy(tapBtn);

        if (timerText != null)
            timerText.text = "时间到！";

        int targetCount = currentEvent.timingContent?.targetCount ?? 20;
        float ratio = Mathf.Min(1f, (float)timingScore / targetCount);

        int bonus = Mathf.RoundToInt(ratio * 15);
        string narrative = ratio >= 1f
            ? $"你点击了 {timingScore} 次，远超目标！\n健康 +{bonus}"
            : $"你点击了 {timingScore} 次（目标 {targetCount}）。\n健康 +{bonus}";

        playerState.AddStat("health", bonus);
        RefreshTopBar();

        ShowResult(new EventOutcome
        {
            narrative = narrative,
            statKeys = new[] { "health" },
            statValues = new[] { bonus }
        });

        string outcomeId = ratio >= 0.8f ? "timing_great" : (ratio >= 0.4f ? "timing_ok" : "timing_poor");
        eventEngine?.ResolveEvent(currentEvent.id, outcomeId);
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

        if (playerState.grade >= 4 && (playerState.selectedSubjects == null || playerState.selectedSubjects.Count == 0))
        {
            if (subjectSelectPanel != null)
            {
                subjectSelectPanel.Show();
                return;
            }
        }

        if (playerState.grade >= 5 || playerState.completedEvents.Count > 3)
        {
            if (!gaokaoEveTriggered)
            {
                ShowGaokaoEveEvent();
                return;
            }

            Debug.Log("[HighSchoolUI] 即将进入高考阶段");
            GameManager.Instance?.ChangePhase(GamePhase.Gaokao);
            return;
        }

        GameStateManager.Instance?.QuickSave();
        ShowNextEvent();
    }

    public void OnSubjectSelectionComplete()
    {
        Debug.Log("[HighSchoolUI] 选科完成，继续游戏");
        if (playerState.grade >= 5 || playerState.completedEvents.Count > 3)
        {
            if (!gaokaoEveTriggered)
            {
                ShowGaokaoEveEvent();
                return;
            }

            Debug.Log("[HighSchoolUI] 即将进入高考阶段");
            GameManager.Instance?.ChangePhase(GamePhase.Gaokao);
            return;
        }

        GameStateManager.Instance?.QuickSave();
        ShowNextEvent();
    }

    void ShowGaokaoEveEvent()
    {
        gaokaoEveTriggered = true;

        if (eventCardPanel != null) eventCardPanel.SetActive(true);
        if (eventNameText != null) eventNameText.text = "高考前夜";
        if (interactionTypeIcon != null) interactionTypeIcon.text = "🌙";

        ClearInteractionArea();

        var descGo = Instantiate(placeholderTextPrefab, interactionArea);
        var descTxt = descGo.GetComponent<Text>();
        if (descTxt != null)
            descTxt.text = "明天就是高考了。你在房间里，翻看着三年的笔记，回想起高中生活的点点滴滴。\n\n你对即将到来的考试心情如何？";

        var choices = new[]
        {
            new { id = "eve_calm", text = "😌 平静如水", desc = "你已经准备充分，心态平和" },
            new { id = "eve_excited", text = "🔥 斗志昂扬", desc = "你充满干劲，期待证明自己" },
            new { id = "eve_nervous", text = "😰 紧张焦虑", desc = "你辗转反侧，担心发挥失常" },
            new { id = "eve_encourage", text = "💪 收到鼓励", desc = "家人/朋友打来电话鼓励你" }
        };

        foreach (var choice in choices)
        {
            var btnGo = Instantiate(choiceBtnPrefab, interactionArea);
            var btnText = btnGo.GetComponentInChildren<Text>();
            if (btnText != null) btnText.text = $"{choice.text}\n<size=12>{choice.desc}</size>";

            string cid = choice.id;
            var btn = btnGo.GetComponent<Button>();
            if (btn != null) btn.onClick.AddListener(() => HandleGaokaoEveChoice(cid));
        }
    }

    void HandleGaokaoEveChoice(string choiceId)
    {
        int mentalBonus = 0;
        int intellectBonus = 0;
        string narrative = "";

        switch (choiceId)
        {
            case "eve_calm":
                mentalBonus = 10;
                intellectBonus = 3;
                narrative = "你深呼吸，内心的平静让你无比专注。你相信自己的准备，相信日复一日的努力。\n\n你安稳地睡了一觉，第二天精神饱满。";
                break;
            case "eve_excited":
                mentalBonus = 5;
                intellectBonus = 5;
                narrative = "你握紧拳头，眼中闪烁着光芒。三年来所有的努力，都将在明天绽放。\n\n你带着必胜的信念进入了考场。";
                break;
            case "eve_nervous":
                mentalBonus = -5;
                intellectBonus = 8;
                narrative = "虽然紧张，但你把这些压力转化为了动力。你反复检查了准考证和文具，强迫自己冷静下来。\n\n这份紧张反而让你在考场上更加谨慎。";
                break;
            case "eve_encourage":
                mentalBonus = 8;
                intellectBonus = 2;
                narrative = "电话那头传来熟悉的声音。'别紧张，你一直都很棒，我们都相信你。'\n\n一句话让你眼眶湿润，但心里踏实了许多。\n\n带着这份温暖，你进入了梦乡。";
                break;
        }

        playerState.AddStat("mental", mentalBonus);
        playerState.AddStat("intellect", intellectBonus);
        RefreshTopBar();

        playerState.RecordChoice("gaokao_eve", choiceId);

        ShowResult(new EventOutcome
        {
            narrative = narrative,
            statKeys = new[] { "mental", "intellect" },
            statValues = new[] { mentalBonus, intellectBonus }
        });
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