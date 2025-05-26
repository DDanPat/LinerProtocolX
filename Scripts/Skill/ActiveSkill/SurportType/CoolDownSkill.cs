using UnityEngine;
/// <summary>
/// 스킬 6300 - 연산 단축 
/// 모든 다른 오퍼레이터의 액티브 스킬 쿨다운을 {a} 초 감소시킵니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class CoolDownSkill : ISkill
{
    public CoolDownSkill()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float reduceTime = data.valueA;
        BattleManager.Instance.CurrentBattle.uiCardHand.ReduceSkillTime(reduceTime, operatorKey);
    }
}
