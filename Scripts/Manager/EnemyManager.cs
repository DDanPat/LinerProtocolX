using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private Dictionary<int, EnemyTable> EnemyDataDict = new Dictionary<int, EnemyTable>();
    [SerializeField] private List<GameObject> enemyModlePrefabs = new List<GameObject>(); // 적 오브젝트 모델 저장 리스트
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>(); // 일반 몬스터 리스트
    [SerializeField] private List<GameObject> bossEnemyPrefabs = new List<GameObject>(); // 보스 몬스터 리스트

    public Sprite[] enemyIcons;
    public Dictionary<int, Sprite> enemyIconDict = new Dictionary<int, Sprite>();


    [SerializeField] private Rect spawnAreas; // 적을 생성할 영역

    public int waveNum = 0; // 스테이지 웨이브
    private int waveEnemyCount; // 웨이브당 소환할 적 수

    ObjectPoolManager objectPoolManager;
    protected override void Awake()
    {
        base.Awake();
        objectPoolManager = ObjectPoolManager.Instance;
        EnemyDataDict = GameManager.Instance.DataManager.EnemyTableLoader.EnemyDict;

        LoadEnemyPrefabs();
        objectPoolManager.InitializeEnemyPool(enemyModlePrefabs);
        SetEnemyIconDict();
    }
    // enemyModlePrefabs를 Resources에서 불러오기, 프리팹 참조만 저장 (인스턴스화 X)
    private void LoadEnemyPrefabs()
    {
        enemyModlePrefabs.Clear();

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Enemies");

        foreach (var prefab in loadedPrefabs)
        {
            if (prefab == null) continue;

            if (int.TryParse(prefab.name, out int key) && EnemyDataDict.ContainsKey(key))
            {
                enemyModlePrefabs.Add(prefab); // 인스턴스화 하지 않음
            }
        }
    }

    //  enemyPrefabs에 프리팹 참조만 보관
    public void GetEnemyData(List<int> keyList)
    {
        enemyPrefabs.Clear();
        bossEnemyPrefabs.Clear();

        foreach (int key in keyList)
        {
            GameObject found = enemyModlePrefabs.Find(p => p.name == key.ToString());
            EnemyType enemyType = found.GetComponent<EnemyState>().enemyType;
            if (found != null)
            {
                switch (enemyType)
                {
                    case EnemyType.None:
                        enemyPrefabs.Add(found);
                        break;

                    case EnemyType.Boss:
                        bossEnemyPrefabs.Add(found);
                        break;

                    default:
                        Debug.LogWarning($"[EnemyManager] 알 수 없는 EnemyType: {enemyType}, key: {key}");
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"[EnemyManager] key: {key}에 해당하는 프리팹 또는 EnemyData가 없습니다.");
            }
        }
    }

    // 15초 동안 waveEnemyCount수 만큼 균일한 속도로 일반 몬스터 소환
    public IEnumerator SpownEnemyCoroutine(int curWave)
    {
        waveEnemyCount = Mathf.FloorToInt(15 * (0.03f * (curWave - 1) + Mathf.Pow(1.05f, (curWave - 1))));
        // 몬스터 증가, 감소 배율
        waveEnemyCount *= Mathf.FloorToInt(1 + (BattleManager.Instance.GetValue(6505) + BattleManager.Instance.GetValue(6506)) / 100f);
        float spwonDelay = 15 / (float)waveEnemyCount;

        for (int i = 0; i < waveEnemyCount; i++)
        {
            SpawnRandomEnemy();

            yield return new WaitForSeconds(spwonDelay);
        }
    }


    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs 또는 Spawn Areas가 설정되지 않았습니다.");
            return;
        }

        // 랜덤한 적 프리팹 선택
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        SpawnPoint(randomPrefab);
    }

    public void SpawnRandomBossEnemy()
    {
        if (bossEnemyPrefabs.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs 또는 Spawn Areas가 설정되지 않았습니다.");
            return;
        }

        // 랜덤한 적 프리팹 선택
        GameObject randomPrefab = bossEnemyPrefabs[Random.Range(0, bossEnemyPrefabs.Count)];

        SpawnPoint(randomPrefab);
    }

    private void SpawnPoint(GameObject randomPrefab)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(spawnAreas.xMin + 0.5f, spawnAreas.xMax - 0.5f),
            0,
            Random.Range(spawnAreas.yMin + 0.5f, spawnAreas.yMax - 0.5f));

        Transform targetTransform = TileManager.Instance.headquarterTransform;

        Vector3 direction = (targetTransform.position - randomPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Y축 회전만

        int enemyKey = int.Parse(randomPrefab.name);
        GameObject enemy = objectPoolManager.GetEnemyObject(enemyKey, new Vector3(randomPosition.x, 1, randomPosition.z), lookRotation);


        enemy.GetComponent<EnemyState>().SetStatus(EnemyDataDict[enemyKey]);
        enemy.GetComponent<EnemyState>().SetStageStatus(BattleManager.Instance.curStage);
        enemy.GetComponent<EnemyState>().StatusUP(BattleManager.Instance.CurrentBattle.curWave);
    }

    public void SetSpawnArea(float posX, float posY, int width, int height)
    {
        spawnAreas.x = posX;
        spawnAreas.y = posY;
        spawnAreas.width = width;
        spawnAreas.height = height;
    }

    // 기즈모를 그려 영역을 시각화 (선택된 경우에만 표시)
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;
        Color gizmoColor = new Color(1, 0, 0, 0.3f); // 기즈모 색상
        Gizmos.color = gizmoColor;
        Vector3 center = new Vector3(spawnAreas.x + spawnAreas.width / 2, 0.5f, spawnAreas.y + spawnAreas.height / 2);
        Vector3 size = new Vector3(spawnAreas.width, 0, spawnAreas.height);
        Gizmos.DrawCube(center, size);
    }


    public void SetEnemyIconDict()
    {
        foreach (var sprite in enemyIcons)
        {
            if (sprite == null) continue;

            // Sprite 이름을 int로 파싱 시도
            if (int.TryParse(sprite.name, out int key))
            {
                if (!enemyIconDict.ContainsKey(key))
                {
                    enemyIconDict.Add(key, sprite);
                }
                else
                {
                    Debug.LogWarning($"Duplicate keyList found: {key}");
                }
            }
            else
            {
                Debug.LogWarning($"Sprite name is not a valid int: {sprite.name}");
            }
        }
    }
    public EnemyTable GetEnemyData(int key)
    {
        if (EnemyDataDict.ContainsKey(key))
        {
            return EnemyDataDict[key];
        }
        return null;
    }
}
