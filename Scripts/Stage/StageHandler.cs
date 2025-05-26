using UnityEngine;
using System.Collections.Generic;

public class StageHandler : MonoBehaviour
{
    [Header("UI Handler")]
    [SerializeField] private StageUIHandler uiHandler;

    [Header("UI References")]
    [SerializeField] private GameObject itemSlots;
    [SerializeField] private GameObject enemySlots;
    [SerializeField] private Transform itemPos;
    [SerializeField] private Transform spwonEnumyPos;

    [Header("Stage Data")]
    public List<StageData> stageList = new();
    private StageInfoTable selectStageData;
    private List<int> spcialItemsKey = new();
    public int curStage;
    public int clearStageCount;
    private void Awake()
    {
        uiHandler = GetComponent<StageUIHandler>();
        StageManager.Instance.SetHandler(this);
    }
    public void Start()
    {
        stageList = StageManager.Instance.stageDataList;
        //clearStageCount = GameManager.Instance.Player.clearStage;
        CountClearStageCount();
        curStage = Mathf.Min(clearStageCount, stageList.Count - 1);

        uiHandler.InitializeUI(OnBackStage, OnNextStage, OnGameStart);
        StageInfoUpdate();
    }

    private void CountClearStageCount()
    {
        int count = 0;
        foreach (var data in stageList)
        {
            if (data.IsClear) count++;
        }
        clearStageCount = count;
    }

    private void OnBackStage()
    {
        if (curStage > 0)
        {
            curStage--;
            if (!GameManager.Instance.loadTutorial)
                curStage = Mathf.Max(1, curStage);
            StageInfoUpdate();
        }
    }

    private void OnNextStage()
    {
        if (curStage < stageList.Count - 1 && curStage < clearStageCount)
        {
            curStage++;
            StageInfoUpdate();
        }
    }

    public void OnGameStart()
    {
        if (!GameManager.Instance.loadTutorial)
        {
            if (curStage < 1)
            {
                curStage = 1;
                StageInfoUpdate();
            }
        }
        if (curStage >= 0 && curStage < stageList.Count)
        {
            // 임의 저장
            GameManager.Instance.SavePlayer();
            if (curStage == 0)
            {
                // 튜토리얼
                LoadingSceneController.Instance.LoadScene("TutorialScenes", () =>
                {
                    // 여기 안에서 씬이 로드 완료됐을 때 실행할 코드만 작성

                    SoundManager.Instance.StopBGM();

                    BattleManager.Instance.TutorialStart(stageList[curStage]);
                    BattleManager.Instance.curStage = this.curStage;
                    TileManager.Instance.LoadData(selectStageData);
                    EnemyManager.Instance.GetEnemyData(selectStageData.enemy);

                    ObjectPoolManager.Instance.PreloadBattleSceneObjects();
                    ObjectPoolManager.Instance.PreloadStageEnemies(selectStageData.enemy);
                    ObjectPoolManager.Instance.InitDamagePool();
                    // 커널 - 튜토리얼
                    AnalyticsManager.Instance.SendStep(0);
                });
            }
            else
            {
                // 커스텀 이벤트 - 시작 시 덱 구성
                AnalyticsManager.Instance.SendEvent(Define.DeckStageStarted, new Dictionary<string, object> {
                    { Define.stageKey, selectStageData.key },
                    { Define.deckTowerJson, JsonUtility.ToJson(BattleManager.Instance.GetTowerCount())},
                    { Define.deckOperatorJson, JsonUtility.ToJson(BattleManager.Instance.GetOperatorList())},
                });

                // 커스텀 이벤트 - 시작 시 플레이어 기록
                AnalyticsManager.Instance.SendEvent(Define.StageStarted, new Dictionary<string, object> {
                    { Define.stageKey, selectStageData.key },
                    { Define.numberPlayedBefore, StageManager.Instance.stageDataList[curStage].StagePlayCount },
                    { Define.numberClearedBefore, StageManager.Instance.stageDataList[curStage].StageClearCount },
                    { Define.numberDeckTower, BattleManager.Instance.GetTowerCount() },
                    { Define.numberDeckOperator, BattleManager.Instance.GetOperatorCount() },
                    { Define.playerCoinBalance, GameManager.Instance.Player.Gold },
                    { Define.playerGrowthItem, GameManager.Instance.Player.inventory.GetItemCount(new int[]{9100, 9101, 9102, 9103}) },
                    { Define.playerSkillGrowthItemLow, GameManager.Instance.Player.inventory.GetItemCount(new int[]{9201, 9203, 9105, 9107})  },
                    { Define.playerSkillGrowthItemHigh, GameManager.Instance.Player.inventory.GetItemCount(new int[]{9202, 9204, 9206, 9208})  },
                });
                //LoadBattleScene();
                LoadingSceneController.Instance.LoadScene("BattleScenes", () =>
                {
                    // 여기 안에서 씬이 로드 완료됐을 때 실행할 코드만 작성

                    SoundManager.Instance.StopBGM();

                    BattleManager.Instance.StageStart(stageList[curStage]);
                    BattleManager.Instance.curStage = curStage;
                    TileManager.Instance.LoadData(selectStageData);
                    EnemyManager.Instance.GetEnemyData(selectStageData.enemy);

                    ObjectPoolManager.Instance.PreloadBattleSceneObjects();
                    ObjectPoolManager.Instance.PreloadStageEnemies(selectStageData.enemy);
                    ObjectPoolManager.Instance.InitDamagePool();
                    // 커널 전송 - 전투 시작 버튼
                    switch (curStage)
                    {
                        case 1:
                            // 1스테이지
                            AnalyticsManager.Instance.SendStep(51);
                            break;
                        case 2:
                            // 2스테이지
                            AnalyticsManager.Instance.SendStep(200);
                            break;
                        case 3:
                            // 3스테이지
                            AnalyticsManager.Instance.SendStep(400);
                            break;
                        case 4:
                            // 4스테이지
                            AnalyticsManager.Instance.SendStep(600);
                            break;
                        case 5:
                            // 5스테이지
                            AnalyticsManager.Instance.SendStep(700);
                            break;
                        case 6:
                            // 6스테이지
                            AnalyticsManager.Instance.SendStep(900);
                            break;
                        case 7:
                            // 7스테이지
                            AnalyticsManager.Instance.SendStep(1000);
                            break;
                        case 8:
                            // 8스테이지
                            AnalyticsManager.Instance.SendStep(1200);
                            break;
                        case 9:
                            // 9스테이지
                            AnalyticsManager.Instance.SendStep(1204);
                            break;
                        case 10:
                            // 10스테이지
                            AnalyticsManager.Instance.SendStep(1208);
                            break;
                        case 11:
                            // 11스테이지
                            AnalyticsManager.Instance.SendStep(1212);
                            break;
                    }
                });
            }
        }
        else
        {
            Debug.LogWarning("유효한 스테이지 데이터가 없습니다.");
        }
    }
    public int GetStageKey()
    {
        return selectStageData.key;
    }
    
