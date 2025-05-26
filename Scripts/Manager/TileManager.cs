using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 장애물 영역
[Serializable]
public class TileArea
{
    public Vector2Int startPos;
    public Vector2Int endPos;
    public TileArea(Vector2Int startPos, Vector2Int endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos;
    }
    public TileArea(int startX, int startY, int endX, int endY)
    {
        startPos = new Vector2Int(startX, startY);
        endPos = new Vector2Int(endX, endY);
    }
    public TileArea(List<int> numbers)
    {
        startPos = new Vector2Int(numbers[0], numbers[1]);
        endPos = new Vector2Int(numbers[2], numbers[3]);
    }
    public void MoveUp(int count)
    {
        startPos.y += count;
        endPos.y += count;
    }
    public void MoveRight(int count)
    {
        startPos.x += count;
        endPos.x += count;
    }
}
public class TileManager : Singleton<TileManager>
{
    private bool Initialized = false;
    [Header("Map Info")]
    public Transform headquarterTransform; // 기지 위치
    [SerializeField] private Vector3 startPos; // 좌하단 기준 좌표
    [SerializeField] private Vector2Int tileSize = new Vector2Int(9, 5); // 타일 총 칸
    [SerializeField] private List<TileArea> preBlocker; // 경계영역
    [SerializeField] private List<TileArea> obstacle; // 장애물 영역
    [SerializeField] private List<TileArea> notBuild; // 배치불가 영역
    [SerializeField] private TileArea spawnArea; // 적 스폰 위치
    [SerializeField] private Vector2Int headquarter; // 기지
    [SerializeField] private float stepSize = 0.5f;

    [SerializeField] private List<Tower> tileInfo; // 타일위에 유닛 정보
    private int[,] tileMap; // 타일 위에 유닛이 있는지 여부, 0 빈 공간, 1 놓을 수 없으나 지나갈 수 있음, 2, 벽
    //private StageInfoTable stageSheet;
    public TileBuilder tileBuilder;
    protected override void Awake()
    {
        base.Awake();
        transform.position = Vector3.zero;
        if (!Initialized)
            Init();
    }
    private void Init()
    {
        tileInfo = new();
        tileMap = new int[tileSize.x, tileSize.y];
        preBlocker = new();
        obstacle = new();
        notBuild = new();
        Initialized = true;
    }

    // 스테이지 정보 불러오기 & 적용
    public void LoadData(StageInfoTable stageSheet)
    {
        if (!Initialized)
            Init();

        // 정보 설정
        tileSize.x = stageSheet.width;
        tileSize.y = stageSheet.length;

        // 재초기화
        Init();

        // 정보 불러오기
        List<int> borderLeft = stageSheet.borderLeft;
        List<int> borderRight = stageSheet.borderRight;
        List<int> borderBottom = stageSheet.borderBottom;
        List<int> unplaceTop = stageSheet.unplaceableTop;
        List<int> unplaceBottom = stageSheet.unplaceableBottom;
        List<int> hq = stageSheet.playerBase;
        List<int> spawn = stageSheet.spawn;

        // 시작점 설정
        startPos.x = -(tileSize.x / 2 * stepSize - (tileSize.x % 2 == 0 ? stepSize / 2 : 0));
        startPos.z = -(tileSize.y / 2 * stepSize - (tileSize.y % 2 == 0 ? stepSize / 2 : 0));


        // 경계구역 설정
        preBlocker.Add(new TileArea(borderLeft[0], borderLeft[1], borderLeft[2], borderLeft[3]));
        preBlocker.Add(new TileArea(borderRight[0], borderRight[1], borderRight[2], borderRight[3]));
        preBlocker.Add(new TileArea(borderBottom[0], borderBottom[1], borderBottom[2], borderBottom[3]));

        // 배치 불가 지역 설정
        notBuild.Add(new TileArea(unplaceTop[0], unplaceTop[1], unplaceTop[2], unplaceTop[3]));
        notBuild.Add(new TileArea(unplaceBottom[0], unplaceBottom[1], unplaceBottom[2], unplaceBottom[3]));

        // 장애물 설정
        for (int i = 0; i <= 21; i++)
        {
            var field = typeof(StageInfoTable).GetField($"obstacle{i}");
            if (field != null)
            {
                List<int> pos = field.GetValue(stageSheet) as List<int>;
                if (pos.Count >= 2)
                {
                    obstacle.Add(new TileArea(pos[0], pos[1], pos[0] + 1, pos[1] + 1));
                }
            }
        }
        // 스폰 지역 설정
        spawnArea = new TileArea(spawn[0], spawn[1], spawn[2], spawn[3]);
        Vector3 spawnStart = GetPosByGrid(spawn[0], spawn[1]) + new Vector3(-stepSize / 2, 0, -stepSize / 2);
        int spawnWidth = Mathf.FloorToInt((spawn[2] - spawn[0] + 1) * stepSize);
        int spawnHeight = Mathf.FloorToInt((spawn[3] - spawn[1] + 1) * stepSize);
        EnemyManager.Instance.SetSpawnArea(spawnStart.x, spawnStart.z, spawnWidth, spawnHeight);

        // 기지
        headquarter = new Vector2Int(hq[0], hq[1]);

        // 오브젝트 생성
        if (tileBuilder != null)
        {
            tileBuilder.SetUp(stageSheet);
            headquarterTransform = tileBuilder.hq.transform;
        }
        // 영역 채우기
        SpawnPreview();
    }
    // 타일 생성기 등록
    public void RegisterBuilder(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }

