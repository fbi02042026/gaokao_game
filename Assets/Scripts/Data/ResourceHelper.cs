using UnityEngine;

public static class ResourceHelper
{
    private static readonly Color DefaultPlaceholderColor = new Color(0.8f, 0.8f, 0.8f);

    public static Sprite LoadSprite(string path, int fallbackW = 128, int fallbackH = 128, Color? fallbackColor = null)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null) return sprite;

        Debug.LogWarning($"[ResourceHelper] resource missing, generating placeholder: {path}");
        var tex = Placeholder.Generate(fallbackW, fallbackH, fallbackColor ?? DefaultPlaceholderColor, path);
        sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public static Sprite LoadSprite(string path, Color fallbackColor)
    {
        return LoadSprite(path, 128, 128, fallbackColor);
    }

    public static Sprite LoadBg(string name)
    {
        return LoadSprite($"Textures/BG/{name}", 750, 1334, ThemeColors.PageBackground);
    }

    public static Sprite LoadBtn(string name)
    {
        return LoadSprite($"Textures/Btn/{name}", 300, 80, ThemeColors.Primary);
    }

    public static Sprite LoadCard(string name)
    {
        return LoadSprite($"Textures/Card/{name}", 680, 400, Color.white);
    }

    public static Sprite LoadIcon(string name)
    {
        return LoadSprite($"Textures/Icon/{name}", 64, 64);
    }

    public static Sprite LoadItem(string name)
    {
        return LoadSprite($"Textures/Item/{name}", 128, 128);
    }

    public static Sprite LoadNpc(string name)
    {
        return LoadSprite($"Textures/NPC/{name}", 400, 600);
    }

    public static Sprite LoadTag(string name)
    {
        return LoadSprite($"Textures/Tag/{name}", 80, 40);
    }

    public static Sprite LoadUI(string name)
    {
        return LoadSprite($"Textures/UI/{name}", 64, 64);
    }

    public static Sprite LoadIllustration(string name)
    {
        return LoadSprite($"Illustrations/{name}", 680, 380);
    }

    public static Sprite LoadEnding(string name)
    {
        return LoadSprite($"Endings/{name}", 750, 1334, ThemeColors.Gold);
    }

    public static Sprite LoadEffect(string name)
    {
        return LoadSprite($"Effects/{name}", 64, 64, new Color(1f, 1f, 1f, 0.5f));
    }

    public static Sprite LoadProvince(int index)
    {
        string path = index switch
        {
            0 => "province_00_北京", 1 => "province_01_天津", 2 => "province_02_河北",
            3 => "province_03_山西", 4 => "province_04_内蒙古",
            5 => "province_05_辽宁", 6 => "province_06_吉林", 7 => "province_07_黑龙江",
            8 => "province_08_上海", 9 => "province_09_江苏", 10 => "province_10_浙江",
            11 => "province_11_安徽", 12 => "province_12_福建", 13 => "province_13_江西", 14 => "province_14_山东",
            15 => "province_15_河南", 16 => "province_16_湖北", 17 => "province_17_湖南", 18 => "province_18_广东",
            19 => "province_19_广西", 20 => "province_20_海南",
            21 => "province_21_重庆", 22 => "province_22_四川", 23 => "province_23_贵州", 24 => "province_24_云南",
            25 => "province_25_西藏",
            26 => "province_26_陕西", 27 => "province_27_甘肃", 28 => "province_28_青海", 29 => "province_29_宁夏",
            30 => "province_30_新疆",
            _ => null
        };
        return path != null ? LoadSprite($"Provinces/{path}", 128, 128) : null;
    }

    public static Sprite LoadTexBg(string bgName)
    {
        return LoadSprite($"Tex/{bgName}", 750, 1334, ThemeColors.PageBackground);
    }

    public static Sprite LoadTexNpc(string npcName)
    {
        return LoadSprite($"Tex/{npcName}", 400, 600);
    }

    public static Sprite LoadTexLoginBg()
    {
        return LoadSprite("Tex/login_v2", 750, 1334, ThemeColors.PageBackground);
    }

    public static Sprite LoadGenderBoy()
    {
        return LoadSprite("Tex/boy", 400, 600, ThemeColors.Primary);
    }

    public static Sprite LoadGenderGirl()
    {
        return LoadSprite("Tex/girl", 400, 600, ThemeColors.Secondary);
    }

    public static Sprite LoadEventIllustration(string illustrationId, Gender gender, string stage)
    {
        string prefix = gender == Gender.Male ? "男illu" : "女illu";
        string stageFolder = stage switch
        {
            "highschool" => "事件高中",
            "college" => "事件大学",
            "life" => "事件大学",
            _ => "事件高中"
        };
        string path = $"Tex/{stageFolder}/{prefix}_{illustrationId}";
        return LoadSprite(path, 680, 380, new Color(0.9f, 0.9f, 0.95f));
    }

    public static Sprite LoadEndingIllustration(string endingId, Gender gender)
    {
        string prefix = gender == Gender.Male ? "男end" : "女end";
        string path = $"Tex/结局/{prefix}_{endingId}";
        return LoadSprite(path, 750, 1334, ThemeColors.Gold);
    }

    public static string GetIllustrationIdForEvent(string eventId, string stage)
    {
        if (illustrationMap.TryGetValue(eventId, out string id))
            return id;

        string fallback = stage switch
        {
            "highschool" => "monthly_exam",
            "college" => "coding",
            "life" => "internship",
            _ => "monthly_exam"
        };
        Debug.LogWarning($"[ResourceHelper] no illustration mapping for event: {eventId}, using fallback: {fallback}");
        return fallback;
    }

    private static readonly System.Collections.Generic.Dictionary<string, string> illustrationMap = new System.Collections.Generic.Dictionary<string, string>
    {
        { "hs_01", "monthly_exam" },
        { "hs_02", "love_letter" },
        { "hs_03", "sports_day" },
        { "hs_04", "parent_meeting" },
        { "hs_05", "art_science" },
        { "hs_06", "insomnia" },
        { "hs_07", "dormitory" },
        { "hs_08", "cram" },
        { "hs_09", "cram" },
        { "hs_10", "graduation" },
        { "hs_11", "monthly_exam" },
        { "hs_12", "dormitory" },
        { "hs_13", "parent_meeting" },
        { "hs_14", "cram" },
        { "hs_15", "parent_meeting" },
        { "hs_16", "monthly_exam" },
        { "hs_17", "sports_day" },
        { "hs_18", "dormitory" },
        { "hs_19", "dormitory" },
        { "hs_20", "graduation" },
        { "cs_01", "coding" },
        { "cs_02", "thesis" },
        { "cs_03", "coding" },
        { "cs_04", "internship" },
        { "cs_05", "teaching" },
        { "biz_01", "teaching" },
        { "biz_02", "design_studio" },
        { "biz_03", "internship" },
        { "biz_04", "thesis" },
        { "law_01", "moot_court" },
        { "law_02", "teaching" },
        { "law_03", "thesis" },
        { "law_04", "internship" },
        { "med_01", "anatomy" },
        { "med_02", "lab" },
        { "med_03", "thesis" },
        { "med_04", "internship" },
        { "med_05", "teaching" },
        { "life_01", "internship" },
        { "life_02", "teaching" },
        { "life_03", "design_studio" },
        { "life_04", "thesis" },
        { "life_05", "dormitory" },
        { "life_06", "lab" },
        { "life_07", "internship" },
        { "life_08", "graduation" },
        { "HS_001", "monthly_exam" },
        { "HS_002", "cram" },
        { "HS_003", "zhiyuan_preview" },
        { "HS_004", "parent_meeting" },
        { "HS_005", "art_science" },
        { "HS_006", "insomnia" },
        { "HS_007", "dormitory" },
        { "HS_008", "cram" },
        { "HS_009", "teaching" },
        { "HS_010", "graduation" },
        { "HS_011", "monthly_exam" },
        { "HS_012", "dormitory" },
        { "HS_013", "parent_meeting" },
        { "HS_014", "cram" },
        { "HS_015", "parent_meeting" },
        { "HS_016", "monthly_exam" },
        { "HS_017", "sports_day" },
        { "HS_018", "dormitory" },
        { "HS_019", "dormitory" },
        { "HS_020", "graduation" },
    };
}
