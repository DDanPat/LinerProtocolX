using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSkillInfo : PopupWindow
{
    OperatorData opData;
    ISkillData skill;
    private int skillLevel;
    public int skillType;
    private int itemID = 9200;

    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private GameObject passiveChangeSlot;
    [SerializeField] private InventoryItemSlot changeItemSlot;
    [SerializeField] private PopupWindow applyPopup;

    public override void SetPopupUI()
    {
        opData = popupData as OperatorData;

        if (skillType == 0)
        {
            skill = opData.activeSkillData;
            skillLevel = opData.activeLv;
            passiveChangeSlot.SetActive(false);

            // TODO : 
            string finalDesc = opData.activeSkillData.GetFormattedDescription(skillLevel - 1);
            skillDescriptionText.text = finalDesc;
        }
        else if (1 <= skillType && skillType <= 3)
        {
            changeItemSlot.data = GameManager.Instance.Player.inventory.items[itemID].data;

            int index = skillType - 1;
            passiveChangeSlot.SetActive(true);
            skill = opData.passiveSkillDatas[index];

            skillLevel = skillType switch
            {
                1 => opData.passive0LV,
                2 => opData.passive1LV,
                3 => opData.passive2LV,
                _ => 0
            };

            // level은 1부터 시작 → 인덱스 0부터 시작하므로 -1 필요
            string finalDesc = opData.passiveSkillDatas[index].GetFormattedDescription(skillLevel - 1);
            skillDescriptionText.text = finalDesc;
        }
        skillIcon.sprite = skill.icon;
        skillNameText.text = skill.skillName + " (LV." + skillLevel + ")";
    }

    public override void CloseWindow()
    {
        passiveChangeSlot.SetActive(false);
        base.CloseWindow();
    }

    public void OpenApplyPopup()
    {
        if (GameManager.Instance.Player.inventory.GetItem(itemID).amount > 0)
            applyPopup.gameObject.SetActive(true);
    }

    public void ApplyButton()
    {
        GameManager.Instance.Player.inventory.SubtractItem(itemID, 1);

        ChangeSkill();

        SetPopupUI();
        applyPopup.CloseWindow();
    }

    private void ChangeSkill()
    {
        int newSkillKey = -1;
        var key = opData.operatorInfoTable.key;
        bool isDuplicated;
        do
        {
            isDuplicated = false;
            newSkillKey = SkillRandomizer.GetRandomSkillKey(opData.operatorInfoTable.role);

            foreach (var passive in opData.passiveSkillDatas)
            {
                if (passive.skillTable.key == newSkillKey)
                {
                    isDuplicated = true;
                    break;
                }
            }

        } while (isDuplicated);

        opData.passiveSkillDatas[skillType - 1] = SkillManager.Instance.GetSkill(newSkillKey);
        opData.passiveSkillDatas[skillType - 1].level = 1;
        // 임의 저장
        GameManager.Instance.SavePlayer();
        // 커스텀 이벤트 - 스킬 재설정
        AnalyticsManager.Instance.SendEvent(Define.SkillReset, new Dictionary<string, object> {
            { Define.skillKeyBefore, key },
            { Define.skillKeyAfter, newSkillKey },
            { Define.operatorKey, opData.operatorInfoTable.key},
            });
    }
}
