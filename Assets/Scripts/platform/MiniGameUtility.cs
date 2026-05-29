using System;
using UnityEngine;

public static class MiniGameUtility
{
    private static readonly System.Random _rng = new System.Random();

    public static string GenerateId()
    {
        long timestamp = DateTime.UtcNow.Ticks;
        int random = _rng.Next(100000, 999999);
        return $"{timestamp:x}_{random:x}";
    }

    public static long GetTimestampSeconds()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan span = DateTime.UtcNow - epoch;
        return (long)span.TotalSeconds;
    }

    public static long GetTimestampMilliseconds()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan span = DateTime.UtcNow - epoch;
        return (long)span.TotalMilliseconds;
    }

    public static int RandomRange(int min, int max)
    {
        return _rng.Next(min, max);
    }

    public static float RandomFloat()
    {
        return (float)_rng.NextDouble();
    }

    public static string GetStreamingAssetsPath(string relativePath)
    {
        if (IsMiniGame())
            return Application.streamingAssetsPath + "/" + relativePath;
        return "file://" + Application.streamingAssetsPath + "/" + relativePath;
    }

    public static string GetPlatformName()
    {
#if UNITY_WEIXINMINIGAME
        return "wechat";
#elif UNITY_DOUYINMINIGAME
        return "douyin";
#elif UNITY_KUAISHOUMINIGAME
        return "kuaishou";
#elif UNITY_EDITOR
        return "editor";
#elif UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "ios";
#else
        return "other";
#endif
    }

    public static bool IsMiniGame()
    {
#if UNITY_WEIXINMINIGAME || UNITY_DOUYINMINIGAME || UNITY_KUAISHOUMINIGAME
        return true;
#else
        return false;
#endif
    }

    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}