    // 경고 창 표기여부(유효 타워 검사)
    public void CheckWarning()
    {
        uiHandler.ShowWarning(selectStageData.enemy);
    }

    private void StageInfoUpdate()
    {
        selectStageData = stageList[curStage].StageInfoTable;
        SetItemSlot();
        SetSpownEnemySlot();
        uiHandler.UpdateStageText(curStage);
        uiHandler.UpdateNavigationButtons(curStage, stageList.Count, clearStageCount);
        CheckWarning();
    }

    private void SetItemSlot()
    {
        ResetIcon(itemPos);

        if (!selectStageData.spcialItems.Contains(-1))
        {
            spcialItemsKey = selectStageData.spcialItems;
            foreach (int key in spcialItemsKey)
            {
                GameObject slotIcon = Instantiate(itemSlots, itemPos);
                var item = GameManager.Instance.Player.inventory.items[key].data;
                if (item?.icon != null)
                {
                    slotIcon.GetComponent<StageRewardSlots>().SetData(new RewardItem
                    {
                        ItemData = item,
                        amount = 1
                    });
                    //slotIcon.GetComponent<UnityEngine.UI.Image>().sprite = item.data.itemSlots;
                }
            }
        }
    }

    private void SetSpownEnemySlot()
    {
        ResetIcon(spwonEnumyPos);
        foreach (int enemyKey in selectStageData.enemy)
        {
            GameObject slotIcon = Instantiate(enemySlots, spwonEnumyPos);
            EnemyTable enemydata = GameManager.Instance.DataManager.EnemyTableLoader.EnemyDict[enemyKey];
            if (EnemyManager.Instance.enemyIconDict.TryGetValue(enemyKey, out var sprite))
            {
                slotIcon.GetComponent<StageRewardSlots>().SetEnemy(enemydata, sprite);
                //slotIcon.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            }
        }
    }

    private void ResetIcon(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
    private TileArea ConvertToArea(List<int> list)
    {
        return new TileArea
        (
            new Vector2Int(list[0], list[1]),
            new Vector2Int(list[2], list[3])
        );
    }

    private TileArea[] ConvertObstacles(List<List<int>> allObstacles)
    {
        List<TileArea> areas = new List<TileArea>();
        foreach (var obs in allObstacles)
        {
            if (obs.Count >= 2)
            {
                var pos = new Vector2Int(obs[0], obs[1]);
                areas.Add(new TileArea(pos, pos));
            }
        }
        return areas.ToArray();
    }
}
