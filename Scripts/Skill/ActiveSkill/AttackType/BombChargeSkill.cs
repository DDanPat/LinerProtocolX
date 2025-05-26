using UnityEngine;
/// <summary>
/// 스킬 6110 - 폭탄 돌격
/// 일정 시간 동안 관리하고 있는 타워의 모든 공격을 {r} 타일 영역의 범위 공격으로 바꾸고, 공격력을 {a}% 증가시킵니다.
/// </summary>
public class BombChargeSkill : ISkill
{
    public BombChargeSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float range = data.range;
        // 버프 생성
        AttackUpBuff attackUpBuff = new AttackUpBuff(data.duration, data.valueA);
        TypeChangeBuff typeChangeBuff = new TypeChangeBuff(data.duration, data.valueA, range, 6110);
        // 버프 적용
        TileManager.Instance.SetTowerBuff(operatorKey, attackUpBuff);
        TileManager.Instance.SetTowerBuff(operatorKey, typeChangeBuff);
    }
}
