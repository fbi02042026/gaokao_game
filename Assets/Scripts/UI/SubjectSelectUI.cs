using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SubjectSelectUI : MonoBehaviour
{
    [Header("面板")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Transform requiredContainer;
    [SerializeField] private Transform optionalContainer;
    [SerializeField] private Text selectedSummaryText;
    [SerializeField] private Text talentHintText;
    [SerializeField] private Button confirmBtn;

    [Header("预制体")]
    [SerializeField] private GameObject subjectBtnPrefab;

    private PlayerState playerState;
    private TalentEngine talentEngine;

    private string selectedRequired = "";
    private List<string> selectedOptional = new List<string>();

    private Dictionary<string, SubjectInfo> allSubjects = new Dictionary<string, SubjectInfo>
    {
        { "物理", new SubjectInfo { name = "物理", icon = "⚛️", type = "required", desc = "涵盖工学、理学等理工科专业" } },
        { "历史", new SubjectInfo { name = "历史", icon = "📜", type = "required", desc = "涵盖文史哲等人文社科专业" } },
        { "化学", new SubjectInfo { name = "化学", icon = "⚗️", type = "optional", desc = "医学、化工、材料方向" } },
        { "生物", new SubjectInfo { name = "生物", icon = "🧬", type = "optional", desc = "医学、农学、生态方向" } },
        { "政治", new SubjectInfo { name = "政治", icon = "⚖️", type = "optional", desc = "法学、公共管理方向" } },
        { "地理", new SubjectInfo { name = "地理", icon = "🌍", type = "optional", desc = "地理科学、城市规划方向" } }
    };

    void Start()
    {
        playerState = GameStateManager.Instance?.GetPlayerState();
        talentEngine = FindObjectOfType<TalentEngine>();

        if (confirmBtn != null)
            confirmBtn.onClick.AddListener(OnConfirm);

        BuildSubjectButtons();
        RefreshTalentHints();
    }

    void BuildSubjectButtons()
    {
        if (titleText != null)
            titleText.text = "选科决策";
        if (descriptionText != null)
            descriptionText.text = "3+1+2模式：在物理/历史中必选一科，在化学/生物/政治/地理中再选一至两科";

        if (requiredContainer != null)
        {
            foreach (Transform child in requiredContainer)
                Destroy(child.gameObject);

            foreach (var subj in new[] { "物理", "历史" })
            {
                CreateSubjectButton(requiredContainer, subj, true);
            }
        }

        if (optionalContainer != null)
        {
            foreach (Transform child in optionalContainer)
                Destroy(child.gameObject);

            foreach (var subj in new[] { "化学", "生物", "政治", "地理" })
            {
                CreateSubjectButton(optionalContainer, subj, false);
            }
        }

        RefreshSummary();
    }

    void CreateSubjectButton(Transform parent, string subjectName, bool isRequired)
    {
        if (subjectBtnPrefab != null)
        {
            var btnGo = Instantiate(subjectBtnPrefab, parent);
            var btnText = btnGo.GetComponentInChildren<Text>();
            var info = allSubjects[subjectName];
            if (btnText != null) btnText.text = $"{info.icon} {info.name}\n{info.desc}";

            var btn = btnGo.GetComponent<Button>();
            if (btn != null)
            {
                string subj = subjectName;
                btn.onClick.AddListener(() => OnSubjectClick(subj, isRequired));
            }
        }
        else
        {
            var btnGo = new GameObject($"Subject_{subjectName}", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(parent);

            var btnRect = btnGo.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(200, 80);

            btnGo.GetComponent<Image>().color = new Color(0.95f, 0.95f, 1f);

            var labelGo = new GameObject("Label", typeof(Text));
            labelGo.transform.SetParent(btnGo.transform);
            var info = allSubjects[subjectName];
            var label = labelGo.GetComponent<Text>();
            label.text = $"{info.icon} {info.name}\n{info.desc}";
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 13;
            label.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(190, 70);
            labelRect.anchoredPosition = Vector2.zero;

            string subj = subjectName;
            btnGo.GetComponent<Button>().onClick.AddListener(() => OnSubjectClick(subj, isRequired));
        }
    }

    void OnSubjectClick(string subjectName, bool isRequired)
    {
        if (isRequired)
        {
            selectedRequired = subjectName;
            RefreshButtonColors();
        }
        else
        {
            if (selectedOptional.Contains(subjectName))
            {
                selectedOptional.Remove(subjectName);
            }
            else if (selectedOptional.Count < 2)
            {
                selectedOptional.Add(subjectName);
            }

            RefreshButtonColors();
        }

        RefreshSummary();
        RefreshTalentHints();
    }

    void RefreshButtonColors()
    {
        RefreshContainerButtons(requiredContainer, subj => subj == selectedRequired);
        RefreshContainerButtons(optionalContainer, subj => selectedOptional.Contains(subj));
    }

    void RefreshContainerButtons(Transform container, System.Func<string, bool> isSelected)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            var btn = child.GetComponent<Button>();
            if (btn == null) continue;

            var txt = child.GetComponentInChildren<Text>();
            string btnText = txt != null ? txt.text : "";
            bool selected = false;

            foreach (var kv in allSubjects)
            {
                if (btnText.Contains(kv.Key))
                {
                    selected = isSelected(kv.Key);
                    break;
                }
            }

            var img = child.GetComponent<Image>();
            if (img != null)
            {
                img.color = selected ? new Color(0.5f, 0.9f, 0.5f) : new Color(0.95f, 0.95f, 1f);
            }
        }
    }

    void RefreshSummary()
    {
        if (selectedSummaryText == null) return;

        string req = string.IsNullOrEmpty(selectedRequired) ? "?" : selectedRequired;
        string opt = selectedOptional.Count > 0 ? string.Join("、", selectedOptional) : "?";

        selectedSummaryText.text = $"当前选择：{req}  +  {opt}";
    }

    void RefreshTalentHints()
    {
        if (talentHintText == null || talentEngine == null)
        {
            if (talentHintText != null) talentHintText.text = "";
            return;
        }

        var hints = new List<string>();
        var talents = talentEngine.GetPlayerTalents(playerState);
        if (talents == null) return;

        foreach (var talent in talents)
        {
            string rec = GetTalentSubjectRecommendation(talent.id);
            if (!string.IsNullOrEmpty(rec))
                hints.Add($"- {talent.name}: 推荐 {rec}");
        }

        talentHintText.text = hints.Count > 0
            ? "建议参考：\n" + string.Join("\n", hints)
            : "";
    }

    string GetTalentSubjectRecommendation(string talentId)
    {
        return talentId switch
        {
            "T01" or "T02" or "T03" => "物理 + 化学",
            "T04" or "T05" => "历史 + 政治",
            "T06" or "T07" => "物理 + 生物",
            "T08" => "历史 + 地理",
            _ => ""
        };
    }

    void OnConfirm()
    {
        if (string.IsNullOrEmpty(selectedRequired))
        {
            ShowWarning("请先选择物理或历史作为必选科目！");
            return;
        }

        if (selectedOptional.Count == 0)
        {
            ShowWarning("请至少选择一科选修科目！");
            return;
        }

        playerState.selectedSubjects.Clear();
        playerState.selectedSubjects.Add(selectedRequired);
        playerState.selectedSubjects.AddRange(selectedOptional);

        if (playerState.subjectScores == null)
            playerState.subjectScores = new Dictionary<string, int>();

        foreach (var subj in playerState.selectedSubjects)
        {
            if (!playerState.subjectScores.ContainsKey(subj))
                playerState.subjectScores[subj] = 50;
        }

        GameStateManager.Instance?.QuickSave();
        Debug.Log($"[SubjectSelectUI] 选科完成: {string.Join(", ", playerState.selectedSubjects)}");
        OnSelectComplete();
    }

    void ShowWarning(string msg)
    {
        Debug.LogWarning($"[SubjectSelectUI] {msg}");
    }

    void OnSelectComplete()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        var hsUI = FindObjectOfType<HighSchoolUI>();
        if (hsUI != null)
            hsUI.OnSubjectSelectionComplete();
    }

    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        selectedRequired = "";
        selectedOptional.Clear();
        BuildSubjectButtons();
        RefreshTalentHints();
    }

    public static Dictionary<string, string[]> GetMajorSubjectRequirements()
    {
        return new Dictionary<string, string[]>
        {
            { "工学", new[] { "物理", "化学" } },
            { "理学", new[] { "物理" } },
            { "医学", new[] { "物理", "化学" } },
            { "农学", new[] { "物理", "生物" } },
            { "哲学", new[] { "历史", "政治" } },
            { "法学", new[] { "政治" } },
            { "经济学", new[] { "物理", "政治" } },
            { "管理学", new[] { "政治" } },
            { "文学", new[] { "历史" } },
            { "历史学", new[] { "历史" } },
            { "教育学", new[] { "物理", "历史" } },
            { "艺术学", new[] { "历史" } }
        };
    }

    public static bool CanApplyToMajor(List<string> playerSubjects, Major major)
    {
        if (playerSubjects == null || playerSubjects.Count == 0) return true;

        var requirements = GetMajorSubjectRequirements();
        if (!requirements.ContainsKey(major.category)) return true;

        var required = requirements[major.category];
        foreach (var req in required)
        {
            if (!playerSubjects.Contains(req))
                return false;
        }

        return true;
    }
}

public class SubjectInfo
{
    public string name;
    public string icon;
    public string type;
    public string desc;
}