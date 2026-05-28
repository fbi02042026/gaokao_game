using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ZhiyuanUI : MonoBehaviour
{
    [Header("顶部信息")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text provinceText;

    [Header("院校列表")]
    [SerializeField] private Transform collegeListContainer;
    [SerializeField] private GameObject collegeCardPrefab;

    [Header("志愿列表")]
    [SerializeField] private Transform zhiyuanContainer;
    [SerializeField] private GameObject zhiyuanItemPrefab;

    [Header("策略面板")]
    [SerializeField] private GameObject strategyPanel;
    [SerializeField] private Text strategyScoreText;
    [SerializeField] private Text strategyRiskText;
    [SerializeField] private Text strategyAdviceText;

    [Header("分层按钮")]
    [SerializeField] private Button firstLayerBtn;
    [SerializeField] private Button secondLayerBtn;
    [SerializeField] private Button thirdLayerBtn;

    [Header("提交")]
    [SerializeField] private Button confirmBtn;

    private List<CollegeEntry> collegeEntries = new List<CollegeEntry>();
    private List<string> selectedCollegeIds = new List<string>();
    private int maxZhiyuan = 6;
    private int currentLayer = 1;
    private string currentTalentId;
    private int playerScore;
    private string playerProvince;

    void Start()
    {
        var state = GameStateManager.Instance?.GetPlayerState();
        playerScore = state?.gaokaoScore ?? 500;
        playerProvince = state?.province ?? "北京";
        currentTalentId = TalentEngine.Instance?.GetCurrentTalent()?.id ?? "";

        if (scoreText != null) scoreText.text = $"你的分数: {playerScore}";
        if (provinceText != null) provinceText.text = $"{playerProvince}省";

        firstLayerBtn?.onClick.AddListener(() => SetLayer(1));
        secondLayerBtn?.onClick.AddListener(() => SetLayer(2));
        thirdLayerBtn?.onClick.AddListener(() => SetLayer(3));
        confirmBtn?.onClick.AddListener(OnConfirm);

        StartCoroutine(LoadColleges());
    }

    IEnumerator LoadColleges()
    {
        List<College> colleges = null;
        yield return StartCoroutine(DataLoader.Instance.LoadCollegesFromStreamingAssets(c => colleges = c));

        if (colleges == null || colleges.Count == 0)
        {
            colleges = DataLoader.Instance?.GetColleges(playerProvince);
        }

        if (colleges == null || colleges.Count == 0)
        {
            Debug.LogWarning("[ZhiyuanUI] 无院校数据");
            yield break;
        }

        foreach (var college in colleges)
        {
            BuildCollegeCard(college);
        }
    }

    void BuildCollegeCard(College college)
    {
        var cardGo = Instantiate(collegeCardPrefab, collegeListContainer);
        var entry = cardGo.AddComponent<CollegeEntry>();
        entry.Init(college);

        int minScore = college.GetMinScore(playerProvince);
        int prob = ScoreEngine.Instance?.CalculateProbability(playerScore, minScore, minScore + 20) ?? 50;

        var nameText = cardGo.transform.Find("NameText")?.GetComponent<Text>();
        if (nameText != null) nameText.text = $"{college.name} · {college.level}";

        var probBar = cardGo.transform.Find("ProbBar")?.GetComponent<Image>();
        if (probBar != null)
        {
            probBar.fillAmount = prob / 100f;
            probBar.color = ThemeColors.ProbabilityBarColor(prob);
        }

        var probText = cardGo.transform.Find("ProbText")?.GetComponent<Text>();
        if (probText != null)
        {
            probText.text = $"{ScoreEngine.Instance?.GetProbabilityLabel(prob)} {prob}%";
        }

        var talentTag = cardGo.transform.Find("TalentTag")?.GetComponent<Text>();
        if (talentTag != null)
        {
            if (college.majors != null && college.majors.Length > 0)
            {
                int matchScore = TalentEngine.Instance?.GetMajorMatchModifier(currentTalentId, college.majors[0]) ?? 0;
                if (matchScore > 0)
                    talentTag.text = "🔥 天赋匹配";
                else if (matchScore < 0)
                    talentTag.text = "⚠️ 天赋冲突";
                else
                    talentTag.text = "";
            }
        }

        var btn = cardGo.GetComponent<Button>();
        if (btn != null)
        {
            string cid = college.id;
            btn.onClick.AddListener(() => AddZhiyuan(cid, college.name, prob));
        }

        collegeEntries.Add(entry);
        entry.cardGo = cardGo;
        entry.probability = prob;
    }

    void AddZhiyuan(string collegeId, string collegeName, int probability)
    {
        if (selectedCollegeIds.Contains(collegeId))
        {
            Debug.Log($"[ZhiyuanUI] 已添加: {collegeName}");
            return;
        }

        if (selectedCollegeIds.Count >= maxZhiyuan)
        {
            Debug.Log("[ZhiyuanUI] 志愿已满");
            return;
        }

        selectedCollegeIds.Add(collegeId);

        var itemGo = Instantiate(zhiyuanItemPrefab, zhiyuanContainer);
        var itemText = itemGo.GetComponentInChildren<Text>();
        if (itemText != null)
            itemText.text = $"{selectedCollegeIds.Count}. {collegeName} [{probability}%]";

        Debug.Log($"[ZhiyuanUI] 添加志愿: {collegeName}");
    }

    void SetLayer(int layer)
    {
        currentLayer = layer;

        foreach (var entry in collegeEntries)
        {
            if (entry.cardGo == null) continue;

            var probBar = entry.cardGo.transform.Find("ProbBar");
            var probText = entry.cardGo.transform.Find("ProbText");
            var talentTag = entry.cardGo.transform.Find("TalentTag");

            switch (layer)
            {
                case 1:
                    break;
                case 2:
                    if (probBar != null) probBar.gameObject.SetActive(true);
                    if (probText != null) probText.gameObject.SetActive(true);
                    if (talentTag != null) talentTag.gameObject.SetActive(true);
                    break;
                case 3:
                    if (probBar != null) probBar.gameObject.SetActive(true);
                    if (probText != null) probText.gameObject.SetActive(true);
                    if (strategyPanel != null) strategyPanel.SetActive(true);
                    UpdateStrategyAnalysis();
                    break;
            }
        }
    }

    void UpdateStrategyAnalysis()
    {
        if (selectedCollegeIds.Count == 0)
        {
            if (strategyScoreText != null) strategyScoreText.text = "方案评分: --";
            if (strategyRiskText != null) strategyRiskText.text = "风险: 未选择志愿";
            if (strategyAdviceText != null) strategyAdviceText.text = "建议: 请先选择志愿";
            return;
        }

        int aboveProb = 0;
        int totalProb = 0;
        foreach (var entry in collegeEntries)
        {
            if (selectedCollegeIds.Contains(entry.college.id))
            {
                totalProb += entry.probability;
                if (entry.probability >= 80) aboveProb++;
            }
        }

        int avgProb = totalProb / selectedCollegeIds.Count;
        int strategyScore = Mathf.RoundToInt(50 + avgProb * 0.5f + aboveProb * 10);

        if (strategyScoreText != null)
            strategyScoreText.text = $"方案评分: {strategyScore}/100";

        if (strategyRiskText != null)
        {
            if (aboveProb >= 3)
                strategyRiskText.text = "风险: 低 (稳保充足)";
            else if (aboveProb >= 1)
                strategyRiskText.text = "风险: 中 (建议再加保底)";
            else
                strategyRiskText.text = "风险: 高 (全是冲刺院校!)";
        }

        if (strategyAdviceText != null)
        {
            if (selectedCollegeIds.Count < 4)
                strategyAdviceText.text = "建议: 至少填4个志愿，冲稳保搭配";
            else if (aboveProb >= 2)
                strategyAdviceText.text = "建议: 方案合理，祝你好运！🍀";
            else
                strategyAdviceText.text = "建议: 考虑增加1-2个保底院校";
        }
    }

    void OnConfirm()
    {
        if (selectedCollegeIds.Count == 0)
            return;

        Debug.Log($"[ZhiyuanUI] 提交志愿: {string.Join(", ", selectedCollegeIds)}");

        GameStateManager.Instance?.SetStage("college");
        GameStateManager.Instance?.QuickSave();

        UnityEngine.SceneManagement.SceneManager.LoadScene("College");
    }
}

public class CollegeEntry : MonoBehaviour
{
    public College college;
    public GameObject cardGo;
    public int probability;

    public void Init(College c)
    {
        college = c;
    }
}