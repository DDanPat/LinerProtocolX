using UnityEngine;
/// <summary>
/// 스킬 6101 - 공격 속도 증가
/// </summary>
public class SpeedUpSkill : ISkill
{
    public SpeedUpSkill(){}
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        AttackRateBuff speedUP = new AttackRateBuff(data.valueA, data.valueB);
        TileManager.Instance.SetTowerBuff(operatorKey, speedUP);
    }
}
