using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupItemWindow : PopupWindow
{
    [SerializeField] private InventoryItemSlot slot;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;

    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject cancelButton;


    public override void SetPopupUI()
    {
        var itemdata = popupData as ItemData;
        if (itemdata != null)
        {
            slot.data = itemdata;
            nameText.text = itemdata.name;
            descText.text = itemdata.desc;

            if (9000 - 1 < itemdata.key && itemdata.key < 9003 + 1)
            {
                useButton.SetActive(true);
                cancelButton.SetActive(false);
            }
            else
            {
                useButton.SetActive(false);
                cancelButton.SetActive(true);
            }
        }
    }

    public void UseItem()
    {
        var itemdata = popupData as ItemData;

        if (GameManager.Instance.Player.inventory.items[itemdata.key].amount > 0)
        {
            GameManager.Instance.Player.inventory.SubtractItem(itemdata.key, 1);
            int gold = itemdata.exp;

            GameManager.Instance.Player.Gold += gold;
            // 임의 저장
            GameManager.Instance.SavePlayer();
            // 커스텀 이벤트 - 골드 아이템 사용
            AnalyticsManager.Instance.SendEvent(Define.GoldItemUsed, new Dictionary<string, object> {
            { Define.itemKey, itemdata.key},
            { Define.coinGained, gold },
            });
            // 커스텀 이벤트 - 코인 변화
            AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
            { Define.changeType, Define.gained },
            { Define.numberCoinChanged, gold},
            { Define.changeSource, 5},
            { Define.numberCoinBefore, GameManager.Instance.Player.Gold - gold},
            });
        }
        SetPopupUI();

        UIManager.Instance.inventoryUI.InventoryDataUpdateSlots();
    }
}
