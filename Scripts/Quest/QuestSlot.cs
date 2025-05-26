using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private Image rewardIcon; // 보상 아이템 아이콘
    [SerializeField] private TextMeshProUGUI rewardValue; // 보상 아이템 수량
    [SerializeField] private GameObject rewardAlertIcon; // 보상 수령 가능 여부 표시
    [SerializeField] private Button questSlotButton;
    [SerializeField] private GameObject clearOverlay;

    public QuestData questData;

    private bool isAlert;
    public bool IsAlert => isAlert;

    private void Start()
    {
        questSlotButton.onClick.AddListener(QuestClear);
    }

    public void SetQuestData(QuestData data)
    {
        this.questData = data;
        questSlotButton.enabled = true;
        questName.text = $"{data.name} ({data.currentConditionCount}/{data.conditionCount})";
        SetIcon();
        SetRewardCount();
        clearOverlay.SetActive(false);

        if (data.isClear)
        {
            rewardAlertIcon.SetActive(false);
            questSlotButton.enabled = false;
            clearOverlay.SetActive(true); // 완료 시 오버레이 활성화
            return;
        }


        isAlert = data.currentConditionCount >= data.conditionCount;

        rewardAlertIcon.SetActive(isAlert);
        if (isAlert)
        {
            this.gameObject.transform.SetAsFirstSibling();
        }
    }

    public void QuestClear()
    {
        if (isAlert)
        {
            questData.isClear = true;
            this.gameObject.transform.SetAsLastSibling();
            rewardAlertIcon.SetActive(false);
            questSlotButton.enabled = false;
            clearOverlay.SetActive(true); // 완료 시 오버레이 활성화

            TakeReward();

            if (questData.period == 0) QuestManager.Instance.UpdateQuestProgress(QuestConditions.questComplete, QuestConditions.none);

            UIManager.Instance.questUIHandler.UpdateAlertIconState();
        }

    }

    public void TakeReward()
    {
        List<int> rewards = questData.rewardKey;
        var inventory = GameManager.Instance.Player.inventory;

        if (questData.randomReward == 0)
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                if (inventory.items.ContainsKey(rewards[i]))
                {
                    inventory.items[rewards[i]].amount += questData.rewardAmount;
                }
                else if (rewards[i] == 9900)
                {
                    GameManager.Instance.Player.Gem += questData.rewardAmount;
                    // 커스텀 이벤트 - 결정석 변화
                    AnalyticsManager.Instance.SendEvent(Define.CrystalChanged, new Dictionary<string, object> {
                        { Define.changeType, Define.gained },
                        { Define.numberCrystalChanged, questData.rewardAmount},
                        { Define.changeSource, 2},
                        { Define.numberCrystalBefore, GameManager.Instance.Player.Gem - questData.rewardAmount },
                    });
                }
                else if (rewards[i] == 9901)
                {
                    GameManager.Instance.Player.Gold += questData.rewardAmount;
                    // 커스텀 이벤트 - 코인 변화
                    AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
                        { Define.changeType, Define.gained },
                        { Define.numberCoinChanged, questData.rewardAmount},
                        { Define.changeSource, 8},
                        { Define.numberCoinBefore, GameManager.Instance.Player.Gold - questData.rewardAmount},
                    });
                }
                else if (inventory.towers.ContainsKey(rewards[i]))
                {
                    inventory.towers[rewards[i]].isOwned = true;
                }
            }
        }
        else
        {
            int randomRewardKey = Random.Range(0, rewards.Count);
            if (inventory.items.ContainsKey(rewards[randomRewardKey]))
            {
                inventory.items[rewards[randomRewardKey]].amount += questData.rewardAmount;
            }
        }
        // 임의 저장
        GameManager.Instance.SavePlayer();
        // 커널 - 업적 달성 - 스테이지 클리어 보상 수령
        switch (questData.key)
        {
            case 13300:
                // 1 스테이지
                AnalyticsManager.Instance.SendStep(101);
                break;
            case 13301:
                // 2 스테이지
                AnalyticsManager.Instance.SendStep(209);
                break;
            case 13302:
                // 3 스테이지
                AnalyticsManager.Instance.SendStep(410);
                break;
            case 13304:
                // 4 스테이지
                AnalyticsManager.Instance.SendStep(610);
                break;
            case 13306:
                // 5 스테이지
                AnalyticsManager.Instance.SendStep(710);
                break;
            case 13308:
                // 6 스테이지
                AnalyticsManager.Instance.SendStep(910);
                break;
            case 13310:
                // 7 스테이지 클리어
                AnalyticsManager.Instance.SendStep(1101);
                break;
            case 13312:
                // 8 스테이지 클리어
                AnalyticsManager.Instance.SendStep(1203);
                break;
            case 13313:
                // 9 스테이지 클리어
                AnalyticsManager.Instance.SendStep(1207);
                break;
            case 13314:
                // 10 스테이지 클리어
                AnalyticsManager.Instance.SendStep(1211);
                break;
            case 13315:
                // 11 스테이지 클리어
                AnalyticsManager.Instance.SendStep(1215);
                break;
        }
    }

    private void SetIcon()
    {
        int itemKey = questData.rewardKey[0];

        if (GameManager.Instance.Player.inventory.items.ContainsKey(itemKey))
        {
            rewardIcon.sprite = GameManager.Instance.Player.inventory.items[itemKey].data.icon;
        }
        else if (GameManager.Instance.Player.inventory.towers.ContainsKey(itemKey))
        {
            rewardIcon.sprite = GameManager.Instance.Player.inventory.towers[itemKey].data.profile;
        }
        else if (itemKey == 9900)
        {
            rewardIcon.sprite = Resources.Load<Sprite>(path: "ItemIcons/12300_Crystal");
        }
        else if (itemKey == 9901)
        {
            rewardIcon.sprite = Resources.Load<Sprite>(path: "ItemIcons/12310_Coin");
        }
    }

    private void SetRewardCount()
    {
        if (questData.rewardAmount == 0)
        {
            rewardValue.text = questData.rewardKey.Count.ToString();
        }
        else
        {
            rewardValue.text = questData.rewardAmount.ToString();
        }
    }

}
