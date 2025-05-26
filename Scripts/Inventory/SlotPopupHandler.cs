using UnityEngine;
using UnityEngine.UI;

public class SlotPopupHandler : MonoBehaviour
{
    private void Awake()
    {
        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnClickSlot);
    }

    public void OnClickSlot()
    {
        InventorySlot slot = GetComponent<InventorySlot>();
        
        UIManager.Instance.inventoryUI.OpenPopup(slot.data);
    }
    
}
