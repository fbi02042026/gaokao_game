using System;
using System.Collections.Generic;
using UnityEngine;
using GaokaoLife.Models;

namespace GaokaoLife.Managers
{
    public class ShareManager : MonoBehaviour
    {
        public static ShareManager Instance { get; private set; }

        private bool isShareAvailable;
        private Texture2D shareImage;

        public event Action OnShareCompleted;
        public event Action OnShareCancelled;

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

            isShareAvailable = PlatformManager.Instance?.HasFeature("share") ?? false;
            
            Debug.Log("[ShareManager] 分享管理器初始化完成");
        }

        public void ShareText(string title, string description, string query = "")
        {
            if (!isShareAvailable)
            {
                Debug.Log("[ShareManager] 分享功能不可用");
                return;
            }

            var platform = PlatformManager.Instance?.CurrentPlatform ?? PlatformType.Editor;

            switch (platform)
            {
                case PlatformType.WeChat:
                    ShareToWeChat(title, description, query);
                    break;
                case PlatformType.Douyin:
                    ShareToDouyin(title, description, query);
                    break;
                default:
                    SimulateShare(title, description);
                    break;
            }
        }

        public void ShareWithImage(string title, string description, Texture2D image, string query = "")
        {
            if (!isShareAvailable)
            {
                Debug.Log("[ShareManager] 分享功能不可用");
                return;
            }

            shareImage = image;
            
            byte[] imageBytes = image.EncodeToPNG();
            string imagePath = Application.persistentDataPath + "/share_image.png";
            System.IO.File.WriteAllBytes(imagePath, imageBytes);

            var platform = PlatformManager.Instance?.CurrentPlatform ?? PlatformType.Editor;

            switch (platform)
            {
                case PlatformType.WeChat:
                    ShareImageToWeChat(title, description, imagePath, query);
                    break;
                case PlatformType.Douyin:
                    ShareImageToDouyin(title, description, imagePath, query);
                    break;
                default:
                    SimulateShareWithImage(title, description);
                    break;
            }
        }

        public void ShareGameResult(PlayerState player, LifeResult result)
        {
            string title = GetShareTitle(result);
            string description = GetShareDescription(player, result);
            
            ShareText(title, description, $"score={result.satisfactionScore}");
        }

        private string GetShareTitle(LifeResult result)
        {
            switch (result.ending)
            {
                case LifeEnding.Excellent:
                    return "🌟 人生巅峰！";
                case LifeEnding.Good:
                    return "✨ 幸福人生！";
                case LifeEnding.Average:
                    return "📊 平凡人生";
                case LifeEnding.Poor:
                    return "💪 继续努力";
                default:
                    return "🎮 高考人生模拟器";
            }
        }

        private string GetShareDescription(PlayerState player, LifeResult result)
        {
            string college = "";
            if (!string.IsNullOrEmpty(player.admittedCollegeId))
            {
                college = $"考入{payer.admittedCollegeId}";
            }

            return $"我在高考人生模拟器中获得{result.satisfactionScore}分满意度！" +
                   $"高考分数:{player.gaokaoScore} " +
                   $"最终职级:{player.careerLevel}级";
        }

        private void ShareToWeChat(string title, string description, string query)
        {
            Debug.Log($"[ShareManager] 分享到微信: {title}");
            OnShareCompleted?.Invoke();
        }

        private void ShareToDouyin(string title, string description, string query)
        {
            Debug.Log($"[ShareManager] 分享到抖音: {title}");
            OnShareCompleted?.Invoke();
        }

        private void ShareImageToWeChat(string title, string description, string imagePath, string query)
        {
            Debug.Log($"[ShareManager] 带图片分享到微信: {title}");
            OnShareCompleted?.Invoke();
        }

        private void ShareImageToDouyin(string title, string description, string imagePath, string query)
        {
            Debug.Log($"[ShareManager] 带图片分享到抖音: {title}");
            OnShareCompleted?.Invoke();
        }

        private void SimulateShare(string title, string description)
        {
            Debug.Log($"[ShareManager] 模拟分享:");
            Debug.Log($"  标题: {title}");
            Debug.Log($"  描述: {description}");
            OnShareCompleted?.Invoke();
        }

        private void SimulateShareWithImage(string title, string description)
        {
            Debug.Log($"[ShareManager] 模拟带图片分享:");
            Debug.Log($"  标题: {title}");
            Debug.Log($"  描述: {description}");
            Debug.Log($"  图片: 已生成");
            OnShareCompleted?.Invoke();
        }

        public Texture2D GenerateShareImage(PlayerState player, LifeResult result)
        {
            int width = 512;
            int height = 512;
            Texture2D texture = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float gradient = (float)(x + y) / (width + height);
                    colors[y * width + x] = Color.Lerp(Color.blue, Color.cyan, gradient);
                }
            }

            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }

        public void SetShareAvailability(bool available)
        {
            isShareAvailable = available;
        }

        public bool IsShareAvailable()
        {
            return isShareAvailable;
        }
    }
}
