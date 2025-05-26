using UnityEngine;
/// <summary>
/// 스킬 6301 - 스마일 드링크
/// 일정 시간 동안 모든 타워의 공격력이 {a}%, 공격 속도가 {b}% 증가합니다.
/// </summary>
public class SmileDrinkSkill : ISkill
{
    public SmileDrinkSkill()
    {
        
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 버프 생성
        AttackUpBuff attackUpBuff = new AttackUpBuff(data.duration, data.valueA, 6301);
        AttackRateBuff attackRateBuff = new AttackRateBuff(data.duration, data.valueB, 6301);
        // 전체 타워에 버프 적용
        TileManager.Instance.SetTowerBuff(attackUpBuff);
        TileManager.Instance.SetTowerBuff(attackRateBuff);
    }
}
