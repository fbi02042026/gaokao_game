using UnityEngine;
using UnityEditor;
using System.IO;

public class PlaceholderGenerator : EditorWindow
{
    [MenuItem("Assets/生成所有占位资源")]
    public static void GenerateAllPlaceholders()
    {
        GenerateSceneBackgrounds();
        GenerateButtons();
        GenerateCards();
        GenerateIcons();
        GenerateItems();
        GenerateNPCs();
        GenerateTags();
        GenerateUIElements();
        GenerateIllustrations();
        GenerateEndings();
        GenerateEffects();

        AssetDatabase.Refresh();
        Debug.Log("[PlaceholderGenerator] 所有占位资源生成完毕！");
    }

    [MenuItem("Assets/生成场景背景 (7张)")]
    public static void GenerateSceneBackgrounds()
    {
        var specs = new (string name, Color color)[]
        {
            ("bg_home",          ThemeColors.GradientBaseTop),
            ("bg_highschool",    new Color(0.78f, 0.88f, 1f)),
            ("bg_gaokao",        new Color(1f, 0.78f, 0.78f)),
            ("bg_zhiyuan",       new Color(0.85f, 0.82f, 1f)),
            ("bg_college",       new Color(0.72f, 0.85f, 0.76f)),
            ("bg_life",          new Color(1f, 0.88f, 0.76f)),
            ("bg_result",        ThemeColors.GradientBaseBottom),
        };

        foreach (var s in specs)
            SaveTexture($"{s.name}", Placeholder.GenerateGradient(750, 1334, s.color, Color.white));
    }

    [MenuItem("Assets/生成按钮 (20个)")]
    public static void GenerateButtons()
    {
        var names = new[]
        {
            "btn_primary", "btn_secondary", "btn_danger", "btn_success",
            "btn_start", "btn_continue", "btn_settings", "btn_back",
            "btn_share", "btn_ad", "btn_confirm", "btn_cancel",
            "btn_next", "btn_prev", "btn_play", "btn_pause",
            "btn_yes", "btn_no", "btn_close", "btn_info"
        };

        var colors = new[]
        {
            ThemeColors.Primary, ThemeColors.Secondary, Color.red, ThemeColors.Success,
            ThemeColors.Primary, ThemeColors.Accent, Color.gray, Color.gray,
            ThemeColors.Primary, ThemeColors.Gold, ThemeColors.Primary, ThemeColors.TextSecondary,
            ThemeColors.Primary, ThemeColors.TextSecondary, ThemeColors.Primary, ThemeColors.TextSecondary,
            ThemeColors.Success, ThemeColors.Secondary, ThemeColors.TextSecondary, ThemeColors.Primary
        };

        for (int i = 0; i < names.Length; i++)
            SaveTexture($"Textures/Btn/{names[i]}", Placeholder.Generate(300, 80, colors[i], names[i]));
    }

    [MenuItem("Assets/生成卡片底板 (8张)")]
    public static void GenerateCards()
    {
        var specs = new (string name, Color color)[]
        {
            ("card_event",      Color.white),
            ("card_talent",     new Color(1f, 0.97f, 0.88f)),
            ("card_result",     new Color(0.95f, 0.97f, 1f)),
            ("card_personality", new Color(1f, 0.94f, 0.96f)),
            ("card_college",    new Color(0.96f, 0.98f, 0.94f)),
            ("card_major",      new Color(0.97f, 0.94f, 0.98f)),
            ("card_summary",    Color.white),
            ("card_inherit",    new Color(1f, 0.95f, 0.85f)),
        };

        foreach (var s in specs)
            SaveTexture($"Textures/Card/{s.name}", Placeholder.Generate(680, 400, s.color, s.name));
    }

