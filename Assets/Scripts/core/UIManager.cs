using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Dictionary<GamePhase, GameObject> panelDict = new Dictionary<GamePhase, GameObject>();

    public LoginUI LoginUI { get; private set; }
    public HomeUI HomeUI { get; private set; }
    public TalentSelectUI TalentSelectUI { get; private set; }
    public HighSchoolUI HighSchoolUI { get; private set; }
    public GaokaoUI GaokaoUI { get; private set; }
    public ZhiyuanUI ZhiyuanUI { get; private set; }
    public CollegeUI CollegeUI { get; private set; }
    public LifeUI LifeUI { get; private set; }
    public ResultUI ResultUI { get; private set; }
    public MonoBehaviour SettingsUI { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void RegisterPanel(GamePhase phase, GameObject panelObj)
    {
        if (panelObj == null) return;
        panelDict[phase] = panelObj;
        panelObj.SetActive(false);

        switch (phase)
        {
            case GamePhase.Login: LoginUI = panelObj.GetComponent<LoginUI>(); break;
            case GamePhase.Home: HomeUI = panelObj.GetComponent<HomeUI>(); break;
            case GamePhase.TalentSelect: TalentSelectUI = panelObj.GetComponent<TalentSelectUI>(); break;
            case GamePhase.HighSchool: HighSchoolUI = panelObj.GetComponent<HighSchoolUI>(); break;
            case GamePhase.Gaokao: GaokaoUI = panelObj.GetComponent<GaokaoUI>(); break;
            case GamePhase.Zhiyuan: ZhiyuanUI = panelObj.GetComponent<ZhiyuanUI>(); break;
            case GamePhase.College: CollegeUI = panelObj.GetComponent<CollegeUI>(); break;
            case GamePhase.Life: LifeUI = panelObj.GetComponent<LifeUI>(); break;
            case GamePhase.Result: ResultUI = panelObj.GetComponent<ResultUI>(); break;
        }
    }

    public void ShowPanel(GamePhase phase)
    {
        foreach (var kv in panelDict)
        {
            if (kv.Value != null)
                kv.Value.SetActive(kv.Key == phase);
        }
    }

    public void HideAll()
    {
        foreach (var kv in panelDict)
        {
            if (kv.Value != null)
                kv.Value.SetActive(false);
        }
    }

    public T GetPanel<T>() where T : MonoBehaviour
    {
        foreach (var kv in panelDict)
        {
            var comp = kv.Value.GetComponent<T>();
            if (comp != null) return comp;
        }
        return null;
    }

    public GameObject GetPanelObject(GamePhase phase)
    {
        if (panelDict.TryGetValue(phase, out var obj))
            return obj;
        return null;
    }
}