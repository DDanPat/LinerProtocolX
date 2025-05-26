using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotsBase : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemAmountText;
    public Button itemButton;

    public RewardItem rewardItem;
    public EnemyTable enemydata;


    public void SetData(RewardItem reward)
    {
        if (reward == null || reward.ItemData == null)
        {
            Debug.LogError("RewardItem 또는 ItemData가 null입니다.");
            return;
        }

        this.rewardItem = reward;

        if (reward.ItemData.icon != null)
        {
            itemIcon.sprite = reward.ItemData.icon;
        }
        else
        {
            Debug.LogWarning($"[SetData] 아이콘 없음: {reward.ItemData.name}");
        }

        if (itemAmountText != null)
        {
            int amount = reward.amount;
            itemAmountText.text = amount.ToString();
            itemAmountText.enabled = amount != 1;
        }
    }

    public void SetEnemy(EnemyTable enemydata, Sprite enemyIcon)
    {
        if (enemydata == null || enemydata == null)
        {
            Debug.LogError("RewardItem 또는 ItemData가 null입니다.");
            return;
        }

        this.enemydata = enemydata;

        itemIcon.sprite = enemyIcon;
    }    
}
