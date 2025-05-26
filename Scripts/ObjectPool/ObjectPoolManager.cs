using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 내에서 다양한 오브젝트를 풀링하여 관리하는 매니저 클래스
/// - EXP, Gold 등 기본 오브젝트 풀 관리
/// - Enemy 오브젝트 풀 별도 관리
/// - 오브젝트 사전 생성(Preload) 기능 지원
/// - 풀 정리 및 씬 전환 대응 기능 제공
/// </summary>
public enum ObjectType
{
    EXP,
    Gold,
    GachaSlot,
    Enemy,
    TURRETSHOOT_SHOTGUN,
    TURRETSHOOT_GUNTURRET,
    TURRETSHOOT_RAILGUN,
    TURRETSHOOT_FIREGUN,
    TURRETSHOOT_GRANADE,
    TURRETSHOOT_MISSILE,
    TURRETSHOOT_TESLACOIL,
    TURRETSHOOT_LAYSERTURRET,
    TURRETSHOOT_ACCELERATETURRET,
    TURRETSHOOT_PLASMATOWER,
    TURRETSHOOT_ENERGYSTOMTURRET,
    TURRETSHOOT_PLASMATURRET,

}

[System.Serializable]
public class ObjectPrefabsInfo
{
    public ObjectType prefabType;
    public GameObject prefab;
}

[System.Serializable]
public class EnemyPrefabInfo
{
    public int enemyKey;
    public GameObject prefab;
}

