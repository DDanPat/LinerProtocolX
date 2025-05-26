using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotLevelUpHandler : MonoBehaviour
{
    public Button iconButton;
    public Button removeButton;
    [SerializeField] private TextMeshProUGUI amountText;
    private int _amount;
    private int amount
    {
        get => _amount;
        set
        {
            _amount = Mathf.Max(0, value); // 음수 방지
            amountText.text = _amount.ToString();

            InventorySlot slot = gameObject.GetComponent<InventorySlot>();
            if (slot != null && slot.data is ItemData data)
            {
                // 버튼 상태 갱신
                iconButton.interactable = GameManager.Instance.Player.inventory.items[data.key].amount > 0;
            }
        }
    }
    
    private OperatorLevelUpPopup popup;     // 팝업창에서 강화 가능한지 검사한후 슬롯 클릭 가능여부 판단

    private void Start()
    {
        var itemPress = iconButton.GetComponent<SlotPressHandler>();
        var removePress = removeButton.GetComponent<SlotPressHandler>();
        itemPress.isRepeating = true;
        removePress.isRepeating = true;
        
        itemPress.onClick = () =>
        {
            PlusExpItemAmount();
            popup.OnSlotChanged();
        };
        itemPress.onLongPress = () =>
        {
            PlusExpItemAmount();
            popup.OnSlotChanged();
        };
        removePress.onClick = () =>
        {
            MinusExpItemAmount();
            popup.OnSlotChanged();
        };
        removePress.onLongPress = () =>
        {
            MinusExpItemAmount();
            popup.OnSlotChanged();
        };
        popup = gameObject.GetComponentInParent<OperatorLevelUpPopup>();
        
        amount = 0;
    }

    public void ClearSlot()
    {
        amount = 0;
        amountText.text = amount.ToString();
    }

    private void PlusExpItemAmount()
    {
        ItemData data = gameObject.GetComponent<InventorySlot>().data as ItemData;

        if (amount < GameManager.Instance.Player.inventory.items[data.key].amount 
            && !popup.isLevelCap && !popup.isGoldCap)
        {
            amount++;
            amountText.text = amount.ToString();
        }
    }

    private void MinusExpItemAmount()
    {
        if (amount > 0)
        {
            amount--;
            amountText.text = amount.ToString();
        }
    }

    // 팝업창에서 apply버튼 적용시 슬롯에 적용된 숫자만큼 인벤토리에서 소모
    public void ApplyLevelUp()
    {
        ItemData data = gameObject.GetComponent<InventorySlot>().data as ItemData;
        
        GameManager.Instance.Player.inventory.SubtractItem(data.key, amount);
    }

    public int GetSlotTotalEXP()
    {
        ItemData data = gameObject.GetComponent<InventorySlot>().data as ItemData;
        return _amount * data.exp;
    }

    public int GetSlotAmount()
    {
        return _amount;
    }
}