    // 좌표로 가장 가까운 중심점 인덱스 반환
    public Vector2Int GetGridIndex(Vector3 pos)
    {
        // 가장 가까운 중심점 계산
        // 좌표별 차이
        float disX = pos.x - startPos.x;
        float disZ = pos.z - startPos.z;

        // 0.5이상은 올리고 미만은 내림, 간격 추가
        int row = (int)Mathf.Floor(disX / stepSize + 0.5f);
        int col = (int)Mathf.Floor(disZ / stepSize + 0.5f);

        return new Vector2Int(row, col);
    }

    // 인덱스로 월드 좌표 반환
    public Vector3 GetPosByGrid(int row, int col, int width = 1, int height = 1)
    {
        Vector3 pos = new Vector3();
        // 범위 오버시 최소, 최대 적용
        //row = Mathf.Clamp(row, 0, tileSize.x - 1);
        //col = Mathf.Clamp(col, 0, tileSize.y - 1);

        pos.x = startPos.x + stepSize * row;
        pos.z = startPos.z + stepSize * col;

        // 폭과 높이에 따라 중심 좌표 보정
        if (width % 2 == 0)
        {
            pos.x -= 0.25f;
        }
        if (height % 2 == 0)
        {
            pos.z -= 0.25f;
        }
        return pos;
    }

    public int IsOnUnit(int indexX, int indexY)
    {
        if (indexX < 0 || indexX >= tileSize.x) return -1;
        if (indexY < 0 || indexY >= tileSize.y) return -1;
        // 소환 가능한 칸인지 판별
        if (tileMap[indexX, indexY] == 0)
        {
            return tileMap[indexX, indexY];
        }
        return -1;
    }

    // 사전 장애물 배치
    public void SpawnPreview()
    {
        // 경계 영역 배치
        for (int i = 0; i < preBlocker.Count; i++)
        {
            FillTile(preBlocker[i].startPos, preBlocker[i].endPos, 2);
        }
        // 장애물 배치
        for (int i = 0; i < obstacle.Count; i++)
        {
            FillTile(obstacle[i].startPos, obstacle[i].endPos, 2);
        }
        // 배치 불가능 영역
        for (int i = 0; i < notBuild.Count; i++)
        {
            FillTile(notBuild[i].startPos, notBuild[i].endPos, 1);
        }
        // 적 스폰 영역 영역
        FillTile(spawnArea.startPos, spawnArea.endPos, 1);

        // 기지 영역
        FillTile(headquarter, headquarter + new Vector2Int(1, 1), 1);
        float hqX = (headquarter.x + 1) / 2f;
        float hqZ = (headquarter.y + 1) / 2f;
    }

    // 타워 리스트에 추가
    public void AddUnit(Tower unit)
    {
        tileInfo.Add(unit);
    }

