using UnityEngine;

public class ScoreEngineTests : MonoBehaviour
{
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("========== [ScoreEngine] 单元测试开始 ==========");
        TestCalculateScore();
        TestSplitSubjects();
        TestCalculateProbability();
        TestGetProbabilityLabel();
        Debug.Log("========== [ScoreEngine] 单元测试结束 ==========");
    }

    private void TestCalculateScore()
    {
        var state = new PlayerState { intellect = 80, mental = 60, health = 70 };
        var engine = FindObjectOfType<ScoreEngine>();
        if (engine == null)
        {
            var go = new GameObject("ScoreEngine_Test");
            engine = go.AddComponent<ScoreEngine>();
        }

        int score = engine.CalculateScore(state, "");
        AssertBetween(score, 200, 750, "CalculateScore范围");
        Debug.Log($"  [PASS] CalculateScore: intellect=80 → score={score}");

        state.intellect = 100;
        score = engine.CalculateScore(state, "");
        AssertBetween(score, 200, 750, "CalculateScore满智力范围");
        Debug.Log($"  [PASS] CalculateScore: intellect=100 → score={score}");

        state.intellect = 0;
        score = engine.CalculateScore(state, "");
        AssertBetween(score, 200, 750, "CalculateScore零智力范围");
        Debug.Log($"  [PASS] CalculateScore: intellect=0 → score={score}");

        Destroy(engine.gameObject);
    }

    private void TestSplitSubjects()
    {
        var engine = FindObjectOfType<ScoreEngine>();
        if (engine == null)
        {
            var go = new GameObject("ScoreEngine_Test");
            engine = go.AddComponent<ScoreEngine>();
        }

        for (int total = 300; total <= 750; total += 150)
        {
            var subjects = engine.SplitSubjects(total);
            int sum = subjects.GetTotal();

            AssertBetween(subjects.chinese, 90, 120, $"Chinese@{total}");
            AssertBetween(subjects.math, 75, 150, $"Math@{total}");
            AssertBetween(subjects.english, 75, 150, $"English@{total}");
            AssertBetween(sum, total - 30, total + 30, $"TotalSum@{total}");

            Debug.Log($"  [PASS] SplitSubjects({total}): 语{subjects.chinese} 数{subjects.math} 英{subjects.english} 综{subjects.comprehensive} = {sum}");
        }

        Destroy(engine.gameObject);
    }

    private void TestCalculateProbability()
    {
        var engine = FindObjectOfType<ScoreEngine>();
        if (engine == null)
        {
            var go = new GameObject("ScoreEngine_Test");
            engine = go.AddComponent<ScoreEngine>();
        }

        int prob500 = engine.CalculateProbability(500, 450, 550);
        AssertBetween(prob500, 5, 95, "Probability500");
        Debug.Log($"  [PASS] CalculateProbability(500,450,550) = {prob500}%");

        int prob700 = engine.CalculateProbability(700, 450, 550);
        AssertBetween(prob700, 5, 95, "Probability700");
        Debug.Log($"  [PASS] CalculateProbability(700,450,550) = {prob700}%");

        int prob300 = engine.CalculateProbability(300, 450, 550);
        AssertBetween(prob300, 5, 95, "Probability300");
        Debug.Log($"  [PASS] CalculateProbability(300,450,550) = {prob300}%");

        Destroy(engine.gameObject);
    }

    private void TestGetProbabilityLabel()
    {
        var engine = FindObjectOfType<ScoreEngine>();
        if (engine == null)
        {
            var go = new GameObject("ScoreEngine_Test");
            engine = go.AddComponent<ScoreEngine>();
        }

        AssertEqual(engine.GetProbabilityLabel(90), "稳", "Label90");
        AssertEqual(engine.GetProbabilityLabel(70), "保", "Label70");
        AssertEqual(engine.GetProbabilityLabel(50), "冲", "Label50");
        AssertEqual(engine.GetProbabilityLabel(20), "垫", "Label20");
        Debug.Log("  [PASS] GetProbabilityLabel: 90→稳, 70→保, 50→冲, 20→垫");

        Destroy(engine.gameObject);
    }

    private void AssertBetween(int value, int min, int max, string testName)
    {
        if (value < min || value > max)
            Debug.LogError($"  [FAIL] {testName}: {value} 不在范围 [{min}, {max}]");
    }

    private void AssertEqual(string actual, string expected, string testName)
    {
        if (actual != expected)
            Debug.LogError($"  [FAIL] {testName}: 期望'{expected}', 实际'{actual}'");
    }
}