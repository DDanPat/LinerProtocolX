using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupSkillUpInfo : PopupWindow
{
    OperatorData opData;
    ISkillData skill;
    private int skillLevel;
    private string skillDescription;
    public int skillType;
    
    [SerializeField] private Image[] skillIcon = new Image[2];
    [SerializeField] private TextMeshProUGUI[] skillNameText = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] skillDescriptionText = new TextMeshProUGUI[2];
    
    public override void SetPopupUI()
    {
        opData = popupData as OperatorData;

        switch (skillType)
        {
            case 0: // active
                skill = opData.activeSkillData;
                skillLevel = opData.activeLv;
                break;
            case 1: // passive 1
                skill = opData.passiveSkillDatas[0];
                skillLevel = opData.passive0LV;
                break;
            case 2:
                skill = opData.passiveSkillDatas[1];
                skillLevel = opData.passive1LV;
                break;
            case 3: // passive 3
                skill = opData.passiveSkillDatas[2];
                skillLevel = opData.passive2LV;
                break;
        }

        foreach (var icon in skillIcon)
        {
            icon.sprite = skill.icon;
        }

        foreach (var name in skillNameText)
        {
            name.text = skill.skillName + "(LV." + skillLevel + ")";
        }
        
        int nextLevel = skillLevel + 1 < 5 ? skillLevel + 1 : 5;
        skillNameText[0].text = skill.skillName + "(LV." + skillLevel + ")";
        skillNameText[1].text = skill.skillName + "(LV." + nextLevel + ")";
        
        // level은 1부터 시작 → 인덱스 0부터 시작하므로 -1 필요
        skillDescriptionText[0].text = skill.GetFormattedDescription(skillLevel - 1);
        skillDescriptionText[1].text = skill.GetFormattedDescription(nextLevel - 1);
        
    }
}
