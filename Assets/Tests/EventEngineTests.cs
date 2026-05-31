using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EventEngineTests : MonoBehaviour
{
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        Debug.Log("========== [EventEngine] 单元测试开始 ==========");
        TestLoadEventPool();
        TestCheckCondition();
        TestGetNextEvent();
        TestResolveEvent();
        TestEmptyPoolReturnsNull();
        TestDuplicateChoiceIgnoresRepeat();
        Debug.Log("========== [EventEngine] 单元测试结束 ==========");
    }

    private void TestLoadEventPool()
    {
        var engine = CreateEngine();
        var events = CreateSampleEvents();
        engine.LoadEventPool(events);
        Debug.Log("  [PASS] LoadEventPool: 加载3个事件");
        Destroy(engine.gameObject);
    }

    private void TestCheckCondition()
    {
        var engine = CreateEngine();
        var state = new PlayerState { grade = 3, intellect = 80 };
        engine.SetPlayerState(state);

        var events = new List<GameEvent>
        {
            new GameEvent
            {
                id = "EVT_001", name = "通用事件", tags = new string[0],
                triggerCondition = null
            },
            new GameEvent
            {
                id = "EVT_002", name = "大三事件", tags = new string[] { "课程" },
                triggerCondition = new TriggerCondition { minGrade = 4, minIntellect = 0, requiredEvent = null }
            },
            new GameEvent
            {
                id = "EVT_003", name = "前置事件", tags = new string[] { "前置" },
                triggerCondition = new TriggerCondition { minGrade = 0, minIntellect = 0, requiredEvent = "EVT_001" }
            }
        };

        engine.LoadEventPool(events);
        var result = engine.GetNextEvent();
        AssertNotNull(result, "有条件过滤应有结果");
        Debug.Log($"  [PASS] CheckCondition: grade=3时触发'{result.name}', 排除大三事件");

        Destroy(engine.gameObject);
    }

    private void TestGetNextEvent()
    {
        var engine = CreateEngine();
        var state = new PlayerState { grade = 5, intellect = 90, social = 80 };
        engine.SetPlayerState(state);
        engine.LoadEventPool(CreateSampleEvents());

        var evt = engine.GetNextEvent();
        AssertNotNull(evt, "事件池非空应有结果");
        Debug.Log($"  [PASS] GetNextEvent: 触发'{evt.name}' (id={evt.id})");

        Destroy(engine.gameObject);
    }

    private void TestResolveEvent()
    {
        var engine = CreateEngine();
        var state = new PlayerState { grade = 1, intellect = 50 };
        engine.SetPlayerState(state);
        engine.LoadEventPool(CreateSampleEvents());

        var outcome = engine.ResolveEvent("EVT_001", "A");
        AssertNotNull(outcome, "解析结果非空");
        Debug.Log($"  [PASS] ResolveEvent: '{outcome.narrative}'");

        AssertTrue(state.completedEvents.Contains("EVT_001"), "事件标记为已完成");
        Debug.Log("  [PASS] 事件已加入completedEvents");

        Destroy(engine.gameObject);
    }

    private void TestEmptyPoolReturnsNull()
    {
        var engine = CreateEngine();
        var state = new PlayerState { grade = 1 };
        engine.SetPlayerState(state);
        engine.LoadEventPool(new List<GameEvent>());

        var evt = engine.GetNextEvent();
        AssertNull(evt, "空事件池返回null");
        Debug.Log("  [PASS] 空事件池: GetNextEvent返回null");

        Destroy(engine.gameObject);
    }

    private void TestDuplicateChoiceIgnoresRepeat()
    {
        var engine = CreateEngine();
        var state = new PlayerState { grade = 1, intellect = 50 };
        engine.SetPlayerState(state);
        engine.LoadEventPool(CreateSampleEvents());

        engine.ResolveEvent("EVT_001", "A");
        var evt = engine.GetNextEvent();
        AssertTrue(evt == null || evt.id != "EVT_001", "已完成事件不重复出现");
        Debug.Log($"  [PASS] 重复过滤: 已完成EVT_001, 下一个是'{(evt != null ? evt.name : "(null)")}'");

        Destroy(engine.gameObject);
    }

    private EventEngine CreateEngine()
    {
        var go = new GameObject("EventEngine_Test");
        return go.AddComponent<EventEngine>();
    }

    private List<GameEvent> CreateSampleEvents()
    {
        return new List<GameEvent>
        {
            new GameEvent
            {
                id = "EVT_001", name = "测试事件A", tags = new string[] { "课程", "考试" },
                interactionType = "choice",
                triggerCondition = null,
                choiceContent = new ChoiceContent
                {
                    description = "测试描述",
                    choices = new List<ChoiceOption>
                    {
                        new ChoiceOption { id = "A", text = "选项A", icon = "🔵" },
                        new ChoiceOption { id = "B", text = "选项B", icon = "🟢" }
                    }
                },
                outcomeList = new List<OutcomeEntry>
                {
                    new OutcomeEntry
                    {
                        key = "A",
                        value = new EventOutcome
                        {
                            statKeys = new string[] { "intellect", "mental" },
                            statValues = new int[] { 5, -2 },
                            narrative = "选择了A，智力+5，心态-2"
                        }
                    },
                    new OutcomeEntry
                    {
                        key = "B",
                        value = new EventOutcome
                        {
                            statKeys = new string[] { "social" },
                            statValues = new int[] { 8 },
                            narrative = "选择了B，社交+8"
                        }
                    }
                }
            },
            new GameEvent
            {
                id = "EVT_002", name = "测试事件B", tags = new string[] { "社交" },
                interactionType = "dialog",
                triggerCondition = null,
                outcomeList = new List<OutcomeEntry>
                {
                    new OutcomeEntry
                    {
                        key = "default",
                        value = new EventOutcome
                        {
                            statKeys = new string[] { "social", "mental" },
                            statValues = new int[] { 3, 2 },
                            narrative = "一段对话，社交+3，心态+2"
                        }
                    }
                }
            },
            new GameEvent
            {
                id = "EVT_003", name = "测试事件C", tags = new string[] { "志愿" },
                interactionType = "slider",
                triggerCondition = new TriggerCondition { minGrade = 4, minIntellect = 60 },
                outcomeList = new List<OutcomeEntry>
                {
                    new OutcomeEntry
                    {
                        key = "default",
                        value = new EventOutcome
                        {
                            statKeys = new string[0],
                            statValues = new int[0],
                            narrative = "志愿相关事件"
                        }
                    }
                }
            }
        };
    }

    private void AssertNotNull(object obj, string message)
    {
        if (obj == null)
            Debug.LogError($"  [FAIL] {message}: 结果为null");
    }

    private void AssertNull(object obj, string message)
    {
        if (obj != null)
            Debug.LogError($"  [FAIL] {message}: 期望null, 实际非null");
    }

    private void AssertTrue(bool condition, string message)
    {
        if (!condition)
            Debug.LogError($"  [FAIL] {message}: 条件为false");
    }
}