using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;


public struct SelectedCardDataKey
{
    public TowerOperList towerOperData;
    public int cardKey;
}
public class SelectedCardDataValue
{
    public TowerOperList towerOperData;
    public CardTable cardData;
    public int amount;
}

public class TestData
{
    public TowerOperList towerOperData;
    public int weight;
}

public class SelectCard : MonoBehaviour
{
    [SerializeField] private List<BattleCard> battleCardList = new List<BattleCard>(); // 게임 화면에서 보이는 카드UI를 담는 리스트
    [SerializeField] private BattleCard battleCardPrefab; // 선택 슬롯 카드 프리팹
    [SerializeField] private Transform battleCardSlot; // 선택 슬롯 카드 프리팹을 생성할 위치

    int isFirst = 0; // 슬롯을 처음 만드는 것인지 확인 하는 변수

    public List<TowerOperList> towerCardList = new List<TowerOperList>(); // 인벤토리에서 편성한 덱
    public List<CardTable> reinforcementKeys = new List<CardTable> (); // 카드 능력

    // Key: 덱 카드 (TowerOperList), Value: 해당 카드에 할당된 강화 번호 리스트
    private Dictionary<TowerOperList, List<CardTable>> reinforcementAssign = new Dictionary<TowerOperList, List<CardTable>>();

    // 슬롯에 부여한 카드를 담는 리스트
    private Dictionary<TowerOperList, List<CardTable>> nowReinforcementAssign = new Dictionary<TowerOperList, List<CardTable>>();
    
    private List<TestData> copyTowerCardList = new List<TestData>(); // 편성 카드를 복제한 리스트


    //테스트 구간
    public List<TowerOperList> towerToolList;

    public Dictionary<SelectedCardDataKey, SelectedCardDataValue> selectedCardDict = new Dictionary<SelectedCardDataKey, SelectedCardDataValue> ();

    bool isToolCardAdded = false;

    public void Init()
    {
        isFirst = 0;
        
        battleCardList.Clear();
        SettingList();
    }

    public void AddToolCard()
    {
        if (isToolCardAdded) return;

        foreach (var data in towerToolList)
        {
            towerCardList.Add(data);
        }

        int j = 0;
        for (int i = towerCardList.Count - 3; i < towerCardList.Count; i++)
        {
            TowerOperList deckCard = towerCardList[i];
            reinforcementAssign.Add(deckCard,
                new List<CardTable> { reinforcementKeys[reinforcementKeys.Count - (3 - j)] });
            j++;
        }

        isToolCardAdded = true;
    }

    public void SettingList()
    {
        towerCardList = BattleManager.Instance.inGameTowerOperList;

        reinforcementKeys = GameManager.Instance.DataManager.CardTableLoader.ItemsList;

        //처음에는 덱 배치카드만 추가
        for (int i = 0; i < towerCardList.Count; i++)
        {
            TowerOperList deckCard = towerCardList[i];
            reinforcementAssign.Add(deckCard, new List<CardTable> { reinforcementKeys[0] });
        }
        
        
    }

    private void CopyCardKey()
    {
        foreach (var data in reinforcementAssign)
        {
            nowReinforcementAssign[data.Key] = new List<CardTable>(data.Value); // 리스트 복제
        }
    }

    private void CopyTowerCardKey()
    {
        copyTowerCardList.Clear();

        int normalWeight = 45;  // 일반 카드 출현 비중
        int toolWeight = 5;    // 도구 카드 출현 비중

        foreach (var data in towerCardList)
        {
            int repeatCount = IsToolCard(data) ? toolWeight : normalWeight;

            copyTowerCardList.Add(new TestData
            {
                towerOperData = data,
                weight = repeatCount,
            });
        }
    }

    bool IsToolCard(TowerOperList data)
    {
        return towerToolList.Any(tool => tool.towerData.towerInfoTable.key == data.towerData.towerInfoTable.key);
    }

    private void AddCardSlot()
    {
        int currentCount = battleCardList.Count;
        int maxSlotCount = 3;

        // 최초 실행 시 덱 수에 따라 최대 3개까지 생성
        if (isFirst < 1)
        {
            int createCount = Mathf.Min(towerCardList.Count, maxSlotCount);
            CreateCardSlots(createCount);
        }
        // 이후 실행 시 슬롯이 부족한 경우만 추가 생성
        else if (isFirst > 1 && currentCount < maxSlotCount)
        {
            int createCount = maxSlotCount - currentCount;
            CreateCardSlots(createCount);
        }
    }

