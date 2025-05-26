using TMPro;
using UnityEngine;

public class InventoryItemSlot : InventorySlot
{
    [SerializeField] private TextMeshProUGUI amountText;

    public override void UpdateDataIcon()
    {
        base.UpdateDataIcon();
        SetAmountText();
    }


    private void SetAmountText()
    {
        if (data is ItemData itemData)
        {
            int amount = GameManager.Instance.Player.inventory.items[itemData.key].amount;
            string formattedAmount = FormatNumber(amount);
            amountText.gameObject.SetActive(true);
            amountText.text = "x" + formattedAmount;
        }

        if (data == null)
        {
            amountText.gameObject.SetActive(false);
        }
        
    }

    public override void SwapSlot(InventorySlot slot)
    {
        base.SwapSlot(slot);
        SetAmountText();
    }
    
    public static string FormatNumber(double number)
    {
        if (number >= 1_000_000_000)
            return (number / 1_000_000_000d).ToString("0.##") + "B";
        else if (number >= 1_000_000)
            return (number / 1_000_000d).ToString("0.##") + "M";
        else if (number >= 1_000)
            return (number / 1_000d).ToString("0.##") + "K";
        else
            return number.ToString();
    }
}
