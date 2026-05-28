using UnityEngine;
using UnityEditor;
using System.IO;

public class ProvinceIconGenerator
{
    private static readonly string[] PROVINCES = {
        "北京", "天津", "河北", "山西", "内蒙古",
        "辽宁", "吉林", "黑龙江",
        "上海", "江苏", "浙江", "安徽", "福建", "江西", "山东",
        "河南", "湖北", "湖南", "广东", "广西", "海南",
        "重庆", "四川", "贵州", "云南", "西藏",
        "陕西", "甘肃", "青海", "宁夏", "新疆"
    };

    private static readonly Color[] PROVINCE_COLORS = {
        new Color(0.91f, 0.30f, 0.24f), new Color(0.16f, 0.50f, 0.73f), new Color(0.90f, 0.49f, 0.13f),
        new Color(0.80f, 0.20f, 0.20f), new Color(0.20f, 0.60f, 0.20f),
        new Color(0.20f, 0.40f, 0.70f), new Color(0.30f, 0.60f, 0.30f), new Color(0.70f, 0.30f, 0.30f),
        new Color(0.80f, 0.20f, 0.20f), new Color(0.20f, 0.50f, 0.20f), new Color(0.20f, 0.60f, 0.80f),
        new Color(0.80f, 0.40f, 0.20f), new Color(0.20f, 0.70f, 0.30f), new Color(0.70f, 0.20f, 0.20f),
        new Color(0.80f, 0.50f, 0.20f), new Color(0.25f, 0.25f, 0.25f),
        new Color(0.20f, 0.50f, 0.50f), new Color(0.80f, 0.20f, 0.20f), new Color(0.90f, 0.60f, 0.10f),
        new Color(0.20f, 0.60f, 0.20f), new Color(0.20f, 0.30f, 0.80f), new Color(0.20f, 0.70f, 0.80f),
        new Color(0.80f, 0.20f, 0.20f), new Color(0.20f, 0.50f, 0.20f), new Color(0.70f, 0.70f, 0.20f),
        new Color(0.20f, 0.50f, 0.70f), new Color(0.70f, 0.20f, 0.20f), new Color(0.50f, 0.20f, 0.50f),
        new Color(0.80f, 0.20f, 0.20f), new Color(0.20f, 0.60f, 0.20f), new Color(0.50f, 0.50f, 0.20f),
        new Color(0.20f, 0.40f, 0.30f), new Color(0.20f, 0.50f, 0.50f), new Color(0.30f, 0.20f, 0.50f),
        new Color(0.80f, 0.30f, 0.10f), new Color(0.10f, 0.40f, 0.10f), new Color(0.60f, 0.80f, 0.20f),
    };

    [MenuItem("Assets/生成省份图标 (31个)")]
    public static void GenerateAll()
    {
        for (int i = 0; i < PROVINCES.Length; i++)
            GenerateOne(i);

        AssetDatabase.Refresh();
        Debug.Log($"[ProvinceIconGenerator] {PROVINCES.Length} 个省份图标生成完毕！");
    }

    private static void GenerateOne(int index)
    {
        string name = PROVINCES[index];
        Color color = PROVINCE_COLORS[index];

        int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

        Color borderColor = color * 0.7f;
        Color innerColor = Color.Lerp(color, Color.white, 0.3f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int b = 4;
                bool isBorder = x < b || x >= size - b || y < b || y >= size - b;
                tex.SetPixel(x, y, isBorder ? borderColor : innerColor);
            }
        }

        tex.Apply();

        string dir = Path.Combine(Application.dataPath, "Resources", "Provinces");
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, $"province_{index:D2}_{name}.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());

        Object.DestroyImmediate(tex);
    }
}
