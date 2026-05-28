using UnityEngine;
using UnityEngine.UI;
using System;

public class TalentCardUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text iconText;
    [SerializeField] private Text rarityText;
    [SerializeField] private Text highschoolText;
    [SerializeField] private Text gaokaoText;
    [SerializeField] private Text majorText;
    [SerializeField] private Text careerText;
    [SerializeField] private Image highlightBorder;
    [SerializeField] private Button selectBtn;

    public string TalentId { get; private set; }
    public event Action OnCardClicked;

    void Awake()
    {
        if (selectBtn != null)
            selectBtn.onClick.AddListener(() => OnCardClicked?.Invoke());
    }

    public void SetTalent(Talent talent)
    {
        TalentId = talent.id;

        if (nameText != null) nameText.text = talent.name;
        if (iconText != null) iconText.text = talent.icon;
        if (rarityText != null)
        {
            rarityText.text = talent.rarity switch
            {
                "common" => "普通",
                "rare" => "稀有",
                "epic" => "史诗",
                "legendary" => "传说",
                _ => talent.rarity
            };

            string color = TalentEngine.RarityColor(talent.rarity);
            ColorUtility.TryParseHtmlString(color, out Color c);
            rarityText.color = c;
        }

        if (highschoolText != null)
            highschoolText.text = $"🎓 {talent.effects?.highschool ?? ""}";
        if (gaokaoText != null)
            gaokaoText.text = $"📝 {talent.effects?.gaokao ?? ""}";
        if (careerText != null)
            careerText.text = $"💼 {talent.effects?.career ?? ""}";

        if (majorText != null)
        {
            string majorStr = "";
            if (talent.effects?.major?.boost != null && talent.effects.major.boost.Length > 0)
                majorStr += $"推荐: {string.Join("/", talent.effects.major.boost)}";
            if (talent.effects?.major?.warn != null && talent.effects.major.warn.Length > 0)
                majorStr += $"  ⚠️{string.Join("/", talent.effects.major.warn)}";
            majorText.text = majorStr;
        }

        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (highlightBorder != null)
            highlightBorder.gameObject.SetActive(highlighted);

        if (highlighted)
        {
            transform.localScale = Vector3.one * 1.05f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}