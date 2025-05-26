using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

// 설정
public enum TILEMODE
{
    IDLE, // 대기
    BUILD, // 타워 생성 및 강화
    REMOVE, // 삭제
    SKILL, // 스킬 
}
// 결과
public enum TILESTATE
{
    NONE,
    BUILD,
    UPGRADE,
    REMOVE,
    SKILL
}
public class TowerFactory : MonoBehaviour
{
    // 외부 요소
    [SerializeField] private GameObject areaPreviewPrefab;
    private Renderer areaPreview;
    [SerializeField] private LayerMask layerMask; // 타워 레이어
    [SerializeField] Image rangeImage; // 사거리 이미지 - world canvas
    [SerializeField] Image shadowImage; // 전체 암영 - world canvas

    private Dictionary<int, GameObject> prefabs; // 임시 프리팹 저장용
    private GameObject currentPrefab; // 임시 프리팹
    private int currentKey;// 현재 타워, 스킬 키
    private int sizeX = 2; // 영역 가로
    private int sizeY = 2; // 영역 세로
    private BaseUnit baseUnit; // 업그레이드 할 타워
    private int operatorKey; // 현재 오퍼레이터 키
    private Collider[] hits; // 삭제시 검사된 타워
    private List<Color> originalColors;
    private Vector3 worldPos; // 보정된 입력 위치
    private TILEMODE mode; // 타워 모드
    private TILESTATE modeState; // 모드 행동
    private Camera _camera; // 입력용 카메라
    private Vector2Int currentIndex = new Vector2Int(); // 입력을 타일로 변경한 좌표

