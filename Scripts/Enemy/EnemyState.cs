using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyType
{
    None,
    Boss
}

public class EnemyState : MonoBehaviour, IPoolable
{
    [SerializeField] private Transform target;
    NavMeshAgent _agent;
    EnemyTable enemyData;

    EnemyDrops enemyDrops;
    private Action<GameObject> returnToPool;

    public List<Renderer> renderers; // 렌더러 리스트 - 인스펙터에서 설정할것
    private List<Color> originalColors; // 원본 색상 리스트
    public int key;
    public string enemyName;
    public int type;
    public float size;
    public float health;
    public float nowHealth;
    public float shield;
    public float defense;
    public float dodge;
    public float speed;
    public float exp;
    public EnemyType enemyType;

    float stageHealth;
    float stageShield;
    float stageDefense;


    Collider _collider;
    Animator _animator;
    EnemyDamage _enemyDamage;

    UnityAction<int> killCallBack; // 죽었을 시 콜백
    // 디버프 상태
    // 
    private Dictionary<(DEBUFF_TYPE, int), Coroutine> debuffCoroutines;
    private Dictionary<DEBUFF_TYPE, Action> finishActions;
    private Dictionary<DEBUFF_TYPE, float> debuffAmount;
    [SerializeField] private float damageAmount;
    private int focusOperkey;
    private float focusAmount;
    private Coroutine focusCoroutine; // 집중 공격 코루틴
    private float defenseScale = 1f;
    private Coroutine flashCoroutine; // 피격 코루틴
    // 비행가능 O/X
    //
    void Awake()
    {
        debuffCoroutines = new();
        finishActions = new();
        debuffAmount = new();
        OriginalColorStore();
        _agent = GetComponent<NavMeshAgent>();
        target = TileManager.Instance.headquarterTransform;
        _collider = GetComponent<Collider>();
        _animator = GetComponent<Animator>();
        _enemyDamage = GetComponent<EnemyDamage>();
    }
    private void Init()
    {
        debuffCoroutines = new();
        finishActions = new();
        debuffAmount = new();


        // 스폰시 참고 데이터 불러오기, 배율 적용
        EnemyRoot enemyRoot = BattleManager.Instance.GetEnemyRoot();
        health *= enemyRoot.healthScale;
        shield *= enemyRoot.shieldScale;
        defense *= enemyRoot.defenseScale;
        speed *= enemyRoot.speedScale;
        exp *= enemyRoot.expScale;
        _agent.speed = speed * 0.75f;
        enemyDrops = EnemyDrops.Instance;

        //Transform child = transform.GetChild(0);
        //// 크기 문제
        //child.localScale = child.localScale * size;

        // 지속 데미지 확인
        InvokeRepeating(nameof(TryTakeDotDamage), 0f, 1f);
    }
    public void SetStatus(EnemyTable data)
    {
        enemyData = data;
        key = enemyData.key;
        enemyName = enemyData.name;
        type = enemyData.type;
        size = enemyData.size;
        health = enemyData.health;
        shield = enemyData.shield;
        defense = enemyData.defense;
        speed = enemyData.speed;
        dodge = enemyData.dodge;
        exp = enemyData.exp;

        Init();
        RestoreColor();
    }

    public void FindTarget()
    {
        _agent.SetDestination(target.position);
    }

    public void StatusUP(int curWave)
    {
        int statusUp = (curWave - 1) / 5;
        health = Mathf.Floor(stageHealth * (statusUp + Mathf.Pow(1.19f, statusUp)));
        shield = Mathf.Floor(stageShield * (statusUp + Mathf.Pow(1.26f, statusUp)));
        defense = Mathf.Floor(stageDefense * (statusUp + Mathf.Pow(1.15f, statusUp)));
        exp = enemyData.exp + (curWave - 1);
        // 현재 체력 설정 및 체력바 초기화
        nowHealth = health;
        _enemyDamage.Init();
    }

    public void SetStageStatus(int stage)
    {
        int stageUp = (stage) / 3;
        stageHealth = health * ((0.15f * stageUp) + Mathf.Pow(1.02f, stageUp));
        stageShield = shield * ((0.15f * stageUp) + Mathf.Pow(1.02f, stageUp));
        stageDefense = defense * ((0.15f * stageUp) + Mathf.Pow(1.02f, stageUp));
    }



