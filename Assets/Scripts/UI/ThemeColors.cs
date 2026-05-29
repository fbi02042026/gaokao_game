using UnityEngine;

public static class ThemeColors
{
    public static Color PageBackground => new Color(1f, 0.97f, 0.94f);

    public static Color GradientBaseTop => new Color(1f, 0.91f, 0.84f);
    public static Color GradientBaseBottom => new Color(1f, 0.96f, 0.93f);

    public static Color CardBackground => Color.white;

    public static Color Primary => new Color(0.42f, 0.62f, 0.97f);
    public static Color Secondary => new Color(1f, 0.71f, 0.71f);
    public static Color Accent => new Color(1f, 0.80f, 0.64f);
    public static Color Success => new Color(0.66f, 0.90f, 0.81f);
    public static Color Gold => new Color(1f, 0.85f, 0.24f);

    public static Color TextPrimary => new Color(0.29f, 0.29f, 0.29f);
    public static Color TextSecondary => new Color(0.60f, 0.60f, 0.60f);

    public static Color Divider => new Color(1f, 0.91f, 0.91f);

    public static Color StatIntellect => new Color(0.42f, 0.62f, 0.97f);
    public static Color StatMental => new Color(1f, 0.71f, 0.71f);
    public static Color StatSocial => new Color(1f, 0.85f, 0.24f);
    public static Color StatHealth => new Color(0.66f, 0.90f, 0.81f);

    public static Color StatVitality => new Color(1f, 0.60f, 0.30f);
    public static Color StatStress => new Color(0.80f, 0.40f, 0.50f);
    public static Color StatFriends => new Color(0.30f, 0.75f, 1f);
    public static Color StatMoney => new Color(0.30f, 0.80f, 0.40f);

    public static Color GetStatColor(string statKey)
    {
        return statKey switch
        {
            "intellect" => StatIntellect,
            "mental" => StatMental,
            "social" => StatSocial,
            "health" => StatHealth,
            "vitality" => StatVitality,
            "stress" => StatStress,
            "friends" => StatFriends,
            "money" => StatMoney,
            _ => TextPrimary
        };
    }

    public static Color ProbabilityBarColor(int probability)
    {
        if (probability >= 80) return Success;
        if (probability >= 60) return Gold;
        if (probability >= 30) return Accent;
        return Secondary;
    }

    public static Color Disabled => new Color(0.5f, 0.5f, 0.5f);

    public static Color DejaVuGlow => new Color(1f, 0.84f, 0.42f, 1f);

    public static Color TimelineActive => Primary;
    public static Color TimelinePast => new Color(0.7f, 0.7f, 0.7f);
    public static Color TimelineFuture => new Color(0.9f, 0.9f, 0.9f);

    public static readonly Color[] ShareCardPersonalityTop = { Secondary, new Color(1f, 0.96f, 0.93f) };
    public static readonly Color[] ShareCardEndingTop = { new Color(0.67f, 0.85f, 0.90f), new Color(0.93f, 0.95f, 0.98f) };
    public static readonly Color[] ShareCardKnowledgeTop = { new Color(0.87f, 0.93f, 0.80f), new Color(0.95f, 0.97f, 0.93f) };
}
