using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GaokaoUI : MonoBehaviour
{
    [SerializeField] private Text totalScoreText;
    [SerializeField] private Text chineseScoreText;
    [SerializeField] private Text mathScoreText;
    [SerializeField] private Text englishScoreText;
    [SerializeField] private Text comprehensiveScoreText;
    [SerializeField] private Button modifyScoreBtn;
    [SerializeField] private Button startZhiyuanBtn;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private Text provinceText;

    private int finalScore;
    private SubjectScores subjects;
    private string talentId;
    private ScoreEngine scoreEngine;

    void Start()
    {
        scoreEngine = FindObjectOfType<ScoreEngine>();
        if (scoreEngine == null)
        {
            var go = new GameObject("ScoreEngine");
            scoreEngine = go.AddComponent<ScoreEngine>();
        }

        var state = GameStateManager.Instance?.GetPlayerState();
        talentId = TalentEngine.Instance?.GetCurrentTalent()?.id ?? "";

        finalScore = scoreEngine.CalculateScore(state, talentId);
        subjects = scoreEngine.SplitSubjects(finalScore);

        if (provinceText != null && state != null)
            provinceText.text = state.province;

        if (scorePanel != null) scorePanel.SetActive(true);

        modifyScoreBtn?.onClick.AddListener(OnModifyScore);
        startZhiyuanBtn?.onClick.AddListener(OnStartZhiyuan);

        StartCoroutine(ScoreRollAnimation());
    }

    IEnumerator ScoreRollAnimation()
    {
        float duration = 1f;
        float elapsed = 0f;
        int startScore = finalScore - Random.Range(30, 50);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f);

            int displayScore = Mathf.RoundToInt(Mathf.Lerp(startScore, finalScore, t));
            if (totalScoreText != null)
                totalScoreText.text = displayScore.ToString();

            yield return null;
        }

        if (totalScoreText != null)
            totalScoreText.text = finalScore.ToString();

        DisplaySubjects();

        if (modifyScoreBtn != null) modifyScoreBtn.interactable = true;
        if (startZhiyuanBtn != null) startZhiyuanBtn.interactable = true;
    }

    void DisplaySubjects()
    {
        if (chineseScoreText != null) chineseScoreText.text = subjects.chinese.ToString();
        if (mathScoreText != null) mathScoreText.text = subjects.math.ToString();
        if (englishScoreText != null) englishScoreText.text = subjects.english.ToString();
        if (comprehensiveScoreText != null) comprehensiveScoreText.text = subjects.comprehensive.ToString();
    }

    async void OnModifyScore()
    {
        var adManager = AdManager.Instance;
        if (adManager == null) return;

        bool success = await adManager.ShowRewardedVideo("modify_score");
        if (success && finalScore < 750)
        {
            finalScore += 5;
            if (finalScore > 750) finalScore = 750;

            subjects = scoreEngine.SplitSubjects(finalScore);

            if (totalScoreText != null) totalScoreText.text = finalScore.ToString();
            DisplaySubjects();

            if (modifyScoreBtn != null)
            {
                modifyScoreBtn.interactable = false;
                var colors = modifyScoreBtn.colors;
                colors.disabledColor = ThemeColors.Disabled;
                modifyScoreBtn.colors = colors;
            }

            Debug.Log($"[GaokaoUI] 广告加分+5, 最终={finalScore}");
        }
    }

    void OnStartZhiyuan()
    {
        var state = GameStateManager.Instance?.GetPlayerState();
        if (state != null)
            state.gaokaoScore = finalScore;

        GameStateManager.Instance?.SetStage("zhiyuan");
        GameStateManager.Instance?.QuickSave();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Zhiyuan");
    }
}