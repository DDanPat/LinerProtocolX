using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6200 - 냉각감속
/// 일정 시간 동안 관리 타워의 사거리 내에 있는 모든 적들의 이동 속도를 {a}% 감소시킵니다. (쿨타임 {t} 초, 지속시간 {u} 초)
/// </summary>
public class SlowSkill : ISkill
{
    public SlowSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float radius = towerInfo.towerInfoTable.range / 2f;
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
