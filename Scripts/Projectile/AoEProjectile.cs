using UnityEngine;

public class AoEProjectile : BaseProjectile
{
    public float range = 5f;       // 투사체가 터질때 확인할 범위


    protected override void HitTarget()
    {
        // 범위 이펙트 생성
        if (explosionEffect != null)
        {
            ObjectPoolManager.Instance.GetParticle((int)objectType, transform.position, Quaternion.identity);
        }

        float radius = range * 0.5f;
        Vector3 point1 = transform.position + Vector3.up * radius;
        Vector3 point2 = transform.position - Vector3.up * radius;

        Collider[] cols = Physics.OverlapCapsule(point1, point2, radius, targetLayer);

        foreach (var col in cols)
        {
            EnemyState enemy = col.GetComponent<EnemyState>();
            if (enemy != null)
            {
                if (IsHit())
                {
                    float damage = projectileStat.damage;
                    bool isCritical = false;
                    if (IsCritical())
                    {
                        isCritical = true;
                        damage *= projectileStat.criticalCoefficient * 0.01f;
                    }
                    enemy.TakeDamage(damage, attackProperty, isCritical, ignoreDefenceAmount, onKillAction);
                }
                else
                {
                    ObjectPoolManager.Instance.ShowDamage("MISS", enemy.transform, Vector3.up * 2, false);
                }
            }
        }
        OnDespawn();
        //Destroy(gameObject);
    }
    public void SetRange(float range)
    {
        this.range = range;
    }
    private void OnDrawGizmos()
    {
        float radius = range * 0.5f;
        Vector3 center = transform.position;
        Vector3 point1 = center + Vector3.up * radius;
        Vector3 point2 = center - Vector3.up * radius;

        Gizmos.color = Color.red;

        // 캡슐의 양 끝을 구체로 그림
        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawWireSphere(point2, radius);

        // 사이를 네 방향으로 연결
        Gizmos.DrawLine(point1 + Vector3.forward * radius, point2 + Vector3.forward * radius);
        Gizmos.DrawLine(point1 - Vector3.forward * radius, point2 - Vector3.forward * radius);
        Gizmos.DrawLine(point1 + Vector3.right * radius, point2 + Vector3.right * radius);
        Gizmos.DrawLine(point1 - Vector3.right * radius, point2 - Vector3.right * radius);
    }
}
