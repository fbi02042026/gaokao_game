using UnityEngine;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    private Dictionary<string, float> lastShowTime = new Dictionary<string, float>();

    private Dictionary<string, float> cooldownConfig = new Dictionary<string, float>
    {
        {"modify_score", -1f},
        {"unlock_npc", -1f},
        {"refresh_item", 30f},
        {"skip_wait", 0f},
        {"daily_bonus", 86400f},
        {"talent_refresh", -1f},
        {"re_admit", -1f},
        {"show_probability", -1f}
    };

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void ShowRewardedVideo(string type, System.Action<bool> callback)
    {
        if (!CanShowAd(type))
        {
            Debug.Log($"[AdManager] 广告冷却中: {type}");
            callback?.Invoke(false);
            return;
        }

        if (PlatformManager.Instance != null)
        {
            PlatformManager.Instance.ShowRewardedAd(type, (success) =>
            {
                if (success)
                {
                    lastShowTime[type] = Time.time;
                    Debug.Log($"[AdManager] 广告观看完成: {type}");
                }
                callback?.Invoke(success);
            });
        }
        else
        {
            Debug.Log($"[AdManager] 模拟广告成功: {type}");
            lastShowTime[type] = Time.time;
            callback?.Invoke(true);
        }
    }

    public bool CanShowAd(string type)
    {
        if (!lastShowTime.ContainsKey(type)) return true;
        float cd = cooldownConfig.ContainsKey(type) ? cooldownConfig[type] : 0f;
        if (cd < 0) return false;
        if (cd == 0) return true;
        return (Time.time - lastShowTime[type]) >= cd;
    }

    public void ResetPerRound()
    {
        var perRound = new List<string>();
        foreach (var kv in cooldownConfig) if (kv.Value < 0) perRound.Add(kv.Key);
        foreach (var key in perRound) lastShowTime.Remove(key);
    }
}