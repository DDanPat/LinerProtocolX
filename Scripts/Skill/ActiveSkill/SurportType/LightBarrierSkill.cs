using UnityEngine;
/// <summary>
/// 스킬 6302 - 빛의 결계
/// 일정 시간 동안 모든 타워의 치명타율이 {a}%, 치명계수가 {b}% 증가합니다. 
/// /// </summary>
public class LightBarrierSkill : ISkill
{
    public LightBarrierSkill()
    {
        
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 버프 생성
        CriticalChanceUpBuff criticalChanceUpBuff = new CriticalChanceUpBuff(data.duration, data.valueA, 6301);
        CriticalScaleBuff criticalScaleBuff = new CriticalScaleBuff(data.duration, data.valueB, 6301);
        // 전체 타워에 버프 적용
        TileManager.Instance.SetTowerBuff(criticalChanceUpBuff);
        TileManager.Instance.SetTowerBuff(criticalScaleBuff);
    }
}
