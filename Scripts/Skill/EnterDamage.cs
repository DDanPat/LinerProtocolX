using UnityEngine;
// 진입 시 데미지 구역
public class EnterDamage : MonoBehaviour
{
    TowerData towerInfo;
    float radius;
    float damage;
    float duration;
    LayerMask targetLayer;
    CapsuleCollider _capsuleCollider;
    void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Set(TowerData towerInfo, float range, float duration, float damage)
    {
        this.towerInfo = towerInfo;
        radius = range / 2f;
        _capsuleCollider.radius = radius;
        this.duration = duration;
        this.damage = damage;
        targetLayer = SkillManager.Instance.enemyLayer;
        Destroy(gameObject, duration);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyState>(out var enemy))
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
    }
}
