using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6204 - 다이내믹 실드
/// 스테이지에 존재하는 모든 적 유닛의 실드를 {a}% 감소시킨다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class DynamicShieldSkill : ISkill
{
    public DynamicShieldSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        List<EnemyState> enemies = ObjectPoolManager.Instance.GetActiveEnemyObjects();
        foreach(EnemyState enemy in enemies)
        {
            enemy.RemoveShield(data.valueA);
        }
    }
}
