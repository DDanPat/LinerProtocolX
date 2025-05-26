using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopup : MonoBehaviour
{
    [SerializeField] private GameObject rewardItemPopup;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private Button popupRetrunButoon;

    private void Start()
    {
        popupRetrunButoon.onClick.AddListener(OnClickPopupRetrunButton);
        rewardItemPopup.SetActive(false);
    }

    private void OnClickPopupRetrunButton()
    {
        rewardItemPopup.SetActive(false);
    }

    public void SetItemPopupInfo(RewardItem item, Sprite icon)
    {
        if (item != null)
        {
            itemIcon.sprite = icon;
            itemName.text = item.ItemData.name;
            itemAmount.text = item.amount.ToString();
            itemInfo.text = item.ItemData.desc;
            itemAmount.gameObject.SetActive(false);
        }
    }
    public void SetEnemyPopupInfo(EnemyTable enemydata, Sprite icon)
    {
        if (enemydata != null)
        {
            string tag = "";
            if (enemydata.key == 2190 || enemydata.key == 2290) tag = $"<color=red><b>boss</b></color>";

            itemIcon.sprite = icon;
            itemName.text = $"{enemydata.name} {tag}";
            itemInfo.text = enemydata.desc;
            itemAmount.gameObject.SetActive(false);
        }
    }
    public void OnPupup()
    {
        rewardItemPopup.SetActive(true);
    }
}
