using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TalentSelectUI : MonoBehaviour
{
    [SerializeField] private Transform talentCardContainer;
    [SerializeField] private GameObject talentCardPrefab;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private GameObject memoryTalentTag;
    [SerializeField] private Text memoryTalentText;

    private List<Talent> drawnTalents;
    private string selectedTalentId;

    private TalentEngine talentEngine;
    private AdManager adManager;
    private InheritEngine inheritEngine;

    void Start()
    {
        talentEngine = FindObjectOfType<TalentEngine>();
        adManager = AdManager.Instance;
        inheritEngine = FindObjectOfType<InheritEngine>();

        confirmBtn?.onClick.AddListener(OnConfirm);
        refreshBtn?.onClick.AddListener(OnRefresh);

        if (memoryTalentTag != null)
            memoryTalentTag.SetActive(false);

        RefreshCards();
    }

    void RefreshCards()
    {
        if (talentEngine == null)
        {
            Debug.LogError("[TalentSelectUI] TalentEngine 未找到");
            return;
        }

        drawnTalents = talentEngine.DrawTalents();

        if (talentCardContainer != null)
        {
            foreach (Transform child in talentCardContainer)
                Destroy(child.gameObject);
        }

        foreach (var talent in drawnTalents)
        {
            var cardGo = Instantiate(talentCardPrefab, talentCardContainer);
            var cardUI = cardGo.GetComponent<TalentCardUI>();
            if (cardUI != null)
            {
                cardUI.SetTalent(talent);
                string tid = talent.id;
                cardUI.OnCardClicked += () => SelectTalent(tid);
            }
        }

        selectedTalentId = "";

        RefreshMemoryTag();
    }

    void SelectTalent(string talentId)
    {
        selectedTalentId = talentId;
        Debug.Log($"[TalentSelectUI] 选择天赋: {talentId}");

        foreach (Transform child in talentCardContainer)
        {
            var cardUI = child.GetComponent<TalentCardUI>();
            if (cardUI != null)
                cardUI.SetHighlighted(cardUI.TalentId == talentId);
        }

        var talent = talentEngine.GetTalentById(talentId);
        if (talent != null)
            talentEngine.SetCurrentTalent(talent);
    }

    void OnConfirm()
    {
        if (string.IsNullOrEmpty(selectedTalentId))
        {
            Debug.Log("[TalentSelectUI] 未选择天赋");
            return;
        }

        Debug.Log($"[TalentSelectUI] 确认天赋: {selectedTalentId}");

        var state = GameStateManager.Instance?.GetPlayerState();
        if (state != null)
        {
            talentEngine.ApplyHighschoolModifiers(selectedTalentId, state);

            var inherited = inheritEngine?.GetData();
            if (inherited?.memoryTalentIds != null)
            {
                foreach (var mid in inherited.memoryTalentIds)
                    Debug.Log($"[TalentSelectUI] 前世记忆天赋: {mid}");
            }
        }

        GameStateManager.Instance?.QuickSave();
    }

    void OnRefresh()
    {
        if (adManager == null)
        {
            Debug.Log("[TalentSelectUI] AdManager 未初始化");
            return;
        }

        adManager.ShowRewardedVideo("talent_refresh", (success) =>
        {
            if (success)
            {
                RefreshCards();
                Debug.Log("[TalentSelectUI] 广告成功后刷新天赋");
            }
        });
    }

    void RefreshMemoryTag()
    {
        if (memoryTalentTag == null || memoryTalentText == null) return;

        var data = inheritEngine?.GetData();
        if (data == null || data.memoryTalentIds == null || data.memoryTalentIds.Count == 0)
        {
            memoryTalentTag.SetActive(false);
            return;
        }

        memoryTalentTag.SetActive(true);
        memoryTalentText.text = $"前世记忆: {data.generation}代";
    }
}