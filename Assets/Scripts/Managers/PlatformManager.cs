using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Managers
{
    public class PlatformManager : MonoBehaviour
    {
        public static PlatformManager Instance { get; private set; }

        private PlatformType currentPlatform;
        private bool isInitialized;
        private Dictionary<string, bool> platformFeatures;

        public event Action<PlatformType> OnPlatformDetected;
        public event Action OnSDKInitialized;

        public PlatformType CurrentPlatform => currentPlatform;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            platformFeatures = new Dictionary<string, bool>();
            DetectPlatform();
        }

        void Start()
        {
            InitializeSDK();
        }

        private void DetectPlatform()
        {
#if UNITY_WECHAT
            currentPlatform = PlatformType.WeChat;
#elif UNITY_TOUTIAO
            currentPlatform = PlatformType.Douyin;
#elif UNITY_TAPTAP
            currentPlatform = PlatformType.TapTap;
#elif UNITY_ANDROID
            currentPlatform = PlatformType.Android;
#elif UNITY_IOS
            currentPlatform = PlatformType.iOS;
#else
            currentPlatform = PlatformType.Editor;
#endif

            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                currentPlatform = PlatformType.PC;
            }

            OnPlatformDetected?.Invoke(currentPlatform);
            Debug.Log($"[PlatformManager] 检测到平台: {currentPlatform}");
        }

        private void InitializeSDK()
        {
            switch (currentPlatform)
            {
                case PlatformType.WeChat:
                    InitializeWeChatSDK();
                    break;
                case PlatformType.Douyin:
                    InitializeDouyinSDK();
                    break;
                case PlatformType.TapTap:
                    InitializeTapTapSDK();
                    break;
                default:
                    InitializeDefaultPlatform();
                    break;
            }

            isInitialized = true;
            OnSDKInitialized?.Invoke();
            Debug.Log($"[PlatformManager] SDK初始化完成: {currentPlatform}");
        }

        private void InitializeWeChatSDK()
        {
            platformFeatures["cloud_save"] = true;
            platformFeatures["share"] = true;
            platformFeatures["ad"] = true;
            platformFeatures["payment"] = true;
            platformFeatures["leaderboard"] = true;
            platformFeatures["user_info"] = true;

            Debug.Log("[PlatformManager] 微信SDK初始化");
        }

        private void InitializeDouyinSDK()
        {
            platformFeatures["cloud_save"] = true;
            platformFeatures["share"] = true;
            platformFeatures["ad"] = true;
            platformFeatures["payment"] = true;
            platformFeatures["leaderboard"] = true;
            platformFeatures["user_info"] = true;

            Debug.Log("[PlatformManager] 抖音SDK初始化");
        }

        private void InitializeTapTapSDK()
        {
            platformFeatures["cloud_save"] = true;
            platformFeatures["share"] = true;
            platformFeatures["ad"] = true;
            platformFeatures["payment"] = true;
            platformFeatures["leaderboard"] = true;
            platformFeatures["user_info"] = true;

            Debug.Log("[PlatformManager] TapTap SDK初始化");
        }

        private void InitializeDefaultPlatform()
        {
            platformFeatures["cloud_save"] = false;
            platformFeatures["share"] = false;
            platformFeatures["ad"] = false;
            platformFeatures["payment"] = false;
            platformFeatures["leaderboard"] = false;
            platformFeatures["user_info"] = false;

            Debug.Log("[PlatformManager] 默认平台初始化");
        }

        public bool HasFeature(string feature)
        {
            return platformFeatures.ContainsKey(feature) && platformFeatures[feature];
        }

        public bool IsMiniGame()
        {
            return currentPlatform == PlatformType.WeChat ||
                   currentPlatform == PlatformType.Douyin ||
                   currentPlatform == PlatformType.TapTap;
        }

        public bool IsMobile()
        {
            return currentPlatform == PlatformType.Android ||
                   currentPlatform == PlatformType.iOS ||
                   IsMiniGame();
        }

        public void Login(Action<bool, string> callback)
        {
            switch (currentPlatform)
            {
                case PlatformType.WeChat:
                    WeChatLogin(callback);
                    break;
                case PlatformType.Douyin:
                    DouyinLogin(callback);
                    break;
                case PlatformType.TapTap:
                    TapTapLogin(callback);
                    break;
                default:
                    callback?.Invoke(true, "editor_user");
                    break;
            }
        }

        private void WeChatLogin(Action<bool, string> callback)
        {
            Debug.Log("[PlatformManager] 微信登录");
            callback?.Invoke(true, "wechat_user");
        }

        private void DouyinLogin(Action<bool, string> callback)
        {
            Debug.Log("[PlatformManager] 抖音登录");
            callback?.Invoke(true, "douyin_user");
        }

        private void TapTapLogin(Action<bool, string> callback)
        {
            Debug.Log("[PlatformManager] TapTap登录");
            callback?.Invoke(true, "taptap_user");
        }

        public void GetUserInfo(Action<UserInfo> callback)
        {
            switch (currentPlatform)
            {
                case PlatformType.WeChat:
                    GetWeChatUserInfo(callback);
                    break;
                default:
                    callback?.Invoke(new UserInfo { nickName = "考生" });
                    break;
            }
        }

        private void GetWeChatUserInfo(Action<UserInfo> callback)
        {
            callback?.Invoke(new UserInfo { nickName = "微信用户" });
        }

        public void SetLoadingProgress(float progress)
        {
            switch (currentPlatform)
            {
                case PlatformType.WeChat:
                    wx.showLoading(new { title = "加载中...", mask = true });
                    break;
                case PlatformType.Douyin:
                    tt.showLoading({ title: "加载中..." });
                    break;
            }
        }

        public void HideLoading()
        {
            switch (currentPlatform)
            {
                case PlatformType.WeChat:
                    wx.hideLoading();
                    break;
                case PlatformType.Douyin:
                    tt.hideLoading();
                    break;
            }
        }

        public void ShowToast(string message)
        {
            Debug.Log($"[Toast] {message}");
        }
    }

    public enum PlatformType
    {
        Editor,
        WeChat,
        Douyin,
        TapTap,
        Android,
        iOS,
        PC
    }

    [Serializable]
    public class UserInfo
    {
        public string openId;
        public string nickName;
        public string avatarUrl;
        public int gender;
        public string country;
        public string province;
        public string city;
    }

    public class wx
    {
        public static void showLoading(object options) { }
        public static void hideLoading() { }
    }

    public class tt
    {
        public static void showLoading(object options) { }
        public static void hideLoading() { }
    }
}
