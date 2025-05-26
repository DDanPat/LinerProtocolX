using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class RewardItem
{
    public ItemData ItemData;
    public int amount;
}


public class RewardSystem : MonoBehaviour
{
    private Dictionary<int, RewardItem> rewardList = new Dictionary<int, RewardItem>();
    public List<DropTable> dropTable = new List<DropTable>();
    public List<DropEntryTable> dropEntryTable = new List<DropEntryTable>();

    private int dropTableKey = 0;
    private float expBonus = 1;
    ItemData ItemData;


    public void SetDropTableKey(int key)
    {
        dropTableKey = key;
    }

    public void Init()
    {
        dropTable = GameManager.Instance.DataManager.DropTableLoader.ItemsList;
        dropEntryTable = GameManager.Instance.DataManager.DropEntryTableLoader.ItemsList;

        rewardList.Clear();
        expBonus = BattleManager.Instance.GetScale(6501);
    }


    public void DropItem()
    {
        // 적이 죽었을 때 호출
        int dropItmeKey = GetDropItemKey(dropTableKey);
        AddItem(dropItmeKey, 1);
    }

    private void AddItem(int item, int amount)
    {
        ItemData itemData = GameManager.Instance.Player.inventory.items[item].data;
        if (rewardList.ContainsKey(item))
        {
            rewardList[item].amount += amount;
        }
        else
        {
            rewardList.Add(item, new RewardItem { ItemData = itemData, amount = amount });
        }
    }

    public void TakeInventory()
    {
        var inventory = GameManager.Instance.Player.inventory;
        foreach (var item in rewardList)
        {
            if (inventory.items.TryGetValue(item.Key, out var inventoryItem))
            {
                inventoryItem.amount += item.Value.amount;
            }
        }
    }


    public int GetDropItemKey(int dropTableKey)
    {
        var filtered = dropEntryTable
            .Where(e => e.dropTable == dropTableKey) // 특정 dropTableKey와 일치하는 항목만 필터링
            .OrderBy(e => Random.value) // 동일 가중치 항목 간의 선택 편향을 방지하기 위해 무작위로 정렬
            .ToList();  // 결과를 List<T> 형태로 반환해 추후 루프 및 추첨에 사용

        float totalWeight = filtered.Sum(e => e.weight);
        float rand = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var entry in filtered)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry.item;
        }

        return -1;
    }

    public void RewardUIUpdate()
    {
        // 패시브 버프로 경험치 아이템 추가
        foreach (var item in rewardList.ToList())
        {
            // 경험치 아이템인지 판별
            if (item.Key == 9100 || item.Key == 9101 || item.Key == 9102 || item.Key == 9103)
            {
                // 보너스 결과로 추가분 계산
                float bonus = rewardList[item.Key].amount * (expBonus - 1f);
                int count = Mathf.RoundToInt(bonus);
                // 추가분 획득
                if (count > 0)
                    AddItem(item.Key, count);
            }
        }

        // 리스트 출력
        BattleManager.Instance.BattleUI.RewardUI.SetRewardUI(rewardList);
    }

    public void TakeExpItem(int exp)
    {
        Dictionary<int, int> expItemDict = ExpItemCal(exp);

        foreach (var pair in expItemDict)
        {
            AddItem(pair.Key, pair.Value); // item ID, count
        }
    }
    public Dictionary<int, int> ExpItemCal(int exp)
    {
        Dictionary<int, int> expItems = new Dictionary<int, int>();

        // 경험치 아이템 key값과 대응되는 단위
        int[] expValues = { 250, 100, 50, 20 };
        int[] itemKeys = { 9103, 9102, 9101, 9100 };

        for (int i = 0; i < expValues.Length; i++)
        {
            int count = exp / expValues[i];
            if (count > 0)
            {
                expItems[itemKeys[i]] = count;
                exp %= expValues[i];
            }
        }

        // 반복문 종료 후, 남은 경험치가 있으면 20짜리 아이템(9100) 1개 추가
        if (exp > 0)
        {
            if (expItems.ContainsKey(9100))
                expItems[9100] += 1;
            else
                expItems[9100] = 1;
        }

        return expItems; // key: 아이템 ID, value: 필요한 개수
    }
    public Dictionary<int, int> GetReword()
    {
        Dictionary<int, int> result = new Dictionary<int, int>();
        int[] targets = { 9100, 9101, 9102, 9103, 9200, 9201, 9202, 9203, 9204, 9205, 9206, 9207, 9208 };
        foreach (int key in targets)
        {
            result[key] = rewardList.ContainsKey(key) ? rewardList[key].amount : 0;
        }
        return result;
    }
}
