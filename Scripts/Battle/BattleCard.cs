using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class BattleCard : MonoBehaviour
{
    [SerializeField] public Button selectButton;

    [Header("Tower Info")]
    public int towerKey;
    public int operatorKey;
    [SerializeField] private Image towerIcon;
    [SerializeField] private Image operatorIcon;
    [SerializeField] private GameObject operatorIconObj;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI selectCountText;

    TowerOperList towerInfo;
    CardTable cardInfo;

    //타워 데이터? 프리팹?

    private void Start()
    {
        SetBattleCard();
    }

    public void SetBattleCard()
    {
        selectButton.onClick.AddListener(BattleManager.Instance.CurrentBattle.OnButtonClick);
        selectButton.onClick.AddListener(SelectCard);

        // 포탑 데이터 가져와서 적용시키기
    }

    public void SetInfo(TowerOperList towerInfo, CardTable cardInfo)
    {
        ResetCardInfo();
        this.towerInfo = towerInfo;
        this.cardInfo = cardInfo;

        towerKey = towerInfo.towerData.towerInfoTable.key;
        towerIcon.sprite = towerInfo.towerData.profile;

        if (towerInfo.operatorData != null)
        {
            operatorKey = towerInfo.operatorData.operatorInfoTable.key;
            operatorIcon.sprite = towerInfo.operatorData.profile;
        }

        SetIcon();
        SetTowerDescText();
        SetSelectCountText();
    }

    private void ResetCardInfo()
    {
        towerKey = 0;
        towerIcon.sprite = null;
        operatorKey = -1;
        operatorIcon.sprite = null;
    }

    public void SelectCard()
    {
        // 선택한 포탑 카드 데이터 보내주기
        // 포탑 키값과 오퍼레이터 키 값을 보내주기
        TowerOperList result;

        if (operatorKey == -1)
        {
            result = BattleManager.Instance.inGameTowerOperList.Find(
            item => item.towerData.towerInfoTable.key == towerKey &&
                    item.operatorData == null);
        }
        else
        {
            result = BattleManager.Instance.inGameTowerOperList.Find(
            item => item.towerData.towerInfoTable.key == towerKey &&
                    item.operatorData.operatorInfoTable.key == operatorKey);
        }

        Dictionary<int, TowerTable> invenList = GameManager.Instance.DataManager.TowerTableLoader.ItemsDict;
        var baseTowerStuts = invenList[towerInfo.towerData.towerInfoTable.key];
        float value = (cardInfo.effect) / 100;

        switch (cardInfo.key)
        {
            case 4100:
                BattleManager.Instance.CurrentBattle.uiCardHand.SpawnCard(towerInfo, operatorKey);
                break;
            case 4101:
                //공격 사거리 25% 증가;
                result.towerData.towerInfoTable.range += (baseTowerStuts.range) * value;
                break;
            case 4102:
                //공격력 33% 증가;
                result.towerData.towerInfoTable.attack += (baseTowerStuts.attack) * value;
                break;
            case 4103:
                //공격속도 25% 증가;
                result.towerData.towerInfoTable.attackRate += (baseTowerStuts.attackRate) * value;
                break;
            case 4104:
                //크리티컬 확률 5% 증가;
                result.towerData.towerInfoTable.criticalRate += (baseTowerStuts.criticalRate) * value;
                break;
            case 4105:
                //크리티컬 배율 10% 증가;
                result.towerData.towerInfoTable.criticalCoefficient += (baseTowerStuts.criticalCoefficient) * value;
                break;
            default:
                BattleManager.Instance.CurrentBattle.uiCardHand.SpawnCard(cardInfo.key);
                break;
        }
        BattleManager.Instance.BattleUI.SelectCard.SelectedCard(towerInfo, cardInfo.key);

        // 커스텀 이벤트 - 카드 선택
        AnalyticsManager.Instance.SendEvent(Define.CardSelected, new Dictionary<string, object> {
            { Define.stageKey, BattleManager.Instance.curStage },
            { Define.towerKey, cardInfo.key >= 4900 ? 0 : towerInfo.towerData.towerInfoTable.key },
            { Define.operatorKey, (cardInfo.key >= 4900  ||  operatorKey != -1 ) ?0 : towerInfo.towerData.towerInfoTable.key },
            { Define.cardKey, cardInfo.key },
            });
    }


    public void SetIcon()
    {
        if (operatorKey < 0)
        {
            operatorIconObj.SetActive(false);
            //operatorIcon.gameObject.SetActive(false);
        }
        else
        {
            operatorIconObj.SetActive(true);
            //operatorIcon.gameObject.SetActive(true);
        }
    }

    public void SetTowerDescText()
    {
        //string desc = cardInfo.desc.Replace("\\n", "\n");
        // ex) [에너지폭풍발생기]\n(에너지)\n범위 공격의 영역 25% 증가

        string towerName = towerInfo.towerData.towerInfoTable.name;
        string name = "";
        string type = "";


        int startIdx = towerName.IndexOf('(');
        int endIdx = towerName.IndexOf(')');

        if (startIdx >= 0 && endIdx > startIdx)
        {
            name = towerName.Substring(0, startIdx); // "에너지폭풍발생기"
            type = towerName.Substring(startIdx + 1, endIdx - startIdx - 1); // "에너지"
        }
        else
        {
            name = towerName;
            type = "기능";
        }

        string desc = cardInfo.desc;
        int index = desc.IndexOf("n");

        if (index >= 0)
        {
            desc = cardInfo.desc.Substring(index + 1);
        }


        descText.text = $"[{name}]\n({type})\n{desc}";
    }

    public void SetSelectCountText()
    {
        SelectedCardDataKey key = new SelectedCardDataKey { 
            towerOperData = towerInfo,
            cardKey = cardInfo.key,
        };

        if (BattleManager.Instance.BattleUI.SelectCard.selectedCardDict.ContainsKey(key))
        {
            string count = BattleManager.Instance.BattleUI.SelectCard.selectedCardDict[key].amount.ToString();
            selectCountText.text = $"선택된 횟수 : {count}";

            selectCountText.gameObject.SetActive(true);
        }
        else
        {
            selectCountText.gameObject.SetActive(false);
        }
    }

}
