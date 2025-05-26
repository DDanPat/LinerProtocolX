using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private Transform spwonPoint;
    [SerializeField] private RewardSlotsBase rewardSlotsPrefabs;

    public void SetRewardUI(Dictionary<int, RewardItem> rewardList)
    {
        foreach (var rewardItem in rewardList.OrderBy(r => r.Key))
        {
            RewardSlotsBase rewardSlots = Instantiate(rewardSlotsPrefabs, spwonPoint);
            rewardSlots.SetData(rewardItem.Value);
        }
    }

}
