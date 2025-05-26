using UnityEngine;
/// <summary>
/// 스킬 6112- 버스트 스파크
/// 일정 시간 동안 관리하고 있는 타워의 모든 공격을 {r} 타일의 범위를 가진 치명타율 100%의 광역 공격으로 바꿉니다.
/// </summary>
public class BurstSparkSkill : ISkill
{
    public BurstSparkSkill()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float range = data.range;
        // 버프 생성
        CriticalChanceUpBuff criticalChanceUpBuff = new CriticalChanceUpBuff(data.duration, 100, 6112);
        TypeChangeBuff typeChangeBuff = new TypeChangeBuff(data.duration, data.valueA, range, 6112);
        // 버프 적용
        TileManager.Instance.SetTowerBuff(operatorKey, criticalChanceUpBuff);
        TileManager.Instance.SetTowerBuff(operatorKey, typeChangeBuff);
    }
}
