using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
public class Tower : BaseUnit
{
    public TowerData towerData;
    public OperatorData operatorData;
    public override int GetTowerKey()
    {
        return towerData.towerInfoTable.key;
    }
    // UI
    [SerializeField] private GameObject worldCanvas; // 월드 UIa
    [SerializeField] private GameObject enforceUI; // 강화 이미지
    [SerializeField] private GameObject rangeUI; // 사거리 이미지
    [SerializeField] private GameObject ProfileUI; // 프로필 오브젝트
    [SerializeField] private Image ProfileImage; // 프로필 오브젝트
    [Header("Attack_Rate")]
    private float timer;
    [SerializeField] private float attackCooldown;
    private float lastAttackRate;
    [SerializeField] private float buffMultiplier = 1f; // 타워에 스킬로 공속 버프 들어갈시
    private float debuffMultiplier = 1f; // 타워에 적의 공속 디버프 들어갈시
    private float traitMultiplier = 1f; // 타워의 오퍼레이터의 공격속도 트레잇 있을경우
    public float totalAttackSpeedMultiplier =>
         buffMultiplier * debuffMultiplier * traitMultiplier;

    [Header("Projectile_Shoot")]
    [SerializeField] private SFXType sfxType;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem fireAfterEffect;
    [SerializeField] Transform spawnProjectile;
    public GameObject projectile;
    private ObjectType projectileType;

