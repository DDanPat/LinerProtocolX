using UnityEngine;
/// <summary>
/// 스킬 6303 - 하이센스 커넥션
/// 일정 시간 동안 모든 타워의 명중률을 {a}%, 치명타율을 {b}% 증가시킨다.
/// /// </summary>
public class HisenseConnectionSkill : ISkill
{
    public HisenseConnectionSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 버프 생성
        CriticalChanceUpBuff criticalChanceUpBuff = new CriticalChanceUpBuff(data.duration, data.valueA, 6301);
        AccuracyUpBuff accuracyUpBuff = new AccuracyUpBuff(data.duration, data.valueB, 6301);
        // 전체 타워에 버프 적용
        TileManager.Instance.SetTowerBuff(criticalChanceUpBuff);
        TileManager.Instance.SetTowerBuff(accuracyUpBuff);
    }
}