[System.Serializable]
public class ParticlePrefabInfo
{
    public int particleKey;
    public GameObject prefab;
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("기본 오브젝트 풀링용 프리팹 리스트")]
    public List<ObjectPrefabsInfo> prefabs;

    private Dictionary<ObjectType, Queue<GameObject>> pools = new Dictionary<ObjectType, Queue<GameObject>>();

    [Header("Enemy 전용 프리팹 리스트")]
    public List<EnemyPrefabInfo> enemyPrefabs;

    [Header("폭발 파티클 전용 리스트")]
    public List<ParticlePrefabInfo> particlePrefabs;
    [Header("데미지 프리팹")]
    public GameObject damagePrefab;

    private Dictionary<int, Queue<GameObject>> enemyPools = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, Queue<GameObject>> particlePools = new Dictionary<int, Queue<GameObject>>();
    private Queue<GameObject> damagePool = new Queue<GameObject>();

    /// <summary>
    /// 풀 매니저 초기화
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // 기본 오브젝트 타입별 큐 초기화
        foreach (var prefab in prefabs)
        {
            var info = prefab;

            if (!pools.ContainsKey(info.prefabType))
            {
                pools[info.prefabType] = new Queue<GameObject>();
            }
        }

        foreach (var prefab in particlePrefabs)
        {
            var info = prefab;

            if (!particlePools.ContainsKey(info.particleKey))
            {
                particlePools[info.particleKey] = new Queue<GameObject>();
            }
        }
        damagePool = new Queue<GameObject>();
    }

    /// <summary>
    /// 적 오브젝트 풀 초기화
    /// (List로 전달받은 프리팹을 기반으로 enemyKey별 큐 등록)
    /// </summary>
    public void InitializeEnemyPool(List<GameObject> enemyPrefabsList)
    {
        enemyPrefabs.Clear(); // 기존 적 프리팹 리스트 초기화

        foreach (GameObject prefab in enemyPrefabsList)
        {
            if (int.TryParse(prefab.name, out int key))
            {
                // enemyKey에 해당하는 큐가 없다면 새로 생성
                if (!enemyPools.ContainsKey(key))
                {
                    enemyPools[key] = new Queue<GameObject>();
                }

                // enemyPrefabs 리스트에도 등록
                if (!enemyPrefabs.Exists(e => e.enemyKey == key))
                {
                    enemyPrefabs.Add(new EnemyPrefabInfo
                    {
                        enemyKey = key,
                        prefab = prefab
                    });
                }
            }
        }
    }

    /// <summary>
    /// 기본 오브젝트 가져오기 (EXP, Gold 등)
    /// </summary>
    public GameObject GetObject(ObjectType type, Vector3 position, Quaternion rotation, Transform transform = null)
    {
        transform = transform == null ? gameObject.transform : transform;

        if (!pools.ContainsKey(type))
        {
            Debug.LogError($"타입 {type}에 대한 풀이 존재하지 않습니다.");
            return null;
        }

        GameObject obj;
        if (pools[type].Count > 0)
        {
            obj = pools[type].Dequeue();
        }
        else
        {
            GameObject prefab = GetPrefabByType(type);
            if (prefab == null)
            {
                Debug.LogError($"{type} 타입에 해당하는 프리팹을 찾을 수 없습니다.");
                return null;
            }

            obj = Instantiate(prefab, position, rotation, transform);
            obj.GetComponent<IPoolable>()?.Initialize(o => ReturnObject(type, o));
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        obj.GetComponent<IPoolable>()?.OnSpawn();
        return obj;
    }

    /// <summary>
    /// Enemy 오브젝트 가져오기
    /// </summary>
    public GameObject GetEnemyObject(int enemyKey, Vector3 position, Quaternion rotation)
    {
        if (!enemyPools.ContainsKey(enemyKey))
        {
            enemyPools[enemyKey] = new Queue<GameObject>();
        }

        GameObject obj;

        if (enemyPools[enemyKey].Count > 0)
        {
            obj = enemyPools[enemyKey].Dequeue();
        }
        else
        {
            GameObject prefab = enemyPrefabs.Find(e => e.enemyKey == enemyKey)?.prefab;
            if (prefab == null)
            {
                Debug.LogError($"EnemyKey {enemyKey}에 해당하는 프리팹을 찾을 수 없습니다.");
                return null;
            }

            obj = Instantiate(prefab, position, rotation, this.transform);
            obj.GetComponent<IPoolable>()?.Initialize(o => ReturnEnemyObject(enemyKey, o));
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        obj.GetComponent<IPoolable>()?.OnSpawn();
        return obj;
    }

    public GameObject GetParticle(int particleKey, Vector3 position, Quaternion rotation)
    {
        if (!particlePools.ContainsKey(particleKey))
        {
            Debug.LogError($"타입 {particleKey}에 대한 풀이 존재하지 않습니다.");
            return null;
        }

        GameObject obj;
        if (particlePools[particleKey].Count > 0)
        {
            obj = particlePools[particleKey].Dequeue();
        }
        else
        {
            GameObject prefab = particlePrefabs.Find(e => e.particleKey == particleKey)?.prefab;
            if (prefab == null)
            {
                Debug.LogError($"{particleKey} 타입에 해당하는 프리팹을 찾을 수 없습니다.");
                return null;
            }

            obj = Instantiate(prefab, position, rotation, transform);
            obj.GetComponent<IPoolable>()?.Initialize(o => ReturnParticleObject(particleKey, o));
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        obj.GetComponent<IPoolable>()?.OnSpawn();
        return obj;
    }
    /// <summary>
    // 데미지 오브젝트 초기화
    /// <summary>
    public void InitDamagePool()
    {
        damagePool = new Queue<GameObject>();
        ExpandDamage();
    }
    /// <summary>
    // 데미지 오브젝트 반환
    /// <summary>
    public GameObject GetDamage()
    {
        if (damagePool.Count == 0)
        {
            ExpandDamage();
        }
        return damagePool.Dequeue();
    }
    /// <summary>
    // 데미지 오브젝트 추가
    /// <summary>
    public void ExpandDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject dt = Instantiate(damagePrefab, transform);
            dt.SetActive(false);
            damagePool.Enqueue(dt);
        }
    }
    /// <summary>
    // 데미지 설정 및 출력
    /// <summary>
    public void ShowDamage(string text, Transform target, Vector3 offset, bool isCritical)
    {
        try
        {
            GameObject obj = GetDamage();
            DamageText dt = obj.GetComponent<DamageText>();
            dt.gameObject.SetActive(true);
            dt.Set(text, target, offset, isCritical);
        }
        catch (Exception e)
        {
                Debug.LogError("데미지 프리팹 오류 (재시도 실패): " + e.Message);
        }
    }
    /// <summary>
    /// 사용 완료된 particle 오브젝트 반환
    /// </summary>
    public void ReturnParticleObject(int particleKey, GameObject obj)
    {
        if (!particlePools.ContainsKey(particleKey))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        particlePools[particleKey].Enqueue(obj);
    }


    /// <summary>
    /// 사용 완료된 Enemy 오브젝트 반환
    /// </summary>
    public void ReturnEnemyObject(int enemyKey, GameObject obj)
    {
        if (!enemyPools.ContainsKey(enemyKey))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        enemyPools[enemyKey].Enqueue(obj);
    }

    /// <summary>
    /// 사용 완료된 기본 오브젝트 반환
    /// </summary>
    public void ReturnObject(ObjectType type, GameObject obj)
    {
        if (!pools.ContainsKey(type))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[type].Enqueue(obj);
    }
    /// <summary>
    /// 사용 완료된 데미지 오브젝트 반환
    /// </summary>
    public void ReturnDamage(GameObject obj)
    {
        obj.SetActive(false);
        damagePool.Enqueue(obj);
    }

    /// <summary>
    /// ObjectType에 해당하는 프리팹 반환
    /// </summary>
    private GameObject GetPrefabByType(ObjectType type)
    {
        foreach (var prefab in prefabs)
        {
            var info = prefab;
            if (info != null && info.prefabType == type)
                return prefab.prefab;
        }
        return null;
    }

    /// <summary>
    /// 풀 내부에서 null 오브젝트를 제거합니다.
    /// (씬 전환 등으로 유실된 오브젝트 대응)
    /// </summary>
    public void RemoveNullObjectsInPool()
    {
        DestroyAllChildren();

        int removedCount = 0;

        foreach (var type in pools.Keys.ToList())
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();

            foreach (var obj in pools[type])
            {
                if (obj != null)
                    newQueue.Enqueue(obj);
                else
                    removedCount++;
            }

            pools[type] = newQueue;
        }

        foreach (var key in enemyPools.Keys.ToList())
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();

            foreach (var obj in enemyPools[key])
            {
                if (obj != null)
                    newQueue.Enqueue(obj);
                else
                    removedCount++;
            }

            enemyPools[key] = newQueue;
        }

        foreach (var key in particlePools.Keys.ToList())
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();

            foreach (var obj in particlePools[key])
            {
                if (obj != null)
                    newQueue.Enqueue(obj);
                else
                    removedCount++;
            }

            particlePools[key] = newQueue;
        }
    }

    /// <summary>
    /// BattleScene 진입 시 필요한 오브젝트들을 미리 생성합니다.
    /// </summary>
    public void PreloadBattleSceneObjects()
    {
        PreloadFixedObject(ObjectType.EXP, 10);
        PreloadFixedObject(ObjectType.Gold, 10);
    }

    /// <summary>
    /// 특정 ObjectType 오브젝트를 지정된 수량만큼 미리 생성합니다.
    /// </summary>
    private void PreloadFixedObject(ObjectType type, int count)
    {
        if (!pools.ContainsKey(type))
        {
            pools[type] = new Queue<GameObject>();
        }

        GameObject prefab = GetPrefabByType(type);
        if (prefab == null)
        {
            Debug.LogError($"[ObjectPoolManager] {type} 타입 프리팹이 존재하지 않습니다.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);

            obj.transform.SetParent(this.transform);

            obj.SetActive(false);
            obj.GetComponent<IPoolable>()?.Initialize(o => ReturnObject(type, o));
            pools[type].Enqueue(obj);
        }
    }

    /// <summary>
    /// 스테이지에 등장할 Enemy를 사전 생성합니다.
    /// </summary>
    public void PreloadStageEnemies(List<int> enemy)
    {
        foreach (int key in enemy)
        {
            if (!enemyPools.ContainsKey(key))
            {
                enemyPools[key] = new Queue<GameObject>();
            }

            GameObject prefab = enemyPrefabs.Find(e => e.enemyKey == key)?.prefab;
            if (prefab == null)
            {
                Debug.LogWarning($"[ObjectPoolManager] enemyKey {key}에 해당하는 프리팹이 존재하지 않습니다.");
                continue;
            }

            int currentCount = enemyPools[key].Count;
            int createCount = 10 - currentCount;

            for (int i = 0; i < createCount; i++)
            {
                GameObject obj = Instantiate(prefab);

                obj.transform.SetParent(this.transform);

                obj.SetActive(false);
                obj.GetComponent<IPoolable>()?.Initialize(o => ReturnEnemyObject(key, o));
                enemyPools[key].Enqueue(obj);
            }
        }
    }

    /// <summary>
    /// 현재 ObjectPoolManager의 자식 오브젝트를 모두 삭제합니다.
    /// </summary>
    public void DestroyAllChildren()
    {
        int count = transform.childCount;

        for (int i = count - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public List<EnemyState> GetActiveEnemyObjects()
    {
        List<EnemyState> activeEnemies = new List<EnemyState>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.GetComponent<EnemyState>() != null)
            {
                activeEnemies.Add(child.GetComponent<EnemyState>());
            }
        }

        return activeEnemies;
    }
}
