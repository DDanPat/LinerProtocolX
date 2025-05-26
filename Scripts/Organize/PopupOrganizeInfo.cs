using TMPro;
using UnityEngine;

public class PopupOrganizeInfo : MonoBehaviour
{
    [Header("Popup Window Data")]
    [SerializeField] private GameObject popupWindow;
    [SerializeField] private OperatorData operatorData;
    [SerializeField] private TowerData towerData;
    
    [SerializeField] private InventorySlot[] slots = new InventorySlot[2];      // 0 오퍼레이터 1 타워 
    [SerializeField] private TextMeshProUGUI[] nameText = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] texts = new TextMeshProUGUI[6];
    [SerializeField] private SkillSlot[] skillSlots = new SkillSlot[4];
    [SerializeField] private TextMeshProUGUI[] skillDescText = new TextMeshProUGUI[4];
    
    public void OpenWindow(OperatorData? operatorData, TowerData? towerData)
    {
        this.operatorData = operatorData;
        this.towerData = towerData;
        
        popupWindow?.SetActive(true);
        SetPopupUI();
    }

    public void CloseWindow()
    {
        popupWindow?.SetActive(false);
        
        operatorData = null;
        towerData = null;
        nameText[0].text = "오퍼레이터";
        nameText[1].text = "타워";

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

    private void SetPopupUI()
    {
        SetInfoIcon();
        SetSkillSlots();
        SetSkillDescText();
        SetInfoText();
    }

    private void SetInfoIcon()
    {
        slots[0].data = operatorData;
        slots[1].data = towerData;
    }
    
    private void SetInfoText()
    {
        if(operatorData != null)
            nameText[0].text = operatorData.operatorInfoTable.name;
        if(towerData != null)
            nameText[1].text = towerData.towerInfoTable.name;
        
        
        for (int i = 0; i < texts.Length; i++)
        {
            string percentText = "(+000%) ";
            string valueText = $"<color=#80C8FF>000</color>";
            
            switch (i)
            {
                case 0:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.attack.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float attack = towerData.towerInfoTable.attack;
                        if (operatorData != null)
                            attack += towerData.towerInfoTable.attack * operatorData.operatorInfoTable.attack * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(attack):D3}</color>";
                    }
                    break;
                case 1:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.attackRate.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float attackRate = towerData.towerInfoTable.attackRate;
                        if (operatorData != null)
                            attackRate += towerData.towerInfoTable.attackRate * operatorData.operatorInfoTable.attackRate * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(attackRate):D3}</color>";
                    }
                    break;
                case 2:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.range.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float range = towerData.towerInfoTable.range;
                        if (operatorData != null)
                            range += towerData.towerInfoTable.range * operatorData.operatorInfoTable.range * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(range):D3}</color>";
                    }
                    break;
                case 3:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.accuracy.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float accuracy = towerData.towerInfoTable.accuracy;
                        if (operatorData != null)
                            accuracy += towerData.towerInfoTable.accuracy * operatorData.operatorInfoTable.accuracy * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(accuracy):D3}</color>";
                    }
                    break;
                case 4:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.criticalRate.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float criticalRate = towerData.towerInfoTable.criticalRate;
                        if (operatorData != null)
                            criticalRate += towerData.towerInfoTable.criticalRate * operatorData.operatorInfoTable.criticalRate * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(criticalRate):D3}</color>";
                    }
                    break;
                case 5:
                    if (operatorData != null)
                        percentText = "(+" + operatorData.operatorInfoTable.criticalMultiplier.ToString("D3") + "%) ";
                    if (towerData != null)
                    {
                        float criticalMultiplier = towerData.towerInfoTable.criticalCoefficient;
                        if (operatorData != null)
                            criticalMultiplier += towerData.towerInfoTable.criticalCoefficient * operatorData.operatorInfoTable.criticalMultiplier * 0.01f;
                        
                        valueText = $"<color=#80C8FF>{Mathf.RoundToInt(criticalMultiplier):D3}</color>";
                    }
                    break;
            }
            
            texts[i].text = percentText + valueText;
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
}