    // 타워 리스트에 제거
    public void Remove(Tower unit)
    {
        tileInfo.Remove(unit);
        unit.Remove();
    }

    // 유닛 정보 채우기, true면 1(있음), false면 0(비어있음)
    public void FillTile(Vector2Int start, Vector2Int end, int fill)
    {
        if (!Initialized)
            Init();
        for (int row = start.x; row <= end.x; row++)
        {
            for (int col = start.y; col <= end.y; col++)
            {
                tileMap[row, col] = fill;
            }
        }
    }
    public void FillTile(int startX, int startY, int endX, int endY, int fill)
    {
        if (!Initialized)
            Init();
        for (int row = startX; row <= endX; row++)
        {
            for (int col = startY; col <= endY; col++)
            {
                tileMap[row, col] = fill;
            }
        }
    }

    // 타일 정보 지우기
    public void ClearTile(Vector2Int start, Vector2Int end)
    {
        for (int row = start.x; row <= end.x; row++)
        {
            for (int col = start.y; col <= end.y; col++)
            {
                tileMap[row, col] = 0;
            }
        }
    }


    // 특정 위치에 놓을 수 있는지 여부 = 길막힘 판별
    public bool CanPut(Vector2Int start, Vector2Int end)
    {
        if (!Initialized)
            Init();

        // 범위 검사
        if (start.x < 0 || end.x >= tileMap.GetLength(0)) return false;
        if (start.y < 0 || end.y >= tileMap.GetLength(1)) return false;
        // 목표 가로, 최소 1칸
        int width = end.x - start.x + 1;

        // 시작점의 처음행부터 마지막점의 행까지 탐색
        for (int col = start.y; col <= end.y; col++)
        {
            int tempWidth = 0; // 현재 가로
            // 연속된 빈 공간 탐색
            for (int row = start.x; row <= end.x; row++)
            {
                int cellValue = tileMap[row, col];
                if (cellValue == 0)
                    tempWidth++;
                else
                {
                    // 데이터 있음 = 연속 되지 않으면 초기화
                    tempWidth = 0;
                }
            }
            // 한 행 탐색 종료시 현재값과 목표비교
            if (width > tempWidth)
            {
                return false;
            }
        }

        // 길 찾기 판단 -> 실패시 지우고, false
        // 임시로 벽으로 채움
        FillTile(start, end, 2);
        if (TileCheck())
        {
            // 길이 막히지 않으면 true
            ClearTile(start, end);
            return true;
        }
        else
        {
            // 길이 막히면 임시벽을 지우고 false
            ClearTile(start, end);
            return false;
        }
    }
    public Vector3 GetStartPos()
    {
        return startPos;
    }
    public float GetStepSize()
    {
        return stepSize;
    }

