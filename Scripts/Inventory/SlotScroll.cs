using System.Collections.Generic;
using UnityEngine;

public class SlotScroll : MonoBehaviour
{
    public GameObject slotPrefab;
    public float maxScroll;
    public List<InventorySlot> slots = new List<InventorySlot>();
    
    // private void Awake()
    // {
    //     for (int i = 0; i < maxScroll; i++)
    //     {
    //         GameObject newButton = Instantiate(slotPrefab, transform, false);
    //         newButton.transform.localScale = Vector3.one;    // 스케일 초기화!
    //     }
    // }

    public void InitSlot()
    {
        slots.Clear();

        for (int i = 0; i < maxScroll; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform, false);
            newSlot.transform.localScale = Vector3.one;    // 스케일 초기화!

            var slot = newSlot.GetComponent<InventorySlot>();
            if (slot != null)
                slots.Add(slot);
        }
    }
    
}
