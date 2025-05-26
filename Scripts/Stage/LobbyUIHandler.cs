using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject popupBG;

    public Button[] mainButtons;  // 메인 ui 패널 버튼
    private Button lastClickedButton = null;  // 마지막 클릭된 버튼을 저장
    private bool isSliding = false;  // 패널 슬라이드 중인지 확인

    [SerializeField] private PanelSlider panelSlider; // PanelSlider 부착된 오브젝트 연결

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI goldValueText;
    [SerializeField] private TextMeshProUGUI gemValueText;
    [SerializeField] private Button topShopPlusBtn;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPopupPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingscloseButton;

    [Header("Messenger")]
    [SerializeField] private GameObject messengerPopupPanel;
    [SerializeField] private Button messengerButton;
    [SerializeField] private Button closeButton;

    [Header("Quests")]
    [SerializeField] private GameObject questsPopupPanel;
    [SerializeField] private Button questsButton;
    [SerializeField] private Button questscloseButton;

    [Header("SelectStage")]
    [SerializeField] private GameObject selectStagePopupPanel;
    [SerializeField] private Button selectStageButton;
    [SerializeField] private Button selectStagecloseButton;
    public ItemInfoPopup ItemInfoPopup;

    [Header("Language")]
    [SerializeField] private GameObject languagePopupPanel;
    [SerializeField] private Button languageSelectButton;
    [SerializeField] private Button languagePopupcloseButton;
    [Header("Credit")]
    [SerializeField] private GameObject creditPopupPanel;
    [SerializeField] private Button creditOpenButton;
    [SerializeField] private Button creditCloseButton;


    [Header("shopPanels")]
    [SerializeField] private GameObject dailyShopPanel;
    [SerializeField] private GameObject shopPanel;

    [SerializeField] private Button dailyShopPanelButton;
    [SerializeField] private Button shopPanelButton;
    [SerializeField] private TextMeshProUGUI dailyShopBtnText;
    [SerializeField] private TextMeshProUGUI shopPanelBtnText;
    [Header("sprites")]
    [SerializeField] private Sprite enableSprite;
    [SerializeField] private Sprite disableSprite;

    OpenSystemPopup openSystemPopup;
    private Color activeButtonColor;

    private void Awake()
    {
        UnityEngine.ColorUtility.TryParseHtmlString("#215F9A", out activeButtonColor);
        openSystemPopup = GetComponent<OpenSystemPopup>();

        // 버튼 이벤트 연결
        messengerButton?.onClick.AddListener(OpenMessengerPopup);
        closeButton?.onClick.AddListener(CloseMessengerPopup);

        settingsButton?.onClick.AddListener(OpenSettingsPopup);
        settingscloseButton?.onClick.AddListener(CloseSettingsPopup);

        questsButton?.onClick.AddListener(OpenQuestsPopup);
        questscloseButton?.onClick.AddListener(CloseQuestsPopup);

        selectStageButton?.onClick.AddListener(OpenSelectStagePopup);
        selectStagecloseButton?.onClick.AddListener(CloseSelectStagePopup);

        languageSelectButton?.onClick.AddListener(OpenLanguagePopup);
        languagePopupcloseButton?.onClick.AddListener(CloseLanguagePopup);

        creditOpenButton?.onClick.AddListener(OpenCreditPopup);
        creditCloseButton?.onClick.AddListener(CloseCreditPopup);

        dailyShopPanelButton.onClick.AddListener(OnClickDailyShopPanelButton);
        shopPanelButton.onClick.AddListener(OnClickShopPanelButton);

        // topShopPlusBtn 클릭 시 4번 패널을 보여줌
        topShopPlusBtn.onClick.AddListener(() =>
        {
            mainButtons[4].onClick.Invoke(); // A버튼 클릭 시 B버튼 강제 클릭
        });

        // 시작 시 팝업 숨김
        popupBG?.SetActive(false);
        messengerPopupPanel?.SetActive(false);
        settingsPopupPanel?.SetActive(false);
        questsPopupPanel?.SetActive(false);
        selectStagePopupPanel?.SetActive(false);
        languagePopupPanel?.SetActive(false);
        creditPopupPanel?.SetActive(false);
    }
    void Start()
    {
        foreach (Button button in mainButtons)
        {
            button.onClick.AddListener(() => OnMenuBtn(button));
        }
        // topShopPlusBtn 클릭 시 4번(상점 열기, 재화 상점)
        topShopPlusBtn.onClick.AddListener(() => mainButtons[4].onClick.Invoke());
        topShopPlusBtn.onClick.AddListener(() => shopPanelButton.onClick.Invoke());
        // 초기 화면 설정
        mainButtons[2].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        lastClickedButton = mainButtons[2];

        SetResourcesUpdate();
        InitButtonState();
    }

    // 메신저
    private void OpenMessengerPopup()
    {
        // 스테이지 진행도 확인
        int stage = GameManager.Instance.Player.clearStage;
        if (stage < 0)
        {
            // 팝업 출력
            ShowPopupMessage("스테이지 1 클리어 이후 해금됩니다.");
            return;
        }
        
        popupBG?.SetActive(true);
        messengerPopupPanel.SetActive(true);
    }

    private void CloseMessengerPopup()
    {
        popupBG?.SetActive(false);
        messengerPopupPanel.SetActive(false);
    }

    // 설정
    private void OpenSettingsPopup()
    {
        popupBG?.SetActive(true);
        settingsPopupPanel.SetActive(true);
    }

    private void CloseSettingsPopup()
    {
        popupBG?.SetActive(false);
        settingsPopupPanel.SetActive(false);
    }

    // 퀘스트
    private void OpenQuestsPopup()
    {
        // 스테이지 진행도 확인
        int stage = GameManager.Instance.Player.clearStage;
        if (stage < 0)
        {
            // 팝업 출력
            ShowPopupMessage("스테이지 1 클리어 이후 해금됩니다.");
            return;
        }


        popupBG?.SetActive(true);
        questsPopupPanel.SetActive(true);
        // 커널 전송
        AnalyticsManager.Instance.SendStep(100);
    }

    private void CloseQuestsPopup()
    {
        popupBG?.SetActive(false);
        questsPopupPanel.SetActive(false);
    }

    // 스테이지 선택 진입
    private void OpenSelectStagePopup()
    {
        // 커널 전송 - 로비에서 전투 시작 클릭
        AnalyticsManager.Instance.SendStep(50);
        // 편성을 스테이지와 비교
        StageManager.Instance.stageHandler.CheckWarning();
        popupBG?.SetActive(true);
        selectStagePopupPanel?.SetActive(true);
    }

    private void CloseSelectStagePopup()
    {
        popupBG?.SetActive(false);
        selectStagePopupPanel?.SetActive(false);
    }

    // 언어 선택
    private void OpenLanguagePopup()
    {
        popupBG?.SetActive(true);
        languagePopupPanel?.SetActive(true);
    }

    private void CloseLanguagePopup()
    {
        languagePopupPanel?.SetActive(false);
    }

    // 크레딧
    private void OpenCreditPopup()
    {
        popupBG?.SetActive(true);
        creditPopupPanel?.SetActive(true);
    }

    private void CloseCreditPopup()
    {
        creditPopupPanel?.SetActive(false);
    }

    private void ShowPanel(int panelNumber)
    {
        dailyShopPanel.SetActive(panelNumber == 1);
        shopPanel.SetActive(panelNumber == 2);
    }

    private void InitButtonState()
    {
        SetButtonColor(dailyShopPanelButton, dailyShopBtnText, true);
        SetButtonColor(shopPanelButton, shopPanelBtnText, false);
    }

    private void SetButtonColor(Button button, TextMeshProUGUI buttonText, bool isActive)
    {
        button.image.color = isActive ? activeButtonColor : Color.white;
        buttonText.color = isActive ? Color.white : Color.black;
    }

    private void OnClickDailyShopPanelButton()
    {
        ShowPanel(1);
        SetButtonColor(dailyShopPanelButton, dailyShopBtnText, true);
        SetButtonColor(shopPanelButton, shopPanelBtnText, false);
    }

    private void OnClickShopPanelButton()
    {
        ShowPanel(2);
        SetButtonColor(dailyShopPanelButton, dailyShopBtnText, false);
        SetButtonColor(shopPanelButton, shopPanelBtnText, true);
    }


    void OnMenuBtn(Button clickedButton)
    {
        // 해당 버튼에 맞는 패널 인덱스를 얻어옴
        int index = System.Array.IndexOf(mainButtons, clickedButton);

        // 해당 기능을 사용할 수 있는지 확인
        if (!CanOpenMenu(index))
        {
            // 못 연다면 팝업 출력 후 종료
            ShowMenuPopup(index);
            return;
        }

        // 슬라이드가 진행 중일 때는 버튼 크기 변경을 막음
        if (isSliding || clickedButton == lastClickedButton || panelSlider.CheckOrganize()) return;

        // 슬라이드 시작
        isSliding = true;

        // 이전 버튼 크기 복구
        if (lastClickedButton != null && lastClickedButton != clickedButton)
        {
            lastClickedButton.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
        }

        // 클릭된 버튼 크기 20% 키움 (피벗을 0으로 세팅하여 0.1f 만 사용하여도 20%상승)
        clickedButton.transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
        lastClickedButton = clickedButton;

        // 슬라이드 애니메이션 실행
        panelSlider.ShowPanel(index);

        // 커널 전송
        switch (index)
        {
            case 0:
                // 인벤토리
                AnalyticsManager.Instance.SendStep(800);
                break;
            case 1:
                // 편성
                AnalyticsManager.Instance.SendStep(300);
                break;
            case 3:
                // 제작
                AnalyticsManager.Instance.SendStep(500);
                break;
            case 4:
                // 상점
                AnalyticsManager.Instance.SendStep(1100);
                break;
        }
    }

    // 슬라이드가 끝났을 때 호출되는 메서드
    public void OnSlideComplete()
    {
        isSliding = false;  // 슬라이드가 끝나면 버튼 클릭 가능 상태로 전환
    }

    public void SetResourcesUpdate()
    {
        goldValueText.text = GameManager.Instance.Player.Gold.ToString("N0");
        gemValueText.text = GameManager.Instance.Player.Gem.ToString("N0");
    }

    private void OnEnable()
    {
        Player.OnResourceChanged += SetResourcesUpdate;
    }

    private void OnDisable()
    {
        Player.OnResourceChanged -= SetResourcesUpdate;
    }

    // 진행 단계에 따른 버튼 활성/비활성
    public void UpdateButton()
    {
        int stage = GameManager.Instance.Player.clearStage;
        if (stage >= 1)
        {
            questsButton.GetComponent<Image>().sprite = enableSprite;
            messengerButton.GetComponent<Image>().sprite = enableSprite;
            openSystemPopup.SetOpenSystemPopup(0);
            openSystemPopup.SetOpenSystemPopup(1);
        }
        foreach (var button in mainButtons)
            button.GetComponent<Image>().sprite = disableSprite;
        mainButtons[2].GetComponent<Image>().sprite = enableSprite;

        if (stage >= 2)
        {
            mainButtons[1].GetComponent<Image>().sprite = enableSprite;
            openSystemPopup.SetOpenSystemPopup(2);
            TutorialManager.Instance?.Intialize();
        }

        if (stage >= 3)
        {
            mainButtons[3].GetComponent<Image>().sprite = enableSprite;
            openSystemPopup.SetOpenSystemPopup(3);
        }

        if (stage >= 5)
        {
            mainButtons[0].GetComponent<Image>().sprite = enableSprite;
            openSystemPopup.SetOpenSystemPopup(4);
        }

        if (stage >= 7)
        {
            mainButtons[4].GetComponent<Image>().sprite = enableSprite;
            topShopPlusBtn.GetComponent<Image>().sprite = enableSprite;
            openSystemPopup.SetOpenSystemPopup(5);
        }
    }
    private bool CanOpenMenu(int index)
    {
        int stage = GameManager.Instance.Player.clearStage;
        if (index == 0)
        {
            // 인벤토리 5 스테이지 클리어 후
            return stage >= 5;
        }
        else if (index == 1)
        {
            // 편성 2 스테이지 클리어 후
            return stage >= 2;
        }
        else if (index == 2)
        {
            // 로비는 항상 가능
            return true;
        }
        else if (index == 3)
        {
            // 제작 3 스테이지 클리어 후
            return stage >= 3;
        }
        else if (index == 4)
        {
            // 상점 7 스테이지 클리어 후
            return stage >= 7;
        }
        return false;
    }
    private void ShowMenuPopup(int index)
    {
        switch (index)
        {
            case 0:
                // 인벤토리
                ShowPopupMessage("스테이지 5 클리어 이후 해금됩니다");
                break;
            case 1:
                // 편성
                ShowPopupMessage("스테이지 2 클리어 이후 해금됩니다");
                break;
            case 3:
                // 제작
                ShowPopupMessage("스테이지 3 클리어 이후 해금됩니다.");
                break;
            case 4:
                // 상점
                ShowPopupMessage("스테이지 7 클리어 이후 해금됩니다.");
                break;
        }
    }
    // 편성성으로 빠른 이동
    public void QuickOrganize()
    {
        // 창 닫기
        CloseSelectStagePopup();
        // 편성 창 활성화
        OnMenuBtn(mainButtons[1]);
    }

    // 팝업 출력
    public void ShowPopupMessage(string message)
    {
        openSystemPopup.ShowPopupMessage(message);
    }


}
