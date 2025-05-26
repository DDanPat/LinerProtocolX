using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 지속 데미지 구역
public class ContainDamage : MonoBehaviour
{
    public GameObject areaObject;
    TowerData towerInfo;
    float radius;
    float damage;
    float duration;
    LayerMask targetLayer;
    CapsuleCollider _capsuleCollider;
    List<EnemyState> enemies;
    void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Set(TowerData towerInfo, float range, float duration, float damage)
    {
        enemies = new();
        this.towerInfo = towerInfo;
        radius = range / 2f;
        _capsuleCollider.radius = radius;
        this.duration = duration;
        this.damage = damage;
        targetLayer = SkillManager.Instance.enemyLayer;

        // 반복 시작
        StartCoroutine(RepeatFunction());

        // 지속 시간 이후 삭제
        Destroy(gameObject, duration);
    }
    private IEnumerator RepeatFunction()
    {
        while (true)
        {
            // 공격
            HitTarget();
            // 1초마다 호출
            yield return new WaitForSeconds(1f);
        }
    }
    private void HitTarget()
    {
        foreach (var enemy in enemies)
        {
            // 명중 계산
            if (enemy != null && Random.value * 100f < towerInfo.towerInfoTable.accuracy)
            {
                // 치명타 계산
                if (Random.value * 100f < towerInfo.towerInfoTable.criticalRate)
                    damage *= towerInfo.towerInfoTable.criticalCoefficient * 0.01f;
                enemy.TakeDamage(damage);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyState>(out var enemy))
            {
                // 목록에 추가
                enemies.Add(enemy);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyState>(out var enemy))
            {
                // 목록에서 제거
                enemies.Remove(enemy);
            }
        }
    }
}