    public void TakeDamage(float damage, BULLETTYPE bulletType, bool isCritical, float ignoreDefenceAmount = 1f, UnityAction<int> killCallBack = null)
    {
        if (nowHealth <= 0) return;
        HitFlash();
        // 방어 타입 확인
        float damageScale = (this.type == (int)bulletType) ? 1.5f : 0.5f;
        // 방어 감소율 적용한 데미지, 최소 1데미지
        float takeDamage = Mathf.Max(damage * damageScale - (defense * defenseScale * ignoreDefenceAmount), 1f);

        // 집중 공격 확인
        if (focusOperkey != 0)
        {
            takeDamage *= 1 + focusAmount * 0.01f;
        }

        nowHealth -= takeDamage;
        _enemyDamage.UpdateHP(nowHealth, health);
        // 데미지는 정수로
        ObjectPoolManager.Instance.ShowDamage(((int)Math.Floor(takeDamage)).ToString(), transform, Vector3.up * 2, isCritical);
        if (nowHealth <= 0)
        {
            // 처치 시 이벤트 실행
            killCallBack?.Invoke(Mathf.RoundToInt(exp));
            this.killCallBack?.Invoke(Mathf.RoundToInt(exp));
            // 죽음 처리
            IsDie();

        }
    }
    // 단순 데미지
    public void TakeDamage(float damage, UnityAction<int> killCallBack = null)
    {
        if (nowHealth <= 0) return;
        HitFlash();
        float takeDamage = Mathf.Max(damage, 1f);
        nowHealth -= takeDamage;
        _enemyDamage.UpdateHP(nowHealth, health);
        ObjectPoolManager.Instance.ShowDamage(((int)Math.Floor(takeDamage)).ToString(), transform, Vector3.up * 2, false);
        if (nowHealth <= 0)
        {
            // 처치 시 이벤트 실행
            killCallBack?.Invoke(Mathf.RoundToInt(exp));
            this.killCallBack?.Invoke(Mathf.RoundToInt(exp));
            // 죽음 처리
            IsDie();
        }
    }
    // 보호막을 퍼센트로 제거
    public void RemoveShield(float percent)
    {
        shield *= (100 - Mathf.Max(percent, 0f)) / 100f;
    }

    public void IsDie()
    {
        _animator.SetBool("isDie", true);
        _agent.isStopped = true;
        _collider.enabled = false;
    }

