using System;
using System.Collections.Generic;
using UnityEngine;

namespace GaokaoLife.Managers
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        private bool isAdAvailable;
        private bool isAdShowing;
        private string currentAdUnitId;
        
        private Dictionary<AdType, bool> adAvailability;
        private int rewardAmount;

        public event Action<AdType, bool> OnAdStatusChanged;
        public event Action OnAdStarted;
        public event Action OnAdClosed;
        public event Action<bool> OnAdRewarded;

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

            adAvailability = new Dictionary<AdType, bool>();
            foreach (AdType type in Enum.GetValues(typeof(AdType)))
            {
                adAvailability[type] = false;
            }

            isAdAvailable = false;
            isAdShowing = false;
            rewardAmount = 0;

            Debug.Log("[AdManager] 广告管理器初始化完成");
        }

        void Start()
        {
            CheckAdAvailability();
        }

        private void CheckAdAvailability()
        {
            var platform = PlatformManager.Instance?.CurrentPlatform ?? PlatformType.Editor;
            
            bool hasAdFeature = PlatformManager.Instance?.HasFeature("ad") ?? false;
            
            foreach (AdType type in Enum.GetValues(typeof(AdType)))
            {
                adAvailability[type] = hasAdFeature;
            }

            isAdAvailable = hasAdFeature;
            Debug.Log($"[AdManager] 广告可用性检查完成: {isAdAvailable}");
        }

        public bool IsAdAvailable(AdType type)
        {
            return adAvailability.ContainsKey(type) && adAvailability[type] && !isAdShowing;
        }

        public void ShowBannerAd(string adUnitId = "")
        {
            if (!IsAdAvailable(AdType.Banner))
            {
                Debug.Log("[AdManager] Banner广告不可用");
                return;
            }

            currentAdUnitId = adUnitId;
            isAdShowing = true;
            OnAdStarted?.Invoke();
            
            Debug.Log($"[AdManager] 显示Banner广告: {adUnitId}");
        }

        public void HideBannerAd()
        {
            if (isAdShowing)
            {
                isAdShowing = false;
                OnAdClosed?.Invoke();
                Debug.Log("[AdManager] 隐藏Banner广告");
            }
        }

        public void ShowInterstitialAd(string adUnitId, Action onAdClosed = null)
        {
            if (!IsAdAvailable(AdType.Interstitial))
            {
                Debug.Log("[AdManager] 插屏广告不可用");
                onAdClosed?.Invoke();
                return;
            }

            currentAdUnitId = adUnitId;
            isAdShowing = true;
            OnAdStarted?.Invoke();
            
            StartCoroutine(WaitForAdClose(onAdClosed));
            
            Debug.Log($"[AdManager] 显示插屏广告: {adUnitId}");
        }

        public void ShowRewardedVideoAd(string adUnitId, int reward, Action<bool> onAdResult)
        {
            if (!IsAdAvailable(AdType.RewardedVideo))
            {
                Debug.Log("[AdManager] 激励视频广告不可用");
                onAdResult?.Invoke(false);
                return;
            }

            currentAdUnitId = adUnitId;
            rewardAmount = reward;
            isAdShowing = true;
            OnAdStarted?.Invoke();
            
            StartCoroutine(WaitForRewardedAd(adUnitId, onAdResult));
            
            Debug.Log($"[AdManager] 显示激励视频广告: {adUnitId}");
        }

        private System.Collections.IEnumerator WaitForAdClose(Action callback)
        {
            yield return new WaitForSeconds(3f);
            
            isAdShowing = false;
            OnAdClosed?.Invoke();
            callback?.Invoke();
        }

        private System.Collections.IEnumerator WaitForRewardedAd(string adUnitId, Action<bool> callback)
        {
            float waitTime = 0f;
            bool rewarded = false;

            while (waitTime < 5f)
            {
                yield return new WaitForSeconds(0.5f);
                waitTime += 0.5f;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rewarded = true;
                    break;
                }
            }

            isAdShowing = false;
            OnAdClosed?.Invoke();
            OnAdRewarded?.Invoke(rewarded);
            callback?.Invoke(rewarded);
            
            Debug.Log($"[AdManager] 激励视频广告完成, 奖励: {rewarded}");
        }

        public void LoadAd(AdType type, string adUnitId)
        {
            Debug.Log($"[AdManager] 预加载广告: {type} - {adUnitId}");
            
            adAvailability[type] = true;
            OnAdStatusChanged?.Invoke(type, true);
        }

        public void SetAdAvailability(AdType type, bool available)
        {
            adAvailability[type] = available;
            OnAdStatusChanged?.Invoke(type, available);
        }

        public int GetRewardAmount()
        {
            return rewardAmount;
        }

        public bool IsShowingAd()
        {
            return isAdShowing;
        }

        public void SetRewardedAmount(int amount)
        {
            rewardAmount = amount;
        }
    }

    public enum AdType
    {
        Banner,
        Interstitial,
        RewardedVideo,
        Native
    }
}