    [MenuItem("Assets/生成图标 (属性/事件/交互)")]
    public static void GenerateIcons()
    {
        var specs = new (string name, Color color)[]
        {
            ("icon_intellect", ThemeColors.StatIntellect),
            ("icon_mental",    ThemeColors.StatMental),
            ("icon_social",    ThemeColors.StatSocial),
            ("icon_health",    ThemeColors.StatHealth),
            ("icon_choice",    ThemeColors.Primary),
            ("icon_merge",     ThemeColors.Accent),
            ("icon_sort",      ThemeColors.Gold),
            ("icon_slider",    ThemeColors.Success),
            ("icon_timing",    ThemeColors.Secondary),
            ("icon_dialog",    ThemeColors.Primary),
            ("icon_star_on",   ThemeColors.Gold),
            ("icon_star_off",  ThemeColors.TextSecondary),
        };

        foreach (var s in specs)
            SaveTexture($"Textures/Icon/{s.name}", Placeholder.Generate(64, 64, s.color, s.name));
    }

    [MenuItem("Assets/生成物品图标 (32个)")]
    public static void GenerateItems()
    {
        string[] names = {
            "item_book", "item_pen", "item_phone", "item_laptop",
            "item_coffee", "item_energy_drink", "item_bread", "item_apple",
            "item_ticket", "item_letter", "item_trophy", "item_medal",
            "item_clock", "item_calendar", "item_money", "item_coin",
            "item_key", "item_lock", "item_lightbulb", "item_magnifier",
            "item_heart", "item_star", "item_shield", "item_sword",
            "item_microphone", "item_camera", "item_palette", "item_music_note",
            "item_plane", "item_compass", "item_gem", "item_crown"
        };

        var random = new System.Random(42);
        for (int i = 0; i < names.Length; i++)
        {
            Color c = Color.HSVToRGB((float)i / names.Length, 0.3f, 0.9f);
            SaveTexture($"Textures/Item/{names[i]}", Placeholder.Generate(128, 128, c, names[i]));
        }
    }

    [MenuItem("Assets/生成NPC立绘 (8个)")]
    public static void GenerateNPCs()
    {
        var specs = new (string name, Color color)[]
        {
            ("npc_teacher",    new Color(0.7f, 0.8f, 0.9f)),
            ("npc_mother",     new Color(1f, 0.85f, 0.85f)),
            ("npc_father",     new Color(0.7f, 0.75f, 0.85f)),
            ("npc_friend_m",   new Color(0.8f, 0.9f, 0.8f)),
            ("npc_friend_f",   new Color(1f, 0.8f, 0.9f)),
            ("npc_professor",  new Color(0.6f, 0.7f, 0.8f)),
            ("npc_xuezhang",   new Color(0.75f, 0.85f, 0.9f)),
            ("npc_elder",      new Color(0.85f, 0.8f, 0.75f)),
        };

        foreach (var s in specs)
            SaveTexture($"Textures/NPC/{s.name}", Placeholder.Generate(400, 600, s.color, s.name));
    }

    [MenuItem("Assets/生成标签 (冲稳保垫)")]
    public static void GenerateTags()
    {
        var specs = new (string name, Color color)[]
        {
            ("tag_chong",   ThemeColors.Secondary),
            ("tag_wen",     ThemeColors.Gold),
            ("tag_bao",     ThemeColors.Success),
            ("tag_dian",    ThemeColors.TextSecondary),
        };

        foreach (var s in specs)
            SaveTexture($"Textures/Tag/{s.name}", Placeholder.Generate(80, 40, s.color, s.name));
    }

    [MenuItem("Assets/生成通用UI元素")]
    public static void GenerateUIElements()
    {
        var specs = new (string name, Color color, int w, int h)[]
        {
            ("ui_slider_bg",     Color.white,               400, 20),
            ("ui_slider_fill",   ThemeColors.Primary,       400, 20),
            ("ui_progress_bg",   ThemeColors.TextSecondary,  600, 24),
            ("ui_progress_fill", ThemeColors.Primary,       600, 24),
            ("ui_panel_bg",      ThemeColors.PageBackground, 700, 1000),
            ("ui_label_bg",      Color.white,               160, 36),
            ("ui_divider",       ThemeColors.Divider,       680, 2),
        };

        foreach (var s in specs)
            SaveTexture($"Textures/UI/{s.name}", Placeholder.Generate(s.w, s.h, s.color, s.name));
    }

