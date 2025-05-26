using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndCause
{
    CLEAR,
    FAILD,
    ABAN
}
public class CurrentBattle : MonoBehaviour
{
    // 재화 리스트는 딕셔너리로 구현하기
    [Header("EXP")]
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private float playerExp = 0; // 획득한 경험치
    [SerializeField] private float playerLevelExp = 0; // 랩업에 필요한 경험치 량
    [SerializeField] private float totalExp = 0;
    [Header("Gold")]
    [SerializeField] private int gold = 0;
    [Header("Wave")]
    [SerializeField] public int curWave = 1;
    [SerializeField] private int totalWave = 0;

    private Coroutine waveCoroutine; // 웨이브 코루틴 저장 변수
    private Coroutine selectCardCoroutin; // 카드 뽑기 코루틴 저장 변수
    private Coroutine enumySpownCoroutine;

    public int allSpownEnemyCount; // 모든 웨이브에서 소환되는 적 수
    public int killEnemeyCount; // 처치한 적 수
    public int selectCardCount;

    public GameObject selectCardUI;
    public UICardHand uiCardHand;
    public RewardSystem RewardSystem;

    bool isButtonClicked = false;

    public bool isBattle = false;
    public bool isEnd = false;
    private bool isGameClear;

    public Transform target;

    public GameObject testAttackObject;
    private int waveWaitScale = 0; // 웨이브 대기 시간 감소

    public bool isSelectCard = false; // 카드 선택중 확인 함수
    private int cardCount = 0; // 해당 스테이지에서 선택한 카드 수
    float startTime; // 시작 시간
    float endTime; // 종료 시간
    public int TowerCount { get; set; }
    public int SkillCount { get; set; }
    private EndCause endCause = EndCause.ABAN;


    public void Initialize()
    {
        selectCardUI.SetActive(false);
        playerLevel = 1;
        playerExp = 0;
        playerLevelExp = 40; // 플레이어 정보나 스테이지 정보에서 랩당 경험치 비율 가져오기
        totalExp = 0;
        gold = 0;
        cardCount = 0;
        totalWave = BattleManager.Instance.curStageData.StageInfoTable.wave;

        RewardSystem.GetComponent<RewardSystem>();
        RewardSystem.Init();
        int dropTableKey = BattleManager.Instance.curStageData.StageInfoTable.dropTable;
        RewardSystem.SetDropTableKey(dropTableKey);
        // 대기 시간 감소 패시브 불러오기
        waveWaitScale = BattleManager.Instance.GetValue(6509);

        isEnd = false;
        isGameClear = false;
    }


    public void OnButtonClick()
    {
        isButtonClicked = true;
    }

    public void BattleStart()
    {
        CountAllSpwnEnemy();
        // startTime
        startTime = Time.time;
        // 스테이지 시작시 초기값(2) + 패시브 증가량만큼 카드 획득
        selectCardCount = 2 + BattleManager.Instance.GetValue(6507);
        selectCardCoroutin = StartCoroutine(GetRandomCardCoroutine());

        // 패시브로 인한 추가 설치 카드 획득
        int bonus = BattleManager.Instance.GetValue(6508);
        for (int i = 0; i < bonus; i++)
        {
            // 무작위 설치 카드 획득
            GetRandomTower();
        }
        endCause = EndCause.ABAN;
    }

