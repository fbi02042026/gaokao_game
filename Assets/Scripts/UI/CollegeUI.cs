using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollegeUI : MonoBehaviour
{
    [Header("顶部")]
    [SerializeField] private Text collegeNameText;
    [SerializeField] private Text majorText;
    [SerializeField] private Text gradeText;

    [Header("属性")]
    [SerializeField] private Slider intellectBar;
    [SerializeField] private Slider mentalBar;
    [SerializeField] private Slider socialBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text intellectText;
    [SerializeField] private Text mentalText;
    [SerializeField] private Text socialText;
    [SerializeField] private Text healthText;

    [Header("属性标签(代码生成)")]
    [SerializeField] private Text intellectLabelText;
    [SerializeField] private Text mentalLabelText;
    [SerializeField] private Text socialLabelText;
    [SerializeField] private Text healthLabelText;

    [Header("事件")]
    [SerializeField] private GameObject eventCardPanel;
    [SerializeField] private Image eventIllustration;
    [SerializeField] private Text eventNameText;
    [SerializeField] private Text interactionTypeIcon;
    [SerializeField] private Transform interactionArea;
    [SerializeField] private GameObject choiceBtnPrefab;
    [SerializeField] private GameObject placeholderTextPrefab;

    [Header("毕业选择")]
    [SerializeField] private GameObject graduationPanel;
    [SerializeField] private Button kaoyanBtn;
    [SerializeField] private Button jiuyeBtn;
    [SerializeField] private Button chuguoBtn;
    [SerializeField] private Button chuangyeBtn;
    [SerializeField] private Button kaogongBtn;

    [Header("结果")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultNarrativeText;
    [SerializeField] private Button resultContinueBtn;

    [Header("既视感")]
    [SerializeField] private GameObject dejaVuBubble;
    [SerializeField] private Text dejaVuHintText;

    private PlayerState playerState;
    private GameEvent currentEvent;
    private EventEngine eventEngine;
    private DejaVuEngine dejaVuEngine;
    private InheritEngine inheritEngine;
    private string currentCollegeId;
    private string currentMajorId;

    void Start()
    {
        eventEngine = FindObjectOfType<EventEngine>();
        dejaVuEngine = FindObjectOfType<DejaVuEngine>();
        inheritEngine = FindObjectOfType<InheritEngine>();
        playerState = GameStateManager.Instance?.GetPlayerState();

        currentCollegeId = playerState?.admittedCollegeId ?? "C01";
        currentMajorId = playerState?.admittedMajorId ?? "M01";

        var college = DataLoader.Instance?.GetCollegeById(currentCollegeId);
        var major = DataLoader.Instance?.GetMajorById(currentMajorId);

        if (collegeNameText != null) collegeNameText.text = college?.name ?? $"院校: {currentCollegeId}";
        if (majorText != null) majorText.text = major?.name ?? $"专业: {currentMajorId}";

        var talentId = TalentEngine.Instance?.GetCurrentTalent()?.id ?? "";
        var achEngine = FindObjectOfType<AchievementEngine>();
        achEngine?.CheckMajorChoice(currentMajorId);
        achEngine?.CheckTalentMajorMatch(talentId, major?.category);
        achEngine?.CheckCollegeAdmission(currentCollegeId, college?.level);

        InitializeAttributeLabels();
        TransitionToStressSystem();
        RefreshTopBar();

        if (graduationPanel != null) graduationPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
        if (dejaVuBubble != null) dejaVuBubble.SetActive(false);

        kaoyanBtn?.onClick.AddListener(() => OnGraduation("考研"));
        jiuyeBtn?.onClick.AddListener(() => OnGraduation("就业"));
        chuguoBtn?.onClick.AddListener(() => OnGraduation("出国"));
        chuangyeBtn?.onClick.AddListener(() => OnGraduation("创业"));
        kaogongBtn?.onClick.AddListener(() => OnGraduation("考公"));

        StartCoroutine(LoadCollegeEvents());
    }

    IEnumerator LoadCollegeEvents()
    {
        string stageFile = currentMajorId switch
        {
            "M03" => "college_med",
            "M01" => "college_cs",
            "M02" => "college_law",
            "M04" => "college_biz",
            _ => "college_cs"
        };

        if (eventEngine != null)
        {
            eventEngine.SetPlayerState(playerState);
            yield return StartCoroutine(eventEngine.LoadEventsFromStreamingAssets(stageFile));
        }

        ShowNextEvent();
    }

    void RefreshTopBar()
    {
        if (gradeText != null) gradeText.text = $"大{playerState.grade}";

        if (intellectBar != null) { intellectBar.value = playerState.intellect / 100f; if (intellectText != null) intellectText.text = playerState.intellect.ToString(); }
        if (mentalBar != null) { mentalBar.value = playerState.mental / 100f; if (mentalText != null) mentalText.text = playerState.mental.ToString(); }
        if (socialBar != null) { socialBar.value = playerState.social / 100f; if (socialText != null) socialText.text = playerState.social.ToString(); }
        if (healthBar != null) { healthBar.value = playerState.health / 100f; if (healthText != null) healthText.text = playerState.health.ToString(); }
    }

    void InitializeAttributeLabels()
    {
        SetLabelText(intellectLabelText, "元气");
        SetLabelText(mentalLabelText, "压力");
        SetLabelText(socialLabelText, "朋友");
        SetLabelText(healthLabelText, "金钱");
    }

    void SetLabelText(Text label, string text)
    {
        if (label != null) label.text = text;
    }

    void TransitionToStressSystem()
    {
        if (playerState == null || playerState.hasTransitionedToStress) return;

        playerState.mental = Mathf.Clamp(100 - playerState.mental, 0, 100);
        playerState.hasTransitionedToStress = true;

        Debug.Log($"[CollegeUI] 心理→压力反转完成: mental={playerState.mental}");
    }

    void ShowNextEvent()
    {
        currentEvent = eventEngine?.GetNextEvent();

        if (currentEvent == null)
        {
            Debug.Log("[CollegeUI] 没有更多事件");

            if (playerState.grade >= 4 || playerState.completedEvents.Count >= 8)
            {
                ShowGraduationPanel();
            }
            else
            {
                playerState.grade++;
                Debug.Log($"[CollegeUI] 大{playerState.grade}，重新加载事件池");
                string stageFile = playerState.admittedMajorId switch
                {
                    var m when m.StartsWith("cs") => "college_cs",
                    var m when m.StartsWith("law") => "college_law",
                    var m when m.StartsWith("med") => "college_med",
                    var m when m.StartsWith("biz") => "college_biz",
                    _ => "college_cs"
                };
                StartCoroutine(eventEngine.LoadEventsFromStreamingAssets(stageFile));
                currentEvent = eventEngine?.GetNextEvent();
                if (currentEvent == null)
                {
                    Debug.LogWarning("[CollegeUI] 事件池耗尽，进入毕业");
                    ShowGraduationPanel();
                    return;
                }

                if (eventCardPanel != null) eventCardPanel.SetActive(true);
                if (eventNameText != null) eventNameText.text = currentEvent.name;
                if (interactionTypeIcon != null) interactionTypeIcon.text = currentEvent.interactionType;
                LoadEventIllustration();
                ClearInteractionArea();
                BuildInteractionArea();
                CheckDejaVu();
            }
            return;
        }

        if (eventCardPanel != null) eventCardPanel.SetActive(true);
        if (eventNameText != null) eventNameText.text = currentEvent.name;
        if (interactionTypeIcon != null) interactionTypeIcon.text = currentEvent.interactionType;

        LoadEventIllustration();

        ClearInteractionArea();
        BuildInteractionArea();
        CheckDejaVu();
    }

    void ClearInteractionArea()
    {
        if (interactionArea == null) return;
        foreach (Transform child in interactionArea) Destroy(child.gameObject);
    }

    void BuildInteractionArea()
    {
        if (currentEvent == null || interactionArea == null) return;

        if (currentEvent.interactionType == "choice" && currentEvent.choiceContent?.choices != null)
        {
            foreach (var c in currentEvent.choiceContent.choices)
            {
                var btnGo = Instantiate(choiceBtnPrefab, interactionArea);
                btnGo.GetComponentInChildren<Text>().text = $"{c.icon} {c.text}";
                string cid = c.id;
                btnGo.GetComponent<Button>().onClick.AddListener(() => OnChoice(cid));
            }
        }
        else
        {
            var go = Instantiate(placeholderTextPrefab, interactionArea);
            go.GetComponent<Text>().text = currentEvent.name;
        }

        if (currentEvent.interactionType != "choice")
        {
            var nextBtn = Instantiate(choiceBtnPrefab, interactionArea);
            nextBtn.GetComponentInChildren<Text>().text = "继续";
            nextBtn.GetComponent<Button>().onClick.AddListener(() => OnChoice("default"));
        }
    }

    void OnChoice(string choiceId)
    {
        var outcome = eventEngine?.ResolveEvent(currentEvent.id, choiceId);
        RefreshTopBar();

        if (outcome != null)
        {
            ShowResult(outcome.narrative);
        }
    }

    void ShowResult(string narrative)
    {
        if (resultPanel == null) return;
        resultPanel.SetActive(true);
        if (resultNarrativeText != null) resultNarrativeText.text = narrative;

        if (resultContinueBtn != null)
        {
            resultContinueBtn.onClick.RemoveAllListeners();
            resultContinueBtn.onClick.AddListener(() =>
            {
                resultPanel.SetActive(false);
                if (dejaVuBubble != null) dejaVuBubble.SetActive(false);
                ShowNextEvent();
            });
        }
    }

    void ShowGraduationPanel()
    {
        if (eventCardPanel != null) eventCardPanel.SetActive(false);
        if (graduationPanel != null) graduationPanel.SetActive(true);
    }

    void OnGraduation(string path)
    {
        Debug.Log($"[CollegeUI] 毕业选择: {path}");

        playerState.graduationChoice = path;

        var changes = new System.Collections.Generic.Dictionary<string, int>();
        switch (path)
        {
            case "考研":
                changes["intellect"] = 10;
                break;
            case "就业":
                changes["social"] = 10;
                break;
            case "出国":
                changes["intellect"] = 5;
                changes["social"] = 5;
                break;
            case "创业":
                changes["social"] = 15;
                changes["health"] = -5;
                break;
            case "考公":
                changes["mental"] = 10;
                changes["social"] = 5;
                break;
        }
        playerState.ApplyChanges(changes);

        GameManager.Instance.ChangePhase(GamePhase.Life);
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

    void LoadEventIllustration()
    {
        if (eventIllustration == null || currentEvent == null) return;

        Gender gender = GameStateManager.Instance?.GetPlayerGender() ?? Gender.Male;
        string illustrationId = ResourceHelper.GetIllustrationIdForEvent(currentEvent.id, currentEvent.stage);
        var sprite = ResourceHelper.LoadEventIllustration(illustrationId, gender, currentEvent.stage);
        if (sprite != null) eventIllustration.sprite = sprite;
    }
}