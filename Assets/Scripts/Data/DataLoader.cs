using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance { get; private set; }

    private List<College> colleges;
    private List<Major> majors;
    private List<Province> provinces;
    private List<GameEvent> eventPool;
    private bool isLoaded = false;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public IEnumerator PreloadAll()
    {
        yield return StartCoroutine(LoadJson<College>("colleges", result => colleges = new List<College>(result)));
        yield return StartCoroutine(LoadJson<Major>("majors", result => majors = new List<Major>(result)));
        yield return StartCoroutine(LoadJson<Province>("provinces", result => provinces = new List<Province>(result)));
        isLoaded = true;
        Debug.Log($"[DataLoader] 加载完成: {colleges.Count}所院校, {majors.Count}个专业, {provinces.Count}个省份");
    }

    private IEnumerator LoadJson<T>(string fileName, System.Action<T[]> callback)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName + ".json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                T[] wrapper = JsonHelper.FromJson<T>(request.downloadHandler.text);
                callback(wrapper);
            }
            else
            {
                Debug.LogError($"[DataLoader] 加载失败 {fileName}: {request.error}");
                callback(new T[0]);
            }
        }
    }

    public IEnumerator LoadEvents(string stageName, System.Action<List<GameEvent>> callback)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Events", stageName + ".json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                GameEvent[] wrapper = JsonHelper.FromJson<GameEvent>(request.downloadHandler.text);
                eventPool = new List<GameEvent>(wrapper);
                Debug.Log($"[DataLoader] 加载事件 {stageName}: {eventPool.Count} 个");
                callback(eventPool);
            }
            else
            {
                Debug.LogError($"[DataLoader] 加载事件失败 {stageName}: {request.error}");
                callback(new List<GameEvent>());
            }
        }
    }

    public IEnumerator LoadTalents(System.Action<List<Talent>> callback)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "talents.json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Talent[] wrapper = JsonHelper.FromJson<Talent>(request.downloadHandler.text);
                var list = new List<Talent>(wrapper);
                Debug.Log($"[DataLoader] 加载天赋: {list.Count} 个");
                callback(list);
            }
            else
            {
                Debug.LogError($"[DataLoader] 加载天赋失败: {request.error}");
                callback(new List<Talent>());
            }
        }
    }

    public IEnumerator LoadCollegesFromStreamingAssets(System.Action<List<College>> callback)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "colleges.json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                College[] wrapper = JsonHelper.FromJson<College>(request.downloadHandler.text);
                colleges = new List<College>(wrapper);
                Debug.Log($"[DataLoader] 加载院校: {colleges.Count} 所");
                callback(colleges);
            }
            else
            {
                Debug.LogError($"[DataLoader] 加载院校失败: {request.error}");
                callback(new List<College>());
            }
        }
    }

    public IEnumerator LoadMajorsFromStreamingAssets(System.Action<List<Major>> callback)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "majors.json");
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Major[] wrapper = JsonHelper.FromJson<Major>(request.downloadHandler.text);
                majors = new List<Major>(wrapper);
                Debug.Log($"[DataLoader] 加载专业: {majors.Count} 个");
                callback(majors);
            }
            else
            {
                Debug.LogError($"[DataLoader] 加载专业失败: {request.error}");
                callback(new List<Major>());
            }
        }
    }

    public List<College> GetColleges(string provinceName = null, int? score = null)
    {
        if (colleges == null) return new List<College>();

        var result = colleges.AsEnumerable();

        if (!string.IsNullOrEmpty(provinceName))
        {
            result = result.Where(c => c.province == provinceName);
        }

        if (score.HasValue)
        {
            result = result.Where(c =>
            {
                int min = c.GetMinScore(provinceName ?? c.province);
                return score.Value >= min;
            });
        }

        return result.OrderByDescending(c =>
        {
            if (!string.IsNullOrEmpty(provinceName))
                return c.GetMinScore(provinceName);
            return c.GetMinScore(c.province);
        }).ToList();
    }

    public List<Major> GetMajors(string category = null)
    {
        if (majors == null) return new List<Major>();

        if (!string.IsNullOrEmpty(category))
            return majors.Where(m => m.category == category).ToList();

        return majors;
    }

    public List<Province> GetProvinces()
    {
        return provinces ?? new List<Province>();
    }

    public College GetCollegeById(string id)
    {
        return colleges?.FirstOrDefault(c => c.id == id);
    }

    public Major GetMajorById(string id)
    {
        return majors?.FirstOrDefault(m => m.id == id);
    }

    public List<Major> GetCollegeMajors(string collegeId)
    {
        if (collegeId == null || majors == null) return new List<Major>();
        College college = GetCollegeById(collegeId);
        if (college == null || college.majors == null) return new List<Major>();

        List<Major> result = new List<Major>();
        foreach (string majorId in college.majors)
        {
            Major m = GetMajorById(majorId);
            if (m != null) result.Add(m);
        }
        return result;
    }

    public bool IsDataLoaded()
    {
        return isLoaded;
    }

    public void LoadAllData(System.Action callback)
    {
        StartCoroutine(LoadAllDataRoutine(callback));
    }

    private IEnumerator LoadAllDataRoutine(System.Action callback)
    {
        yield return StartCoroutine(LoadJson<College>("colleges", result => colleges = new List<College>(result)));
        yield return StartCoroutine(LoadJson<Major>("majors", result => majors = new List<Major>(result)));
        yield return StartCoroutine(LoadJson<Province>("provinces", result => provinces = new List<Province>(result)));
        isLoaded = true;
        Debug.Log($"[DataLoader] LoadAllData 完成: {colleges.Count}所院校, {majors.Count}个专业, {provinces.Count}个省份");
        callback?.Invoke();
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrapped = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}