    private void CreateCardSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            BattleCard battleCard = Instantiate(battleCardPrefab, battleCardSlot);
            battleCardList.Add(battleCard);
        }
    }


    public void SetRandomCards()
    {
        AddCardSlot();
        ReloadingCards();
        if (isFirst > 1) AddToolCard();
        isFirst++;
    }

    public void ReloadingCards()
    {
        // 강화 카드 키 초기화
        CopyTowerCardKey();
        CopyCardKey();

        BattleManager.Instance.BattleUI.ShowReloadButton();
        // TODO 처음 배치 카드는 타워 카드만

        for (int i = 0; i < battleCardList.Count; i++)
        {
            TowerOperList selectTowerCardListData = GetWeightedRandomTowerCard();
            int index = copyTowerCardList.FindIndex(d => d.towerOperData == selectTowerCardListData);
            index = GetTowerReinforcementKey(selectTowerCardListData, index);

            //int randomNum = Random.Range(0, copyTowerCardList.Count);
            //TowerOperList selectTowerCardListData = copyTowerCardList[randomNum].towerOperData;
            //randomNum = GetTowerReinforcementKey(selectTowerCardListData, randomNum);


            int selectCardListData = GetAvailableReinforcementKey(selectTowerCardListData);
            if (reinforcementAssign.ContainsKey(selectTowerCardListData))
            {
                battleCardList[i].GetComponent<BattleCard>().SetInfo(selectTowerCardListData, nowReinforcementAssign[selectTowerCardListData][selectCardListData]);
                nowReinforcementAssign[selectTowerCardListData].RemoveAt(selectCardListData);
            }
        }
    }

    private TowerOperList GetWeightedRandomTowerCard()
    {
        int totalWeight = copyTowerCardList.Sum(data => data.weight);
        int randomWeight = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var data in copyTowerCardList)
        {
            currentSum += data.weight;
            if (randomWeight < currentSum)
            {
                return data.towerOperData;
            }
        }

        // fallback: 오류 방지를 위해 마지막 원소 반환
        return copyTowerCardList.Last().towerOperData;
    }

    int GetAvailableReinforcementKey(TowerOperList deckCard)
    {
        // 덱 카드에 배치 카드만 있으면 배치카드 return
        // 아니면 강화 카드 번호를 부여
        if (reinforcementAssign[deckCard].Count == 1)
        {
            return 0;
        }
        else
        {
            
            int i = Random.Range(0, nowReinforcementAssign[deckCard].Count);
            return i;
        }
    }

    int GetTowerReinforcementKey(TowerOperList deckCard, int num)
    {
        // 타워 카드가 한번이라도 뽑힌 적이 없으면 배치 카드의 중복 제거
        // 아니면 타워 카드 중복 가능
        if (nowReinforcementAssign != null)
        {
            if (reinforcementAssign[deckCard].Count == 1)
            {
                copyTowerCardList.RemoveAt(num);
                return num;
            }
            else
            {
                return num;
            }
        }
        return num;
    }

    public void SelectedCard(TowerOperList deckCard, int cardKey = 4100)
    {
        SelectedCardDataKey cardData = new SelectedCardDataKey { towerOperData = deckCard, cardKey = cardKey};
        CardTable cardTable = reinforcementKeys.Find(card => card.key == cardKey);
        if (selectedCardDict.ContainsKey(cardData))
        {
            selectedCardDict[cardData].amount++;

            int currentAmount = selectedCardDict[cardData].amount;

            if (reinforcementAssign.TryGetValue(deckCard, out var list))
            {
                if ((cardKey == 4101 && currentAmount >= 3) ||
                    (cardKey == 4103 && currentAmount >= 4) ||
                    (cardKey == 4104 && currentAmount >= 3))
                {
                    list.RemoveAll(card => card.key == cardKey);
                }
            }
        }
        else
        {
            selectedCardDict.Add(cardData, new SelectedCardDataValue
            {
                towerOperData = deckCard,
                cardData = cardTable,
                amount = 1
            });
        }
        ListDebug(deckCard, cardKey);

        if (isFirst > 1)
        {
            // 한번이라도 뽑은 적이 있으면 return
            if (reinforcementAssign[deckCard].Count > 1 || reinforcementAssign[deckCard][0].key >= 4900)
                return;
            // 한번도 뽑은 적이 없는 카드면 강화 카드 키 값들 추가
            else
            {
                for (int i = 1; i < reinforcementKeys.Count - 3; i++)
                {
                    reinforcementAssign[deckCard].Add(reinforcementKeys[i]);
                }
            }
        }
        
    }

    /// <summary>
    /// 디버그용 메서드
    private void ListDebug(TowerOperList deckCard, int cardKey)
    {
        // reinforcementAssign 리스트의 key값 전부 출력
        if (reinforcementAssign.TryGetValue(deckCard, out var assignedCards))
        {
            string keyList = $"[SelectedCard] cardKey: {cardKey} / reinforcementAssign key list: ";
            foreach (var card in assignedCards)
            {
                keyList += card.key + ", ";
            }
        }
    }
    /// </summary>

    public Sprite GetSprite(int itemKey)
    {
        int key = 3000 + itemKey % 100;
        foreach (var info in towerToolList)
        {
            if (info.towerData.towerInfoTable.key != key)
                continue;
            else
            {
                return info.towerData.profile;
            }
        }
        // 찾지 못함
        return null;
    }
}
