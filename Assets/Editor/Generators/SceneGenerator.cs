using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneGenerator : EditorWindow
{
    private const string SCENE_DIR = "Assets/Scenes";

    [MenuItem("Tools/生成场景 (启动场景+主游戏场景)")]
    public static void GenerateScenes()
    {
        if (!Directory.Exists(SCENE_DIR))
            Directory.CreateDirectory(SCENE_DIR);

        CreateBootScene();
        CreateMainGameScene();
        SetupBuildSettings();

        AssetDatabase.Refresh();
        Debug.Log("[SceneGenerator] 场景生成完成！请在 Build Settings 中确认场景顺序。");
    }

    private static void CreateBootScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject bootstrapper = new GameObject("GameBootstrapper");
        bootstrapper.AddComponent<GameBootstrapper>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        EditorSceneManager.SaveScene(scene, Path.Combine(SCENE_DIR, "BootScene.unity"));
        Debug.Log("[SceneGenerator] BootScene 已创建");
    }

    private static void CreateMainGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject canvasGO = new GameObject("UIRoot", typeof(RectTransform));
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(750, 1334);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        GameObject uiManagerGO = new GameObject("UIManager");
        UIManager uiManager = uiManagerGO.AddComponent<UIManager>();

        GameObject bg = new GameObject("Background", typeof(RectTransform));
        bg.transform.SetParent(canvasGO.transform, false);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.95f, 0.95f, 0.98f);

        string prefabDir = "Assets/Resources/Prefabs";
        string[] panelNames = {
            "LoginPanel", "HomePanel", "TalentSelectPanel", "HighSchoolPanel",
            "GaokaoPanel", "ZhiyuanPanel", "CollegePanel",
            "LifePanel", "ResultPanel", "SettingsPanel"
        };
        GamePhase[] phases = {
            GamePhase.Login, GamePhase.Home, GamePhase.TalentSelect, GamePhase.HighSchool,
            GamePhase.Gaokao, GamePhase.Zhiyuan, GamePhase.College,
            GamePhase.Life, GamePhase.Result, GamePhase.Home
        };

        for (int i = 0; i < panelNames.Length; i++)
        {
            string prefabPath = Path.Combine(prefabDir, panelNames[i] + ".prefab");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject panelGO;

            if (prefab != null)
            {
                panelGO = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvasGO.transform);
                panelGO.name = panelNames[i];
                Debug.Log($"[SceneGenerator] 从预制体创建: {panelNames[i]}");
            }
            else
            {
                panelGO = new GameObject(panelNames[i], typeof(RectTransform));
                panelGO.transform.SetParent(canvasGO.transform, false);
                RectTransform prect = panelGO.GetComponent<RectTransform>();
                prect.anchorMin = Vector2.zero;
                prect.anchorMax = Vector2.one;
                prect.offsetMin = Vector2.zero;
                prect.offsetMax = Vector2.zero;
                Image pimg = panelGO.AddComponent<Image>();
                pimg.color = new Color(0, 0, 0, 0);
                Debug.LogWarning($"[SceneGenerator] 预制体不存在，创建空面板: {panelNames[i]}");
            }

            panelGO.SetActive(i == 0);
            uiManager.RegisterPanel(phases[i], panelGO);
        }

        EditorSceneManager.SaveScene(scene, Path.Combine(SCENE_DIR, "MainGame.unity"));
        Debug.Log("[SceneGenerator] MainGame 场景已创建，共注册 " + panelNames.Length + " 个面板");
    }

    private static void SetupBuildSettings()
    {
        var scenes = EditorBuildSettings.scenes;
        bool hasBoot = false, hasMain = false;
        foreach (var s in scenes)
        {
            if (s.path.Contains("BootScene")) hasBoot = true;
            if (s.path.Contains("MainGame")) hasMain = true;
        }

        var sceneList = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        sceneList.Add(new EditorBuildSettingsScene("Assets/Scenes/BootScene.unity", true));
        sceneList.Add(new EditorBuildSettingsScene("Assets/Scenes/MainGame.unity", true));

        foreach (var s in scenes)
        {
            if (!s.path.Contains("BootScene") && !s.path.Contains("MainGame"))
                sceneList.Add(s);
        }

        EditorBuildSettings.scenes = sceneList.ToArray();
        Debug.Log("[SceneGenerator] Build Settings 已更新");
    }
}
