using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;

public class BuildConfig
{
    private const string COMPANY_NAME = "苹果核工坊";
    private const string PRODUCT_NAME = "中国式高考人生";
    private const string BUNDLE_ID = "com.pingguohe.gaokaolife";

    [MenuItem("Build/配置通用 PlayerSettings")]
    public static void ConfigurePlayerSettings()
    {
        PlayerSettings.productName = PRODUCT_NAME;
        PlayerSettings.companyName = COMPANY_NAME;

        PlayerSettings.defaultScreenWidth = 750;
        PlayerSettings.defaultScreenHeight = 1334;

        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        PlayerSettings.stripEngineCode = true;

        Debug.Log("[BuildConfig] 通用设置完成");
        Debug.Log("  分辨率: 750x1334 (9:16竖版)");
        Debug.Log("  朝向: 竖屏锁定");
        Debug.Log("  代码裁剪: 开启");
    }

    [MenuItem("Build/配置微信小游戏")]
    public static void ConfigureWeChatMiniGame()
    {
        TrySetAppIdForWeChat();

        PlayerSettings.productName = PRODUCT_NAME;
        PlayerSettings.companyName = COMPANY_NAME;

        PlayerSettings.stripEngineCode = true;

        try
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WeixinMiniGame, ManagedStrippingLevel.High);
            Debug.Log("  微信小游戏 Managed Stripping → High");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetManagedStrippingLevel 失败: {e.Message}"); }

        try
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.WeixinMiniGame,
                new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
            Debug.Log("  Graphics APIs → OpenGLES3 only");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetGraphicsAPIs 失败: {e.Message}"); }

