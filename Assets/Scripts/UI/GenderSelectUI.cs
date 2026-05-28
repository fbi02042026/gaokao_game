using UnityEngine;
using UnityEngine.UI;

public class GenderSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button boyBtn;
    [SerializeField] private Button girlBtn;
    [SerializeField] private Image boyImage;
    [SerializeField] private Image girlImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Text boyLabelText;
    [SerializeField] private Text girlLabelText;

    public System.Action<Gender> OnGenderSelected;

    void Start()
    {
        if (boyBtn != null) boyBtn.onClick.AddListener(() => SelectGender(Gender.Male));
        if (girlBtn != null) girlBtn.onClick.AddListener(() => SelectGender(Gender.Female));

        LoadImages();
    }

    private void LoadImages()
    {
        if (boyImage != null)
        {
            var sprite = ResourceHelper.LoadGenderBoy();
            if (sprite != null) boyImage.sprite = sprite;
        }

        if (girlImage != null)
        {
            var sprite = ResourceHelper.LoadGenderGirl();
            if (sprite != null) girlImage.sprite = sprite;
        }
    }

    public void Show()
    {
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    private void SelectGender(Gender gender)
    {
        Debug.Log($"[GenderSelectUI] 选择性别: {gender}");

        PlayerState state = null;
        if (GameManager.Instance != null)
            state = GameManager.Instance.CurrentPlayer;
        if (state == null && SaveManager.Instance != null)
            state = SaveManager.Instance.GetCurrentPlayer();
        if (state == null)
            state = new PlayerState();

        state.gender = gender;

        if (SaveManager.Instance != null)
            SaveManager.Instance.SetCurrentPlayer(state);

        if (GameManager.Instance != null)
            GameManager.Instance.UpdatePlayerState(p => p.gender = gender);

        OnGenderSelected?.Invoke(gender);
        Hide();
    }
}
