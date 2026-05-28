using UnityEngine;

public static class Placeholder
{
    public static Texture2D Generate(int w, int h, Color color, string label)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        tex.SetPixels(pixels);

        if (!string.IsNullOrEmpty(label))
        {
            DrawRoundedRect(tex, color * 0.8f, w / 4, h / 3, w / 2, h / 3);
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D GenerateGradient(int w, int h, Color top, Color bottom)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            float t = (float)y / h;
            Color c = Color.Lerp(top, bottom, t);
            for (int x = 0; x < w; x++)
                pixels[y * w + x] = c;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static Texture2D GenerateCheckered(int w, int h, Color a, Color b, int cellSize)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int cx = x / cellSize;
                int cy = y / cellSize;
                pixels[y * w + x] = (cx + cy) % 2 == 0 ? a : b;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private static void DrawRoundedRect(Texture2D tex, Color color, int rx, int ry, int rw, int rh)
    {
        int radius = Mathf.Min(rw, rh) / 4;
        for (int y = ry + radius; y < ry + rh - radius; y++)
        {
            for (int x = rx + radius; x < rx + rw - radius; x++)
            {
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    tex.SetPixel(x, y, color);
            }
        }
    }
}