        try
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WeixinMiniGame, ScriptingImplementation.IL2CPP);
            Debug.Log("  Scripting Backend → IL2CPP");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetScriptingBackend 失败: {e.Message}"); }

        try
        {
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WeixinMiniGame, ApiCompatibilityLevel.NET_4_6);
            Debug.Log("  API Compatibility → .NET 4.x");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetApiCompatibilityLevel 失败: {e.Message}"); }

        Debug.Log($"[BuildConfig] 微信小游戏配置完成");
        Debug.Log($"  Company: {COMPANY_NAME}");
        Debug.Log($"  Product: {PRODUCT_NAME}");
        Debug.Log($"  Bundle: {BUNDLE_ID}");
    }

    [MenuItem("Build/配置抖音小游戏")]
    public static void ConfigureDouyinMiniGame()
    {
        PlayerSettings.productName = PRODUCT_NAME;
        PlayerSettings.companyName = COMPANY_NAME;

        PlayerSettings.stripEngineCode = true;

        try
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
            Debug.Log("  抖音小游戏 Managed Stripping → High");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetManagedStrippingLevel 失败: {e.Message}"); }

        try
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, ScriptingImplementation.IL2CPP);
            Debug.Log("  Scripting Backend → IL2CPP");
        }
        catch (System.Exception e) { Debug.LogWarning($"  SetScriptingBackend 失败: {e.Message}"); }

        Debug.Log($"[BuildConfig] 抖音小游戏配置完成（请在 Build Settings 中手动切换平台并补充 AppID）");
        Debug.Log($"  Company: {COMPANY_NAME}");
        Debug.Log($"  Product: {PRODUCT_NAME}");
        Debug.Log($"  Bundle: {BUNDLE_ID}");
    }

    [MenuItem("Build/配置所有平台 PlayerSettings")]
    public static void ConfigureAllPlatforms()
    {
        ConfigurePlayerSettings();
        ConfigureWeChatMiniGame();
        ConfigureDouyinMiniGame();
        Debug.Log("[BuildConfig] 所有平台配置完成！");
    }

    private static void TrySetAppIdForWeChat()
    {
        try
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WeixinMiniGame, BUNDLE_ID);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[BuildConfig] 无法设置微信 Bundle ID: {e.Message}");
        }
    }

    [MenuItem("Build/配置包体优化方案")]
    public static void ConfigurePackageOptimization()
    {
        Debug.Log("[BuildConfig] 包体优化方案:");

        try
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WeixinMiniGame, ManagedStrippingLevel.High);
            Debug.Log("  微信小游戏 Managed Stripping → High");
        }
        catch (System.Exception e) { Debug.LogWarning($"  {e.Message}"); }

        try
        {
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
            Debug.Log("  WebGL Managed Stripping → High");
        }
        catch (System.Exception e) { Debug.LogWarning($"  {e.Message}"); }

        try
        {
            PlayerSettings.SetGraphicsAPIs(BuildTarget.WeixinMiniGame,
                new[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
            Debug.Log("  微信小游戏 Graphics APIs → OpenGLES3 only");
        }
        catch (System.Exception e) { Debug.LogWarning($"  {e.Message}"); }

        try
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.WeixinMiniGame, ScriptingImplementation.IL2CPP);
            Debug.Log("  微信小游戏脚本后端 → IL2CPP");
        }
        catch (System.Exception e) { Debug.LogWarning($"  {e.Message}"); }

        Debug.Log("  Shader内存优化: 仅加载使用的变体 + 开启Lightmap/Fog剔除");
        Debug.Log("  纹理压缩: iOS=ASTC / Android=ETC2 / 微信=ASTC");
        Debug.Log("  大数据 → AssetBundle 按需加载（StreamingAssets下）");
        Debug.Log("  Audio → 压缩格式（MP3/Vorbis），Streaming加载");
        Debug.Log("  首包≤4MB: File → Build Settings → 微信 → AutoStreaming 手动开启");
    }

    [MenuItem("Build/构建微信小游戏")]
    public static void BuildWeChat()
    {
        ConfigureWeChatMiniGame();
        ConfigurePlayerSettings();

        string buildPath = EditorUtility.SaveFolderPanel("选择微信小游戏构建目录", "", "wechat-build");
        if (string.IsNullOrEmpty(buildPath)) return;

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = buildPath,
            target = BuildTarget.WeixinMiniGame,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(opts);
        Debug.Log(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded
            ? $"[BuildConfig] 微信小游戏构建成功! 路径: {buildPath}"
            : $"[BuildConfig] 构建失败: {report.summary.totalErrors} 个错误");
    }

    [MenuItem("Build/构建抖音小游戏")]
    public static void BuildDouyin()
    {
        ConfigureDouyinMiniGame();
        ConfigurePlayerSettings();

        Debug.Log("[BuildConfig] 抖音小游戏构建 — 请确保已在 Build Settings 中切换到抖音小游戏平台");
        EditorUtility.DisplayDialog("提示",
            "请先在 File → Build Settings 中手动切换到字节小游戏（抖音）平台，\n然后使用团结引擎的 Build 按钮进行构建。",
            "知道了");
    }

    [MenuItem("Build/查看当前构建配置")]
    public static void PrintCurrentConfig()
    {
        Debug.Log("========== 当前构建配置 ==========");
        Debug.Log($"Product: {PlayerSettings.productName}");
        Debug.Log($"Company: {PlayerSettings.companyName}");
        Debug.Log($"Version: {PlayerSettings.bundleVersion}");
        Debug.Log($"Resolution: {PlayerSettings.defaultScreenWidth}x{PlayerSettings.defaultScreenHeight}");
        Debug.Log($"Strip Engine Code: {PlayerSettings.stripEngineCode}");
        Debug.Log($"Scenes in Build: {EditorBuildSettings.scenes.Count(s => s.enabled)} 个");

        try
        {
            Debug.Log($"WeChat Stripping: {PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.WeixinMiniGame)}");
        }
        catch { Debug.Log("微信平台: ManagedStrippingLevel 不可查"); }

        try
        {
            var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.WeixinMiniGame);
            Debug.Log($"WeChat Graphics APIs: {string.Join(", ", apis)}");
        }
        catch { Debug.Log("微信平台: GraphicsAPIs 不可查"); }

        Debug.Log("====================================");
        Debug.Log("提示: 微信小游戏 AutoStreaming 请在 Build Settings 手动开启");
    }
}
