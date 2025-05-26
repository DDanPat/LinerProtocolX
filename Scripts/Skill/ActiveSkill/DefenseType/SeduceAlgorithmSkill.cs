using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6202 - 유혹 알고리즘
/// 일정 시간 동안, 타워 사거리 내에 있는 모든 적들의 방어력을 {a}% 감소시키고, 회피율을 0으로 감소시킨다.
/// </summary>
public class SeduceAlgorithmSkill : ISkill
{
    public SeduceAlgorithmSkill()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float radius = towerInfo.towerInfoTable.range / 2f;
        // 디버프 객체 생성
        DefenseDebuff defenseDebuff = new DefenseDebuff(data.duration, data.valueA);
        DodgeDebuff dodgeDebuff = new DodgeDebuff(data.duration, 999, new Color(153 / 255f, 0f, 0f));
        // 타워 근처의 적 탐색
        List<EnemyState> enemies = TileManager.Instance.GetNeighborEnemy(operatorKey, radius, SkillManager.Instance.enemyLayer);
        foreach (var enemy in enemies)
        {
            // 디버프 적용
            enemy.ExecuteDebuff(defenseDebuff);
            enemy.ExecuteDebuff(dodgeDebuff);
        }
    }
}
