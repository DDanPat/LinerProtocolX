using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> inventoryList;
    [SerializeField] private Sprite[] buttonSprite= new Sprite[2];
    [SerializeField] private List<GameObject> inventoryButtons;
    
    [SerializeField] private SlotScroll operatorScroll;
    [SerializeField] private SlotScroll itemScroll;
    
    public List<InventorySlot> operatorInventorySlots;
    public List<InventorySlot> itemInventorySlots;
    
    [SerializeField] private List<GameObject> operatorArrayButtons;
    [SerializeField] private List<GameObject> itemArrayButtons;
    private int operatorArrayIndex = 0;
    private int itemArrayIndex = 0;
    
    public GameObject operatorInfoPopupWindow;
    public GameObject itemInfoPopupWindow;
    
    private void Awake()
    {
        StartCoroutine(WaitForUIManagerAndRegister());
        
        operatorScroll.InitSlot();
        itemScroll.InitSlot();
        
        operatorInventorySlots = operatorScroll.slots;
        itemInventorySlots = itemScroll.slots;
    }

    private IEnumerator WaitForUIManagerAndRegister()
    {
        yield return new WaitUntil(() => UIManager.Instance != null);
        UIManager.Instance.inventoryUI = this;
    }

    private void OnEnable()
    {
        InventoryDataUpdateSlots();
        SwapInventory(0);
    }
    
    public void InventoryDataUpdateSlots()
    {
        // 소유x 정렬
        List<KeyValuePair<int, OwnedOperator>> sortedOperators = new List<KeyValuePair<int, OwnedOperator>>();
        sortedOperators = GameManager.Instance.Player.inventory.operators?
            .OrderByDescending(op => op.Value.isOwned).ToList();
        
        for (int i = 0; i < operatorInventorySlots.Count; i++)
        {
            if (i < sortedOperators?.Count)
            {
                operatorInventorySlots[i].data = (sortedOperators[i].Value.data);

                ItemLock slotLock = operatorInventorySlots[i].gameObject.GetComponent<ItemLock>();
                slotLock.Lock(sortedOperators[i].Value.isOwned);
            }
        }
        
        var sorted = GameManager.Instance.Player.inventory.items?
            .OrderBy(i => i.Value.data.key)
            .ToList();

        for (int i = 0; i < sorted?.Count; i++)
        {
            itemInventorySlots[i].data = sorted[i].Value.data;
        }
        
        SwapOperatorInventorySlot(operatorArrayIndex);
        SwapItemInventorySlot(itemArrayIndex);
    }

    // 오퍼레이터 인벤토리 정렬버튼
    public void SwapOperatorInventorySlot(int swapType)
    {
        operatorArrayIndex = swapType;
        List<KeyValuePair<int, OwnedOperator>> sortedOperators = new List<KeyValuePair<int, OwnedOperator>>();

        if ((SWAPOPERATORTYPE)swapType == SWAPOPERATORTYPE.KEY)
            sortedOperators = GameManager.Instance.Player.inventory.operators?
                .OrderBy(op => op.Key).ToList();
        if ((SWAPOPERATORTYPE)swapType == SWAPOPERATORTYPE.GRADE)
            sortedOperators = GameManager.Instance.Player.inventory.operators?
                .OrderByDescending(op => op.Value.grade).ToList();
        if ((SWAPOPERATORTYPE)swapType == SWAPOPERATORTYPE.ROLE)
            sortedOperators = GameManager.Instance.Player.inventory.operators?
                .OrderBy(op => op.Value.data.operatorInfoTable.role).ToList();
        if ((SWAPOPERATORTYPE)swapType == SWAPOPERATORTYPE.LV)
            sortedOperators = GameManager.Instance.Player.inventory.operators?
                .OrderByDescending(op => op.Value.level).ToList();
        
        // 소유x 정렬
        sortedOperators = sortedOperators?.OrderByDescending(op => op.Value.isOwned).ToList();

        for (int i = 0; i < operatorInventorySlots.Count; i++)
        {
            if (i < sortedOperators?.Count)
            {
                operatorInventorySlots[i].data = (sortedOperators[i].Value.data);

                ItemLock slotLock = operatorInventorySlots[i].gameObject.GetComponent<ItemLock>();
                slotLock.Lock(sortedOperators[i].Value.isOwned);
            }
        }

        SwapOperatorButton(swapType);

    }
    
    // 오퍼레이터 인벤토리 정렬버튼
    public void SwapItemInventorySlot(int swapType)
    {
        itemArrayIndex = swapType;
        List<KeyValuePair<int, Item>> sorted = new List<KeyValuePair<int, Item>>();
        
        if((SWAPITEMTYPE)swapType == SWAPITEMTYPE.KEY)
            sorted = GameManager.Instance.Player.inventory.items?
            .OrderBy(i => i.Key)
            .ToList();
        if((SWAPITEMTYPE)swapType == SWAPITEMTYPE.AMOUNT)
            sorted = GameManager.Instance.Player.inventory.items?
                .OrderByDescending(i => i.Value.amount)
                .ToList();
        
        for (int i = 0; i < itemInventorySlots.Count; i++)
        {
            if (i < sorted?.Count)
            {
                itemInventorySlots[i].data = (sorted[i].Value.data);
            }
        }
        SwapItemInventoryButton(swapType);
    }
    
    // 오퍼레이터 인벤토리 정렬시 버튼 on off
    private void SwapOperatorButton(int i)
    {
        foreach (var button in operatorArrayButtons)
        {
            button.SetActive(false);
        }
        operatorArrayButtons[i].gameObject.SetActive(true);
    }
    
    // 아이템 인벤토리 정렬시 버튼 on off
    public void SwapItemInventoryButton(int i)
    {
        foreach (var button in itemArrayButtons)
        {
            button.SetActive(false);
        }
        itemArrayButtons[i].gameObject.SetActive(true);
    }

    // 오퍼레이터 , 아이템 , ???(기타추가) 변경 버튼
    public void SwapInventory(int i)
    {
        foreach (var inven in inventoryList)
        {
            inven.SetActive(false);
        }
        inventoryList[i].gameObject.SetActive(true);

        foreach (var button in inventoryButtons)
        {
            Image image = button.gameObject.GetComponent<Image>();
            image.sprite = buttonSprite[0];
            
            TextMeshProUGUI text = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            text.color = Color.black;
            
        }
        Image nowImage = inventoryButtons[i].gameObject.GetComponent<Image>();
        nowImage.sprite = buttonSprite[1];
        TextMeshProUGUI nowText = inventoryButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>();
        nowText.color = Color.white;
        
    }

    public void OpenPopup(ScriptableObject data)
    {
        if (data is OperatorData operatorData)
        {
            PopupWindow window = operatorInfoPopupWindow.GetComponent<PopupWindow>();
            window.OpenWindow(operatorData);
        }

        if (data is ItemData itemData)
        {
            PopupWindow window = itemInfoPopupWindow.GetComponent<PopupWindow>();
            window.OpenWindow(itemData);
        }
    }

    public void OpenTutorial()
    {
        UIManager.Instance.OpenTutorial(TUTORIALTYPE.INVENTORY);
    }
}

public enum SWAPOPERATORTYPE
{
    KEY = 0,
    GRADE = 1,
    ROLE = 2,
    LV = 3,
}

public enum SWAPITEMTYPE
{
    KEY = 0,
    AMOUNT = 1,
}