    [MenuItem("Assets/生成事件插图 (18张)")]
    public static void GenerateIllustrations()
    {
        Color[] palette = {
            new Color(0.8f, 0.85f, 1f),    new Color(0.9f, 0.8f, 0.9f),
            new Color(0.8f, 0.9f, 0.8f),   new Color(1f, 0.85f, 0.8f),
            new Color(0.85f, 0.8f, 1f),    new Color(0.9f, 0.9f, 0.8f),
            new Color(0.8f, 0.8f, 0.9f),   new Color(1f, 0.88f, 0.9f),
            new Color(0.85f, 0.9f, 0.85f), new Color(0.9f, 0.8f, 0.85f),
            new Color(0.8f, 0.85f, 0.9f),  new Color(0.95f, 0.88f, 0.8f),
            new Color(0.82f, 0.9f, 0.88f), new Color(0.88f, 0.82f, 0.92f),
            new Color(0.9f, 0.85f, 0.85f), new Color(0.8f, 0.88f, 0.85f),
            new Color(0.92f, 0.85f, 0.9f), new Color(0.85f, 0.85f, 0.88f),
        };

        string[] names = {
            "ill_highschool_01", "ill_highschool_02", "ill_highschool_03",
            "ill_gaokao_01", "ill_gaokao_02",
            "ill_zhiyuan_01", "ill_zhiyuan_02",
            "ill_college_01", "ill_college_02", "ill_college_03",
            "ill_life_01", "ill_life_02", "ill_life_03",
            "ill_work_01", "ill_work_02",
            "ill_family_01", "ill_family_02",
            "ill_final_01"
        };

        for (int i = 0; i < names.Length; i++)
            SaveTexture($"Illustrations/{names[i]}", Placeholder.Generate(680, 380, palette[i], names[i]));
    }

    [MenuItem("Assets/生成结局插画 (8张)")]
    public static void GenerateEndings()
    {
        var specs = new (string name, Color color)[]
        {
            ("ending_perfect",     ThemeColors.Gold),
            ("ending_good",        ThemeColors.Success),
            ("ending_normal",      ThemeColors.Primary),
            ("ending_bittersweet", ThemeColors.Secondary),
            ("ending_ceo",         new Color(0.3f, 0.4f, 0.6f)),
            ("ending_scholar",     new Color(0.6f, 0.5f, 0.7f)),
            ("ending_artist",      new Color(1f, 0.7f, 0.8f)),
            ("ending_nomad",       new Color(0.5f, 0.7f, 0.7f)),
        };

        foreach (var s in specs)
            SaveTexture($"Endings/{s.name}", Placeholder.GenerateGradient(750, 1334, s.color, Color.white));
    }

    [MenuItem("Assets/生成特效帧")]
    public static void GenerateEffects()
    {
        var specs = new (string name, Color color, int w, int h)[]
        {
            ("fx_sparkle",   ThemeColors.Gold,       32, 32),
            ("fx_glow",      new Color(1f,1f,1f,0.5f), 128, 128),
            ("fx_ring",      ThemeColors.Primary,     64, 64),
            ("fx_particle",  ThemeColors.Secondary,    16, 16),
            ("fx_dejavu",    ThemeColors.Gold,       128, 128),
        };

        foreach (var s in specs)
            SaveTexture($"Effects/{s.name}", Placeholder.Generate(s.w, s.h, s.color, s.name));
    }

    private static void SaveTexture(string relativePath, Texture2D tex)
    {
        string fullPath = Path.Combine(Application.dataPath, "Resources", relativePath + ".png");
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngData);

        Object.DestroyImmediate(tex);
    }
}
