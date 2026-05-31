using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultUI : MonoBehaviour
{
    [Header("人生总结")]
    [SerializeField] private Image endingIllustration;
    [SerializeField] private Text summaryCareerText;
    [SerializeField] private Text summaryIncomeText;
    [SerializeField] private Text summaryFamilyText;
    [SerializeField] private Transform summaryStarsContainer;
    [SerializeField] private GameObject starOnPrefab;
    [SerializeField] private GameObject starOffPrefab;

    [Header("高考人格")]
    [SerializeField] private Text personalityIconText;
    [SerializeField] private Text personalityNameText;
    [SerializeField] private Text personalityTaglineText;
    [SerializeField] private Text rationalValueText;
    [SerializeField] private Slider rationalBar;
    [SerializeField] private Text impulsiveValueText;
    [SerializeField] private Slider impulsiveBar;
    [SerializeField] private Text adventurousValueText;
    [SerializeField] private Slider adventurousBar;

    [Header("分享")]
    [SerializeField] private Button sharePersonalityBtn;
    [SerializeField] private Button shareEndingBtn;
    [SerializeField] private Button shareKnowledgeBtn;

    [Header("再来一局")]
    [SerializeField] private Button playAgainBtn;

    [Header("传承结算")]
    [SerializeField] private GameObject inheritPanel;
    [SerializeField] private Text inheritTitleText;
    [SerializeField] private Text inheritStatPreviewText;
    [SerializeField] private Text inheritMemoryText;
    [SerializeField] private Text inheritAlumniText;
    [SerializeField] private Button startNewGenBtn;

    private PlayerState playerState;
    private PersonalityEngine personalityEngine;
    private InheritEngine inheritEngine;
    private TalentEngine talentEngine;

    void Start()
    {
        playerState = GameStateManager.Instance?.GetPlayerState();
        personalityEngine = FindObjectOfType<PersonalityEngine>();
        inheritEngine = FindObjectOfType<InheritEngine>();
        talentEngine = FindObjectOfType<TalentEngine>();

        DisplaySummary();
        GameManager.Instance?.RecordCurrentPlaythrough();
        DisplayPersonality();

        sharePersonalityBtn?.onClick.AddListener(() => ShareManager.Instance?.Share("personality"));
        shareEndingBtn?.onClick.AddListener(() => ShareManager.Instance?.Share("ending"));
        shareKnowledgeBtn?.onClick.AddListener(() => ShareManager.Instance?.Share("knowledge"));

        playAgainBtn?.onClick.AddListener(OnPlayAgain);
        startNewGenBtn?.onClick.AddListener(OnStartNewGeneration);

        if (inheritPanel != null) inheritPanel.SetActive(false);

        StartCoroutine(ShowInheritAfterDelay());
    }

    void DisplaySummary()
    {
        if (playerState == null) return;

        LoadEndingIllustration();

        if (summaryCareerText != null)
            summaryCareerText.text = $"职业: {playerState.careerPath}";

        if (summaryIncomeText != null)
            summaryIncomeText.text = $"月收入: {playerState.monthlyIncome / 1000f:F1}k";

        if (summaryFamilyText != null)
            summaryFamilyText.text = $"家庭: {playerState.familyStatus}";

        if (summaryStarsContainer != null)
        {
            foreach (Transform child in summaryStarsContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < 5; i++)
            {
                var prefab = i < playerState.satisfaction ? starOnPrefab : starOffPrefab;
                Instantiate(prefab, summaryStarsContainer);
            }
        }
    }

    void DisplayPersonality()
    {
        if (personalityEngine == null || playerState == null) return;

        var result = personalityEngine.GetPersonalityScores(playerState);
        if (result == null || result.personality == null) return;

        if (personalityIconText != null)
            personalityIconText.text = result.personality.icon;

        if (personalityNameText != null)
            personalityNameText.text = result.personality.name;

        if (personalityTaglineText != null)
            personalityTaglineText.text = result.personality.tagline;

        SetPersonalityStat(rationalValueText, rationalBar, result.rational);
        SetPersonalityStat(impulsiveValueText, impulsiveBar, result.impulsive);
        SetPersonalityStat(adventurousValueText, adventurousBar, result.adventurous);

        Debug.Log($"[ResultUI] 人格: {result.personality.name}, " +
                  $"理性={result.rational} 冲动={result.impulsive} 冒险={result.adventurous}");
    }

    void SetPersonalityStat(Text label, Slider bar, int value)
    {
        if (label != null) label.text = $"{value}";
        if (bar != null) bar.value = value / 100f;
    }

    System.Collections.IEnumerator ShowInheritAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        ShowInheritPanel();
    }

    void ShowInheritPanel()
    {
        if (inheritPanel == null) return;

        inheritPanel.SetActive(true);

        if (inheritTitleText != null)
            inheritTitleText.text = inheritEngine != null
                ? $"第{(inheritEngine.GetData()?.generation ?? 0) + 1}代 · 你的记忆将传承到下一代"
                : "你的记忆将传承到下一代";

        var inheritedStats = CalculateInheritedStats();
        if (inheritStatPreviewText != null)
        {
            var statParts = new List<string>();
            foreach (var kv in inheritedStats)
                statParts.Add($"{kv.Key}+{kv.Value}");
            inheritStatPreviewText.text = $"属性加成: {(statParts.Count > 0 ? string.Join(" ", statParts) : "无")}";
        }

        if (inheritMemoryText != null)
        {
            var memoryTalents = GetMemoryTalents();
            inheritMemoryText.text = $"前世记忆天赋: {(memoryTalents.Count > 0 ? string.Join(" / ", memoryTalents) : "无")}";
        }

        if (inheritAlumniText != null)
        {
            var colleges = inheritEngine?.GetData()?.alumniColleges;
            var collegeNames = new List<string>();
            if (colleges != null)
            {
                foreach (var cid in colleges)
                {
                    var college = DataLoader.Instance?.GetCollegeById(cid);
                    if (college != null) collegeNames.Add(college.name);
                }
            }
            if (!string.IsNullOrEmpty(playerState?.admittedCollegeId))
            {
                var currentCollege = DataLoader.Instance?.GetCollegeById(playerState.admittedCollegeId);
                if (currentCollege != null && !collegeNames.Contains(currentCollege.name))
                    collegeNames.Add(currentCollege.name);
            }
            inheritAlumniText.text = $"家族校友院校: {(collegeNames.Count > 0 ? string.Join(", ", collegeNames) : "无")}";
        }
    }

    Dictionary<string, int> CalculateInheritedStats()
    {
        var result = new Dictionary<string, int>();
        if (playerState == null) return result;

        float ratio = Random.Range(0.10f, 0.15f);

        int intellectBonus = Mathf.RoundToInt(playerState.intellect * ratio);
        int mentalBonus = Mathf.RoundToInt(playerState.mental * ratio);
        int socialBonus = Mathf.RoundToInt(playerState.social * ratio);
        int healthBonus = Mathf.RoundToInt(playerState.health * ratio);

        if (intellectBonus > 0) result["智力"] = intellectBonus;
        if (mentalBonus > 0) result["心态"] = mentalBonus;
        if (socialBonus > 0) result["社交"] = socialBonus;
        if (healthBonus > 0) result["健康"] = healthBonus;

        return result;
    }

    List<string> GetMemoryTalents()
    {
        var list = new List<string>();
        var memTalentIds = inheritEngine?.GetData()?.memoryTalentIds;
        if (memTalentIds != null)
        {
            var dejaVuEngine = FindObjectOfType<DejaVuEngine>();
            foreach (var id in memTalentIds)
            {
                list.Add(id switch
                {
                    "D1" => "回忆闪现 Lv1",
                    "D2" => "前世记忆 Lv2",
                    "D3" => "灵魂共鸣 Lv3",
                    "D4" => "轮回觉悟 Lv4",
                    _ => $"记忆 {id}"
                });
            }
        }
        return list;
    }

    void OnPlayAgain()
    {
        Debug.Log("[ResultUI] 再来一局");
        GameManager.Instance.ChangePhase(GamePhase.Home);
    }

    void OnStartNewGeneration()
    {
        Debug.Log("[ResultUI] 开始下一代");

        var save = GameStateManager.Instance?.GetCurrentSave();

        var gen1Memory = new PlaythroughMemory
        {
            generation = (inheritEngine?.GetData()?.generation ?? 0) + 1,
            talentId = talentEngine?.GetCurrentTalent()?.id ?? "",
            finalCollege = playerState?.admittedCollegeId ?? "",
            finalMajor = playerState?.admittedMajorId ?? "",
            endingType = GetEndingType(),
            finalStatsList = new List<IntPair>
            {
                new IntPair { key = "intellect", value = playerState?.intellect ?? 0 },
                new IntPair { key = "mental", value = playerState?.mental ?? 0 },
                new IntPair { key = "social", value = playerState?.social ?? 0 },
                new IntPair { key = "health", value = playerState?.health ?? 0 }
            }
        };

        if (inheritEngine != null)
        {
            inheritEngine.SaveCurrentMemory(gen1Memory);
            inheritEngine.StartNewGeneration(gen1Memory);
        }

        if (save != null)
        {
            save.inheritData = inheritEngine?.GetData();
            GameStateManager.Instance?.SetStage("highschool");

            var newState = new PlayerState();
            if (save.inheritData?.inheritedStatsList != null)
            {
                foreach (var kv in save.inheritData.inheritedStatsList)
                {
                    var changes = new Dictionary<string, int> { { kv.key, kv.value } };
                    newState.ApplyChanges(changes);
                }
            }

            var dejaVuEngine = FindObjectOfType<DejaVuEngine>();
            if (dejaVuEngine != null && save.pastMemories != null)
                dejaVuEngine.LoadMemories(save.pastMemories);

            save.playerState = newState;
            GameStateManager.Instance?.QuickSave();
        }

        GameManager.Instance.ChangePhase(GamePhase.Home);
    }

    string GetEndingType()
    {
        if (playerState == null) return "normal";

        if (playerState.monthlyIncome >= 30000 && playerState.satisfaction >= 4)
            return "perfect";
        if (playerState.monthlyIncome >= 15000 && playerState.satisfaction >= 3)
            return "good";
        if (playerState.satisfaction <= 2)
            return "bittersweet";

        return "normal";
    }

    void LoadEndingIllustration()
    {
        if (endingIllustration == null || playerState == null) return;

        Gender gender = GameStateManager.Instance?.GetPlayerGender() ?? Gender.Male;
        string endingType = GetEndingType();
        string endingId = GetEndingIllustrationId(endingType);
        var sprite = ResourceHelper.LoadEndingIllustration(endingId, gender);
        if (sprite != null) endingIllustration.sprite = sprite;
    }

    string GetEndingIllustrationId(string endingType)
    {
        return endingType switch
        {
            "perfect" => playerState.graduationChoice switch
            {
                "考研" => "scholar",
                "就业" => "business",
                "出国" => "freedom",
                "创业" => "freedom",
                "考公" => "official",
                _ => "freedom"
            },
            "good" => playerState.graduationChoice switch
            {
                "考研" => "scholar",
                "就业" => "programmer",
                "出国" => "artist",
                "创业" => "business",
                "考公" => "civil_servant",
                _ => "civil_servant"
            },
            "bittersweet" => "hardship",
            _ => "civil_servant"
        };
    }
}
