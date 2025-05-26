using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6103 - 사운드 블래스트
/// 고출력 음파를 발생시켜, 관리 타워의 사거리 안에 있는 모든 적 유닛에게 타워 공격력 {a}%의 피해를 입힙니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class SoundBlastSkill : ISkill
{
    public SoundBlastSkill() { }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 타워 정보 구하기
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        float radius = towerInfo.towerInfoTable.range / 2f;
        float towerAttack = towerInfo.towerInfoTable.attack;
        float damage = towerAttack * data.valueA / 100f;
        // 타워 근처의 적 탐색
        List<EnemyState> enemies = TileManager.Instance.GetNeighborEnemy(operatorKey, radius, SkillManager.Instance.enemyLayer);
        foreach (var enemy in enemies)
        {
            // 범위 내 적에게 데미지
            enemy.TakeDamage(damage);
        }
    }

}
