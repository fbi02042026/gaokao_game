using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LifeUI : MonoBehaviour
{
    [Header("顶部")]
    [SerializeField] private Text ageText;
    [SerializeField] private Text careerText;
    [SerializeField] private Text incomeText;
    [SerializeField] private Slider satisfactionBar;
    [SerializeField] private Text satisfactionText;

    [Header("属性")]
    [SerializeField] private Slider intellectBar;
    [SerializeField] private Slider mentalBar;
    [SerializeField] private Slider socialBar;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text intellectValueText;
    [SerializeField] private Text mentalValueText;
    [SerializeField] private Text socialValueText;
    [SerializeField] private Text healthValueText;

    [Header("属性标签(代码生成)")]
    [SerializeField] private Text intellectLabelText;
    [SerializeField] private Text mentalLabelText;
    [SerializeField] private Text socialLabelText;
    [SerializeField] private Text healthLabelText;

    [Header("事件")]
    [SerializeField] private GameObject eventCardPanel;
    [SerializeField] private Image eventIllustration;
    [SerializeField] private Text eventDescriptionText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private GameObject choiceBtnPrefab;

    [Header("结果")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultNarrativeText;
    [SerializeField] private Button resultContinueBtn;

    [Header("人生总结")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private Text summaryCareerText;
    [SerializeField] private Text summaryIncomeText;
    [SerializeField] private Text summaryFamilyText;
    [SerializeField] private Transform summaryStarsContainer;
    [SerializeField] private GameObject starOnPrefab;
    [SerializeField] private GameObject starOffPrefab;
    [SerializeField] private Button toResultBtn;

    private PlayerState playerState;
    private int currentEventIndex;
    private List<LifeEventData> lifeEvents;

    void Start()
    {
        playerState = GameStateManager.Instance?.GetPlayerState();
        if (playerState == null)
        {
            playerState = new PlayerState();
            GameStateManager.Instance?.SetPlayerState(playerState);
        }

        playerState.currentAge = 22;

        if (resultPanel != null) resultPanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(false);

        resultContinueBtn?.onClick.AddListener(OnContinueAfterResult);
        toResultBtn?.onClick.AddListener(GoToResult);

        InitializeCareer();
        InitializeAttributeLabels();
        BuildLifeEvents();
        RefreshTopBar();
        ShowCurrentEvent();

        if (LifeEngine.Instance != null)
        {
            LifeEngine.Instance.StartLifeSimulation(playerState);
            Debug.Log("[LifeUI] LifeEngine 已同步启动");
        }
    }

    void InitializeCareer()
    {
        var major = DataLoader.Instance?.GetMajorById(playerState.admittedMajorId);

        playerState.careerPath = playerState.graduationChoice switch
        {
            "考研" => "研究生在读",
            "就业" => major?.career?[0] ?? "职场新人",
            "出国" => "海外留学生",
            "创业" => "初创公司创始人",
            "考公" => "基层公务员",
            _ => "职场新人"
        };

        playerState.monthlyIncome = playerState.graduationChoice switch
        {
            "考研" => 3000,
            "就业" => 8000,
            "出国" => 5000,
            "创业" => 2000,
            "考公" => 5000,
            _ => 6000
        };

        playerState.familyStatus = "单身";
        playerState.satisfaction = 3;
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

    void BuildLifeEvents()
    {
        lifeEvents = new List<LifeEventData>();

        if (playerState.graduationChoice == "考研")
        {
            lifeEvents.Add(new LifeEventData(22, "研一开学，导师给了三个研究方向让你选",
                new LifeChoice("机器学习（热门好就业）", "你在AI方向上深耕，论文发得不错", 0, "intellect", 5, "social", -3),
                new LifeChoice("古典理论（导师擅长）", "导师很喜欢你，但方向有点冷门", 0, "mental", 3, "social", 5),
                new LifeChoice("交叉学科（探索未知）", "你开创了新方向，风险大但上限高", 0, "intellect", 8, "mental", -5)
            ));

            lifeEvents.Add(new LifeEventData(24, "实验室的师兄邀你一起参加国际会议",
                new LifeChoice("积极参会做报告", "锻炼了口才，结识了海外教授", 0, "social", 8, "intellect", 3),
                new LifeChoice("留在实验室赶论文", "论文进度飞快，但错失了人脉", 0, "intellect", 5),
                new LifeChoice("顺便旅游放松一下", "身心愉悦，但没有学术收获", 0, "health", 10, "mental", 5)
            ));

            lifeEvents.Add(new LifeEventData(26, "硕士毕业，面临选择",
                new LifeChoice("继续读博深造", "你留在学术圈，成了年轻学者", 0, "intellect", 10, "social", -3),
                new LifeChoice("进大厂做研究员", "年薪可观，用上了所有所学", 15000, "intellect", 3, "mental", -3),
                new LifeChoice("考公务员求稳定", "工作稳定，但有点浪费专业", 5000, "health", 5, "mental", 5)
            ));
        }
        else if (playerState.graduationChoice == "考公")
        {
            lifeEvents.Add(new LifeEventData(22, "公务员上岸！你被分配到了基层岗位",
                new LifeChoice("扎根基层服务群众", "虽然辛苦但很有成就感", 4000, "mental", 8, "social", 3),
                new LifeChoice("利用业余时间备考遴选", "为以后的发展打基础", 4000, "intellect", 8, "health", -3),
                new LifeChoice("躺平求稳享受生活", "工作轻松，下班后的生活很丰富", 4000, "health", 5, "social", 5)
            ));

            lifeEvents.Add(new LifeEventData(24, "单位内部有竞聘副科的机会",
                new LifeChoice("全力准备竞聘", "成功了！职级提升", 6000, "social", 5, "mental", -3),
                new LifeChoice("踏实做好分内事", "稳扎稳打，口碑很好", 5000, "mental", 5, "health", 3),
                new LifeChoice("考虑辞职去企业", "体制内一眼望到头有点不甘心", 10000, "intellect", 5, "social", -8)
            ));

            lifeEvents.Add(new LifeEventData(27, "工作五年了，你开始思考人生意义",
                new LifeChoice("继续在体制内深耕", "稳步晋升，生活安稳", 8000, "mental", 8, "social", 3),
                new LifeChoice("发展副业增加收入", "开个网店/做自媒体", 12000, "social", 8, "health", -5),
                new LifeChoice("申请调动到大城市", "换个环境，机会更多", 9000, "intellect", 5, "social", 5)
            ));
        }
        else if (playerState.graduationChoice == "就业")
        {
            lifeEvents.Add(new LifeEventData(22, "入职第一天，你的直属领导给你安排了第一个任务",
                new LifeChoice("主动争取核心项目", "你很快脱颖而出，但压力也大", 2000, "intellect", 5, "mental", -5),
                new LifeChoice("先做好基础工作", "稳扎稳打，同事都觉得你靠谱", 1000, "social", 3, "mental", 3),
                new LifeChoice("和同事搞好关系", "办公室社交满分，但业务能力一般", 0, "social", 8, "intellect", -3)
            ));

            lifeEvents.Add(new LifeEventData(24, "公司组织架构调整，你在考虑新去处",
                new LifeChoice("跳槽去更高薪的公司", "薪资翻倍，但新环境需要适应", 10000, "intellect", 3, "mental", -5),
                new LifeChoice("留下等晋升机会", "两年后升了小组长", 3000, "social", 5),
                new LifeChoice("裸辞去旅行找自己", "精神恢复了，但钱包瘪了", -3000, "health", 10, "mental", 8)
            ));

            lifeEvents.Add(new LifeEventData(27, "30岁前最后冲刺，你决定",
                new LifeChoice("全力拼事业升管理层", "你成了最年轻的部门总监", 15000, "social", 5, "health", -5),
                new LifeChoice("平衡工作与生活", "钱够用就好，生活幸福最重要", 3000, "health", 8, "mental", 5),
                new LifeChoice("辞职创业单干", "前路未知，但你充满激情", 0, "intellect", 8, "mental", -8)
            ));
        }
        else if (playerState.graduationChoice == "出国")
        {
            lifeEvents.Add(new LifeEventData(22, "刚到国外，语言和文化都需要适应",
                new LifeChoice("加入中国学生会", "迅速找到组织，生活步入正轨", 0, "social", 8, "mental", 3),
                new LifeChoice("强迫自己和外国人交流", "语言进步飞快，就是有点社恐发作", 0, "intellect", 5, "mental", -5),
                new LifeChoice("窝在宿舍打游戏", "很舒服但很孤单", 0, "health", -5, "mental", -5)
            ));

            lifeEvents.Add(new LifeEventData(24, "毕业了，OPT还剩一年",
                new LifeChoice("争取留在国外工作", "拿到H1B，薪资可观但思乡", 20000, "intellect", 5, "mental", -3),
                new LifeChoice("回国进外企", "海归身份加分，生活舒适", 12000, "social", 5),
                new LifeChoice("先旅居一年再决定", "见了很多世界，心态超好", 0, "health", 5, "mental", 10)
            ));

            lifeEvents.Add(new LifeEventData(27, "而立之年快到了，你开始思考人生",
                new LifeChoice("继续在国外发展", "绿卡有望，生活稳定", 18000, "intellect", 3, "social", -5),
                new LifeChoice("回国创业", "带着海外经验回国打拼", 0, "intellect", 8, "social", 5),
                new LifeChoice("做自由职业者", "数字游民，满世界跑", 8000, "health", 8, "mental", 5)
            ));
        }
        else
        {
            lifeEvents.Add(new LifeEventData(22, "创业初期，你终于找到了第一个客户",
                new LifeChoice("专注打磨产品", "产品质量很好，口碑慢慢起来", 1000, "intellect", 8, "social", -3),
                new LifeChoice("大力推广营销", "用户量暴涨，但产品有点粗糙", 3000, "social", 8, "intellect", -3),
                new LifeChoice("找个合伙人互补", "搭档带来了资金和人脉", 5000, "social", 5, "mental", -3)
            ));

            lifeEvents.Add(new LifeEventData(24, "投资人给了你一份TS（投资意向书）",
                new LifeChoice("接受投资快速发展", "公司估值翻倍，但你股份被稀释", 10000, "social", 5, "mental", -5),
                new LifeChoice("拒绝投资独立生长", "虽然慢但完全自主", 3000, "mental", 5, "health", 3),
                new LifeChoice("把公司卖掉套现", "账上多了几百万，但心里空落落", 50000, "health", -5, "mental", -8)
            ));
        }

        lifeEvents.Add(new LifeEventData(28, "家人催你考虑人生大事",
            new LifeChoice("认真找对象安定下来", "遇到了对的人，结婚了", 0, "social", 5, "mental", 8),
            new LifeChoice("事业为重暂不考虑", "家人有点失望，但你很坚定", 5000, "intellect", 5, "social", -3),
            new LifeChoice("顺其自然随缘吧", "该来的总会来，心态平和", 0, "mental", 8, "social", 3)
        ));

        lifeEvents.Add(new LifeEventData(29, "一个改变人生的机会来了",
            new LifeChoice("全力以赴抓住机会", "你成功了！人生上了一个大台阶", 20000, "intellect", 5, "social", 3),
            new LifeChoice("谨慎评估风险后放弃", "虽然没有惊喜但也没有意外", 3000, "mental", 5, "health", 5),
            new LifeChoice("把这个机会推荐给朋友", "朋友成功了，你收获了珍贵的人脉", 0, "social", 10, "health", 3)
        ));
    }

    void RefreshTopBar()
    {
        if (ageText != null) ageText.text = $"{playerState.currentAge}岁";
        if (careerText != null) careerText.text = playerState.careerPath;
        if (incomeText != null) incomeText.text = $"月入: {playerState.monthlyIncome / 1000f:F1}k";
        if (satisfactionBar != null) satisfactionBar.value = playerState.satisfaction / 5f;
        if (satisfactionText != null) satisfactionText.text = $"{playerState.satisfaction}/5";

        SetStatBar(intellectBar, intellectValueText, playerState.intellect);
        SetStatBar(mentalBar, mentalValueText, playerState.mental);
        SetStatBar(socialBar, socialValueText, playerState.social);
        SetStatBar(healthBar, healthValueText, playerState.health);
    }

    void SetStatBar(Slider bar, Text label, int value)
    {
        if (bar != null) bar.value = value / 100f;
        if (label != null) label.text = value.ToString();
    }

    void ShowCurrentEvent()
    {
        if (lifeEvents == null || currentEventIndex >= lifeEvents.Count)
        {
            ShowLifeSummary();
            return;
        }

        var evt = lifeEvents[currentEventIndex];
        playerState.currentAge = evt.age;

        if (eventCardPanel != null) eventCardPanel.SetActive(true);

        if (eventDescriptionText != null)
            eventDescriptionText.text = $"📍 {evt.age}岁 · {evt.description}";

        LoadLifeIllustration();

        if (choicesContainer != null)
        {
            foreach (Transform child in choicesContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < evt.choices.Length; i++)
            {
                var choice = evt.choices[i];
                var btnGo = Instantiate(choiceBtnPrefab, choicesContainer);
                var btnText = btnGo.GetComponentInChildren<Text>();
                if (btnText != null) btnText.text = choice.text;

                int idx = i;
                btnGo.GetComponent<Button>().onClick.AddListener(() => OnChoice(idx));
            }
        }

        RefreshTopBar();
    }

    void OnChoice(int choiceIndex)
    {
        if (lifeEvents == null || currentEventIndex >= lifeEvents.Count) return;

        var evt = lifeEvents[currentEventIndex];
        var choice = evt.choices[choiceIndex];

        playerState.monthlyIncome += choice.incomeChange;
        if (playerState.monthlyIncome < 0) playerState.monthlyIncome = 0;

        var changes = new Dictionary<string, int>();
        for (int i = 0; i < choice.statKeys.Length; i++)
            changes[choice.statKeys[i]] = choice.statValues[i];
        playerState.ApplyChanges(changes);

        if (evt.age == 28)
        {
            playerState.familyStatus = choice.text.Contains("结婚") || choice.text.Contains("安定")
                ? "已婚" : "单身";
        }

        playerState.satisfaction = Mathf.Clamp(
            playerState.satisfaction + (choice.incomeChange > 5000 ? 1 : 0) +
            (changes.ContainsKey("health") && changes["health"] > 0 ? 1 : 0),
            1, 5);

        var careerParts = new List<string>();
        if (playerState.monthlyIncome >= 30000) careerParts.Add("高管");
        else if (playerState.monthlyIncome >= 15000) careerParts.Add("资深");
        else if (playerState.monthlyIncome >= 8000) careerParts.Add("中级");

        if (!string.IsNullOrEmpty(playerState.careerPath) &&
            !playerState.careerPath.Contains("研究生") && !playerState.careerPath.Contains("留学"))
            careerParts.Add(playerState.careerPath);

        if (careerParts.Count > 0) playerState.careerPath = string.Join(" ", careerParts);

        ShowResult(choice.result);
    }

    void ShowResult(string narrative)
    {
        if (resultPanel == null) return;
        resultPanel.SetActive(true);
        if (resultNarrativeText != null) resultNarrativeText.text = narrative;
        RefreshTopBar();
    }

    void OnContinueAfterResult()
    {
        if (resultPanel != null) resultPanel.SetActive(false);

        currentEventIndex++;

        if (currentEventIndex >= lifeEvents.Count || playerState.currentAge >= 30)
        {
            playerState.currentAge = 30;
            RefreshTopBar();
            ShowLifeSummary();
        }
        else
        {
            ShowCurrentEvent();
        }

        GameStateManager.Instance?.QuickSave();
    }

    void ShowLifeSummary()
    {
        if (eventCardPanel != null) eventCardPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(true);

        playerState.currentAge = 30;
        RefreshTopBar();

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

        Debug.Log($"[LifeUI] 人生总结: {playerState.careerPath}, " +
                  $"月入{playerState.monthlyIncome}, {playerState.familyStatus}, 满意度{playerState.satisfaction}");
    }

    void GoToResult()
    {
        GameStateManager.Instance?.SetStage("result");
        GameStateManager.Instance?.QuickSave();
        GameManager.Instance.ChangePhase(GamePhase.Result);
    }

    void LoadLifeIllustration()
    {
        if (eventIllustration == null) return;

        Gender gender = GameStateManager.Instance?.GetPlayerGender() ?? Gender.Male;
        string illustrationId = currentEventIndex switch
        {
            0 => "internship",
            1 => "teaching",
            2 => "thesis",
            3 => "design_studio",
            4 => "graduation",
            _ => "internship"
        };
        var sprite = ResourceHelper.LoadEventIllustration(illustrationId, gender, "life");
        if (sprite != null) eventIllustration.sprite = sprite;
    }

    private class LifeEventData
    {
        public int age;
        public string description;
        public LifeChoice[] choices;

        public LifeEventData(int age, string description, params LifeChoice[] choices)
        {
            this.age = age;
            this.description = description;
            this.choices = choices;
        }
    }

    private class LifeChoice
    {
        public string text;
        public string result;
        public int incomeChange;
        public string[] statKeys;
        public int[] statValues;

        public LifeChoice(string text, string result, int incomeChange,
            string k1, int v1, string k2 = null, int v2 = 0)
        {
            this.text = text;
            this.result = result;
            this.incomeChange = incomeChange;
            if (k2 != null)
            {
                statKeys = new[] { k1, k2 };
                statValues = new[] { v1, v2 };
            }
            else
            {
                statKeys = new[] { k1 };
                statValues = new[] { v1 };
            }
        }
    }
}