    // Start is called before the first frame update
    void Start()
    {
        prefabs = new();
        hits = new Collider[3];
        _camera = Camera.main;
        areaPreview = Instantiate(areaPreviewPrefab).GetComponent<Renderer>();
        if (areaPreview != null)
        {
            areaPreview.gameObject.SetActive(false);
        }
        if (rangeImage != null)
        {
            rangeImage.gameObject.SetActive(false);
        }
        BattleManager.Instance.CurrentBattle.TowerCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode != TILEMODE.IDLE)
        {

            // orthographic 일 경우
            Vector3 inputPos = new Vector3();
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            inputPos = Input.mousePosition;
#endif
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                inputPos = Input.GetTouch(0).position;
            }
#endif
            Ray ray = _camera.ScreenPointToRay(inputPos);
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));// y = 0 탐색
            if (groundPlane.Raycast(ray, out float distance))
            {
                worldPos = ray.GetPoint(distance);
            }
            else
            {
                worldPos = Vector3.zero;
            }

            // 보정값
            worldPos.z += 1;
            // 평면으로 보정
            worldPos.y = 0;

            // 어느 칸에 들어 있는지 판별
            Vector2Int index = TileManager.Instance.GetGridIndex(worldPos);

            // 같은 좌표면 계산 건너뜀
            if (currentIndex == index) return;
            currentIndex = index;

            // 타일 위치로
            if (mode == TILEMODE.BUILD || mode == TILEMODE.REMOVE)
            {
                // 칸 정보를 월드 좌표로 변환
                worldPos = TileManager.Instance.GetPosByGrid(index.x, index.y, sizeX, sizeY);
            }

            // 사거리 이미지 표시
            rangeImage.rectTransform.anchoredPosition = new Vector2(worldPos.x, worldPos.z);


            // 타워 탐색 - 설치와 삭제 시도중인 경우
            if (mode == TILEMODE.BUILD || mode == TILEMODE.REMOVE)
            {
                // 탐색한 타워 키
                int towerKey = -1;

                // 탐색한 타워의 오퍼레이터 키
                int tmpOperatorKey = -1;

                // 입력 위치기준으로 0.1안에 있는 타워 탐색
                int count = Physics.OverlapSphereNonAlloc(worldPos, 0.1f, hits, layerMask);

                // 이전 타워가 있으면 색 초기화
                if (baseUnit != null)
                {
                    var unitrenders = baseUnit.renderers;

                    int colorIndex = 0;
                    for (int i = 0; i < unitrenders.Count; i++)
                    {
                        var materials = unitrenders[i].materials;

                        for (int j = 0; j < materials.Length; j++)
                        {
                            if (colorIndex < originalColors.Count)
                            {
                                materials[j].color = originalColors[colorIndex];
                                colorIndex++;
                            }
                        }
                    }
                    if (baseUnit.GetComponent<Tower>() != null)
                        baseUnit.SetGradeColor();
                    else

                        baseUnit = null;
                }

                // 탐색 결과
                for (int i = 0; i < count; i++)
                {
                    var hitObject = hits[i].gameObject;
                    // 임시 타워 프리팹 이외의 타워
                    if (currentPrefab != hitObject)
                    {
                        if (hitObject.TryGetComponent<BaseUnit>(out var unit))
                        {
                            baseUnit = unit.GetComponent<BaseUnit>();
                            // 타워인 경우
                            if (baseUnit.TryGetComponent<Tower>(out var tower))
                            {
                                // 타워 정보 가져오기
                                towerKey = tower.GetTowerKey();
                                if (tower.operatorData != null)
                                    tmpOperatorKey = tower.operatorData.operatorInfoTable.key;
                                break;
                            }
                            else
                            {
                                // 장애물
                            }
                        }
                    }
                }
                if (mode == TILEMODE.BUILD)
                {
                    // 배치 판단용 좌표
                    int startX = index.x - sizeX / 2;
                    int startY = index.y - sizeY / 2;
                    Vector2Int start = new Vector2Int(startX, startY);
                    Vector2Int end = new Vector2Int(startX + sizeX - 1, startY + sizeY - 1);

                    // 임시 프리팹 이동
                    if (currentPrefab != null)
                    {
                        currentPrefab.transform.position = worldPos;
                        // 영역 표시
                        float size = TileManager.Instance.GetStepSize() + 0.1f;
                        areaPreview.transform.position = worldPos;
                        areaPreview.transform.localScale = new Vector3(sizeX * size, 0.1f, sizeY * size);
                        areaPreview.gameObject.SetActive(true);
                    }

                    // 유효 범위 확인 및 배치 가능 여부 판단
                    if (TileManager.Instance.CanPut(start, end))
                    {
                        // 정상 범위
                        modeState = TILESTATE.BUILD;
                        baseUnit = null;
                        areaPreview.material.color = new Color(0f, 1f, 0f, 0.7f);
                    }
                    else if (count > 2 && towerKey == currentKey && tmpOperatorKey == operatorKey)
                    {
                        if (baseUnit != null && baseUnit.CanUpgrade())
                        {
                            // 임시 타워와 같은 키를 가지고 오퍼레이터가 같다면
                            modeState = TILESTATE.UPGRADE;
                            // 업그레이드할 타워도 노랑게 표시
                            areaPreview.material.color = new Color(1f, 1f, 0f, 0.5f);
                            // 원본 색상 저장
                            OriginalColorStore();
                            // 대상 타워 색상 변경
                            var unitrenders = baseUnit.renderers;
                            foreach (var unitrender in unitrenders)
                            {
                                unitrender.material.color = new Color(1f, 1f, 0f);
                            }
                        }
                        else
                        {
                            // 강화 불가능
                            modeState = TILESTATE.NONE;
                            baseUnit = null;
                            areaPreview.material.color = new Color(1f, 0f, 0f, 0.5f);
                        }
                    }
                    else if (towerKey == -1)
                    {
                        // 유효하지 않은 위치, 놓을 수 없음
                        modeState = TILESTATE.NONE;
                        baseUnit = null;
                        areaPreview.material.color = new Color(1f, 0f, 0f, 0.5f);
                    }
                    else
                    {
                        // 키가 다른 타워, 놓을 수 없음
                        modeState = TILESTATE.NONE;
                        baseUnit = null;
                        areaPreview.material.color = new Color(1f, 0f, 0f, 0.5f);
                    }
                }
                else if (mode == TILEMODE.REMOVE)
                {
                    // 곡괭이, 타워 삭제
                    if (count > 0)
                    {
                        // 대상 타워가 있는 경우
                        if (baseUnit != null)
                        {
                            modeState = TILESTATE.REMOVE;
                            // 대상 타워 원본 색상 저장
                            OriginalColorStore();
                            // 대상 타워 색상 변경
                            var unitrenders = baseUnit.renderers;
                            foreach (var unitrender in unitrenders)
                            {
                                unitrender.material.color = new Color(1f, 1f, 0f);
                            }
                        }
                    }
                }
            }
        }
    }
    // 카드 사용 시도
    public bool TryUseCard()
    {
        // 영역, 타일, 암영 숨기기
        areaPreview.gameObject.SetActive(false);
        rangeImage.gameObject.SetActive(false);
        shadowImage.gameObject.SetActive(false);
        BattleManager.Instance.BattleUI.SetDeActiveButton();
        TILEMODE prev = mode;
        mode = TILEMODE.IDLE;
        switch (prev)
        {
            case TILEMODE.BUILD:
                // 강화 UI 숨김
                TileManager.Instance.ShowEnforce(currentKey, operatorKey, false);
                TileManager.Instance.ShowRange(currentKey, operatorKey, false);
                if (baseUnit != null && modeState == TILESTATE.UPGRADE)
                {
                    // 타워 랭크 업
                    baseUnit.UpGrade();
                    // 임시 오브젝트 삭제
                    Destroy(currentPrefab);
                    currentPrefab = null;
                    // 대상 타워가 있으면 색 초기화
                    if (baseUnit != null)
                    {
                        var unitrenders = baseUnit.renderers;

                        int colorIndex = 0;
                        for (int i = 0; i < unitrenders.Count; i++)
                        {
                            var materials = unitrenders[i].materials;

                            for (int j = 0; j < materials.Length; j++)
                            {
                                if (colorIndex < originalColors.Count)
                                {
                                    materials[j].color = originalColors[colorIndex];
                                    colorIndex++;
                                }
                            }
                        }
                    }

                    // 업그레이드 색상 반영
                    baseUnit.SetGradeColor();

                    return true;
                }
                else if (currentPrefab == null || modeState == TILESTATE.NONE)
                {
                    // 임시 프리팹이 존재하고 놓을 수 있는지 확인
                    // 놓을 수 없는 상황 - 실패
                    Destroy(currentPrefab);
                    currentPrefab = null;
                    return false;
                }
                else if (modeState == TILESTATE.BUILD)
                {
                    // 타워 생성
                    BuildTower();
                    return true;
                }
                break;

            case TILEMODE.REMOVE:
                // 곡괭이 - 대상 오브젝트가 있다면
                if (baseUnit != null && modeState == TILESTATE.REMOVE)
                {
                    // 대상이 타워이고 삽이면 타워 회수
                    if (baseUnit.TryGetComponent<Tower>(out var tower))
                    {
                        TileManager.Instance.Remove(tower);
                        // 삽이라면 타워 회수
                        if (currentKey == 4902)
                        {
                            TowerOperList towerOperList = new TowerOperList
                            {
                                towerData = tower.towerData,
                                operatorData = tower.operatorData
                            };
                            if (towerOperList.operatorData != null)
                            {
                                BattleManager.Instance.CurrentBattle.uiCardHand.SpawnCard(towerOperList, towerOperList.operatorData.operatorInfoTable.key);
                            }
                            else
                                BattleManager.Instance.CurrentBattle.uiCardHand.SpawnCard(towerOperList);
                        }
                    }
                    // 삭제
                    else if (baseUnit.TryGetComponent<BaseUnit>(out var unit))
                    {
                        // 오브젝트 삭제
                        unit.Remove();
                    }
                    return true;
                }
                break;

            case TILEMODE.SKILL:
                // 스킬 사용
                SkillManager.Instance.UseSkill(currentKey, operatorKey, worldPos);
                // 스킬 정보 표시
                Sprite operatorSprite = BattleManager.Instance.GetTowerInfo(operatorKey).operatorData.profile;
                Sprite skillSprite = BattleManager.Instance.GetTowerInfo(operatorKey).operatorData.activeSkillData.icon;
                string skillName = SkillManager.Instance.skillDict[currentKey / 10].name;
                BattleManager.Instance.BattleUI.AddSkillQueue(operatorSprite, skillSprite, skillName);
                return true;
        }
        return false;
    }

    // 타워 생성
    private void BuildTower()
    {
        // 임시 오브젝트 활성화
        Vector3 pos = currentPrefab.transform.position;
        Vector2Int index = TileManager.Instance.GetGridIndex(pos);

        // 배열에 저장
        // 시작점
        int startX = index.x - sizeX / 2;
        int startY = index.y - sizeY / 2;
        Vector2Int start = new Vector2Int(startX, startY);
        // 끝점 설정
        Vector2Int end = new Vector2Int(startX + sizeX - 1, startY + sizeY - 1);
        // 배열에 벽으로 저장
        TileManager.Instance.FillTile(start, end, 2);

        // 타워 설정
        if (currentPrefab.TryGetComponent<Tower>(out var tower))
        {
            // 업그레이드 반영
            TowerOperList towerOperList = BattleManager.Instance.GetTowerInfo(currentKey, operatorKey);
            if (towerOperList != null)
            {
                if (towerOperList.operatorData != null)
                    tower.Init(towerOperList.towerData, towerOperList.operatorData);
                else
                    tower.Init(towerOperList.towerData, null);
            }

            tower.isBuild = true;
            TileManager.Instance.AddUnit(tower);
            tower.start = start;
            tower.end = end;
            tower.SetGradeColor();
        }
        else if (currentPrefab.TryGetComponent<BaseUnit>(out var unit))
        {
            unit.start = start;
            unit.end = end;
        }

        // 장애물 요소 활성화
        if (currentPrefab.TryGetComponent<NavMeshObstacle>(out var obstacle))
            obstacle.enabled = true;
        else
        {
            currentPrefab.AddComponent<NavMeshObstacle>();
        }
        if (currentPrefab.TryGetComponent<BoxCollider>(out var boxCollider))
            boxCollider.enabled = true;

        currentPrefab = null;

        if (BattleManager.Instance.CurrentBattle.TowerCount == 0 && BattleManager.Instance.curStage == 0)
        {
            // 커널 전송 - 첫번째 타워 설치
            AnalyticsManager.Instance.SendStep(3);
        }
        // 커스텀 이벤트 - 타워 배치
        AnalyticsManager.Instance.SendEvent(Define.TowerPlaced, new Dictionary<string, object> {
            { Define.stageKey, BattleManager.Instance.curStage },
            { Define.towerKey, currentKey},
            { Define.numberTowerPlacedBefore,  BattleManager.Instance.CurrentBattle.TowerCount},
            });

        // 카운트 증가
        BattleManager.Instance.CurrentBattle.TowerCount++;
    }
    private GameObject GetPrefab(int towerKey)
    {
        GameObject prefab;
        // 미리 가져온게 있으면 사용
        if (prefabs.ContainsKey(towerKey))
        {
            prefab = prefabs[towerKey];
        }
        else
        {
            // 없으면 Resources에서 가져오기
            prefab = Resources.Load<GameObject>(Define._towerRoot + "/" + towerKey);
            if (prefab != null)
                prefabs[towerKey] = prefab;
        }
        return prefab;
    }
    // 사용 취소
    public void CancelCard()
    {
        // 영역, 타일 숨기기
        areaPreview.gameObject.SetActive(false);
        rangeImage.gameObject.SetActive(false);
        shadowImage.gameObject.SetActive(false);
        TileManager.Instance.ShowEnforce(currentKey, operatorKey, false);
        TileManager.Instance.ShowRange(currentKey, operatorKey, false);
        Destroy(currentPrefab);
        currentPrefab = null;
        mode = TILEMODE.IDLE;
    }

    // 카드 모드 설정 - 범위 표시
    public void SetCard(int mainKey, int subKey, float size)
    {
        // 범위 표시 후 설정
        SetRange(size);
        SetCard(mainKey, subKey);
    }
    // 카드 모드 설정
    public void SetCard(int mainKey, int subKey)
    {
        BattleManager.Instance.BattleUI.SetActiveButton();
        if (currentPrefab != null)
            Destroy(currentPrefab);
        // 곡괭이
        if (mainKey == 4901 || mainKey == 4902)
        {
            mode = TILEMODE.REMOVE;
            currentKey = mainKey;
            sizeX = 1;
            sizeY = 1;
            //SetRange(sizeX);

            return;
        }
        else if (3100 <= mainKey && mainKey < 4000 || 4900 == mainKey)
        {
            // 타워 설치
            mode = TILEMODE.BUILD;
            // 프리팹 가져오기
            GameObject prefab;
            if (mainKey == 4900)
            {
                // 장애물이면 해당 스테이지의 장애물 오브젝트
                prefab = Resources.Load<GameObject>(
                    TileManager.Instance.tileBuilder.GetPath()
                    + "/"
                    + TileManager.Instance.tileBuilder.currentTheme.obstacle[0]);
            }
            // 일반 타워
            else
                prefab = GetPrefab(mainKey);
            if (prefab != null)
            {
                // 임시 프리팹 생성
                currentPrefab = Instantiate(prefab);

                // 사이즈, 정보 가져오기
                BaseUnit unit = currentPrefab.GetComponent<BaseUnit>();
                Vector2Int size = GetSize(unit.shape);
                sizeX = size.x;
                sizeY = size.y;
                currentKey = mainKey;
                operatorKey = subKey;
                SetRange(BattleManager.Instance.GetTowerInfo(mainKey, subKey).towerData.towerInfoTable.range * 0.5f);
            }
        }
        else if (61000 <= mainKey && mainKey < 70000)
        {
            // 오퍼레이터 스킬
            mode = TILEMODE.SKILL;
            currentKey = mainKey;
            operatorKey = subKey;
            // 암영 표시
            shadowImage.gameObject.SetActive(true);
        }
    }
    // 영역 크기 설정
    public void SetRange(float size)
    {
        rangeImage.gameObject.SetActive(true);
        rangeImage.rectTransform.sizeDelta = new Vector2(size, size);
    }

    // 모양 정수를 통해 크기 구하기
    private Vector2Int GetSize(int shape)
    {
        int x = 2;
        int y = 2;
        // 10의 배수 = 정사각형
        if (shape % 10 == 0)
        {
            x = (int)Mathf.Sqrt(shape / 10);
            y = (int)Mathf.Sqrt(shape / 10);
        }
        return new Vector2Int(x, y);
    }

    private void OriginalColorStore()
    {
        var unitrenders = baseUnit.renderers;
        List<Color> colors = new List<Color>();

        foreach (var unitrender in unitrenders)
        {
            Material[] materials = unitrender.materials;
            foreach (var material in materials)
            {
                colors.Add(material.color);
            }
        }
        originalColors = colors;
    }

}
