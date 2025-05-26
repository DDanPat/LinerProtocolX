using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 스킬 6201 - 도루 금지
/// 가장 체력이 높은 적 유닛(보스 포함) 1기를 {n} 회 저격해 타워 공격력의 {a} % 피해를 입힙니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class PositionSlowSkill : ISkill
{
    GameObject prefab;
    public PositionSlowSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6100"); // 미리 불러오기
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        float radius = data.range / 2;
        // 디버프 객체 생성
        SlowDebuff slowDebuff = new SlowDebuff(999f, data.valueA);
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
