using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 스킬 6102 - 블랭크샷
/// 가장 체력이 높은 적 유닛(보스 포함) 1기를 {n} 회 저격해 타워 공격력의 {a} % 피해를 입힙니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class BlankShotSkill : ISkill
{
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        // 타워 정보로 데미지 계산
        float damage = towerInfo.towerInfoTable.attack * data.valueA;
        List<EnemyState> enemies = ObjectPoolManager.Instance.GetActiveEnemyObjects();
        if (enemies != null && enemies.Count > 0)
        {
            // n번 반복
            for (int k = 0; k < data.interation; k++)
            {
                // 가장 체력이 많은 적들 탐색
                EnemyState target = enemies[0];
                float maxHP = target.health;
                for (int i = 1; i < enemies.Count; i++)
                {
                    if (enemies[i].health > maxHP)
                        target = enemies[i];
                }
                // 데미지 적용
                target.TakeDamage(damage);
            }
        }

    }
}
