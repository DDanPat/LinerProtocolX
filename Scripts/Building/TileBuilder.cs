using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
public class TileBuilder : MonoBehaviour
{
    [SerializeField] private GameObject tileViewerPrefab; // 타일 프리팹
    [SerializeField] private GameObject tileViewera; // 타일 표시
    [HideInInspector] public GameObject hq;
    public StageThemeTable currentTheme;
    private StageInfoTable stageSheet;
    private Vector3 startPos;
    private Vector2Int startIndex;
    private float stepSize;
    private GameObject plane;
    private NavMeshSurface navSurface;
    private Dictionary<int, StageThemeTable> stageTheme;
    void Awake()
    {
        navSurface = GetComponent<NavMeshSurface>();
        TileManager.Instance.RegisterBuilder(this);
        stageTheme = new();
        stageTheme = GameManager.Instance.DataManager.StageThemeTableLoader.ItemsDict;
    }
    public void SetUp(StageInfoTable stageSheet)
    {
        this.stageSheet = stageSheet;
        // 이전 오브젝트가 있다면 삭제
        if (plane != null)
            Destroy(plane);
        if (hq != null)
            Destroy(hq);
        // 자식 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var startPos = TileManager.Instance.GetStartPos();
        startIndex = TileManager.Instance.GetGridIndex(startPos);
        stepSize = TileManager.Instance.GetStepSize();

        // 테마 찾기
        currentTheme = stageTheme[stageSheet.theme];

        // 바닥 생성
        plane = Instantiate(Resources.Load<GameObject>(GetPath() + "/" + currentTheme.floor));
        plane.transform.parent = transform;
        plane.transform.position = new Vector3(0, -0.6f, 0);
        plane.transform.localScale = new Vector3(20, 1, 20);

        // 타일 뷰 생성, 크기 변경
        if (tileViewera == null)
        {
            tileViewera = Instantiate(tileViewerPrefab);
            tileViewera.transform.localScale = new Vector3(stageSheet.width, 0.2f, stageSheet.length) * stepSize;
            tileViewera.SetActive(false);
        }


        // 경계 생성
        SpawnBorderWalls(new Vector2Int(stageSheet.borderLeft[0], stageSheet.borderLeft[1]), stageSheet.length);
        SpawnBorderWalls(new Vector2Int(stageSheet.borderRight[0], stageSheet.borderRight[1]), stageSheet.length);


        // 장애물 생성
        MakeObstacle();

        // 기지 생성
        GameObject Base = Resources.Load<GameObject>(GetPath() + "/" + currentTheme.playerBase);
        hq = Instantiate(Base);
        int[] pos = stageSheet.playerBase.ToArray();
        SetPosition(hq, pos[0], pos[1], pos[0] + 1, pos[1] + 1, 0.3f);

        // 메시 베이크
        navSurface.BuildNavMesh();
    }

    // 경계 벽 생성
    void SpawnBorderWalls(Vector2Int startPos, int stageLength)
    {
        TileArea area = new TileArea(startPos.x, startPos.y, startPos.x + 3, startPos.y + 3);
        int count = 1 + stageLength / 4;

        for (int i = 0; i < count; i++)
        {
            string randomBorder = currentTheme.border[Random.Range(0, currentTheme.border.Count)];
            GameObject wall = Resources.Load<GameObject>(GetPath() + "/" + randomBorder);
            var obj = Instantiate(wall, transform);
            SetPosition(obj, area, 0.1f);
            area.MoveUp(4);
        }
    }

    // 장애물 생성
    private void MakeObstacle()
    {
        // 리플렉션
        for (int i = 0; i <= 21; i++)
        {
            GameObject obstacle = Resources.Load<GameObject>(GetPath() + "/" + currentTheme.obstacle[Random.Range(0, currentTheme.obstacle.Count)]);
            var field = typeof(StageInfoTable).GetField($"obstacle{i}");
            if (field != null)
            {
                List<int> pos = field.GetValue(stageSheet) as List<int>;
                // 0, -1은 무시
                if (pos[0] == -1 || pos[0] == 0 || pos[1] == 0 || pos[1] == -1) continue; // -1은 없음
                GameObject ob = Instantiate(obstacle, transform);
                SetPosition(ob, pos[0], pos[1], pos[0] + 1, pos[1] + 1, 0f);
                TileManager.Instance.FillTile(pos[0], pos[1], pos[0] + 1, pos[1] + 1, 2);
                if (ob.TryGetComponent<BaseUnit>(out var unit))
                {
                    unit.start = new Vector2Int(pos[0], pos[1]);
                    unit.end = new Vector2Int(pos[0] + 1, pos[1] + 1);
                }
            }
        }
    }
    private void SetPosition(GameObject obj, int startX, int startY, int endX, int endY, float posY)
    {

        int width = 1 + endX - startX;
        int height = 1 + endY - startY;
        int row = (int)Mathf.Floor((endX + startX) / 2f) + 1;
        int col = (int)Mathf.Floor((endY + startY) / 2f) + ((height % 2 == 0) ? 1 : 0);
        // 위치 변경
        obj.transform.localPosition = TileManager.Instance.GetPosByGrid(row, col, width, height) + Vector3.up * posY;
        //obj.transform.localScale = new Vector3(width * scale, 1, height * scale);
    }
    private void SetPosition(GameObject obj, TileArea tileArea, float posY)
    {

        int width = 1 + tileArea.endPos.x - tileArea.startPos.x;
        int height = 1 + tileArea.endPos.y - tileArea.startPos.y;
        int row = (int)Mathf.Floor((tileArea.endPos.x + tileArea.startPos.x) / 2f) + 1;
        int col = (int)Mathf.Floor((tileArea.endPos.y + tileArea.startPos.y) / 2f) + ((height % 2 == 0) ? 1 : 0);
        // 위치 변경
        obj.transform.localPosition = TileManager.Instance.GetPosByGrid(row, col, width, height) + Vector3.up * posY;
        //obj.transform.localScale = new Vector3(width * scale, 1, height * scale);
    }
    // 리소스 주소 반환
    public string GetPath()
    {
        return Define._EnviromentRoot + "/" + currentTheme.name;
    }
    public GameObject GetTileViewer()
    {
        return tileViewera;
    }
}
