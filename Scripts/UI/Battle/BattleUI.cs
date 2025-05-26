using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameObject battleUI;

    [Header("Reward")]
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI totalKillScore;
    [SerializeField] private TextMeshProUGUI playTime;
    [SerializeField] private TextMeshProUGUI goldAmount;
    [SerializeField] private TextMeshProUGUI expAmount;
    public RewardUI RewardUI;
    public ItemInfoPopup ItemInfoPopup;

    [SerializeField] private Button returnButton;

    [Header("PleyrLevel")]
    [SerializeField] private TextMeshProUGUI playerLevelText;

    [Header("Wave")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveTimerText;
    private Coroutine timerCoroutine;
    private Coroutine waveTimerCoroutine;

    [Header("KillCount")]
    [SerializeField] private TextMeshProUGUI killCountText;

    [Header("EXP Bar")]
    [SerializeField] private Image expBar;
    [SerializeField] private float expBarFill;

    [Header("Select Card")]
    public SelectCard SelectCard;
    [SerializeField] private int reloadingCount;
    [SerializeField] private Button reloadingButton;
    [SerializeField] private TextMeshProUGUI reloadingText;

    [Header("DescriptPopup")]
    public DescriptPopup descriptPopup;
    public BattleTowerInfo battleTowerInfo;

    [Header("skillQueue")]
    public Transform skillQueueParent;
    public GameObject skillQueuePrefab;

    [Header("GameSpeed")]
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedText;

    public int curGameSpeed = 1;
    public bool isFastBuy = false;

    [Header("SettingUI")]
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button returnButtonSPBack;
    [SerializeField] private Button returnButtonSP;

    [Header("selectCardList")]
    [SerializeField] private Button selectCardListButton;
    [SerializeField] private GameObject selectCardListPanel;
    [SerializeField] private GameObject settingPanelBG;
    [SerializeField] private Button returnButtonSCLP;

    public GameObject selectCardListSlotPrefab;
    public Transform selectCardListSlottransform;

    [Header("CameraMove")]
    public BattleCameraMove battleCameraMove;

    bool isSetting = false;
    bool isViewSelectCard = false;

    public void Initialize()
    {
        playerLevelText.text = "1";
        waveText.text = $"01/{(BattleManager.Instance.curStageData.StageInfoTable.wave).ToString("D2")}";
        killCountText.text = "0";
        expBar.fillAmount = 0;
        expBarFill = 0;
        SelectCard.Init();

        // TODO 카드 다시 뽑기 기능 횟수 제한 기능 추가하기
        reloadingCount = 3;

        curGameSpeed = 1;
        isFastBuy = false;
        UpdateUI();
    }



    private void Start()
    {
        SettingButtons();
        gameEndPanel.SetActive(false);
        settingPanel.SetActive(false);
        selectCardListPanel.SetActive(false);
        settingPanelBG.SetActive(false);
    }

    private void SettingButtons()
    {
        returnButton.onClick.AddListener(OnClickReturnButton);
        reloadingButton.onClick.AddListener(OnClickReloadingButton);
        speedButton.onClick.AddListener(OnClickGameSpeedButton);
        settingButton.onClick.AddListener(OnClickSettingButton);
        selectCardListButton.onClick.AddListener(OnClickSelectCardListButton);
        giveUpButton.onClick.AddListener(OnClickGiveUpButton);
        returnButtonSP.onClick.AddListener(OnClickReturnButtonSP);
        returnButtonSPBack.onClick.AddListener(OnClickReturnButtonSP);
        returnButtonSCLP.onClick.AddListener(OnClickReturnButtonSCLP);
    }

    private void OnClickReturnButton()
    {
        DOTween.KillAll(); // 모든 Tween 강제 종료

        

        //획득한 아이템과 골드 인벤토리에 넣기

        BattleManager.Instance.CurrentBattle.RewardSystem.TakeInventory();
        BattleManager.Instance.CurrentBattle.ToInventoryGold();
        //메인 씬으로 돌아가기
        Time.timeScale = 1f;
        //SceneManager.LoadScene("MainScenes");
        // 커널 전송 - 결과창 닫기
        BattleManager.Instance.CurrentBattle.OnCloseButton();

        LoadingSceneController.Instance.LoadScene("MainScenes");
    }

    private void OnClickReloadingButton()
    {
        if (reloadingCount > 0)
        {
            SelectCard.ReloadingCards();
            reloadingCount--;
            reloadingButton.gameObject.SetActive(false);
        }
        UpdateReloadingText();
    }
    public void UpdateReloadingText()
    {
        reloadingText.text = $"새로고침\n({reloadingCount}/3)";
    }

    public void ShowReloadButton()
    {
        bool isShow = reloadingCount > 0 ? true : false;
        reloadingButton.gameObject.SetActive(isShow);
    }

    private void OnClickSettingButton()
    {
        isSetting = true;
        settingPanel.SetActive(isSetting);
        selectCardListButton.gameObject.SetActive(isSetting || BattleManager.Instance.CurrentBattle.isSelectCard);
        settingPanelBG.SetActive(isSetting || isViewSelectCard);
        Time.timeScale = 0f;
    }
    private void OnClickReturnButtonSP()
    {
        isSetting = false;
        settingPanel.SetActive(isSetting);
        selectCardListButton.gameObject.SetActive(isSetting || BattleManager.Instance.CurrentBattle.isSelectCard);
        settingPanelBG.SetActive(isSetting || isViewSelectCard);

        if (isSetting || BattleManager.Instance.CurrentBattle.isSelectCard) Time.timeScale = 0f;
        else Time.timeScale = curGameSpeed;

        isViewSelectCard = false;
        selectCardListPanel.SetActive(false);
    }
    private void OnClickSelectCardListButton()
    {
        isViewSelectCard = true;
        selectCardListPanel.SetActive(isViewSelectCard);
        settingPanelBG.SetActive(isSetting || isViewSelectCard);
        SettingSelectedCardListUI();
    }

    private void OnClickReturnButtonSCLP()
    {
        isViewSelectCard = false;
        selectCardListPanel.SetActive(isViewSelectCard);
        settingPanelBG.SetActive(isSetting || isViewSelectCard);
    }

    private void OnClickGiveUpButton()
    {
        settingPanel.SetActive(false);
        settingPanelBG.SetActive(false);
        BattleManager.Instance.CurrentBattle.GiveUP();

    }


    public void StartTimer()
    {
        timerCoroutine = StartCoroutine(GamePlayTimer());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    IEnumerator GamePlayTimer()
    {
        float timer = 0f;
        string playTimeText = "00:00";


        while (BattleManager.Instance.CurrentBattle.isBattle)
        {
            timer += Time.unscaledDeltaTime;
            playTimeText = SetTimeText(timer);

            playTime.text = SetTimeText(timer);

            yield return null;
        }

        playTime.text = SetTimeText(timer);
    }

    public string SetTimeText(float time)
    {
        int finalMinutes = (int)(time / 60);
        int finalSeconds = (int)(time % 60);
        return string.Format("{0:00}:{1:00}", finalMinutes, finalSeconds);
    }

    public void KillCountUpdate(int count)
    {
        killCountText.text = BattleManager.Instance.CurrentBattle.killEnemeyCount.ToString("N0");
    }

    public void PlayerLevelUpdate(int count)
    {
        playerLevelText.text = count.ToString();
    }

    public void WaveUpdate(int count, int totalWave)
    {
        waveText.text = $"{count.ToString("D2")}/{totalWave.ToString("D2")}";
    }

    public void ExpBarUpdate(float levelexp, float exp)
    {
        expBarFill += exp / levelexp;
        expBarFill = expBarFill >= 1 ? expBarFill - 1 : expBarFill;
        expBar.fillAmount = expBarFill;
    }

    public void ResultTextUpdate(bool isWin)
    {
        resultText.text = isWin ? "승리!" : "패배..";
    }

    public void GameEndPanelUpdate()
    {
        gameEndPanel.SetActive(true);

        //playTime.text = timerText.text;
        totalKillScore.text = killCountText.text;
    }

    public void GlodUpdate(int amount)
    {
        goldAmount.text = amount.ToString("N0");
    }

    public void ExpUpdate(int amount)
    {
        expAmount.text = amount.ToString("N0");
    }


    public void StartWaveTimer(float duration)
    {
        if (waveTimerCoroutine != null)
            StopCoroutine(waveTimerCoroutine);

        waveTimerCoroutine = StartCoroutine(UpdateWaveTimer(duration));
    }

    private IEnumerator UpdateWaveTimer(float duration)
    {

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            waveTimerText.text = SetTimeText(timer);
            yield return null;
        }

        waveTimerText.text = "00:00";
    }

    // 스킬 사용 큐 생성
    public void AddSkillQueue(Sprite profle, Sprite skillSprite, string skillName)
    {
        UISKillQueue skill = Instantiate(skillQueuePrefab, skillQueueParent).GetComponent<UISKillQueue>();
        skill.Set(profle, skillSprite, skillName);
    }


    public void OnClickGameSpeedButton()
    {
        if (!BattleManager.Instance.CurrentBattle.isBattle) return;

        curGameSpeed++;
        // 3배속 허용 여부에 따라 최대 배속 제한
        if (isFastBuy)
        {
            if (curGameSpeed > 3) curGameSpeed = 1;
        }
        else
        {
            if (curGameSpeed > 2) curGameSpeed = 1;
        }

        if (!isSetting && !BattleManager.Instance.CurrentBattle.isSelectCard) Time.timeScale = curGameSpeed;
        UpdateUI();
    }
    private void UpdateUI()
    {
        speedText.text = $"X{curGameSpeed}";
    }

    private List<SelectedCardDataSlot> selectedCardSlot = new List<SelectedCardDataSlot>();

    public void SettingSelectedCardListUI()
    {
        int dataCount = BattleManager.Instance.BattleUI.SelectCard.selectedCardDict.Count;
        int activeIndex = 0;


        // 필요한 슬롯 개수만큼 활성화하고 데이터 세팅
        foreach (var data in BattleManager.Instance.BattleUI.SelectCard.selectedCardDict)
        {
            SelectedCardDataSlot slot;

            if (activeIndex < selectedCardSlot.Count)
            {
                slot = selectedCardSlot[activeIndex];
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Instantiate(selectCardListSlotPrefab, selectCardListSlottransform)
                       .GetComponent<SelectedCardDataSlot>();
                selectedCardSlot.Add(slot);
            }

            slot.SetSelectedCardDataSlot(data.Value);
            activeIndex++;
        }

        // 남은 풀링된 오브젝트는 비활성화
        for (int i = activeIndex; i < selectedCardSlot.Count; i++)
        {
            selectedCardSlot[i].gameObject.SetActive(false);
        }
    }

    public void SelectCardButtonOn()
    {
        selectCardListButton.gameObject.SetActive(true);

        // 이동 중지
        if (battleCameraMove != null)
            battleCameraMove.Stop();
    }
    public void SelectCardButtonOff()
    {
        selectCardListButton.gameObject.SetActive(false);
    }
    // 카메라 컨트롤러에 스테이지 넓이 전달
    public void SetStage(StageInfoTable stageInfoTable)
    {
        if(battleCameraMove != null)
            battleCameraMove.SetStage(stageInfoTable);
    }

    public float SettingCurGameSpeed()
    {
        if (BattleManager.Instance.CurrentBattle.isSelectCard)
        {
            return 0f;
        }
        else
        {
            return curGameSpeed;
        }
    }

    public void SetActiveButton()
    {
        speedButton.interactable = false;
        settingButton.interactable = false;
    }

    public void SetDeActiveButton()
    {
        speedButton.interactable = true;
        settingButton.interactable = true;
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            SelectCard.ReloadingCards();
        }
#endif
    }
}

