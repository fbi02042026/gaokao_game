using UnityEngine;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private GameObject provincePanel;
    [SerializeField] private Transform provinceGrid;
    [SerializeField] private GameObject provinceBtnPrefab;
    [SerializeField] private Text selectedProvinceText;
    [SerializeField] private GenderSelectUI genderSelectUI;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Image bgImage;

    private string selectedProvince = "";
    private bool genderSelected = false;

    void Start()
    {
        LoadBackground();

        bool hasSave = SaveManager.Instance != null && SaveManager.Instance.HasSave();

        if (continueBtn != null)
            continueBtn.gameObject.SetActive(hasSave);

        if (startBtn != null)
            startBtn.onClick.AddListener(OnStartGame);

        if (continueBtn != null)
            continueBtn.onClick.AddListener(OnContinueGame);

        if (settingsBtn != null)
            settingsBtn.onClick.AddListener(OnSettings);

        if (provincePanel != null)
            provincePanel.SetActive(false);

        if (selectedProvinceText != null)
            selectedProvinceText.text = "";

        if (!hasSave)
        {
            ShowGenderSelection();
        }
        else
        {
            genderSelected = true;
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
            if (genderSelectUI != null)
                genderSelectUI.Hide();
        }

        Debug.Log("[HomeUI] 主页初始化");
    }

    private void LoadBackground()
    {
        if (bgImage != null)
        {
            var sprite = ResourceHelper.LoadTexLoginBg();
            if (sprite == null)
                sprite = ResourceHelper.LoadTexBg("bg_home");
            if (sprite != null)
                bgImage.sprite = sprite;
        }
    }

    private void ShowGenderSelection()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (genderSelectUI != null)
        {
            genderSelectUI.OnGenderSelected += OnGenderSelected;
            genderSelectUI.Show();
        }
        else
        {
            genderSelected = true;
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);
            Debug.LogWarning("[HomeUI] GenderSelectUI 未赋值，跳过性别选择");
        }
    }

    private void OnGenderSelected(Gender gender)
    {
        genderSelected = true;
        if (genderSelectUI != null)
            genderSelectUI.OnGenderSelected -= OnGenderSelected;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        Debug.Log($"[HomeUI] 性别已选择: {gender}, 显示主菜单");
    }

    void OnStartGame()
    {
        if (!genderSelected)
        {
            Debug.LogWarning("[HomeUI] 请先选择性别");
            return;
        }

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.NewGame();

        var player = SaveManager.Instance?.GetCurrentPlayer();
        if (player != null && !string.IsNullOrEmpty(selectedProvince))
            player.province = selectedProvince;

        Debug.Log("[HomeUI] 开始新游戏");
        GameManager.Instance.ChangePhase(GamePhase.HighSchool);
    }

    void OnContinueGame()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.QuickLoad();
    }

    void OnSettings()
    {
        Debug.Log("[HomeUI] 打开设置");
    }

    public void ToggleProvincePanel()
    {
        if (provincePanel == null) return;

        bool show = !provincePanel.activeSelf;
        provincePanel.SetActive(show);

        if (show && provinceGrid != null && provinceBtnPrefab != null)
        {
            foreach (Transform child in provinceGrid)
                Destroy(child.gameObject);

            var provinces = DataLoader.Instance?.GetProvinces();
            if (provinces != null)
            {
                foreach (var p in provinces)
                {
                    var btnGo = Instantiate(provinceBtnPrefab, provinceGrid);
                    var btnText = btnGo.GetComponentInChildren<Text>();
                    if (btnText != null) btnText.text = p.name;
                    var btn = btnGo.GetComponent<Button>();
                    if (btn != null)
                    {
                        string provinceName = p.name;
                        btn.onClick.AddListener(() => SelectProvince(provinceName));
                    }
                }
            }
        }
    }

    void SelectProvince(string name)
    {
        selectedProvince = name;
        if (selectedProvinceText != null)
            selectedProvinceText.text = $"已选: {name}";
        provincePanel.SetActive(false);
        Debug.Log($"[HomeUI] 选择省份: {name}");
    }
}