    // 테스트를 위해서 serializeField 타워밸런스 패치 끝나면 정리할것
    [Header("Targeting")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float baseProximityTolerance = 0.1f;
    [SerializeField] private Transform baseTransform; // 플레이어 기지
    [SerializeField] private GameObject headModel;  // 타겟팅시 회전할 포탑 머리모델
    [SerializeField] private bool headLock;         // 포탑이 회전X면 사용
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float attackAngleThreshold = 20f; // 몇 도 이내면 "일치"라고 판단할지
    private bool isHeadToTarget;
    private Transform currentTarget;
    private Collider[] colliders = new Collider[64]; // OverlapSphereNonAlloc 용

    private int killCount = 0; // 처치 수
    private List<int> skillKeys; // 스킬 키
    private List<int> chances; // 확률
    private List<int> reqCounts; // 필요 킬 수
    private List<int> durations; // 지속 시간
    private List<int> rewardScales; // 보상 배율 수
    private CapsuleCollider capsuleTrigger; // 사거리 트리거 콜라이더

    // 버프 / 디버프 상태
    private Dictionary<(BUFF_TYPE, int), Coroutine> buffCoroutines; // 자신의 버프 코루틴
    private Dictionary<BUFF_TYPE, float> buffAmount; // 버프 배율 저장 딕셔너리
    private Dictionary<BUFF_TYPE, Action> finishActions; // 자신의 버프 종료 행동

    private Dictionary<BUFF_TYPE, ITowerBuff> buffs; // 다른 타워에 주는 버프 목록
    private Dictionary<DEBUFF_TYPE, IDeBuffData> debuffs; // 적에게 주는 디버프 딕셔너리
    // 기본 스텟
    private float originAccuracy;

    // 자신의 디버프, 버프 값
    private int slowAmount = 0;
    private int defenseAmount = 0;
    private int attackAmount = 0;
    private float attackMultiplier = 1f;
    private int attackRateAmount = 0;
    private int accuracyAmount = 0;
    private float accuracyMultiplier = 1f;
    private int criticalRateAmount = 0;
    private float criticalRateBonus = 0;
    private float criticalScale = 0;
    private float criticalScaleMultiplier = 1f;
    private float rangeMultiplier = 1f;
    private float operatorRangeAdd = 1f;
    private float ignoreDefenceAmount = 1f;

    private int throughAmount;
    private float focusBase;
    private float focusAmount;
    private int bounceAmount;
    private float damageDebuffAmount;
    private float damageDebuffDuration;
    private bool forceAOE; // 강제적으로 범위 공격화
    private float aOERange = 1f; // 범위 공격의 스플래시 범위

    [Header("Test")]
    public bool DrawGizmos;

    public void Init(TowerData towerData, [CanBeNull] OperatorData operatorData)
    {
        this.towerData = towerData;

        key = towerData.towerInfoTable.key;

        lastAttackRate = towerData.towerInfoTable.attackRate;
        projectileType = projectile.GetComponent<BaseProjectile>().objectType;

        // 컬렉션 초기화
        skillKeys = new();
        chances = new();
        reqCounts = new();
        durations = new();
        rewardScales = new();
        buffCoroutines = new();
        finishActions = new();
        buffAmount = new();
        buffs = new();
        debuffs = new();

        // 오퍼레이터 데이터 가져오기
        if (operatorData != null)
        {
            this.operatorData = operatorData;
            operatorKey = operatorData.operatorInfoTable.key;

            // 프로필 표시
            ProfileUI.SetActive(true);
            ProfileImage.sprite = operatorData.profile;

            // 사거리 증가
            operatorRangeAdd = towerData.towerInfoTable.range * operatorData.operatorInfoTable.range * 0.01f;

            // 사거리 트리거 콜라이더 크기 설정
            capsuleTrigger = GetComponent<CapsuleCollider>();
            capsuleTrigger.isTrigger = true;
            capsuleTrigger.height = 0;
            capsuleTrigger.center = new Vector3(0, 0, 0);
            // TODO : 콜라이더 방식 개선
            //capsuleTrigger.radius = towerData.towerInfoTable.range * 0.5f;

            // 사거리 내 버프, 디버프 검사
            throughAmount = BattleManager.Instance.GetEffectN(operatorData.operatorInfoTable.key, 6606);
            focusBase = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6607);
            bounceAmount = BattleManager.Instance.GetEffectN(operatorData.operatorInfoTable.key, 6608);
            damageDebuffAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6609);
            damageDebuffDuration = BattleManager.Instance.GetDuration(operatorData.operatorInfoTable.key, 6609);

            slowAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6700);
            defenseAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6701);

            attackAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6800);
            attackRateAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6801);
            accuracyAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6802);
            criticalRateAmount = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6803);
            criticalScale = BattleManager.Instance.GetValue(operatorData.operatorInfoTable.key, 6804);

            // 기본 능력치 저장
            originAccuracy = towerData.towerInfoTable.accuracy;

            // 처치 시 패시브 검사
            foreach (var passiveKey in BattleManager.Instance.GetKillPassives(operatorData.operatorInfoTable.key))
            {
                skillKeys.Add(passiveKey);
                chances.Add(BattleManager.Instance.GetValue(passiveKey));
                reqCounts.Add(BattleManager.Instance.GetEnemyCondition(passiveKey));
                durations.Add(BattleManager.Instance.GetDuration(passiveKey));
                rewardScales.Add(BattleManager.Instance.GetEffectN(passiveKey));
            }
        }

        AttackRateSet();
    }

    protected void Start()
    {
        baseTransform = TileManager.Instance.headquarterTransform;

        // 테스트용 테스트 완료후 삭제 TODO:
        projectileType = projectile.GetComponent<BaseProjectile>().objectType;
    }

    private void Update()
    {
        if (isBuild)
        {
            timer += Time.deltaTime;

            //int count = Physics.OverlapSphereNonAlloc(transform.position, towerData.range, colliders, targetLayer);
            float radius = GetRange() * 0.5f;

            Vector3 point1 = transform.position;
            if (towerData.towerInfoTable.antiAir == 1)
                point1 = transform.position + Vector3.up * (2f * radius);
            Vector3 point2 = transform.position;

            int count = Physics.OverlapCapsuleNonAlloc(point1, point2, radius, colliders, targetLayer);

            Transform selectedTarget = null;
            float minBaseDistance = float.MaxValue;
            float minTowerDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider col = colliders[i];

                // 중심점이 범위 안에 있는지 확인 (OverlapSphere는 콜라이더 전체 기준이므로 약간의 보정 필요 가능)
                Vector3 enemyPos = col.transform.position;
                float baseDist = Vector3.Distance(enemyPos, baseTransform.position);
                float towerDist = Vector3.Distance(enemyPos, transform.position);

                // 가장 기지에 가까운 적 우선
                if (baseDist < minBaseDistance - baseProximityTolerance)
                {
                    selectedTarget = col.transform;
                    minBaseDistance = baseDist;
                    minTowerDistance = towerDist;
                }
                else if (Mathf.Abs(baseDist - minBaseDistance) <= baseProximityTolerance)
                {
                    // 기지 거리 동일 → 타워 거리 비교
                    if (towerDist < minTowerDistance)
                    {
                        selectedTarget = col.transform;
                        minTowerDistance = towerDist;
                    }
                }
            }

            currentTarget = selectedTarget;
        }

        // 범위내 타겟이 설정되있으면 포탑의 머리 회전하여 타겟팅
        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - headModel.transform.position;
            /// 
            /// 포탑 모델링 얻으면 포탑 머리도 위아래 바라볼지 확인해서 결정할것
            /// 지금은 좌우 이동만 가능하게 만듬
            direction.y = 0; // 위아래 방향 제거
            if (direction == Vector3.zero) return; // 방향 벡터가 0이면 리턴


            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 포탑 머리를 천천히 회전
            if (!headLock)
            {
                headModel.transform.rotation = Quaternion.RotateTowards(
                    headModel.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            float angle = Quaternion.Angle(headModel.transform.rotation, targetRotation);
            if (angle < attackAngleThreshold || headLock)
            {
                isHeadToTarget = true;
            }
            else
            {
                isHeadToTarget = false;
            }

        }

        // 포탑의 머리가 타겟팅 되어있고 공격 쿨타임 돌아오면 공격
        if (timer >= attackCooldown)
        {

            if (currentTarget != null && isHeadToTarget)
            {
                Attack(currentTarget);
                timer = 0f;
            }
        }

        // 업그레이드 확인 ( 기능 동작중 : 공격속도 / ??? / ??? )
        ApplyUpgrade();
    }
    public override void SetGradeColor()
    {
        TowerUpgradeColor color = gameObject.GetComponent<TowerUpgradeColor>();
        color.SetGradeMaterial(grade - 1);
    }
    // 투사체를 스플래시 투사체로 변경
    public void ForceChangeBulletType(float range)
    {
        aOERange = range;
        forceAOE = true;
    }
    // 투사체를 복원
    public void RestoreBulletType()
    {
        forceAOE = false;
    }
    private ObjectType SelectBulletType()
    {
        return projectileType;

        //int btype = towerData.towerInfoTable.attackType;
        // 
        // if (forceAOE)
        //     btype = 1;
        // switch (btype)
        // {
        //     case 0:
        //         return ObjectType.SingleBullet;
        //     case 1:
        //         return ObjectType.AOEBullet;
        //     default:
        //         return ObjectType.SingleBullet;
        // }
    }
    private void Attack(Transform target)
    {

        BaseProjectile pro = ObjectPoolManager.Instance.GetObject(SelectBulletType(),
            spawnProjectile.position, spawnProjectile.rotation).GetComponent<BaseProjectile>();

        float _damage = GetAttackPower(towerData.towerInfoTable.attack, grade);
        float _criticalChance = towerData.towerInfoTable.criticalRate;
        float _criticalCoefficient = towerData.towerInfoTable.criticalCoefficient;
        float _accuracy = towerData.towerInfoTable.accuracy;
        if (operatorData != null)
        {
            _damage += _damage * operatorData.operatorInfoTable.attack * 0.01f;
            _criticalChance += _criticalChance * operatorData.operatorInfoTable.criticalRate * 0.01f;
            _criticalCoefficient += _criticalCoefficient * operatorData.operatorInfoTable.criticalMultiplier * 0.01f;
            _accuracy += _accuracy * operatorData.operatorInfoTable.accuracy * 0.01f;
        }
        ProjectileStat projStat = new ProjectileStat
        {
            damage = _damage,
            criticalChance = _criticalChance + criticalRateBonus,
            criticalCoefficient = _criticalCoefficient * criticalScaleMultiplier,
            accuracy = _accuracy,
        };
        // 범위 공격이면 범위 지정

        if (SelectBulletType() == ObjectType.TURRETSHOOT_FIREGUN || SelectBulletType() == ObjectType.TURRETSHOOT_GRANADE ||
            SelectBulletType() == ObjectType.TURRETSHOOT_MISSILE || SelectBulletType() == ObjectType.TURRETSHOOT_PLASMATOWER ||
            SelectBulletType() == ObjectType.TURRETSHOOT_ENERGYSTOMTURRET || SelectBulletType() == ObjectType.TURRETSHOOT_PLASMATURRET)
        {
            pro.GetComponent<AoEProjectile>().SetRange(aOERange);
        }
        // 적 설정 및 이벤트 전달
        pro.SetTarget(target, projStat, (BULLETTYPE)towerData.towerInfoTable.bulletType, OnKillEvent);
        // 사거리 설정
        pro.SetDistance(GetRange());
        // 데미지 옵션들 설정
        pro.SetIgnoreDefence(ignoreDefenceAmount);
        if (damageDebuffAmount != 0)
        {
            pro.SetDamageDebuff(damageDebuffAmount, damageDebuffDuration, 6609);
        }
        if (bounceAmount != 0)
        {
            pro.SetBounce(bounceAmount);
        }
        if ((focusAmount + focusAmount) != 0)
        {
            pro.SetFocus(operatorData.operatorInfoTable.key, focusAmount + focusAmount);
        }
        if (throughAmount != 0)
        {
            pro.SetThrough(throughAmount);
        }

        SoundManager.Instance.PlaySFX(sfxType);
        PlayMuzzleEffect();

    }

    private void PlayMuzzleEffect()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        if (fireAfterEffect != null)
        {
            fireAfterEffect.Play();
        }

    }

    // towerData(SO) 변경시 설치된 타워들이 SO를 참조하여 실시간 반영됨
    // tower는 내부 데이터가 변경되었는지 확인
    private void ApplyUpgrade()
    {
        // 공속변환시 타워의 쿨타임 변환
        if (!Mathf.Approximately(lastAttackRate, towerData.towerInfoTable.attackRate))
        {
            lastAttackRate = towerData.towerInfoTable.attackRate;
            AttackRateSet();
        }
    }

    // 건물 등급 상승시 합연산 base + ( base * n^grade )
    public float GetAttackPower(float basePower, int level, float multiplierPerLevel = 0.7f)
    {
        float power = basePower;

        for (int i = 2; i <= level; i++)
        {
            float upgradeRate = Mathf.Pow(multiplierPerLevel, i - 1); // 0.7^(i - 1)
            power += power * upgradeRate;
        }

        return power * attackMultiplier;
    }

    private void AttackRateSet()
    {
        float attackRate = towerData.towerInfoTable.attackRate;
        if (operatorData != null)
        {
            attackRate += attackRate * operatorData.operatorInfoTable.attackRate * 0.01f;
        }

        attackCooldown = 1f / (attackRate * totalAttackSpeedMultiplier); // 공격/초 = 초당 n번 공격
    }

    public void ApplyBuff(float percentIncrease, float duration)
    {
        StartCoroutine(BuffCoroutine(percentIncrease, duration));
    }

    private IEnumerator BuffCoroutine(float percentIncrease, float duration)
    {
        buffMultiplier *= (percentIncrease * 0.01f);
        AttackRateSet();
        yield return new WaitForSeconds(duration);
        buffMultiplier /= (percentIncrease * 0.01f);
        AttackRateSet();
    }

    public override float GetRange()
    {
        return (towerData.towerInfoTable.range + operatorRangeAdd)* 0.5f * rangeMultiplier;
    }
    public float GetAttackRate()
    {
        return attackCooldown;
    }
    public int GetKillCount()
    {
        return killCount;
    }

    // 처치 시 함수 실행
    public void OnKillEvent(int exp)
    {
        killCount++;
        // 처지 시 발동 패시브 모두 실행
        for (int i = 0; i < reqCounts.Count; i++)
        {
            // 처치 수 만족 확인
            if (killCount % reqCounts[i] == 0)
            {
                // 확률 확인 , 0 ~ 100
                if (chances[i] == 0 || UnityEngine.Random.Range(0, 101) < chances[i])
                {
                    // 스킬 적용
                    switch (skillKeys[i])
                    {
                        case 6510:
                            // 골드 획득
                            BattleManager.Instance.CurrentBattle.LevelUP(exp * rewardScales[i]);
                            break;
                        case 6511:
                            // 경험치 획득
                            BattleManager.Instance.CurrentBattle.TakeGold(exp * rewardScales[i]);
                            break;
                        case 6512:
                            // 아이템 획득
                            BattleManager.Instance.CurrentBattle.RewardSystem.DropItem();
                            break;
                        case 6513:
                            // 설치 카드 획득
                            BattleManager.Instance.CurrentBattle.GetRandomTower();
                            break;
                        case 6610:
                            // 공격력, 공격 속도 상승
                            // 공격 버프
                            if (!buffs.TryGetValue(BUFF_TYPE.ATK_UP, out var atkBuff))
                            {
                                atkBuff = new AttackUpBuff(durations[i], rewardScales[i], skillKeys[i]);
                                buffs[BUFF_TYPE.ATK_UP] = atkBuff;
                            }
                            ExecuteBuff((AttackUpBuff)atkBuff);
                            // 공격 속도 버프
                            if (!buffs.TryGetValue(BUFF_TYPE.ATK_RATE_UP, out var atkRateBuff))
                            {
                                atkRateBuff = new AttackRateBuff(durations[i], rewardScales[i], skillKeys[i]);
                                buffs[BUFF_TYPE.ATK_RATE_UP] = atkRateBuff;
                            }
                            ExecuteBuff((AttackRateBuff)atkRateBuff);
                            break;
                        case 6611:
                            // 사거리, 명중률 스텟 증가
                            // 사거리 버프
                            if (!buffs.TryGetValue(BUFF_TYPE.RANGE_UP, out var rangeBuff))
                            {
                                rangeBuff = new RangeUpBuff(durations[i], rewardScales[i], skillKeys[i]);
                                buffs[BUFF_TYPE.RANGE_UP] = rangeBuff;
                            }
                            ExecuteBuff((RangeUpBuff)rangeBuff);
                            // 명중 버프
                            if (!buffs.TryGetValue(BUFF_TYPE.ACCURACY_UP, out var accuBuff))
                            {
                                accuBuff = new AccuracyUpBuff(durations[i], rewardScales[i], skillKeys[i]);
                                buffs[BUFF_TYPE.ACCURACY_UP] = accuBuff;
                            }
                            ExecuteBuff((AccuracyUpBuff)accuBuff);
                            break;
                        case 6612:
                            // 치명타 100퍼센트
                            if (!buffs.TryGetValue(BUFF_TYPE.CRI_CHANCE_UP, out var buff))
                            {
                                buff = new CriticalChanceUpBuff(durations[i], 2000, skillKeys[i]);
                                buffs[BUFF_TYPE.CRI_CHANCE_UP] = buff;
                            }
                            ExecuteBuff((CriticalChanceUpBuff)buff);
                            break;
                        case 6703:
                            // 스폰 시 스피드 디버프
                            BattleManager.Instance.LootSlow(skillKeys[i], (100 - rewardScales[i]) / 100f, durations[i]);
                            break;
                        case 6704:
                            // 스폰 시 방어 디버프
                            BattleManager.Instance.LootDefenseDown(skillKeys[i], (100 - rewardScales[i]) / 100f, durations[i]);
                            break;
                    }
                }
            }
        }
    }

    // 트리거(사거리 내부) 버프/ 디버프
    void OnTriggerEnter(Collider other)
    {
        if (isBuild)
        {
            // 적 디버프
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<EnemyState>();
                if (slowAmount != 0)
                {
                    // 스피드 디버프
                    if (!debuffs.TryGetValue(DEBUFF_TYPE.SLOW, out var debuff))
                    {
                        debuff = new SlowDebuff(999f, slowAmount);
                        debuffs[((SlowDebuff)debuff).DebuffType] = debuff;
                    }
                    enemy.ExecuteDebuff((SlowDebuff)debuff);
                }
                if (defenseAmount != 0)
                {
                    // 방어 디버프
                    if (!debuffs.TryGetValue(DEBUFF_TYPE.DEFENSE_DOWN, out var debuff))
                    {
                        debuff = new DefenseDebuff(999f, defenseAmount);
                        debuffs[DEBUFF_TYPE.DEFENSE_DOWN] = debuff;
                    }
                    enemy.ExecuteDebuff((DefenseDebuff)debuff);
                }
            }
            else if (other.CompareTag("Tower"))
            {
                // 타워 버프
                var tower = other.GetComponent<Tower>();
                // 건설 된 타워만
                if (!tower.isBuild) return;
                if (attackAmount != 0)
                {
                    // 공격 버프
                    if (!buffs.TryGetValue(BUFF_TYPE.ATK_UP, out var buff))
                    {
                        buff = new AttackUpBuff(999f, attackAmount, 6800);
                        buffs[BUFF_TYPE.ATK_UP] = buff;
                    }
                    tower.ExecuteBuff((AttackUpBuff)buff);
                }
                if (attackRateAmount != 0)
                {
                    // 공격 속도 버프
                    if (!buffs.TryGetValue(BUFF_TYPE.ATK_RATE_UP, out var buff))
                    {
                        buff = new AttackRateBuff(999f, attackRateAmount, 6801);
                        buffs[BUFF_TYPE.ATK_RATE_UP] = buff;
                    }
                    tower.ExecuteBuff((AttackRateBuff)buff);
                }
                if (accuracyAmount != 0)
                {
                    // 명중률 버프
                    if (!buffs.TryGetValue(BUFF_TYPE.ACCURACY_UP, out var buff))
                    {
                        buff = new AccuracyUpBuff(999f, accuracyAmount, 6802);
                        buffs[BUFF_TYPE.ACCURACY_UP] = buff;
                    }
                    tower.ExecuteBuff((AccuracyUpBuff)buff);
                }
                if (criticalRateAmount != 0)
                {
                    // 크리티컬 확률 버프
                    if (!buffs.TryGetValue(BUFF_TYPE.CRI_CHANCE_UP, out var buff))
                    {
                        buff = new CriticalChanceUpBuff(999f, criticalRateAmount, 6803);
                        buffs[BUFF_TYPE.CRI_CHANCE_UP] = buff;
                    }
                    tower.ExecuteBuff((CriticalChanceUpBuff)buff);
                }
                if (criticalScale != 0)
                {
                    // 크리티컬 배율 버프
                    if (!buffs.TryGetValue(BUFF_TYPE.CRI_SCALE_UP, out var buff))
                    {
                        buff = new CriticalScaleBuff(999f, criticalScale, 6804);
                        buffs[BUFF_TYPE.CRI_SCALE_UP] = buff;
                    }
                    tower.ExecuteBuff((CriticalScaleBuff)buff);
                }
            }
        }
    }
    // 트리거(사거리 내부) 버프/ 디버프
    void OnTriggerExit(Collider other)
    {
        if (isBuild)
        {
            // 적 디버프
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<EnemyState>();
                if (slowAmount != 0)
                {
                    // 스피드 디버프 
                    enemy.EndDebuff(debuffs[DEBUFF_TYPE.SLOW]);
                }
                if (defenseAmount != 0)
                {
                    // 방어 디버프
                    enemy.EndDebuff(debuffs[DEBUFF_TYPE.DEFENSE_DOWN]);
                }
            }
            // 타워 버프
            else if (other.CompareTag("Tower"))
            {
                var tower = other.GetComponent<Tower>();
                // 건설 된 타워만
                if (!tower.isBuild) return;
                if (attackAmount != 0)
                {
                    tower.EndBuff(buffs[BUFF_TYPE.ATK_UP]);
                }
                if (attackRateAmount != 0)
                {
                    tower.EndBuff(buffs[BUFF_TYPE.ATK_RATE_UP]);
                }
                if (accuracyAmount != 0)
                {
                    tower.EndBuff(buffs[BUFF_TYPE.ACCURACY_UP]);
                }
                if (criticalRateAmount != 0)
                {
                    tower.EndBuff(buffs[BUFF_TYPE.CRI_CHANCE_UP]);
                }
                if (criticalScale != 0)
                {
                    tower.EndBuff(buffs[BUFF_TYPE.CRI_SCALE_UP]);
                }
            }
        }
    }

    // 변경 버프를 적용, +은 증가, -는 감소, -5%면 scale을 -0.05
    public void SetBuff(BUFF_TYPE buffType, float scale, float defaultValue = 1f)
    {
        // 값이 있는지 확인
        if (!buffAmount.TryGetValue(buffType, out float currentValue))
        {
            currentValue = defaultValue;
        }
        // 효과 증감
        currentValue += scale;

        // 양수(버프 시작이면) 저장, 음수(버프 종료면) 해제
        if (scale > 0)
            buffAmount[buffType] = currentValue;
        else
            buffAmount.Remove(buffType);

        switch (buffType)
        {
            case BUFF_TYPE.ATK_UP:
                attackMultiplier = currentValue;
                break;
            case BUFF_TYPE.ATK_RATE_UP:
                buffMultiplier = currentValue;
                AttackRateSet();
                break;
            case BUFF_TYPE.ACCURACY_UP:
                accuracyMultiplier = originAccuracy * currentValue;
                break;
            case BUFF_TYPE.CRI_CHANCE_UP:
                criticalRateBonus = currentValue;
                break;
            case BUFF_TYPE.CRI_SCALE_UP:
                criticalScaleMultiplier = currentValue;
                break;
            case BUFF_TYPE.RANGE_UP:
                rangeMultiplier = currentValue;
                //capsuleTrigger.radius = towerData.towerInfoTable.range * 0.5f * rangeMultiplier;
                break;
            case BUFF_TYPE.IGNORE_DEFENCE:
                ignoreDefenceAmount = currentValue;
                break;
            case BUFF_TYPE.FOCUS:
                focusAmount = currentValue;
                break;
        }
    }
    // 버프 실행
    public void ExecuteBuff(ITowerBuff data)
    {
        var key = (data.BUFF_TYPE, data.ID);
        // 이미 적용중인 같은 타입의 디버프가 있다면 종료
        if (buffCoroutines.TryGetValue(key, out var coroutine))
        {
            StopCoroutine(coroutine);
            // 종료 함수 실행
            finishActions[data.BUFF_TYPE]?.Invoke();
        }

        // 새로운 디버프 적용 코루틴 실행F
        buffCoroutines[key] = StartCoroutine(ApplyBuff(data));
        // 종료 함수 등록
        finishActions[data.BUFF_TYPE] = () => EndBuff(data);
    }

    // 버프 효과 적용 코루틴
    private IEnumerator ApplyBuff(ITowerBuff data)
    {
        // 적용함수 실행
        BuffEffect(data);
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
        finishActions[data.BUFF_TYPE]?.Invoke();
    }
    // 효과 적용
    private void BuffEffect(ITowerBuff data)
    {
        data.ApplyDebuff(this);
    }
    // 효과 종료
    public void EndBuff(ITowerBuff data)
    {
        data.RemoveDebuff(this);
    }
    // 강화 UI 표시
    public void TryShowEnforce(bool active)
    {
        if (active)
        {
            if (grade <= 5 && worldCanvas != null && enforceUI != null)
            {
                enforceUI.SetActive(true);
            }
        }
        else
        {
            if (worldCanvas != null && enforceUI != null)
            {
                enforceUI.SetActive(false);
            }
        }
    }

    // 사거리 표시
    public void ShowRange(bool active)
    {
        rangeUI.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(GetRange(), GetRange());
        rangeUI.SetActive(active);
    }
    private void OnEnable()
    {
        StopAllCoroutines();
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            float radius = GetRange() * 0.5f;
            Vector3 center = transform.position;
            Vector3 point1 = transform.position;
            if (towerData.towerInfoTable.antiAir == 1)
                point1 = transform.position + Vector3.up * (2f * radius);
            Vector3 point2 = center;

            Gizmos.color = Color.green;

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

}
