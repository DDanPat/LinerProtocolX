using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
///  타워가 발사할 투사체 베이스
/// </summary>

public struct ProjectileStat
{
    // 투사체 데미지 크리티컬 확률, 크리티컬 계수, 명중률 
    public float damage;
    public float criticalChance;
    public float criticalCoefficient;
    public float accuracy;
}

public class BaseProjectile : MonoBehaviour, IPoolable
{
    [Header("Attack Type")]
    [SerializeField] protected BULLETTYPE attackProperty;
    public ObjectType objectType;   // 총알의 종류
    
    [SerializeField] protected Transform target;
    [SerializeField] private float speed = 10f;
    public float distanceToTargetDestroy = 0.1f;
    public GameObject explosionEffect;
    protected ProjectileStat projectileStat;
    public float hitRadius = 0.3f;
    public LayerMask targetLayer;
    Vector3 dir; // 진행 방향
    private Collider[] hits;
    private HashSet<EnemyState> alreadyHit = new HashSet<EnemyState>();
    [SerializeField] private bool isHoming = false;
    protected UnityAction<int> onKillAction;
    protected Dictionary<int, IDeBuffData> debuffs;
    protected int bounce; // 주변 타겟 수
    protected int through; // 관통 수
    protected int dotdamageID; // 지속 데미지 키
    protected float dotdamageAmount; // 지속 데미지 량
    protected float dotdamageDuration; // 지속 데미지 시간
    protected int focusKey; // 집중 공격 구별 키
    protected float focusAmount; // 집중 공격 데미지
    protected float ignoreDefenceAmount = 1f; // 방어 배율 - 감소시 방어력 무시

    private Action<GameObject> returnToPool;
    private Vector3 startPos;
    private float distance = float.MaxValue;

    private void Awake()
    {
        hits = new Collider[5];
        // 임시 삭제
        StartCoroutine(DelayDespanw());
    }
    void OnEnable()
    {
        startPos = transform.position;
    }
    private IEnumerator DelayDespanw()
    {
        yield return new WaitForSeconds(1f);
        OnDespawn();
    }
    private void Update()
    {
        // 타겟이 죽으면 발사체도 사라짐
        // if (target == null && through == 0)
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        // 원기둥형으로 검사
        //Vector3 p0 = transform.position + Vector3.down * 2f;
        //Vector3 p1 = p0 + Vector3.up * 4f;
        //int count = Physics.OverlapCapsuleNonAlloc(p0, p1, hitRadius, hits, targetLayer);
        //for (int i = 0; i < count; i++)
        //{
        //    if (hits[i].gameObject.TryGetComponent<EnemyState>(out var enemy))
        //    {
        //        if (!alreadyHit.Contains(enemy))
        //        {
        //            target = enemy;
        //            alreadyHit.Add(enemy);
        //            HitTarget();
        //            break;
        //        }
        //    }
        //}
        // 타겟 위치에 도달하면 히트 실행
        
    }

    private void FixedUpdate()
    {
        float distanceThisFrame = speed * Time.deltaTime;
        
        // 이동 방향
        Vector3 dir = target.position - transform.position;
        
        // 목표 지점에 도달함
        if (Vector3.Distance(transform.position, target.position) <= distanceToTargetDestroy)
        {
            transform.position = target.position; 
            HitTarget();
            return;
        }

        // 이동
        transform.position += dir.normalized * distanceThisFrame;
        
        // 이동 넘어가서 사정거리 넘어가면 disable
        if (Vector3.Distance(startPos, transform.position) >= distance)
        {
            OnDespawn();
        }
    }

    public void SetTarget(Transform target, ProjectileStat projectileStat, BULLETTYPE property, UnityAction<int> killCallBack)
    {
        this.target = target;
        // y축 보정으로 떨어지지 않도록
        Vector3 firePos = new Vector3(transform.position.x, 0.5f, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, 0.5f, target.position.z);
        dir = targetPos - firePos;
        this.projectileStat.damage = projectileStat.damage;
        this.projectileStat.criticalChance = projectileStat.criticalChance;
        this.projectileStat.criticalCoefficient = projectileStat.criticalCoefficient;
        this.projectileStat.accuracy = projectileStat.accuracy;
        this.attackProperty = property;
        // 처치 시 이벤트 저장
        onKillAction = killCallBack;
    }
    protected virtual void HitTarget()
    {

    }

    protected bool IsCritical()
    {
        return Random.value * 100f < projectileStat.criticalChance;
    }

    protected bool IsHit()
    {
        // 회피율 적용
        EnemyState enemy = target.gameObject.GetComponent<EnemyState>();
        return (Random.value * 100f - enemy.dodge) < projectileStat.accuracy;
        
        // return (Random.value * 100f - target.dodge) < projectileStat.accuracy;
    }
    // 사거리 적용
    public void SetDistance(float distance)
    {
        this.distance = distance;
    }
    // 지속 데미지 디버프
    public void SetDamageDebuff(float amout, float duration, int id)
    {
        dotdamageID = id;
        dotdamageAmount = amout;
        dotdamageDuration = duration;
    }
    // 주변 적 추가 피해
    public void SetFocus(int operKey, float amout)
    {
        focusKey = operKey;
        focusAmount = amout;
    }

    // 주변 적 추가 피해
    public void SetBounce(int count)
    {
        bounce = count;
    }
    // 적 관통
    public void SetThrough(int count)
    {
        through = count;
    }
    // 방어 무시
    public void SetIgnoreDefence(float amount)
    {
        ignoreDefenceAmount = amount;
    }

    public void Initialize(System.Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        distance = float.MaxValue;
        alreadyHit = new HashSet<EnemyState>();
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(gameObject);
    }
}