    public void Die()
    {
        OnDespawn();
        if (enemyType != EnemyType.Boss) BattleManager.Instance.CurrentBattle.killEnemeyCount++;
        if (enemyType == EnemyType.Boss) QuestManager.Instance.UpdateQuestProgress(QuestConditions.bossKill);

        // 랜덤 수치의 경험치 드롭하기 
        float randomExp = exp * UnityEngine.Random.Range(0.8f, 1.2f);
        enemyDrops.Drops(ObjectType.EXP, this.transform, randomExp);

        // 골드 드랍
        // 골드 드랍(10퍼로 바꾸기),  확률 수정 - 0 ~ 100 정수
        int randomDropGold = UnityEngine.Random.Range(0, 100);
        // 추가 확률 보정
        randomDropGold += BattleManager.Instance.GetEnemyRoot().goldChance;
        // 기본 10퍼센트
        if (randomDropGold > 90)
        {
            // 골드 보너스 배율 적용
            float goldBonus = BattleManager.Instance.GetEnemyRoot().goldScale;
            enemyDrops.Drops(ObjectType.Gold, this.transform, exp * goldBonus);
        }

        // 아이템 드랍
        // 아이템 드랍 (2퍼로 바꾸기), 0 ~100
        int randomDropItem = UnityEngine.Random.Range(0, 100);
        // 보너스 확률
        randomDropItem += BattleManager.Instance.GetEnemyRoot().itemChance;
        // 드랍 2퍼센트
        if (randomDropItem > 97) BattleManager.Instance.CurrentBattle.RewardSystem.DropItem();

        //임시 코드
        BattleManager.Instance.BattleUI.KillCountUpdate(1);
        BattleManager.Instance.CurrentBattle.GameClear();
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Headquarter"))
    //    {
    //        BattleManager.Instance.CurrentBattle.GameOver();
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Headquarter") && BattleManager.Instance.CurrentBattle.isBattle)
        {
            BattleManager.Instance.CurrentBattle.GameOver();
        }
    }
    // 스피드 반환
    public float GetSpeed()
    {
        return _agent.speed;
    }
    public void AddDodge(float value)
    {
        dodge += value;
    }
    // 변경 버프 적용, +은 증가, -는 감소 3% -> +0.03f
    public void SetBuff(DEBUFF_TYPE buffType, float scale, float defaultValue = 1f)
    {
        // 값이 있는지 확인
        if (!debuffAmount.TryGetValue(buffType, out float current))
        {
            current = defaultValue;
        }
        current += scale;
        debuffAmount[buffType] = current;
        switch (buffType)
        {
            case DEBUFF_TYPE.SLOW:
                _agent.speed *= current;
                //_agent.speed = 1f * current;
                break;
            case DEBUFF_TYPE.DEFENSE_DOWN:
                defenseScale = current;
                break;
            case DEBUFF_TYPE.DAMAGE:
                damageAmount = current;
                break;
        }

    }
    // 색 변경
    public void SetBaseColor(Color color)
    {
        foreach (var unitrender in renderers)
        {
            unitrender.material.color = color;
        }
    }
    // 색 원래 복윈
    public void RestoreColor()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material.color = originalColors[i];
        }
    }
    private void OriginalColorStore()
    {
        List<Color> colors = new List<Color>();

        foreach (var unitrender in renderers)
        {
            Material[] materials = unitrender.materials;
            foreach (var material in materials)
            {
                colors.Add(unitrender.material.color);
            }
        }
        originalColors = colors;
    }
    // 피격시 색 변경
    public void HitFlash(float flashDuration = 0.1f, float fadeSpeed = 5f)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(HitFlashCoroutine(flashDuration, fadeSpeed));
    }

    private IEnumerator HitFlashCoroutine(float flashDuration, float fadeSpeed)
    {
        // 검정색으로 변경
        SetBaseColor(Color.black);

        // 대기
        yield return new WaitForSeconds(flashDuration);

        // 점점 원래 색으로
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material.color = Color.Lerp(Color.black, originalColors[i], t);
            }
            yield return null;
        }

        // 복원
        RestoreColor();
        flashCoroutine = null;
    }
    // 집중 공격용 타워 설정
    public void SetFocus(int operKey, float focusAmount)
    {
        // 비활성화시 무시
        if (!gameObject.activeInHierarchy)
            return;

        // 이미 있으면 중지
        if (focusCoroutine != null)
            StopCoroutine(focusCoroutine);

        // 1초후 초기화 코루틴 호출
        focusCoroutine = StartCoroutine(WaitFocus());
        // 대상 설정
        focusOperkey = operKey;
        this.focusAmount = focusAmount;
    }
    // 집중 공격 대상 유지 코루틴
    private IEnumerator WaitFocus()
    {
        yield return new WaitForSeconds(1);
        focusOperkey = 0;
        focusCoroutine = null;
    }
    // 지속 데미지
    public void TryTakeDotDamage()
    {
        // 값이 있는지 확인
        if (damageAmount > 0)
            TakeDamage(damageAmount);
    }
    public void SetKillCallback(UnityAction<int> killCallBack)
    {
        this.killCallBack = killCallBack;
    }
    public void ReSetKillCallback()
    {
        this.killCallBack = null;
    }
    // 디버프 실행
    public void ExecuteDebuff(IDeBuffData data)
    {
        if (nowHealth <= 0) return;
        var key = (data.DebuffType, data.ID);
        if (debuffCoroutines == null) debuffCoroutines = new();
        // 이미 적용중인 같은 타입의 디버프가 있다면 종료
        if (debuffCoroutines.TryGetValue(key, out var coroutine))
        {
            StopCoroutine(coroutine);
            // 종료 함수 실행
            finishActions[data.DebuffType]?.Invoke();
        }

        // 새로운 디버프 적용 코루틴 실행
        debuffCoroutines[key] = StartCoroutine(ApplyBuff(data));
        // 종료 함수 등록
        finishActions[data.DebuffType] = () => EndDebuff(data);
    }

    // 디버프 효과 적용 코루틴
    private IEnumerator ApplyBuff(IDeBuffData data)
    {
        // 적용함수 실행
        DebuffEffect(data);
        float elapsedTime = data.Duration;
        // 0초는 지속시간 무시
        if (Mathf.Approximately(elapsedTime, 0f))
        {
            elapsedTime = 999f;
        }
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        // 지속 시간 종료시 종료 함수 실행
        finishActions[data.DebuffType]?.Invoke();

    }
    // 효과 적용
    private void DebuffEffect(IDeBuffData data)
    {
        data.ApplyDebuff(this);
    }
    // 효과 종료
    public void EndDebuff(IDeBuffData data)
    {
        data.RemoveDebuff(this);
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Initialize(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        _agent.isStopped = false;
        _collider.enabled = true;
        _animator.SetBool("isDie", false);
        damageAmount = 0;
        StopAllCoroutines();
        FindTarget();
        _enemyDamage.Init();
    }

    public void OnDespawn()
    {
        damageAmount = 0;
        StopAllCoroutines();
        returnToPool?.Invoke(gameObject);
    }
}