    //카드 뽑기 코루틴
    private IEnumerator GetRandomCardCoroutine()
    {
        selectCardUI.SetActive(true);
        BattleManager.Instance.BattleUI.SelectCardButtonOn();

        while (selectCardCount > 0)
        {
            isSelectCard = true;
            BattleManager.Instance.BattleUI.SelectCard.SetRandomCards();

            isButtonClicked = false;

            Time.timeScale = 0;

            yield return new WaitUntil(() => isButtonClicked); // 버튼을 누를때 까지 대기

            selectCardCount--;

            // 커널 전송
            // 초기 카드 선택
            if (cardCount == 0)
            {
                switch (BattleManager.Instance.curStage)
                {
                    case 0:
                        // 0 스테이지
                        AnalyticsManager.Instance.SendStep(1);
                        break;
                    case 1:
                        // 1 스테이지
                        AnalyticsManager.Instance.SendStep(52);
                        break;
                    case 2:
                        // 2 스테이지
                        AnalyticsManager.Instance.SendStep(201);
                        break;
                }
            }
            else if (cardCount == 1)
            {
                // 커널 전송 - 두번째 카드 선택
                switch (BattleManager.Instance.curStage)
                {
                    case 0:
                        // 0 스테이지
                        AnalyticsManager.Instance.SendStep(2);
                        break;
                    case 1:
                        // 1 스테이지
                        AnalyticsManager.Instance.SendStep(53);
                        break;
                    case 2:
                        // 2 스테이지
                        AnalyticsManager.Instance.SendStep(202);
                        break;
                    case 3:
                        // 3 스테이지
                        AnalyticsManager.Instance.SendStep(401);
                        break;
                    case 4:
                        // 4 스테이지
                        AnalyticsManager.Instance.SendStep(601);
                        break;
                    case 5:
                        // 5 스테이지
                        AnalyticsManager.Instance.SendStep(701);
                        break;
                    case 6:
                        // 6 스테이지
                        AnalyticsManager.Instance.SendStep(901);
                        break;
                    case 7:
                        // 7 스테이지
                        AnalyticsManager.Instance.SendStep(1001);
                        break;
                }
            }
            else if (cardCount == 2 && BattleManager.Instance.curStage == 0)
            {
                AnalyticsManager.Instance.SendStep(4);
            }
            cardCount++;
            isSelectCard = false;
            Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();


            // 튜토리얼중이라면 다음 팝업으로
            if (TutorialUIManager.Instance != null)
            {
                TutorialUIManager.Instance.OnCardSelect();
            }
        }


        BattleManager.Instance.BattleUI.SelectCardButtonOff();
        selectCardUI.SetActive(false);
        selectCardCoroutin = null;

        if (!isBattle)
        {
            isBattle = true;
            //임시 코드
            BattleManager.Instance.BattleUI.StartTimer();
            // [대기 중] UI 타이머
            BattleManager.Instance.BattleUI.StartWaveTimer(3f);

            yield return new WaitForSeconds(3);
            //라운드 시작
            WaveStart();
        }
    }

