using UnityEngine;
/// <summary>
/// 스킬 6111 - 역전의 질주
/// 일정 시간 동안 관리하고 있는 타워의 공격속도를 {a}% 증가시키고,
//  타워의 모든 공격이 적 유닛의 방어력을 무시한다. (쿨타임 {t} 초, 지속시간 {u}초)
/// </summary>
public class ReversalRushSkill : ISkill
{
    public ReversalRushSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 공격 속도 증가
        AttackRateBuff speedUP = new AttackRateBuff(data.duration, data.valueA, 6101);
        TileManager.Instance.SetTowerBuff(operatorKey, speedUP);
        // 방어력 무시
        IgnoreDefenceBuff ignoreDefenceBuff = new IgnoreDefenceBuff(data.duration, 100, 6101);
        TileManager.Instance.SetTowerBuff(operatorKey, ignoreDefenceBuff);
    }
}
