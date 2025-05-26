using UnityEngine;
/// <summary>
/// 스킬 6304 - 내추럴 브리즈
/// 모든 다른 오퍼레이터의 액티브 스킬 쿨다운을 {a}% 감소시킵니다.
/// </summary>
public class NaturalBreezeSkill : ISkill
{
    public NaturalBreezeSkill()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float reduceTimePercent = data.valueA;
        BattleManager.Instance.CurrentBattle.uiCardHand.ReduceSkillTimePersent(reduceTimePercent, operatorKey);
    }
}
