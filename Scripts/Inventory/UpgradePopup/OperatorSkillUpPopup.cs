using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorSkillUpPopup : PopupWindow
{
    private OperatorData opData;
    public Button applyButton;
    public PopupSkillUpInfo skillInfo;
    [SerializeField] private GameObject maxLevelBlock;

    [Header("OperatorSkillUI")]
    [SerializeField] private List<Image> skillSlotBackground;
    [SerializeField] private List<SkillSlot> skillSlots;
    [SerializeField] private List<TextMeshProUGUI> skillLvTexts;

    [Header("Upgrade Required Info")]
    [SerializeField] private TextMeshProUGUI requireUpgradeGoldText;
    [SerializeField] private int _requireUpgradeGold;

    [SerializeField] private List<InventorySlot> requiredItemSlots;
    [SerializeField] private List<TextMeshProUGUI> requiredItemTexts;
    private int skillLevel;     // 버튼클릭시 레벨 할당 후 레벨별 스킬강화로
    private int MAXLEVEL = 5;
    private int skillType;
    private OperatorSkillUpgradeData items = null;

    public int requiredUpgradeGold
    {
        get { return _requireUpgradeGold; }
        set
        {
            _requireUpgradeGold = value;
            UpdateGoldText();
        }
    }

    private void UpdateGoldText()
    {
        requireUpgradeGoldText.text = _requireUpgradeGold.ToString("N0");
    }

    private void Start()
    {
        applyButton.onClick.AddListener(ApplyButton);
    }

    public override void SetPopupUI()
    {
        opData = popupData as OperatorData;
        if (opData == null) return;

        skillLevel = 0;
        skillType = 0;
        items = null;

        ClearSelectSkillSlots();
        SetSkillSlots();
        SetSkillLvTexts();
        ClearRequires();
    }

    public override void CloseWindow()
    {
        maxLevelBlock.SetActive(false);
        base.CloseWindow();
    }

    // 스킬 UI세팅 - 슬롯
    private void SetSkillSlots()
    {
        if (opData.activeSkillData != null)
        {
            skillSlots[0].data = opData.activeSkillData;
            skillSlots[0].icon.enabled = opData.activeLv > 0;
        }
        else
        {
            skillSlots[0].ClearSlot();
        }

        for (int i = 1; i < skillSlots.Count; i++)
        {
            if (opData.passiveSkillDatas != null && i - 1 < opData.passiveSkillDatas.Count)
            {
                skillSlots[i].data = opData.passiveSkillDatas[i - 1];
            }
            else
            {
                skillSlots[i].ClearSlot();
            }

            switch (i)
            {
                case 1:
                    skillSlots[i].icon.enabled = opData.passive0LV > 0;
                    break;
                case 2:
                    skillSlots[i].icon.enabled = opData.passive1LV > 0;
                    break;
                case 3:
                    skillSlots[i].icon.enabled = opData.passive2LV > 0;
                    break;
            }
        }

    }
    // 스킬 UI세팅 - 텍스트
    private void SetSkillLvTexts()
    {
        skillLvTexts[0].text = "LV . " + opData.activeLv.ToString();
        skillLvTexts[1].text = "LV . " + opData.passive0LV.ToString();
        skillLvTexts[2].text = "LV . " + opData.passive1LV.ToString();
        skillLvTexts[3].text = "LV . " + opData.passive2LV.ToString();
    }

    private void ClearSelectSkillSlots()
    {
        foreach (var background in skillSlotBackground)
        {
            background.color = Color.white;
        }
    }

    private void SetSelectSkillSlots(int skillType)
    {
        ClearSelectSkillSlots();
        skillSlotBackground[skillType].color = Color.cyan;
    }

    private void ClearRequires()
    {
        requiredUpgradeGold = 0;
        requireUpgradeGoldText.color = Color.black;
        foreach (var slot in requiredItemSlots)
        {
            slot.ClearSlot();
        }

        foreach (var itemText in requiredItemTexts)
        {
            itemText.text = "";
        }

        applyButton.interactable = false;
    }

    public void SetSkillUpgradeRequired(ScriptableObject skillData, int skillType)
    {
        this.skillType = skillType;
        SetSelectSkillSlots(skillType);
        maxLevelBlock.SetActive(false);
        if (skillData is ActiveSkillData activeSkillData)
        {
            skillLevel = opData.activeLv - 1;

            if (skillLevel >= MAXLEVEL - 1)
            {
                // 최대레벨 가림막 true;
                maxLevelBlock.SetActive(true);
                ClearRequires();
                return;
            }

            requiredUpgradeGold = activeSkillData.requiredItems[skillLevel].requiredGold;
            for (int i = 0; i < requiredItemSlots.Count; i++)
            {
                if (i < activeSkillData.requiredItems[skillLevel].requiredItems.Count)
                {
                    requiredItemSlots[i].data = activeSkillData.requiredItems[skillLevel].requiredItems[i].item;
                }
                else
                {
                    requiredItemSlots[i].ClearSlot();
                }
            }

            applyButton.interactable = IsUpgradeRequired(activeSkillData);

        }
        else if (skillData is PassiveSkillData passiveSkillData)
        {
            switch (this.skillType)
            {
                case 1:
                    skillLevel = opData.passive0LV - 1;
                    break;
                case 2:
                    skillLevel = opData.passive1LV - 1;
                    break;
                case 3:
                    skillLevel = opData.passive2LV - 1;
                    break;
            }

            if (skillLevel >= MAXLEVEL - 1)
            {
                // 최대레벨 가림막 true;
                maxLevelBlock.SetActive(true);
                ClearRequires();
                return;
            }

            requiredUpgradeGold = passiveSkillData.requiredItems[skillLevel].requiredGold;

            for (int i = 0; i < requiredItemSlots.Count; i++)
            {
                if (i < passiveSkillData.requiredItems[skillLevel].requiredItems.Count)
                {
                    requiredItemSlots[i].data = passiveSkillData.requiredItems[skillLevel].requiredItems[i].item;
                }
                else
                {
                    requiredItemSlots[i].ClearSlot();
                }
            }

            applyButton.interactable = IsUpgradeRequired(passiveSkillData);
        }
    }


    private bool IsUpgradeRequired(ISkillData skillData)
    {
        bool result = true;
        items = skillData.requiredItems[skillLevel];

        for (int i = 0; i < requiredItemSlots.Count; i++)
        {
            if (i >= items.requiredItems.Count)
            {
                requiredItemSlots[i].ClearSlot();
                requiredItemTexts[i].text = "";
                continue;
            }

            requiredItemSlots[i].data = items.requiredItems[i].item;

            int key = skillData.requiredItems[skillLevel].requiredItems[i].item.key;
            int haveCount = GameManager.Instance.Player.inventory.items[key].amount;
            int requiredCount = skillData.requiredItems[skillLevel].requiredItems[i].count;
            requiredItemTexts[i].text = $"{haveCount}/{requiredCount}";

            if (haveCount < requiredCount)
            {
                requiredItemTexts[i].color = new Color(192/255f, 0/255f, 0/255f);
                result = false;
            }
            else
            {
                requiredItemTexts[i].color = new Color(0/255f, 176/255f, 80/255f);
            }
        }

        // 소지중인 골드 / 업그레이드 필요한 골드 부족
        if (GameManager.Instance.Player.Gold < requiredUpgradeGold)
        {
            requireUpgradeGoldText.color = new Color(192/255f, 0/255f, 0/255f);
            result = false;
        }
        else
        {
            requireUpgradeGoldText.color = new Color(0/255f, 176/255f, 80/255f);
        }

        return result;
    }

    public void OpenSkillUpInfoPopup(int skillType)
    {
        skillInfo.skillType = skillType;
        skillInfo.OpenWindow(opData);
    }

    private void ApplyButton()
    {

        // 커스텀 이벤트 - 스킬강화
        int skillKey = 1;
        int skillLevel = 1;
        switch (skillType)
        {
            case 0:
                skillKey = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.activeSkillData.skillTable.key;
                skillLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.activeLv;
                break;
            case 1:
                skillKey = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passiveSkillDatas[0].skillTable.key;
                skillLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passive0LV;
                break;
            case 2:
                skillKey = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passiveSkillDatas[1].skillTable.key;
                skillLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passive1LV;
                break;
            case 3:
                skillKey = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passiveSkillDatas[2].skillTable.key;
                skillLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].data.passive2LV;
                break;

        }
        AnalyticsManager.Instance.SendEvent(Define.SkillUpgrade, new Dictionary<string, object> {
            { Define.skillKey, skillKey },
            { Define.skillKeyBefore, skillLevel },
            { Define.operatorKey, opData.operatorInfoTable.key},
            });

        GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].SkillUpgrade(skillType);

        // 아이템 소모
        for (int i = 0; i < requiredItemSlots.Count; i++)
        {
            if (requiredItemSlots[i].data != null && requiredItemSlots[i].data is ItemData itemData)
            {
                int requiredAmount = items.requiredItems[i].count;

                GameManager.Instance.Player.inventory.SubtractItem(itemData.key, requiredAmount);
            }
        }
        GameManager.Instance.Player.Gold -= requiredUpgradeGold;
        // 커스텀 이벤트 - 코인 변화
        AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
            { Define.changeType, Define.spent },
            { Define.numberCoinChanged, requiredUpgradeGold},
            { Define.changeSource, 3},
            { Define.numberCoinBefore, GameManager.Instance.Player.Gold + requiredUpgradeGold},
        });
        
        // 퀘스트 - 스킬 레벨업 반영
        QuestManager.Instance.UpdateQuestProgress(QuestConditions.skillLevelup);
        // 임의 저장
        GameManager.Instance.SavePlayer();
        SetPopupUI();
    }
}
