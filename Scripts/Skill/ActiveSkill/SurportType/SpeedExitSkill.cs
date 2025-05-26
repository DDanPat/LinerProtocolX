using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6308 - 스피드 엑시트
/// 일정 시간 동안 관리 타워 사거리 내 타워의 공격 속도를 {a}% 증가시키고, 
/// 명중률을 {b}% 증가시킨다.
/// /// </summary>
public class SpeedExitSkill : ISkill
{
    public SpeedExitSkill()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float radius = towerInfo.towerInfoTable.range / 2f;
        // 사거리 내 타워 탐색 
        // 버프 객체 생성
        AttackRateBuff attackRateBuff = new AttackRateBuff(data.duration, data.valueA, 6308);
        AccuracyUpBuff accuracyUpBuff = new AccuracyUpBuff(data.duration, data.valueB, 6308);
        // 타워 근처의 적 탐색
        List<Tower> towers = TileManager.Instance.GetNeighborTower(operatorKey, radius, SkillManager.Instance.towerLayer);
        foreach (var tower in towers)
        {
            // 버프 적용
            tower.ExecuteBuff(attackRateBuff);
            tower.ExecuteBuff(accuracyUpBuff);
        }
    }
}
