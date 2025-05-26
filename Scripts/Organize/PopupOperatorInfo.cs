using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupOperatorInfo : PopupWindow
{
    private OperatorData operatorData;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI[] texts = new TextMeshProUGUI[6];
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[4];
    [SerializeField] private TextMeshProUGUI[] skillDescText = new TextMeshProUGUI[4];
    
    
    public override void SetPopupUI()
    {
        operatorData = popupData as OperatorData;
        image.sprite = operatorData?.profile;
        SetInfoText();
        SetSkillSlots();
        SetSkillDescText();
    }

    private void SetInfoText()
    {
        nameText.text = operatorData.operatorInfoTable.name;
        
        for (int i = 0; i < texts.Length; i++)
        {
            string percentText = "(+000%) ";
            switch (i)
            {
                case 0:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.attack.ToString("D3") + "%) ";
                    
                    break;
                case 1:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.attackRate.ToString("D3") + "%) ";
                    
                    break;
                case 2:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.range.ToString("D3") + "%) ";
                    
                    break;
                case 3:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.accuracy.ToString("D3") + "%) ";
                   
                    break;
                case 4:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.criticalRate.ToString("D3") + "%) ";
                    
                    break;
                case 5:
                    if (operatorData != null)
                        percentText = " (+" + operatorData.operatorInfoTable.criticalMultiplier.ToString("D3") + "%) ";
                    
                    break;
            }
            texts[i].text = percentText;
        }
    }
    
    private void SetSkillSlots()
    {
        if (operatorData != null)
        {
            if (0 < operatorData.activeLv)
                skillSlots[0].data = operatorData.activeSkillData;
            if(0 < operatorData.passive0LV)
                skillSlots[1].data = operatorData.passiveSkillDatas[0];
            if(0 < operatorData.passive1LV)
                skillSlots[2].data = operatorData.passiveSkillDatas[1];
            if(0 < operatorData.passive2LV)
                skillSlots[3].data = operatorData.passiveSkillDatas[2];
        }
    }

    private void SetSkillDescText()
    {
        if (skillSlots[0].data != null)
        {
            var activeSkill= skillSlots[0].data as ActiveSkillData;
            var skillName = "<b>" + activeSkill.skillName + "</b> ";
            var skillLv = "(LV" + operatorData.activeLv + ") ";
            var skillDesc = activeSkill?.GetFormattedDescription(operatorData.activeLv);
            skillDescText[0].text = skillName + skillLv + skillDesc;
        }

        for (int i = 1; i < skillSlots.Length; i++)
        {
            if (skillSlots[i].data != null)
            {
                var passiveSkill = skillSlots[i].data as PassiveSkillData;
                string skillName = "<b>" + passiveSkill.skillName + "</b> ";
                string skillLv = "";
                string skillDesc = "";

                switch (i)
                {
                    case 1:
                        skillLv = "(LV" + operatorData.passive0LV + ") ";
                        skillDesc = passiveSkill?.GetFormattedDescription(operatorData.passive0LV);
                        break;
                    case 2:
                        skillLv = "(LV" + operatorData.passive1LV + ") ";
                        skillDesc = passiveSkill?.GetFormattedDescription(operatorData.passive1LV);
                        break;
                    case 3:
                        skillLv = "(LV" + operatorData.passive2LV + ") ";
                        skillDesc = passiveSkill?.GetFormattedDescription(operatorData.passive2LV);
                        break;
                }
                skillDescText[i].text = skillName + skillLv + skillDesc;
            }
        }
        
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        foreach (var skillSlot in skillSlots)
        {
            skillSlot.data = null; 
        }

        int i = 0;
        foreach (var skillDesc in skillDescText)
        {
            switch (i)
            {
                case 1 :
                case 2 :
                case 3 :
                    skillDesc.text = "패시브 스킬 슬롯 오퍼레이터" + $" {i + 2}" + "성 승급시 해금";
                    break;
                default:
                    skillDesc.text = "슬롯 비어 있음";
                    break;
            }
            i++;
        }
    }
}
