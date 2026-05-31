using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject[] systemPrefabs;

    void Awake()
    {
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        if (systemPrefabs != null && systemPrefabs.Length > 0)
        {
            foreach (var prefab in systemPrefabs)
            {
                if (prefab != null)
                    Instantiate(prefab);
            }
        }
        else
        {
            CreateRequiredSystems();
        }

        Debug.Log("[Bootstrapper] 所有系统初始化完成");
    }

    private void CreateRequiredSystems()
    {
        CreateSystem<DejaVuEngine>("DejaVuEngine");
        CreateSystem<TalentEngine>("TalentEngine");
        CreateSystem<SeniorEvalEngine>("SeniorEvalEngine");
        CreateSystem<GameManager>("GameManager");
        CreateSystem<SaveManager>("SaveManager");
        CreateSystem<DataLoader>("DataLoader");
        CreateSystem<PlatformManager>("PlatformManager");

        Debug.Log("[Bootstrapper] 自动创建了 GameManager、SaveManager、DataLoader、PlatformManager");
    }

    private void CreateSystem<T>(string name) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }
    }

    void Start()
    {
        SceneManager.LoadScene("MainGame");
    }
}
