using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SeniorEvalPanel : MonoBehaviour
{
    [Header("标题")]
    [SerializeField] private Text titleText;
    [SerializeField] private Button closeBtn;

    [Header("免费评价区")]
    [SerializeField] private Text freeReviewerIdentity;
    [SerializeField] private Text freeContentText;
    [SerializeField] private Transform freeTagsContainer;
    [SerializeField] private GameObject tagChipPrefab;
    [SerializeField] private Text dejaVuTag;
    [SerializeField] private GameObject dejaVuHint;

    [Header("更多评价区")]
    [SerializeField] private Transform lockedListContainer;
    [SerializeField] private GameObject lockedItemPrefab;
    [SerializeField] private Button unlockAllBtn;
    [SerializeField] private Text unlockAllLabel;

    [Header("NPC类型卡片")]
    [SerializeField] private Transform npcCardContainer;
    [SerializeField] private GameObject npcCardPrefab;

    [Header("无评价提示")]
    [SerializeField] private GameObject noEvalTip;

    private string currentMajorId;
    private SeniorEval freeTrialEval;
    private List<SeniorEval> allEvalsForMajor;
    private List<SeniorEval> lockedEvals;
    private int currentUnlockIndex;

    void Awake()
    {
        closeBtn?.onClick.AddListener(Close);
        unlockAllBtn?.onClick.AddListener(OnUnlockAll);

        if (dejaVuHint != null) dejaVuHint.SetActive(false);
        if (noEvalTip != null) noEvalTip.SetActive(false);
    }

    public void Show(string majorId)
    {
        currentMajorId = majorId;
        gameObject.SetActive(true);

        if (titleText != null) titleText.text = majorId;

        allEvalsForMajor = SeniorEvalEngine.Instance?.GetEvalsByMajor(majorId) ?? new List<SeniorEval>();

        if (allEvalsForMajor.Count == 0)
        {
            ShowNoEvals();
            return;
        }

        if (noEvalTip != null) noEvalTip.SetActive(false);

        freeTrialEval = SeniorEvalEngine.Instance?.GetFreeTrial(majorId);
        lockedEvals = allEvalsForMajor.Where(e => e.adLocked && !SeniorEvalEngine.Instance.IsUnlocked(e.evalId)).ToList();
        currentUnlockIndex = 0;

        RefreshFreeTrial();
        RefreshLockedList();
        RefreshNPCTypeCards();
        CheckDejaVu();
    }

    void ShowNoEvals()
    {
        if (freeReviewerIdentity != null) freeReviewerIdentity.text = "";
        if (freeContentText != null) freeContentText.text = "";
        if (noEvalTip != null) noEvalTip.SetActive(true);

        if (freeTagsContainer != null)
        {
            foreach (Transform child in freeTagsContainer)
                Destroy(child.gameObject);
        }

        if (lockedListContainer != null)
        {
            foreach (Transform child in lockedListContainer)
                Destroy(child.gameObject);
        }

        if (unlockAllBtn != null) unlockAllBtn.gameObject.SetActive(false);
    }

    void RefreshFreeTrial()
    {
        if (freeTrialEval == null)
        {
            if (freeReviewerIdentity != null) freeReviewerIdentity.text = "暂无免费评价";
            if (freeContentText != null) freeContentText.text = "该专业暂时没有提供免费评价。";
            return;
        }

        if (freeReviewerIdentity != null)
            freeReviewerIdentity.text = $"👤 {freeTrialEval.reviewer?.identity ?? ""} | {freeTrialEval.reviewer?.collegeType ?? ""} | {freeTrialEval.reviewer?.currentStatus ?? ""}";

        if (freeContentText != null)
            freeContentText.text = $"\"{freeTrialEval.content}\"";

        if (freeTagsContainer != null)
        {
            foreach (Transform child in freeTagsContainer)
                Destroy(child.gameObject);

            if (freeTrialEval.tags != null && tagChipPrefab != null)
            {
                foreach (var tag in freeTrialEval.tags)
                {
                    var chip = Instantiate(tagChipPrefab, freeTagsContainer);
                    var chipText = chip.GetComponentInChildren<Text>();
                    if (chipText != null) chipText.text = $"#{tag}";
                }
            }
        }
    }

    void RefreshLockedList()
    {
        if (lockedListContainer == null) return;

        foreach (Transform child in lockedListContainer)
            Destroy(child.gameObject);

        bool hasLocked = lockedEvals != null && lockedEvals.Count > 0;

        if (unlockAllBtn != null)
        {
            unlockAllBtn.gameObject.SetActive(hasLocked);
            if (unlockAllLabel != null && hasLocked)
                unlockAllLabel.text = $"🎬 观看广告解锁全部 ({lockedEvals.Count}条)";
        }

        if (!hasLocked || lockedItemPrefab == null) return;

        for (int i = 0; i < lockedEvals.Count; i++)
        {
            var eval = lockedEvals[i];
            var item = Instantiate(lockedItemPrefab, lockedListContainer);
            var texts = item.GetComponentsInChildren<Text>();

            string preview = eval.content.Length > 60 ? eval.content.Substring(0, 60) + "..." : eval.content;

            if (texts.Length >= 2)
            {
                texts[0].text = $"{eval.reviewer?.identity ?? ""} | {eval.reviewer?.npcType ?? ""}";
                texts[1].text = preview;
            }

            var adBtn = item.transform.Find("AdBtn")?.GetComponent<Button>();
            if (adBtn != null)
            {
                var btnLabel = adBtn.GetComponentInChildren<Text>();
                if (btnLabel != null) btnLabel.text = "🎬解锁这条";
                string capturedId = eval.evalId;
                adBtn.onClick.AddListener(() => UnlockSingleEval(capturedId));
            }
        }
    }

    void UnlockSingleEval(string evalId)
    {
        if (SeniorEvalEngine.Instance == null) return;

        AdManager.Instance?.ShowRewardedVideo("senior_eval_unlock", (success) =>
        {
            if (success)
            {
                SeniorEvalEngine.Instance.UnlockViaAd(evalId);
                Show(currentMajorId);
                Debug.Log($"[SeniorEvalPanel] 广告解锁评价: {evalId}");
            }
        });
    }

    void OnUnlockAll()
    {
        if (lockedEvals == null || lockedEvals.Count == 0) return;

        AdManager.Instance?.ShowRewardedVideo("senior_eval_unlock_all", (success) =>
        {
            if (success)
            {
                foreach (var eval in lockedEvals)
                    SeniorEvalEngine.Instance?.UnlockEval(eval.evalId);

                Show(currentMajorId);
                Debug.Log($"[SeniorEvalPanel] 广告解锁全部{lockedEvals.Count}条评价");
            }
        });
    }

    void RefreshNPCTypeCards()
    {
        if (npcCardContainer == null || npcCardPrefab == null) return;

        foreach (Transform child in npcCardContainer)
            Destroy(child.gameObject);

        var npcTypes = SeniorEvalEngine.Instance?.GetAllReviewerNPCs() ?? new List<string>();

        foreach (var npcType in npcTypes)
        {
            int count = allEvalsForMajor.Count(e => e.reviewer?.npcType == npcType);
            var card = Instantiate(npcCardPrefab, npcCardContainer);
            var cardText = card.GetComponentInChildren<Text>();
            if (cardText != null)
                cardText.text = $"{npcType}\n({count}条)";
        }
    }

    void CheckDejaVu()
    {
        if (dejaVuHint == null) return;

        var state = GameStateManager.Instance?.GetPlayerState();
        if (state?.hasPastLifeMemory == true)
        {
            dejaVuHint.SetActive(true);
            if (dejaVuTag != null)
                dejaVuTag.text = "💭 似曾相识的感觉...";
        }
        else
        {
            dejaVuHint.SetActive(false);
        }
    }

    void Close()
    {
        Destroy(gameObject);
    }
}