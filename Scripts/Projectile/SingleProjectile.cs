using System.Collections.Generic;
using UnityEngine;

public class SingleProjectile : BaseProjectile
{
    protected override void HitTarget()
    {
        // 데미지 적용
        EnemyState enemy = target.GetComponent<EnemyState>();
        if (enemy != null)
        {
            if (IsHit())
            {

                // 이펙트 생성
                if (explosionEffect != null)
                {
                    ObjectPoolManager.Instance.GetParticle((int)objectType, transform.position, Quaternion.identity);
                }

                float damage = projectileStat.damage;
                bool isCritical = false;
                if (IsCritical())
                {
                    isCritical = true;
                    damage *= projectileStat.criticalCoefficient * 0.01f;
                }

                enemy.TakeDamage(damage, attackProperty, isCritical, ignoreDefenceAmount, onKillAction);

                // 주변 적 공격
                if (bounce != 0)
                {
                    int remain = bounce;
                    Collider[] hits = Physics.OverlapSphere(transform.position, 0.5f, SkillManager.Instance.enemyLayer);
                    foreach (var obj in hits)
                    {
                        if (remain <= 0) return;
                        if (obj.TryGetComponent<EnemyState>(out var enemyState) && enemyState != enemy)
                        {
                            enemyState.TakeDamage(damage, attackProperty, isCritical, ignoreDefenceAmount, onKillAction);
                            remain--;
                        }
                    }
                }
                // 지속 데미지
                if (dotdamageID != 0)
                {
                    float amout = dotdamageAmount * 0.01f * projectileStat.damage;
                    DamageDebuff damageDebuff = new(dotdamageDuration, dotdamageAmount, Color.green, dotdamageID, onKillAction);
                    enemy.ExecuteDebuff(damageDebuff);
                }
                // 집중 공격
                if (focusKey != 0)
                {
                    enemy.SetFocus(focusKey, focusAmount);
                }
            }
            else
            {
                ObjectPoolManager.Instance.ShowDamage("MISS", enemy.transform, Vector3.up * 2, false);
            }
        }

        // 통과 여부 확인
        if (through <= 0)
        {
            OnDespawn();
        }
        else
        {
            through--;
        }
    }
}
