using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject questSlot;
    private List<QuestSlot> questSlotPool = new List<QuestSlot>();

    [SerializeField] private Button tutorialButton;



    [SerializeField] private GameObject questAlertIcon;

    [Header("일간")]
    [SerializeField] private GameObject dailyQuestPanel;
    [SerializeField] private Button dailyQuestButton;
    [SerializeField] private TextMeshProUGUI dailyQuestButtonText;
    [SerializeField] private Transform dailyQuestTransform;
    [SerializeField] private TextMeshProUGUI dailyQuestTimer;
    [SerializeField] private ScrollRect dailyScrollRect;
    [SerializeField] private GameObject dailyAlert;

    [Header("주간")]
    [SerializeField] private GameObject weeklyQuestPanel;
    [SerializeField] private Button weeklyQuestButton;
    [SerializeField] private TextMeshProUGUI weeklyQuestButtonText;
    [SerializeField] private Transform weeklyQuestTransform;
    [SerializeField] private TextMeshProUGUI weeklyQuestTimer;
    [SerializeField] private ScrollRect weeklyScrollRect;
    [SerializeField] private GameObject weeklyAlert;

    [Header("업적")]
    [SerializeField] private GameObject achievementableQuestPanel;
    [SerializeField] private Button achievementableQuestButton;
    [SerializeField] private TextMeshProUGUI achievementableQuestButtonText;
    [SerializeField] private Transform achievementableQuestTransform;
    [SerializeField] private ScrollRect achievementableScrollRect;
    [SerializeField] private GameObject achieveAlert;

    private Color activeButtonColor;
    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#215F9A", out activeButtonColor);
    }

    private void Start()
    {
        tutorialButton.onClick.AddListener(OpenTutorial);

        dailyQuestButton.onClick.AddListener(OnClickDailyQuestButton);
        weeklyQuestButton.onClick.AddListener(OnClickWeeklyQuestButton);
        achievementableQuestButton.onClick.AddListener(OnClickAchievementableQuestButton);

        dailyQuestPanel.SetActive(true);
        weeklyQuestPanel.SetActive(false);
        achievementableQuestPanel.SetActive(false);

        dailyAlert.SetActive(false);
        weeklyAlert.SetActive(false);
        achieveAlert.SetActive(false);

        SettingQuestSlots();
        InitButtonState();

    }
    private void SetButtonColor(Button button, TextMeshProUGUI buttonText, bool isActive)
    {
        button.image.color = isActive ? activeButtonColor : Color.white;
        buttonText.color = isActive ? Color.white : Color.black;
    }

    private void InitButtonState()
    {
        SetButtonColor(dailyQuestButton, dailyQuestButtonText, true);
        SetButtonColor(weeklyQuestButton, weeklyQuestButtonText, false);
        SetButtonColor(achievementableQuestButton, achievementableQuestButtonText, false);
    }

    private void OnClickDailyQuestButton()
    {
        dailyQuestPanel.SetActive(true);
        weeklyQuestPanel.SetActive(false);
        achievementableQuestPanel.SetActive(false);

        SetButtonColor(dailyQuestButton, dailyQuestButtonText, true);
        SetButtonColor(weeklyQuestButton, weeklyQuestButtonText, false);
        SetButtonColor(achievementableQuestButton, achievementableQuestButtonText, false);

        // 스크롤 맨 위로 초기화
        dailyScrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnClickWeeklyQuestButton()
    {
        dailyQuestPanel.SetActive(false);
        weeklyQuestPanel.SetActive(true);
        achievementableQuestPanel.SetActive(false);

        SetButtonColor(dailyQuestButton, dailyQuestButtonText, false);
        SetButtonColor(weeklyQuestButton, weeklyQuestButtonText, true);
        SetButtonColor(achievementableQuestButton, achievementableQuestButtonText, false);

        weeklyScrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnClickAchievementableQuestButton()
    {
        dailyQuestPanel.SetActive(false);
        weeklyQuestPanel.SetActive(false);
        achievementableQuestPanel.SetActive(true);

        SetButtonColor(dailyQuestButton, dailyQuestButtonText, false);
        SetButtonColor(weeklyQuestButton, weeklyQuestButtonText, false);
        SetButtonColor(achievementableQuestButton, achievementableQuestButtonText, true);

        achievementableScrollRect.verticalNormalizedPosition = 1f;
    }

    public void SettingQuestSlots()
    {
        int index = 0;

        foreach (var data in QuestManager.Instance.GetAllQuestList())
        {
            Transform targetParent = data.period switch
            {
                0 => dailyQuestTransform,
                1 => weeklyQuestTransform,
                _ => achievementableQuestTransform
            };

            QuestSlot slot;
            if (index < questSlotPool.Count)
            {
                slot = questSlotPool[index];
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot = Instantiate(questSlot, targetParent).GetComponent<QuestSlot>();
                questSlotPool.Add(slot);
            }

            slot.transform.SetParent(targetParent); // 혹시나 부모 재지정
            slot.SetQuestData(data);
            index++;
        }

        for (int i = 0; i < questSlotPool.Count; i++)
        {
            if (questSlotPool[i].questData.isClear) questSlotPool[i].gameObject.transform.SetAsLastSibling();
        }

        // 사용하지 않는 슬롯 숨기기
        for (int i = index; i < questSlotPool.Count; i++)
        {
            questSlotPool[i].gameObject.SetActive(false);
        }

        UpdateAlertIconState();
    }

    public void UpdateAlertIconState()
    {
        bool hasAlert = false;
        bool hasDailyAlert = false;
        bool hasWeeklyAlert = false;
        bool hasAchieveAlert = false;
        foreach (var data in QuestManager.Instance.GetAllQuestList())
        {
            //일일
            if (data.period == 0 && data.currentConditionCount >= data.conditionCount && !data.isClear)
            {
                hasDailyAlert = true;
                continue;
            }
            // 주간
            if (data.period == 1 && data.currentConditionCount >= data.conditionCount && !data.isClear)
            {
                hasWeeklyAlert = true;
                continue;
            }
            // 업적
            if ((data.period == 2 || data.period == 3) && data.currentConditionCount >= data.conditionCount && !data.isClear)
            {
                hasAchieveAlert = true;
                continue;
            }
        }
        // 알림 표시
        dailyAlert.SetActive(hasDailyAlert);
        weeklyAlert.SetActive(hasWeeklyAlert);
        achieveAlert.SetActive(hasAchieveAlert);

        int stage = GameManager.Instance.Player.clearStage;
        bool isIconOn = stage > 0 ? true : false;
        questAlertIcon.SetActive(isIconOn && (hasDailyAlert || hasWeeklyAlert || hasAchieveAlert));
    }

    public void SetUITimer(string daily, string weekly)
    {
        dailyQuestTimer.text = $"다음 초기화 까지 남은 시간\n{daily}";
        weeklyQuestTimer.text = $"다음 초기화 까지 남은 시간\n{weekly}";
    }
    private void OnEnable()
    {
        QuestManager.Instance.OnQuestUpdated += SettingQuestSlots;
        SettingQuestSlots();

        // 전체 UI 켜질 때도 스크롤 초기화
        dailyQuestPanel.SetActive(true);
        weeklyQuestPanel.SetActive(false);
        achievementableQuestPanel.SetActive(false);
        dailyScrollRect.verticalNormalizedPosition = 1f;
        weeklyScrollRect.verticalNormalizedPosition = 1f;
        achievementableScrollRect.verticalNormalizedPosition = 1f;

        InitButtonState();
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestUpdated -= SettingQuestSlots;
    }

    public void OpenTutorial()
    {
        UIManager.Instance.tutorialUIHandler.OpenTutorialPanel(TUTORIALTYPE.QUEST);
    }

}
