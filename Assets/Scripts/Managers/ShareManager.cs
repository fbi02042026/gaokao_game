using UnityEngine;
using UnityEngine.UI;

public class ShareManager : MonoBehaviour
{
    public static ShareManager Instance { get; private set; }

    [Header("分享卡片模板")]
    public RectTransform cardTemplate;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public Texture2D GeneratePersonalityCard(PersonalityResult data)
    {
        int w = 750, h = 1334;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color bgTop = ThemeColors.Secondary;
        Color bgBottom = ThemeColors.GradientBaseBottom;

        for (int y = 0; y < h; y++)
        {
            float t = (float)y / h;
            Color bg = Color.Lerp(bgTop, bgBottom, t);
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, bg);
        }

        tex.Apply();
        Debug.Log($"[ShareManager] 生成人格卡片: {data?.personality?.name}");
        return tex;
    }

    public Texture2D GenerateEndingCard(string endingText, string college, string major, int satisfaction)
    {
        int w = 750, h = 1334;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color bgTop = ThemeColors.ShareCardEndingTop[0];
        Color bgBottom = ThemeColors.ShareCardEndingTop[1];

        for (int y = 0; y < h; y++)
        {
            float t = (float)y / h;
            Color bg = Color.Lerp(bgTop, bgBottom, t);
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, bg);
        }

        tex.Apply();
        Debug.Log($"[ShareManager] 生成结局卡片: {endingText}");
        return tex;
    }

    public Texture2D GenerateKnowledgeCard(string majorName, string funFact)
    {
        int w = 750, h = 1334;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color bgTop = ThemeColors.ShareCardKnowledgeTop[0];
        Color bgBottom = ThemeColors.ShareCardKnowledgeTop[1];

        for (int y = 0; y < h; y++)
        {
            float t = (float)y / h;
            Color bg = Color.Lerp(bgTop, bgBottom, t);
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, bg);
        }

        tex.Apply();
        Debug.Log($"[ShareManager] 生成冷知识卡片: {majorName}");
        return tex;
    }

    public void Share(string type)
    {
#if UNITY_WECHAT_GAME
        Debug.Log($"[ShareManager] 微信分享: {type}");
#elif UNITY_BYTE_DANCE_MINI_GAME
        Debug.Log($"[ShareManager] 抖音分享: {type}");
#else
        GUIUtility.systemCopyBuffer = GetShareText(type);
        Debug.Log($"[ShareManager] 复制分享文案: {type}");
#endif
    }

    private string GetShareTitle(string type)
    {
        return type switch
        {
            "personality" => "我是🔬理性探索者！你呢？",
            "ending" => "我选了计算机，30岁居然...",
            "knowledge" => "心理学原来不是读心术！",
            _ => "来测测你的高考人格！"
        };
    }

    private string GetShareText(string type)
    {
        return GetShareTitle(type) + " #高考人生";
    }
}