    public void WaveStart()
    {
        waveCoroutine = StartCoroutine(WaveCoroutine());
    }
    public void WaveStop()
    {
        isBattle = false;

        BattleManager.Instance.BattleUI.StopTimer();
        BattleManager.Instance.BattleUI.GameEndPanelUpdate();

        if (waveCoroutine != null)
        {
            //웨이브 종료
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        if (enumySpownCoroutine != null)
        {
            //소환 종료
            StopCoroutine(enumySpownCoroutine);
            enumySpownCoroutine = null;

        }

        if (selectCardCoroutin != null)
        {
            //카드 선택 종료
            StopCoroutine(selectCardCoroutin);
            selectCardCoroutin = null;
        }

        Time.timeScale = 0;
    }

    private IEnumerator WaveCoroutine()
    {
        while (curWave <= totalWave && isBattle)
        {
            BattleManager.Instance.BattleUI.WaveUpdate(curWave, totalWave);
            //웨이브 시작

            // [적 소환 중] UI 타이머
            BattleManager.Instance.BattleUI.StartWaveTimer(15f);
            if (curWave % 10 == 0 || curWave == totalWave) EnemyManager.Instance.SpawnRandomBossEnemy();
            yield return enumySpownCoroutine = StartCoroutine(EnemyManager.Instance.SpownEnemyCoroutine(curWave));
            // 커널 전송 - 웨이브 종료
            switch (BattleManager.Instance.curStage)
            {
                // 0 스테이지
                case 0:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(6);
                    }
                    break;
                // 1 스테이지
                case 1:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(54);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(55);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(56);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(57);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(58);
                    }
                    break;
                // 2 스테이지
                case 2:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(203);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(204);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(205);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(206);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(207);
                    }
                    break;
                // 3 스테이지
                case 3:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(402);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(403);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(404);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(405);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(406);
                    }
                    else if (curWave == 6)
                    {
                        AnalyticsManager.Instance.SendStep(407);
                    }
                    else if (curWave == 7)
                    {
                        AnalyticsManager.Instance.SendStep(408);
                    }
                    break;
                // 4 스테이지
                case 4:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(602);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(603);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(604);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(605);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(606);
                    }
                    else if (curWave == 6)
                    {
                        AnalyticsManager.Instance.SendStep(607);
                    }
                    else if (curWave == 7)
                    {
                        AnalyticsManager.Instance.SendStep(608);
                    }
                    break;
                // 5 스테이지
                case 5:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(702);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(703);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(704);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(705);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(706);
                    }
                    else if (curWave == 6)
                    {
                        AnalyticsManager.Instance.SendStep(707);
                    }
                    else if (curWave == 7)
                    {
                        AnalyticsManager.Instance.SendStep(708);
                    }
                    break;
                // 6 스테이지
                case 6:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(902);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(903);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(904);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(905);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(906);
                    }
                    else if (curWave == 6)
                    {
                        AnalyticsManager.Instance.SendStep(907);
                    }
                    else if (curWave == 7)
                    {
                        AnalyticsManager.Instance.SendStep(908);
                    }
                    break;
                // 7 스테이지
                case 7:
                    if (curWave == 1)
                    {
                        AnalyticsManager.Instance.SendStep(1002);
                    }
                    else if (curWave == 2)
                    {
                        AnalyticsManager.Instance.SendStep(1003);
                    }
                    else if (curWave == 3)
                    {
                        AnalyticsManager.Instance.SendStep(1004);
                    }
                    else if (curWave == 4)
                    {
                        AnalyticsManager.Instance.SendStep(1005);
                    }
                    else if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(1006);
                    }
                    else if (curWave == 6)
                    {
                        AnalyticsManager.Instance.SendStep(1007);
                    }
                    else if (curWave == 7)
                    {
                        AnalyticsManager.Instance.SendStep(1008);
                    }
                    break;
                // 8 스테이지
                case 8:
                    if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(1201);
                    }
                    break;
                // 9 스테이지
                case 9:
                    if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(1205);
                    }
                    break;
                // 10 스테이지
                case 10:
                    if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(1209);
                    }
                    break;
                // 11 스테이지
                case 11:
                    if (curWave == 5)
                    {
                        AnalyticsManager.Instance.SendStep(1213);
                    }
                    break;

            }


           // 웨이브 종료, 5초후 웨이브 시작
            yield return new WaitForSeconds(5);
            curWave++;
        }
        //모든 웨이브 끝
    }


    public void CountAllSpwnEnemy()
    {
        allSpownEnemyCount = 0;
        killEnemeyCount = 0;
        for (int i = 1; i <= totalWave; i++)
        {
            allSpownEnemyCount += Mathf.FloorToInt(15 * (0.03f * (i - 1) + Mathf.Pow(1.05f, (i - 1))));
            allSpownEnemyCount *= Mathf.FloorToInt(1 + (BattleManager.Instance.GetValue(6505) + BattleManager.Instance.GetValue(6506)) / 100f);
        }
    }



    // 각자 알맞는 UI 표시
    // 결과창 UI에서 버튼을 누르면 메인씬으로 가면서 StageEnd()메서드 호출
    public void GameClear()
    {
        if (isEnd || isGameClear) return;
        if (killEnemeyCount >= allSpownEnemyCount)
        {
            isGameClear = true;
            endCause = EndCause.CLEAR;
            GameEnd(true);
        }
    }

    public void GameOver()
    {
        if (isEnd) return;
        endCause = EndCause.ABAN;
        GameEnd(false);
    }

    public void GameEnd(bool isWin)
    {
        isEnd = true;
        WaveStop();

        BattleManager.Instance.BattleUI.GlodUpdate(gold);
        BattleManager.Instance.BattleUI.ResultTextUpdate(isWin);

        int changeExp = isWin ? (int)(totalExp * 0.05f) : (int)(totalExp * 0.025f);
        RewardSystem.TakeExpItem(changeExp);
        BattleManager.Instance.BattleUI.ExpUpdate((int)totalExp);

        bool isNotTuto = BattleManager.Instance.curStageData.StageInfoTable.key != 1000;


        if (isNotTuto)
        {
            QuestManager.Instance.UpdateQuestProgress(QuestConditions.stagePlay, BattleManager.Instance.IsSquadState());
            QuestManager.Instance.UpdateQuestProgress(QuestConditions.enemyKill, QuestConditions.none, killEnemeyCount);
        }

        if (isWin)
        {
            if (BattleManager.Instance.curStageData.IsClear == false)
            {
                BattleManager.Instance.curStageData.IsClear = true;
            }

            if (isNotTuto)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestConditions.stageClear, QuestConditions.none,
                    1, BattleManager.Instance.curStageData.StageInfoTable.key);
            }
        }
        else
        {
            if (isNotTuto)
            {
                QuestManager.Instance.UpdateQuestProgress(QuestConditions.stageFail, QuestConditions.none);
            }
        }

        BattleManager.Instance.curStageData.StagePlayCount++;
        //GameManager.Instance.Player.clearStage == BattleManager.Instance.curStage
        RewardSystem.RewardUIUpdate();

        // 종료 시간
        endTime = Time.time;
        int elapsedSeconds = (int)(endTime - startTime);

        // 승리 시 UI 갱신 정보
        if (isWin)
        {
            if (BattleManager.Instance.curStage > GameManager.Instance.Player.clearStage)
                GameManager.Instance.Player.clearStage = BattleManager.Instance.curStage;
        }
        if (BattleManager.Instance.curStage < 2)
        {
            // 0, 1 스테이지는 승패여부 상관없이
            if (BattleManager.Instance.curStage > GameManager.Instance.Player.clearStage)
                GameManager.Instance.Player.clearStage = BattleManager.Instance.curStage;
        }

        // 임의 저장
        GameManager.Instance.SavePlayer();

        // 커스텀 이벤트 - 종료 시 덱 구성
        string deckTower = JsonConvert.SerializeObject(BattleManager.Instance.GetTowerList());
        string deckOper = JsonConvert.SerializeObject(BattleManager.Instance.GetOperatorList());
        AnalyticsManager.Instance.SendEvent(Define.DeckStageEnded, new Dictionary<string, object> {
            { Define.stageKey, StageManager.Instance.stageHandler.GetStageKey() },
            { Define.deckTowerJson, deckTower},
            { Define.deckOperatorJson, deckOper},
            });

        // 커스텀 이벤트 - 종료 시 기록
        AnalyticsManager.Instance.SendEvent(Define.StageEnded, new Dictionary<string, object> {
            { Define.stageKey, StageManager.Instance.stageHandler.GetStageKey() },
            { Define.isCleared, isWin},
            { Define.stageEndCause, (int)endCause },
            { Define.stagePlayTime, elapsedSeconds },
            { Define.numberPlayedBefore, StageManager.Instance.stageDataList[BattleManager.Instance.curStage].StagePlayCount },
            { Define.numberClearedBefore, StageManager.Instance.stageDataList[BattleManager.Instance.curStage].StageClearCount },
            { Define.numberTowerPlaced,  TowerCount},
            { Define.numberSkillUsed,  SkillCount},
            { Define.stageExpGained,  changeExp},
            { Define.stageCoinGained,  gold},
            });
        // 커스터 이벤트 - 보상 아이템
        string gainedItemJson = JsonConvert.SerializeObject(RewardSystem.GetReword());
        AnalyticsManager.Instance.SendEvent(Define.ItemStageEnded, new Dictionary<string, object> {
            { Define.stageKey, StageManager.Instance.stageHandler.GetStageKey() },
            { Define.isCleared, isWin},
            { Define.gainedItemJson, gainedItemJson},
            });
    }
    public void OnCloseButton()
    {
        // 커널 전송 - 결과창 닫기
        switch (BattleManager.Instance.curStage)
        {
            case 0:
                // 튜토리얼
                AnalyticsManager.Instance.SendStep(7);
                break;
            case 1:
                // 1스테이지
                AnalyticsManager.Instance.SendStep(59);
                break;
            case 2:
                // 2스테이지
                AnalyticsManager.Instance.SendStep(208);
                break;
            case 3:
                // 3스테이지
                AnalyticsManager.Instance.SendStep(409);
                break;
            case 4:
                // 4스테이지
                AnalyticsManager.Instance.SendStep(609);
                break;
            case 5:
                // 5스테이지
                AnalyticsManager.Instance.SendStep(709);
                break;
            case 6:
                // 6스테이지
                AnalyticsManager.Instance.SendStep(909);
                break;
            case 7:
                // 7스테이지
                AnalyticsManager.Instance.SendStep(1009);
                break;
            case 8:
                // 8스테이지
                AnalyticsManager.Instance.SendStep(1202);
                break;
            case 9:
                // 9스테이지
                AnalyticsManager.Instance.SendStep(1206);
                break;
            case 10:
                // 10스테이지
                AnalyticsManager.Instance.SendStep(1210);
                break;
            case 11:
                // 11스테이지
                AnalyticsManager.Instance.SendStep(1214);
                break;
        }
    }

    public void LevelUP(float exp)
    {
        EnemyRoot enemyRoot = BattleManager.Instance.enemyRoot;
        playerExp += exp * enemyRoot.expScale;
        totalExp += exp * enemyRoot.expScale;
        BattleManager.Instance.BattleUI.ExpBarUpdate(playerLevelExp, exp); // 경험치바UI 업데이트
        if (playerExp >= playerLevelExp)
        {
            // 만약에 레벨에 필요한 경험치량 보다 더많은 경험치를 획득 했을 때 그만큼 레벨업을 가능하게 해준다
            int takeLevel = Mathf.FloorToInt(playerExp / playerLevelExp);
            playerExp = playerExp - (playerLevelExp * takeLevel);
            playerLevel += takeLevel;

            // 레벨이 올라갈수록 필요한 경험치 요구량 변경
            playerLevelExp = Mathf.FloorToInt(40 * Mathf.Pow(1.075f, (playerLevel - 1)));
            // 카드 사용 중이라면 취소
            uiCardHand.CancelCard();
            //카드 뽑기 표시
            selectCardCount += takeLevel;
            if (selectCardCoroutin == null)
            {
                selectCardCoroutin = StartCoroutine(GetRandomCardCoroutine());
            }
            //임시 코드
            BattleManager.Instance.BattleUI.PlayerLevelUpdate(playerLevel);
        }
    }

    public void TakeGold(int gold)
    {
        this.gold += gold;
    }

    public void ToInventoryGold()
    {
        GameManager.Instance.Player.Gold += gold;
        // 커스텀 이벤트 - 코인 변화
        AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
            { Define.changeType, Define.gained },
            { Define.numberCoinChanged, gold},
            { Define.changeSource, 7},
            { Define.numberCoinBefore, GameManager.Instance.Player.Gold - gold},
        });
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            killEnemeyCount = allSpownEnemyCount;
            GameClear();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetRandomTower();
        }
#endif
    }


    // 무작위 설치 카드 획득
    public void GetRandomTower()
    {
        TowerOperList towerOperList = BattleManager.Instance.GetRandomInfo();
        int operatorKey = towerOperList.operatorData != null ? towerOperList.operatorData.operatorInfoTable.key : -1;
        // 카드 획득
        BattleManager.Instance.CurrentBattle.uiCardHand.SpawnCard(towerOperList, operatorKey);
        // 강화 카드 추가 여부 확인
        BattleManager.Instance.BattleUI.SelectCard.SelectedCard(towerOperList);
    }

    // 중도 포기 
    public void GiveUP()
    {
        endCause = EndCause.ABAN;
        GameOver();
    }
}
