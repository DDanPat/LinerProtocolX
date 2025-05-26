using UnityEngine;
/// <summary>
/// 스킬 6206 - 방패 격파
/// 일정 시간 동안 등장하는 모든 적 유닛의 방어력을 {a}% 감소시킵니다. (쿨타임 {t} 초, 지속 시간 {u}초)
/// </summary>
public class ShieldBreakSkill : ISkill
{
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        BattleManager.Instance.LootDefenseDown(6206, data.valueA / 100f, data.duration);
    }
}
