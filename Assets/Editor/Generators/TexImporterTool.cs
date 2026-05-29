using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureImporterPreset : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        if (importer.assetPath.ToLower().Contains("tex/"))
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.npotScale = TextureImporterNPOTScale.None;
        }
    }
}

public class TexImporterTool : EditorWindow
{
    [MenuItem("Tools/整理 Tex 资源：按文件名归类")]
    public static void SortTexFiles()
    {
        string texRoot = Application.dataPath + "/Resources/Tex";
        string tempFolder = Application.dataPath + "/TempTex";
        if (!Directory.Exists(tempFolder))
        {
            Debug.Log($"请把所有待分类的 Tex 图片放到 Assets/TempTex 文件夹，再运行此命令。");
            Directory.CreateDirectory(tempFolder);
            AssetDatabase.Refresh();
            return;
        }

        string[] files = Directory.GetFiles(tempFolder, "*.png");
        foreach (string f in files)
        {
            string name = Path.GetFileNameWithoutExtension(f);
            string targetPath = "";

            if (name.StartsWith("bg_")) targetPath = texRoot + "/";
            else if (name.StartsWith("boy") || name.StartsWith("girl")) targetPath = texRoot + "/";
            else if (name.StartsWith("男illu") || name.StartsWith("女illu"))
            {
                targetPath = texRoot + (name.Contains("大学") ? "/事件大学/" : "/事件高中/");
            }
            else if (name.StartsWith("男end") || name.StartsWith("女end")) targetPath = texRoot + "/结局/";
            else if (name.StartsWith("icon_") || name.StartsWith("slider_") || name.EndsWith("_slider")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("login_")) targetPath = texRoot + "/";
            else if (name.StartsWith("frame_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("button") || name.Contains("btn")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("tag_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("label_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("card_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("panel_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("border_")) targetPath = texRoot + "/UI/";
            else if (name.StartsWith("setting") || name.Contains("gear") || name.Contains("cog")) targetPath = texRoot + "/UI/";
            else if (name.Contains("arrow") || name.Contains("back") || name.Contains("close")) targetPath = texRoot + "/UI/";
            else
            {
                targetPath = texRoot + "/";
            }

            string dest = targetPath + Path.GetFileName(f);
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(f, dest);
            Debug.Log($"[Sort] {name} → {targetPath}");
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ 资源分类完成，请检查 Assets/Resources/Tex 下的文件是否正确。");
    }
}