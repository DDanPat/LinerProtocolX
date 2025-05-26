using UnityEngine;
/// <summary>
/// 스킬 6101 - 고속 사격
/// 오퍼레이터가 관리하고 있는 타워의 공격 속도를 {a}% 증가시킵니다. (쿨타임 {t} 초, 지속시간 {u}초)
/// </summary>
public class SpeedAttackSkill : ISkill
{
    public SpeedAttackSkill()
    {
        
    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        AttackRateBuff speedUP = new AttackRateBuff(data.duration, data.valueA, 6101);
        TileManager.Instance.SetTowerBuff(operatorKey, speedUP);
    }
}
