using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileSkill : MonoBehaviour
{
    public LayerMask targetLayer;
    public GameObject projectileObject;
    public ParticleSystem particle;
    private float radius;
    private TowerData towerInfo;
    private float attackMultiplier; // 배율
    private int attackCount; // 공격 횟수
    private Vector3 originPos;
    private bool ready = true; // 중복 실행 방지
    Animator _animator;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        originPos = projectileObject.transform.localPosition;
    }
    void Update()
    {
        projectileObject.transform.position += Vector3.down * Time.deltaTime * 6f;
        if (ready)
        {
            // 지면에 닿으면 충돌
            if (projectileObject.transform.position.y <= 0)
            {
                if (particle != null)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }
                ready = false;
                HitTarget();
                return;
            }
        }
    }

    // 데이터 세팅
    public void Set(TowerData towerInfo, float radius, float attackMultiplier, int attackCount = 1)
    {
        ready = true;
        projectileObject.SetActive(true);
        projectileObject.transform.localPosition = originPos;
        this.attackCount = attackCount;
        this.towerInfo = towerInfo;
        this.attackMultiplier = attackMultiplier;
        this.radius = radius * TileManager.Instance.GetStepSize();
    }

    // 범위 공격
    public void HitTarget()
    {
        projectileObject.SetActive(false);
        for (int i = 0; i < attackCount; i++)
        {
            Vector3 point0 = transform.position;
            Vector3 point1 = transform.position + Vector3.up * 3f;
            Collider[] cols = Physics.OverlapCapsule(point0, point1, radius, targetLayer);
            foreach (var col in cols)
            {
                EnemyState enemy = col.GetComponent<EnemyState>();
                // 명중 계산
                if (enemy != null && Random.value * 100f < towerInfo.towerInfoTable.accuracy)
                {
                    float towerAttack = towerInfo.towerInfoTable.attack;
                    float damage = towerAttack * attackMultiplier;
                    // 치명타 계산
                    if (Random.value * 100f < towerInfo.towerInfoTable.criticalRate)
                        damage *= towerInfo.towerInfoTable.criticalCoefficient * 0.01f;
                    enemy.TakeDamage(damage);
                }
            }
        }

        // 비활성화
        StartCoroutine(Deactivate(0.25f));
    }
    private IEnumerator Deactivate(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        if (particle != null)
        {
            particle.gameObject.SetActive(false);
        }
    }
    void OnDrawGizmos()
    {
        // 시작 위치에 블록 크기의 구형
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
