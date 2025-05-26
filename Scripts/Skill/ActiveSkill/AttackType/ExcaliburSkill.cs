using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6107 - 약속된 승리의 검
/// 스테이지에 존재하는 모든 적 유닛에게 타워 공격력의 {a}% 피해를 입힌다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class ExcaliburSkill : ISkill
{
    public ExcaliburSkill(){}

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float damage = towerInfo.towerInfoTable.attack * data.valueA;
        List<EnemyState> enemies = ObjectPoolManager.Instance.GetActiveEnemyObjects();
        foreach(EnemyState enemy in enemies)
        {
            enemy.TakeDamage(damage);
        }
    }
}
