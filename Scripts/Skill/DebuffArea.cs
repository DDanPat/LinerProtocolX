using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 영역내 유닛에게 디버프 적용
public class DebuffArea : MonoBehaviour
{
    public ParticleSystem areaParticle;
    float radius;
    CapsuleCollider _capsuleCollider;
    List<EnemyState> enemies;
    IDeBuffData debuff;
    void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Set(float range, float duration, IDeBuffData debuff)
    {
        if (areaParticle != null)
            areaParticle.Play();

        enemies = new();
        radius = range / 2f;
        _capsuleCollider.radius = radius;
        this.debuff = debuff;

        // 지속 시간 이후 삭제
        Invoke(nameof(Delete), duration);
    }
    private void Delete()
    {
        // 삭제 전 디버프 해제
        foreach (var enemy in enemies)
        {
            enemy.EndDebuff(debuff);
        }
        Destroy(gameObject);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyState>(out var enemy))
            {
                // 목록 추가
                enemies.Add(enemy);
                // 디버프 적용
                enemy.ExecuteDebuff(debuff);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyState>(out var enemy))
            {
                // 목록 제거
                enemies.Remove(enemy);
                // 디버프 종료
                enemy.EndDebuff(debuff);
            }
        }
    }
}