    // 오퍼레이터 키로 타워 위치 리스트 반환
    public List<Vector3> GetTowerPositions(int operatorKey)
    {
        List<Vector3> posList = new List<Vector3>();
        foreach (var tower in tileInfo)
        {
            if (tower.operatorData == null)
                continue;
            if (tower.operatorData.operatorInfoTable.key == operatorKey)
                posList.Add(tower.gameObject.transform.position);
        }
        return posList;
    }
    // 오퍼레이터 관리 타워 전체에 버프
    public void SetTowerBuff(int operatorKey, ITowerBuff towerBuff)
    {
        foreach (var tower in tileInfo)
        {
            if (tower.operatorData == null)
                continue;
            if (tower.operatorData.operatorInfoTable.key == operatorKey)
            {
                tower.ExecuteBuff(towerBuff);
            }
        }
    }
    // 전체 타워에 버프 적용
    public void SetTowerBuff(ITowerBuff towerBuff)
    {
        foreach (var tower in tileInfo)
        {
            tower.ExecuteBuff(towerBuff);
        }
    }
    // 특정 오퍼레이터를 가진 타워의 사거리 내부의 적을 반환
    public List<EnemyState> GetNeighborEnemy(int operatorKey, float radius, LayerMask targetLayer)
    {
        HashSet<Collider> colliders = new();
        List<Vector3> positions = GetTowerPositions(operatorKey);
        List<EnemyState> enemies = new();

        // 타워별 충돌체 구하기
        foreach (var towerPos in positions)
        {
            Vector3 point0 = towerPos;
            Vector3 point1 = towerPos + Vector3.up * 3f;

            Collider[] cols = Physics.OverlapCapsule(point0, point1, radius, targetLayer);
            foreach (var col in cols)
            {
                colliders.Add(col); // 중복 자동 제거
            }
        }

        // 충돌체별로 적 컴포넌트 가져오기
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out EnemyState enemy))
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }
    // 특정 오퍼레이터를 가진 타워의 사거리 내부 타워를 반환
    public List<Tower> GetNeighborTower(int operatorKey, float radius, LayerMask targetLayer)
    {
        HashSet<Collider> colliders = new();
        List<Vector3> positions = GetTowerPositions(operatorKey);
        List<Tower> towers = new();

        // 타워별 충돌체 구하기
        foreach (var towerPos in positions)
        {
            Vector3 point0 = towerPos;
            Vector3 point1 = towerPos + Vector3.up * 3f;

            Collider[] cols = Physics.OverlapCapsule(point0, point1, radius, targetLayer);
            foreach (var col in cols)
            {
                colliders.Add(col); // 중복 자동 제거
            }
        }

        // 충돌체별로 적 컴포넌트 가져오기
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Tower enemy))
            {
                towers.Add(enemy);
            }
        }
        return towers;
    }
    // 타워 강화 가능 여부 표시
    public void ShowEnforce(int towerKey, int operatorKey, bool show)
    {
        foreach (var tower in tileInfo)
        {
            if (tower.GetTowerKey() == towerKey && tower.operatorKey == operatorKey && tower.CanUpgrade())
            {
                tower.TryShowEnforce(show);
            }
        }
    }
    // 타워 사거리 표시
    public void ShowRange(int towerKey, int operatorKey, bool show)
    {
        foreach (var tower in tileInfo)
        {
            if (tower.GetTowerKey() == towerKey && tower.operatorKey == operatorKey)
            {
                tower.ShowRange(show);
            }
        }
    }

    // 타일에 길이 있는지 확인, a* 알고리즘 사용, 대각선 이동 없이, 비용 상관없이 
    public bool TileCheck()
    {
        int row = tileMap.GetLength(0);
        int col = tileMap.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            // 가장 위에서부터 탐색 시작
            Vector2Int startNode = new Vector2Int(i, col - 1);
            if (tileMap[startNode.x, startNode.y] == 2) continue;
            Vector2Int endNode = headquarter;
            if (FindPathAStar(startNode, endNode))
            {
                // 길 찾음
                return true;
            }
            else
            {
                // 길 없음
                continue;
            }
        }
        return false;
    }

    // 경로가 존재하는지 검사, a*
    public bool FindPathAStar(Vector2Int start, Vector2Int end)
    {
        int width = tileMap.GetLength(0);
        int height = tileMap.GetLength(1);

        HashSet<Vector2Int> closeSet = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
        // 출발점
        openSet.Enqueue(start, 0);

        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>
        {
            [start] = 0
        };

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();
            if (current == end)
            {
                // 도착 지점 도착
                return true;
            }

            closeSet.Add(current);

            foreach (var dir in directions)
            {
                // 방향에 따른 탐색 지점
                Vector2Int neighbor = current + dir;

                // 범위 조건 확인
                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height)
                {
                    continue;
                }
                // 벽인지 확인
                if (tileMap[neighbor.x, neighbor.y] == 2 || closeSet.Contains(neighbor))
                {
                    continue;
                }

                // 거리 비교하여 추가
                int tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeG;
                    int fScore = tentativeG + Heuristic(neighbor, end);
                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }
        return false; // 경로 없음
    }
    public static int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    void OnDrawGizmosSelected()
    {
        // 시작 위치에 블록 크기의 구형
        Gizmos.color = Color.red;
        for (int i = 0; i < tileSize.x; i++)
        {
            for (int k = 0; k < tileSize.y; k++)
            {
                Vector3 pointPos = startPos;
                pointPos.x += stepSize * i;
                pointPos.z += stepSize * k;
                Gizmos.DrawWireSphere(pointPos, stepSize / 2);
            }
        }
    }
}
