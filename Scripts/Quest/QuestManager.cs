using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public int key; // 고유 ID
    public string name;
    public int period; // 0: 일일, 1: 주간 등
    public QuestConditions firstCondition;
    public QuestConditions secondCondition;
    public int conditionIdx;
    public string questDesc;
    public int conditionCount; // ex) 목표 횟수 (3회 등)
    public int currentConditionCount;
    public int randomReward;
    public List<int> rewardKey; // 보상 ID
    public int rewardAmount;
    public bool isClear;
}

public class QuestManager : Singleton<QuestManager>
{
    public List<QuestData> dailyQuestList = new List<QuestData>();
    public List<QuestData> weeklyQuestList = new List<QuestData>();
    public List<QuestData> achievementQuestList = new List<QuestData>();

    public bool dailyQuestReset = false;
    public bool weeklyQuestReset = false;


    public event System.Action OnQuestUpdated;
    private bool isInit; // 초기화 - 퀘스트 불러왔는지 여부
    private TimerHandler _timerHandler;
    protected override void Awake()
    {
        base.Awake();
        _timerHandler = GetComponent<TimerHandler>();
        if (_timerHandler == null)
        {
            gameObject.AddComponent<TimerHandler>();
        }
    }
    private void Start()
    {
        SetQuestData();
        DailyLogin();
    }
    public void SetQuestData()
    {
        if (isInit) return;
        foreach (var data in GameManager.Instance.DataManager.QuestTableLoader.ItemsList)
        {
            QuestData questData = new QuestData
            {
                key = data.key,
                name = data.name,
                period = data.period,
                questDesc = data.conditionDesc,
                firstCondition = data.firstCondition,
                secondCondition = data.secondCondition,
                conditionIdx = data.conditionIdx,
                conditionCount = data.conditionCount,
                currentConditionCount = 0,
                randomReward = data.randomReward,
                rewardKey = data.rewardKey,
                rewardAmount = data.rewardNum,
                isClear = false
            };

            switch (data.period)
            {
                case 0: // 일일 퀘스트
                    dailyQuestList.Add(questData);
                    break;

                case 1: // 주간 퀘스트
                    weeklyQuestList.Add(questData);
                    break;

                default: // 반복형 퀘스트 또는 업적 등
                    achievementQuestList.Add(questData);
                    break;
            }
        }
        achievementQuestList = GetAvailableRepeatableQuests(achievementQuestList);
        if (UIManager.Instance != null)
            UIManager.Instance.questUIHandler.UpdateAlertIconState();
        isInit = true;
    }

    public IEnumerable<QuestData> GetAllQuestList()
    {
        foreach (var q in dailyQuestList) yield return q;
        foreach (var q in weeklyQuestList) yield return q;
        foreach (var q in achievementQuestList) yield return q;
    }



    public void UpdateQuestProgress(QuestConditions firstType, QuestConditions secondType = 0, int amount = 1, int conditionIdx = 0)
    {
        foreach (var quest in GetAllQuestList())
        {
            // 퀘스트 설명이 조건 타입과 매칭되는지 확인 (조건 확인용 필드가 필요함)
            if (quest.isClear) continue; // 이미 클리어된 퀘스트는 스킵

            if (quest.firstCondition == firstType && 
                (quest.secondCondition == secondType || quest.secondCondition == QuestConditions.repeatable )&&
                (quest.conditionIdx == conditionIdx || quest.conditionIdx == 0))
            {
                quest.currentConditionCount += amount;
            }
        }
        OnQuestUpdated?.Invoke(); // UI 갱신 알림
        if (UIManager.Instance != null)
        {
            UIManager.Instance.questUIHandler.SettingQuestSlots();
            UIManager.Instance.questUIHandler.UpdateAlertIconState();
        }
    }


    public List<QuestData> GetAvailableRepeatableQuests(List<QuestData> allRepeatQuests)
    {
        List<QuestData> availableQuests = new List<QuestData>();

        foreach (var quest in allRepeatQuests)
        {
            if (quest.period != 2)
            {
                availableQuests.Add(quest);
            }
            else
            {
                // 선행 퀘스트가 없거나 이미 클리어한 경우만 표시
                if (quest.conditionIdx == 0 ||
                    IsQuestCleared(quest.conditionIdx))
                {
                    availableQuests.Add(quest);
                }
            }
        }

        return availableQuests;
    }

    private bool IsQuestCleared(int key)
    {
        var quest = achievementQuestList.Find(q => q.key == key);
        return quest.isClear;
    }

    private void OnEnable()
    {
        TimerHandler.OnDailyReset += ResetDailyQuests;
        TimerHandler.OnWeeklyReset += ResetWeeklyQuests;
    }

    private void OnDisable()
    {
        TimerHandler.OnDailyReset -= ResetDailyQuests;
        TimerHandler.OnWeeklyReset -= ResetWeeklyQuests;
    }

    // 타이머 초기화
    public void InitTimer()
    {
        if (_timerHandler == null)
        {
            _timerHandler = GetComponent<TimerHandler>();
        }
        _timerHandler.Init();
    }
    // 초기화 시간 반환
    public DateTime GetNextDailyResetTime()
    {
        return _timerHandler.GetNextDailyResetTime();
    }
    // 초기화 시간 반환
    public DateTime GetNextWeeklyResetTime()
    {
        return _timerHandler.GetNextWeeklyResetTime();
    }

    private void ResetDailyQuests()
    {
        // 일일 퀘스트 초기화 로직
        foreach (var data in dailyQuestList)
        {
            data.isClear = false;
            data.currentConditionCount = 0;
        }
        OnQuestUpdated?.Invoke();
        
    }

    private void ResetWeeklyQuests()
    {
        // 주간 퀘스트 초기화 로직
        foreach (var data in weeklyQuestList)
        {
            data.isClear = false;
            data.currentConditionCount = 0;
        }
        OnQuestUpdated?.Invoke();
    }

    public void DailyLogin()
    {
        if (UIManager.Instance.ShouldDailyLogin)
        {
            UpdateQuestProgress(QuestConditions.login);
            UIManager.Instance.ShouldDailyLogin = false;
        }
    }
}