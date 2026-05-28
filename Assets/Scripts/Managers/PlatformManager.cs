using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance { get; private set; }

    public enum GamePlatform
    {
        WeChat,
        Douyin,
        TapTap,
        Web,
        Native
    }

    public GamePlatform CurrentPlatform { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DetectPlatform();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void DetectPlatform()
    {
#if UNITY_WECHAT_GAME
        CurrentPlatform = GamePlatform.WeChat;
#elif UNITY_BYTE_DANCE_MINI_GAME
        CurrentPlatform = GamePlatform.Douyin;
#elif UNITY_WEBGL
        CurrentPlatform = GamePlatform.Web;
#elif UNITY_ANDROID || UNITY_IOS
        CurrentPlatform = GamePlatform.Native;
#else
        CurrentPlatform = GamePlatform.Native;
#endif

        Debug.Log($"[PlatformManager] 检测到平台: {CurrentPlatform}");
    }

    public bool IsMiniGame => CurrentPlatform == GamePlatform.WeChat || CurrentPlatform == GamePlatform.Douyin;
}