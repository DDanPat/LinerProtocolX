using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Data;

[CreateAssetMenu(fileName ="Passive", menuName = "Skill/PassiveSkill")]
public class PassiveSkillData : ScriptableObject, ISkillData
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private List<OperatorSkillUpgradeData> _requiredItems;
    
    public int level = 1;
    public Sprite icon => _icon;
    public string skillName => skillTable != null ? skillTable.name : "???";
    public PassiveSkillTable skillTable;
    public List<OperatorSkillUpgradeData> requiredItems => _requiredItems;
    
    
    public string GetFormattedDescription(int level)
    {
        if (skillTable == null)
            return "???";

        var values = new Dictionary<string, float>
        {
            { "x", GetSafeValue(skillTable.effectX, level, 0f) },
            { "n", GetSafeValue(skillTable.effectN, level, 0f) },
            { "e", GetSafeValue(skillTable.enemyCondition, level, 0f) },
            { "t", GetSafeValue(skillTable.duration, level, 0f) }
        };

        return FormatDescription(skillTable.desc, values);
    }
    
    private float GetSafeValue(List<float> list, int level, float defaultValue = 0f)
    {
        if (list == null || list.Count <= level || level < 0)
            return defaultValue;

        return list[level];
    }
    
    public string FormatDescription(string raw, Dictionary<string, float> values)
    {
        return Regex.Replace(raw, @"\{([^{}]+)\}", match =>
        {
            string expr = match.Groups[1].Value; // 예: n+1
            foreach (var pair in values)
            {
                expr = expr.Replace(pair.Key, pair.Value.ToString());
            }

            try
            {
                var result = new DataTable().Compute(expr, null); // 수식 계산
                float f = Convert.ToSingle(result);
                return $"<color=#80C8FF>{f:0.#}</color>";
            }
            catch
            {
                return "<color=red>?</color>";
            }
        });
    }
}
