using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6307- 전속력 돌진
/// 일정 시간 동안 관리 타워 사거리 내 타워의 공격 속도를 {a}% 증가시키고, 
/// 관리 타워 사거리 내 적 유닛의 이동 속도를 {b}% 감소시킨다.
/// /// </summary>
public class FullSpeedDashSkill : ISkill
{
    public FullSpeedDashSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float radius = towerInfo.towerInfoTable.range / 2f;
        // 사거리 내 타워 탐색 
        // 버프 객체 생성
        AttackRateBuff attackRateBuff = new AttackRateBuff(data.duration, data.valueA);
        // 타워 근처의 적 탐색
        List<Tower> towers = TileManager.Instance.GetNeighborTower(operatorKey, radius, SkillManager.Instance.towerLayer);
        foreach (var tower in towers)
        {
            // 버프 적용
            tower.ExecuteBuff(attackRateBuff);
        }

        // 디버프 객체 생성
        SlowDebuff slowDebuff = new SlowDebuff(data.duration, data.valueA);
        // 타워 근처의 적 탐색
        List<EnemyState> enemies = TileManager.Instance.GetNeighborEnemy(operatorKey, radius, SkillManager.Instance.enemyLayer);
        foreach (var enemy in enemies)
        {
            // 디버프 적용
            enemy.ExecuteDebuff(slowDebuff);
        }
    }
}
