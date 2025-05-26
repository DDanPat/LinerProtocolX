using System.Collections.Generic;
using UnityEngine;

public interface ISkillData
{
    Sprite icon { get; }
    string skillName { get; }
    List<OperatorSkillUpgradeData> requiredItems { get; }
    string GetFormattedDescription(int level);
}

[CreateAssetMenu(fileName ="Active", menuName = "Skill/ActiveSkill")]
public class ActiveSkillData : ScriptableObject, ISkillData
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<OperatorSkillUpgradeData> _requiredItems;
    public Sprite icon => _icon;
    public string skillName => skillTable != null ? skillTable.name : "???";
    
    public ActiveSkillTable skillTable;
    public List<OperatorSkillUpgradeData> requiredItems => _requiredItems;      // 레벨별 필요한 재화 리스트
    
    
    public string GetFormattedDescription(int level)
    {
        if (skillTable == null)
            return "???";

        string formatted = skillTable.desc;
        
        // TODO : ActiveSkillTable 수정후
        int skillKey = (skillTable.key * 10) + level + 1;
        formatted = SkillManager.Instance.GetDescript(skillKey);
        
        return formatted;
    }
    
    private float GetSafeValue(List<float> list, int level, float defaultValue = 0f)
    {
        if (list == null || list.Count <= level || level < 0)
            return defaultValue;

        return list[level];
    }
}
