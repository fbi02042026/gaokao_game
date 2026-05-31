using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EventEngine : MonoBehaviour
{
    private List<GameEvent> eventPool = new List<GameEvent>();
    private PlayerState playerState;

    public void LoadEventPool(List<GameEvent> events)
    {
        eventPool = events ?? new List<GameEvent>();
        Debug.Log($"[EventEngine] 加载 {eventPool.Count} 个事件");
    }

    public void SetPlayerState(PlayerState state)
    {
        playerState = state;
    }

    public GameEvent GetNextEvent()
    {
        if (playerState == null)
        {
            Debug.LogWarning("[EventEngine] playerState 为空，无法获取事件");
            return null;
        }

        if (eventPool.Count == 0)
        {
            Debug.LogWarning("[EventEngine] eventPool 为空");
            return null;
        }

        var candidates = eventPool
            .Where(e => !playerState.completedEvents.Contains(e.id))
            .Where(e => CheckCondition(e.triggerCondition))
            .ToList();

        if (candidates.Count == 0)
        {
            Debug.Log("[EventEngine] 没有可触发的事件");
            return null;
        }

        var weights = candidates.Select(e => CalculateWeight(e)).ToList();
        int totalWeight = weights.Sum();

        if (totalWeight <= 0)
        {
            return candidates[Random.Range(0, candidates.Count)];
        }

        int random = Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            cumulative += weights[i];
            if (random < cumulative)
                return candidates[i];
        }

        return candidates[0];
    }

    private int CalculateWeight(GameEvent evt)
    {
        int weight = 10;
        if (evt.tags != null)
        {
            if (evt.tags.Contains("考试") && playerState.intellect > 70) weight += 5;
            if (evt.tags.Contains("情感") && playerState.social > 60) weight += 3;
            if (evt.tags.Contains("志愿") && playerState.grade >= 5) weight += 8;
        }
        return weight;
    }

    public EventOutcome ResolveEvent(string eventId, string choiceId)
    {
        var evt = eventPool.Find(e => e.id == eventId);
        if (evt == null)
        {
            Debug.LogWarning($"[EventEngine] 找不到事件: {eventId}");
            return null;
        }

        var outcomes = evt.GetOutcomeDict();
        if (!outcomes.ContainsKey(choiceId))
        {
            Debug.LogWarning($"[EventEngine] 事件 {eventId} 没有选择 {choiceId}");
            return null;
        }

        var outcome = outcomes[choiceId];
        playerState.ApplyChanges(outcome.GetStatChanges());
        playerState.completedEvents.Add(eventId);
        playerState.RecordChoice(eventId, choiceId);

        Debug.Log($"[EventEngine] 事件完成 {evt.name} -> {choiceId}: {outcome.narrative}");

        return outcome;
    }

    public void SwitchToMajorPool(string majorId)
    {
        Debug.Log($"[EventEngine] SwitchToMajorPool({majorId}) — 加载专业专属事件");

        var major = DataLoader.Instance?.GetMajorById(majorId);
        if (major == null)
        {
            Debug.LogWarning($"[EventEngine] 未找到专业: {majorId}");
            return;
        }

        if (eventPool != null)
        {
            var categorySpecific = eventPool
                .Where(e => e.tags != null && e.tags.Contains(major.category))
                .ToList();

            if (categorySpecific.Count > 0)
            {
                Debug.Log($"[EventEngine] 专业'{major.name}'(类别:{major.category}) 匹配 {categorySpecific.Count} 个定向事件");
            }
        }

        playerState?.UnlockTalent(majorId);
        Debug.Log($"[EventEngine] 专业'{major.name}' 事件池已切换");
    }

    private bool CheckCondition(TriggerCondition cond)
    {
        if (cond == null) return true;
        if (cond.minGrade > 0 && playerState.grade < cond.minGrade) return false;
        if (cond.minIntellect > 0 && playerState.intellect < cond.minIntellect) return false;
        if (!string.IsNullOrEmpty(cond.requiredEvent) &&
            !playerState.completedEvents.Contains(cond.requiredEvent)) return false;
        return true;
    }

    public IEnumerator LoadEventsFromStreamingAssets(string stage)
    {
        if (DataLoader.Instance == null)
        {
            Debug.LogError("[EventEngine] DataLoader 未初始化");
            yield break;
        }

        List<GameEvent> loaded = null;
        yield return StartCoroutine(DataLoader.Instance.LoadEvents(stage, events => loaded = events));
        LoadEventPool(loaded);
    }

    public void RunSelfTest()
    {
        StartCoroutine(SelfTest());
    }

    private IEnumerator SelfTest()
    {
        Debug.Log("========== [EventEngine] 开始自测（从 StreamingAssets 加载） ==========");

        var testPlayer = new PlayerState();
        testPlayer.intellect = 80;
        testPlayer.social = 70;
        testPlayer.grade = 5;
        SetPlayerState(testPlayer);

        yield return StartCoroutine(LoadEventsFromStreamingAssets("highschool"));

        if (eventPool.Count == 0)
        {
            Debug.LogError("[EventEngine] 自测失败：未能加载 highschool.json");
            yield break;
        }

        Debug.Log($"--- 已加载 {eventPool.Count} 个事件，遍历验证 ---");
        foreach (var evt in eventPool)
        {
            Debug.Log($"  [{evt.id}] {evt.name} | interactionType={evt.interactionType} | " +
                      $"tags=[{string.Join(",", evt.tags ?? new string[0])}] | " +
                      $"trigger={evt.triggerCondition?.minGrade ?? 0}学期");
        }

        Debug.Log("--- GetNextEvent（初始 grade=5, intellect=80）---");
        var evt1 = GetNextEvent();
        Debug.Log(evt1 != null
            ? $"  触发: {evt1.name} (id={evt1.id}, type={evt1.interactionType})"
            : "  结果为 null（异常）");

        if (evt1 != null)
        {
            var outcomes = evt1.GetOutcomeDict();
            Debug.Log($"  可选结局数: {outcomes.Count}");
            foreach (var kv in outcomes)
                Debug.Log($"    [{kv.Key}] {kv.Value.narrative}");

            string pickKey = outcomes.Keys.First();
            Debug.Log($"  --- ResolveEvent({evt1.id}, {pickKey}) ---");
            var outcome1 = ResolveEvent(evt1.id, pickKey);
            if (outcome1 != null)
            {
                Debug.Log($"  叙事: {outcome1.narrative}");
                Debug.Log($"  属性: intellect={testPlayer.intellect} mental={testPlayer.mental} " +
                          $"social={testPlayer.social} health={testPlayer.health}");
                Debug.Log($"  已完成: {testPlayer.completedEvents.Count} 个");
            }
        }

        Debug.Log("--- 再 GetNextEvent（排除+条件过滤后）---");
        var evt2 = GetNextEvent();
        Debug.Log(evt2 != null
            ? $"  触发: {evt2.name} (id={evt2.id}, type={evt2.interactionType})"
            : "  没有更多可触发事件");

        Debug.Log("========== [EventEngine] 自测结束 ==========");
    }
}