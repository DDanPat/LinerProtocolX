using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6203 - 방어저해역장
/// 지정된 위치에서 {r} 타일 내의 모든 적 유닛의 방어력을 {a}% 감소시킵니다. (쿨타임 {t} 초)
/// </summary>
public class DefenseDownStationSkill : ISkill
{
    public DefenseDownStationSkill()
    {

    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float radius = data.range / 2;
        // 디버프 객체 생성
        DefenseDebuff slowDebuff = new DefenseDebuff(999f, data.valueA);
        // 타워 근처의 적 탐색
        List<EnemyState> enemies = new();
        Vector3 point0 = pos;
        Vector3 point1 = pos + Vector3.up * 3f;

        Collider[] cols = Physics.OverlapCapsule(point0, point1, radius, SkillManager.Instance.enemyLayer);
        foreach (var col in cols)
        {
            enemies.Add(col.GetComponent<EnemyState>());
        }
        foreach (var enemy in enemies)
        {
            // 디버프 적용
            enemy.ExecuteDebuff(slowDebuff);
        }
    }
}
