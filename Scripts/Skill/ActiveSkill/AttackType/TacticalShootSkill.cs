using UnityEngine;
/// <summary>
/// 스킬 6113 - 전술 사격
/// 일정 시간 동안 관리하고 있는 타워의 치명타율을 {a}% 상승시킨다.
//  지속 시간 동안 같은 적 유닛을 반복 공격할 경우 타워 공격력의 {b}% 만큼의 추가 피해를 입힌다.
//  (쿨타임 {t} 초, 지속시간 {u}초)
/// </summary>
public class TacticalShootSkill : ISkill
{
    public TacticalShootSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
         // 크리티컬 확률 증가
        CriticalChanceUpBuff criticalChanceUpBuff = new CriticalChanceUpBuff(data.duration, data.valueA, 6113);
        TileManager.Instance.SetTowerBuff(operatorKey, criticalChanceUpBuff);
        // 집중 공격 버프 부여
        FocusBuff focusBuff = new FocusBuff(data.duration, data.valueB, 6113);
        TileManager.Instance.SetTowerBuff(operatorKey, focusBuff);
    }

}
