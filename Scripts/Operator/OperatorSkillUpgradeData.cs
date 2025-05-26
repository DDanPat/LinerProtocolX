using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Data/Upgrade/OperatorSkill")]
public class OperatorSkillUpgradeData : ScriptableObject
{
    public int skillLevel;  // 스킬 레벨 1레벨 -> 2레벨 업그레이드 할시 = 2
    public int requiredGold;    // 스킬 상승시 필요한 골드재화
    public List<RequiredItem> requiredItems; // 필요한 재화들
}