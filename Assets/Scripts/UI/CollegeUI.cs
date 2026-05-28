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

    [Header("结果")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultNarrativeText;
    [SerializeField] private Button resultContinueBtn;

    private PlayerState playerState;
    private GameEvent currentEvent;
    private EventEngine eventEngine;
    private string currentCollegeId;
    private string currentMajorId;

    void Start()
    {
        eventEngine = FindObjectOfType<EventEngine>();
        playerState = GameStateManager.Instance?.GetPlayerState();

        currentCollegeId = playerState?.admittedCollegeId ?? "C01";
        currentMajorId = playerState?.admittedMajorId ?? "M01";

        var college = DataLoader.Instance?.GetCollegeById(currentCollegeId);
        var major = DataLoader.Instance?.GetMajorById(currentMajorId);

        if (collegeNameText != null) collegeNameText.text = college?.name ?? $"院校: {currentCollegeId}";
        if (majorText != null) majorText.text = major?.name ?? $"专业: {currentMajorId}";

        RefreshTopBar();

        if (graduationPanel != null) graduationPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        kaoyanBtn?.onClick.AddListener(() => OnGraduation("考研"));
        jiuyeBtn?.onClick.AddListener(() => OnGraduation("就业"));
        chuguoBtn?.onClick.AddListener(() => OnGraduation("出国"));
        chuangyeBtn?.onClick.AddListener(() => OnGraduation("创业"));

        StartCoroutine(LoadCollegeEvents());
    }

    IEnumerator LoadCollegeEvents()
    {
        string stageFile = currentMajorId switch
        {
            "M03" => "college_med",
            "M01" => "college_cs",
            "M02" => "college_law",
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

    void ShowNextEvent()
    {
        currentEvent = eventEngine?.GetNextEvent();

        if (currentEvent == null)
        {
            Debug.Log("[CollegeUI] 没有更多事件");

            if (playerState.grade >= 4 || playerState.completedEvents.Count >= 4)
            {
                ShowGraduationPanel();
            }
            else
            {
                playerState.grade++;
                RefreshTopBar();
            }
            return;
        }

        if (eventCardPanel != null) eventCardPanel.SetActive(true);
        if (eventNameText != null) eventNameText.text = currentEvent.name;
        if (interactionTypeIcon != null) interactionTypeIcon.text = currentEvent.interactionType;

        LoadEventIllustration();

        ClearInteractionArea();
        BuildInteractionArea();
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

        var changes = path switch
        {
            "考研" => new System.Collections.Generic.Dictionary<string, int> { { "intellect", 10 } },
            "就业" => new System.Collections.Generic.Dictionary<string, int> { { "social", 10 } },
            "出国" => new System.Collections.Generic.Dictionary<string, int> { { "intellect", 5 }, { "social", 5 } },
            "创业" => new System.Collections.Generic.Dictionary<string, int> { { "social", 10 }, { "mental", -5 } },
            _ => new System.Collections.Generic.Dictionary<string, int>()
        };
        playerState.ApplyChanges(changes);

        GameStateManager.Instance?.SetStage("life");
        GameStateManager.Instance?.QuickSave();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Life");
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