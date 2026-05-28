using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InheritData
{
    public int generation;
    public List<IntPair> inheritedStatsList;
    public List<string> memoryTalentIds;
    public List<string> unlockedEndings;
    public List<string> alumniColleges;
    public List<string> experiencedMajors;
}

public class InheritEngine : MonoBehaviour
{
    public static InheritEngine Instance { get; private set; }

    private InheritData data;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public InheritData GetData()
    {
        return data;
    }

    public void LoadData(InheritData saved)
    {
        data = saved;
        Debug.Log($"[InheritEngine] 加载传承数据, 世代={data?.generation ?? 0}");
    }

    public InheritData StartNewGeneration(PlaythroughMemory previousMemory)
    {
        data = new InheritData
        {
            generation = (data?.generation ?? 0) + 1,
            inheritedStatsList = new List<IntPair>(),
            memoryTalentIds = new List<string>(),
            unlockedEndings = data?.unlockedEndings ?? new List<string>(),
            alumniColleges = data?.alumniColleges ?? new List<string>(),
            experiencedMajors = data?.experiencedMajors ?? new List<string>()
        };

        if (previousMemory?.finalStatsList != null)
        {
            float ratio = Random.Range(0.10f, 0.15f);
            foreach (var kv in previousMemory.finalStatsList)
                data.inheritedStatsList.Add(new IntPair { key = kv.key, value = Mathf.RoundToInt(kv.value * ratio) });
        }

        float roll = Random.value;
        string memoryId = roll < 0.50f ? "D1" : roll < 0.85f ? "D2" : "D3";
        data.memoryTalentIds.Add(memoryId);
        if (data.generation > 2) data.memoryTalentIds.Add("D4");

        if (!string.IsNullOrEmpty(previousMemory?.finalCollege))
            data.alumniColleges.Add(previousMemory.finalCollege);
        if (!string.IsNullOrEmpty(previousMemory?.finalMajor))
            data.experiencedMajors.Add(previousMemory.finalMajor);

        Debug.Log($"[InheritEngine] 第{data.generation}代开始, 记忆天赋={string.Join(",", data.memoryTalentIds)}, " +
                  $"继承属性={data.inheritedStatsList.Count}项, 校友院校={data.alumniColleges.Count}所");

        return data;
    }

    public int GetMaxDejaVuLevel()
    {
        if (data?.memoryTalentIds == null || data.memoryTalentIds.Count == 0) return 0;

        int maxLevel = 0;
        foreach (var id in data.memoryTalentIds)
        {
            switch (id)
            {
                case "D1": maxLevel = Mathf.Max(maxLevel, 1); break;
                case "D2": maxLevel = Mathf.Max(maxLevel, 2); break;
                case "D3": maxLevel = Mathf.Max(maxLevel, 3); break;
                case "D4": maxLevel = Mathf.Max(maxLevel, 4); break;
            }
        }
        return maxLevel;
    }

    public bool IsAlumniCollege(string collegeId)
    {
        return data?.alumniColleges?.Contains(collegeId) ?? false;
    }

    public bool HasExperiencedMajor(string majorId)
    {
        return data?.experiencedMajors?.Contains(majorId) ?? false;
    }

    public void SaveCurrentMemory(PlaythroughMemory memory)
    {
        Debug.Log($"[InheritEngine] 保存第{memory.generation}代记忆到存档");
    }

    public void SelfTest()
    {
        Debug.Log("========== [InheritEngine] 自测: 模拟2代既视感 ==========");

        var gen1Memory = new PlaythroughMemory
        {
            generation = 1,
            talentId = "T01",
            finalCollege = "C01",
            finalMajor = "M01",
            endingType = "good",
            finalStatsList = new List<IntPair>
            {
                new IntPair { key = "intellect", value = 80 },
                new IntPair { key = "mental", value = 60 },
                new IntPair { key = "social", value = 50 },
                new IntPair { key = "health", value = 70 }
            },
            choiceRecords = new List<ChoiceRecord>
            {
                new ChoiceRecord
                {
                    eventId = "HS_001",
                    choiceId = "A",
                    outcomeNarrative = "熬了三个通宵，成绩上去了，但黑眼圈也上去了...",
                    statChangesList = new List<IntPair>
                    {
                        new IntPair { key = "intellect", value = 8 },
                        new IntPair { key = "mental", value = -15 }
                    }
                }
            }
        };

        var gen2Data = StartNewGeneration(gen1Memory);

        Debug.Log($"  世代: {gen2Data.generation}");
        Debug.Log($"  继承属性: {string.Join(", ", gen2Data.inheritedStatsList.ConvertAll(p => $"{p.key}={p.value}"))}");
        Debug.Log($"  记忆天赋: {string.Join(", ", gen2Data.memoryTalentIds)}");
        Debug.Log($"  最大既视感等级: {GetMaxDejaVuLevel()}");
        Debug.Log($"  校友院校: C01 -> {IsAlumniCollege("C01")}");
        Debug.Log($"  非校友: C02 -> {IsAlumniCollege("C02")}");
        Debug.Log($"  体验过专业: M01 -> {HasExperiencedMajor("M01")}");

        var dejaVuEngine = FindObjectOfType<DejaVuEngine>();
        if (dejaVuEngine == null)
        {
            var go = new GameObject("DejaVuEngine_Test");
            dejaVuEngine = go.AddComponent<DejaVuEngine>();
        }

        var memories = new List<PlaythroughMemory> { gen1Memory };
        dejaVuEngine.LoadMemories(memories);

        var result = dejaVuEngine.CheckDejaVu("HS_001", GetMaxDejaVuLevel());
        if (result != null)
        {
            Debug.Log($"  既视感结果:");
            Debug.Log($"    eventId: {result.eventId}");
            Debug.Log($"    previousChoice: {result.previousChoice}");
            Debug.Log($"    hint: \"{result.hint}\"");
            Debug.Log($"    highlightChoiceId: {(result.highlightChoiceId ?? "null")}");
        }
        else
        {
            Debug.Log("  未触发既视感");
        }

        var noResult = dejaVuEngine.CheckDejaVu("HS_999", GetMaxDejaVuLevel());
        Debug.Log($"  无前世记忆事件 HS_999: {(noResult == null ? "null (正确)" : "异常")}");

        Debug.Log("========== [InheritEngine] 自测结束 ==========");
    }
}