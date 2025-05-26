using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool loadSaveData;
    public bool loadTutorial;
    public DataManager DataManager { get; private set; }
    public Player Player;

    private bool initialized = false;
    private SaveSystem saveSystem;

    protected override void Awake()
    {
        base.Awake();

        // 중복 인스턴스는 base.Awake()에서 이미 Destroy됨
        // 따라서 살아남은 인스턴스에서만 Init 실행
        if (Instance == this && !initialized)
        {
            Init();
            initialized = true;
        }
    }


    void Start()
    {
        //StartCoroutine(AutoSaveRoutine());
        Application.targetFrameRate = 60; // 60프레임 고정
        if (loadTutorial && Player.clearStage == -1)
        {
            // 튜토리얼 진행
            StartCoroutine(LoadTutorial());
        }
    }
    IEnumerator LoadTutorial()
    {
        yield return null;
        StageManager.Instance.stageHandler.OnGameStart();
    }
    IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60);
            //SavePlayer();
        }
    }

    private void Init()
    {
        DataManager = new DataManager();

        DataManager.Initialize();
        //Player.Initialize();
        // 매너저 초기화
        QuestManager.Instance.SetQuestData();
        StageManager.Instance.SetStageData();
        Player.inventory.InitBuildDict();
        // 데이터 불러오기
        if (loadSaveData)
        {
            LoadPlayer();
        }
        // 일일, 주간 퀘스트 초기화
        QuestManager.Instance.InitTimer();
    }

    // 데이터 불러오기
    private void LoadPlayer()
    {
        saveSystem = new SaveSystem();
        saveSystem.LoadPlayer(Player);
    }
    // 데이터 저장
    public void SavePlayer()
    {
        if (Player != null && saveSystem != null)
        {
            saveSystem.SavePlayer(Player);
        }
    }
    // 앱 종료시
    void OnApplicationQuit()
    {
        SavePlayer();
    }

    void OnApplicationPause(bool pause)
    {
        // 앱이 백그라운드로 이동시
        if (pause)
        {
            SavePlayer();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // 앱이 포커스를 잃은 경우
        if (!hasFocus)
        {
            SavePlayer();
        }
    }
}
