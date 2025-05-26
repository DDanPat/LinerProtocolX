using UnityEngine;
/// <summary>
/// 스킬 6205 - 절전 셸터
/// 일정 시간 동안 등장하는 모든 적 유닛의 이동 속도를 {a}% 감소시킵니다. (쿨타임 {t} 초, 지속 시간 {u}초)
/// </summary>
public class PowerOutShelter : ISkill
{
    public PowerOutShelter()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        BattleManager.Instance.LootSlow(6205, data.valueA / 100f, data.duration);
    }
}
