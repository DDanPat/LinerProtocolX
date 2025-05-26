using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DelayBomb : MonoBehaviour
{
    [SerializeField] private GameObject BoxObject;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject rangeObject;
    TowerData towerInfo;
    float radius;
    float damage;
    float speedMultiplier;
    float duration;
    float delay;
    LayerMask targetLayer;
    public void Set(TowerData towerInfo, float range, float duration, float damage, float speedMultiplier, float delay)
    {
        BoxObject.SetActive(true);
        if (particle != null)
        {
            particle.gameObject.SetActive(false);
            particle.Play();
        }
        this.towerInfo = towerInfo;
        radius = range / 2;
        rangeObject.transform.localScale = new Vector3(range, range, range);
        this.duration = duration;
        this.delay = delay;
        this.damage = damage;
        this.speedMultiplier = speedMultiplier;
        targetLayer = SkillManager.Instance.enemyLayer;
        StartCoroutine(Explode());
    }
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);
        BoxObject.SetActive(false);
        if (particle != null)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
        SlowDebuff slowDebuff = new(duration, speedMultiplier, 6505);
        // 원기둥으로 검사
        Vector3 point0 = transform.position;
        Vector3 point1 = transform.position + Vector3.up * 3f;
        Collider[] cols = Physics.OverlapCapsule(point0, point1, radius, targetLayer);
        foreach (var col in cols)
        {
            EnemyState enemy = col.GetComponent<EnemyState>();
            // 명중 계산
            if (enemy != null && Random.value * 100f < towerInfo.towerInfoTable.accuracy)
            {
                // 치명타 계산
                if (Random.value * 100f < towerInfo.towerInfoTable.criticalRate)
                    damage *= towerInfo.towerInfoTable.criticalCoefficient * 0.01f;
                enemy.TakeDamage(damage);
                enemy.ExecuteDebuff(slowDebuff);
            }
        }
        // 임시 삭제
        StartCoroutine(Deactivate(0.45f));
    }
    private IEnumerator Deactivate(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
        if (particle != null)
        {
            particle.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }
}
