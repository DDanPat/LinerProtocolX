using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrganizeUI : MonoBehaviour
{
    [SerializeField] private GameObject towerSlot;
    [SerializeField] private GameObject operatorSlot;

    [SerializeField] private Sprite[] buttonSprite = new Sprite[2];
    [SerializeField] private Image swapButton;
    public List<InventorySlot> towerSlots;
    public List<InventorySlot> operatorSlots;
    [SerializeField] private SlotScroll towerScroll;
    [SerializeField] private SlotScroll operatorScroll;
    
    public OrganizeSlotManger organizeSlotManger;
    
    public Canvas dragCanvas;
    [SerializeField] private PopupOrganizeInfo organizeInfoPopup;
    [SerializeField] private PopupOperatorInfo operatorInfoPopup;
    [SerializeField] private PopupTowerInfo towerInfoPopup;
    [SerializeField] private OrganizePopupUI organizedPopup;     // 편성완료, 편성불가 팝업창

    private void Awake()
    {
        StartCoroutine(WaitForUIManagerAndRegister());
        
        towerScroll.InitSlot();
        operatorScroll.InitSlot();
        
        towerSlots = towerScroll.slots;
        operatorSlots = operatorScroll.slots;
    }
    
    private IEnumerator WaitForUIManagerAndRegister()
    {
        yield return new WaitUntil(() => UIManager.Instance != null);
        UIManager.Instance.organizeUI = this;
    }

    private void OnEnable()
    {
        InventoryDataUpdateSlots();

        if (TutorialManager.Instance?.tutorial_Organize != null)
        {
            TutorialManager.Instance?.tutorial_Organize.BeforeDelayStart();
            StartCoroutine(TutorialManager.Instance.tutorial_Organize.DelayedTutorialStart());
            
        }

    }

    private void Start()
    {
        organizeSlotManger = GetComponent<OrganizeSlotManger>();
        dragCanvas = DragCanvas.Instance.GetComponent<Canvas>();
        operatorSlot.SetActive(false);
    }

    public void InventoryDataUpdateSlots()
    {
        int SlotCount = 0;
        foreach (var inventoryTower in GameManager.Instance.Player.inventory.towers)
        {
            if (inventoryTower.Value.isOwned && 
                SlotCount < GameManager.Instance.Player.inventory.towers.Count)
            {
                towerSlots[SlotCount].data = (inventoryTower.Value.data);
                // towerSlots[SlotCount].UpdateDataIcon();
                towerSlots[SlotCount].UpdateUsed(inventoryTower.Value.isEquipped);
                SlotCount++;
            }
        }
        // 남은 슬롯 초기화
        for (int i = SlotCount; i < towerSlots.Count; i++)
        {
            towerSlots[i].ClearSlot();
        }
        

        SlotCount = 0;
        foreach (var inventoryOperator in GameManager.Instance.Player.inventory.operators)
        {
            if (inventoryOperator.Value.isOwned && 
                SlotCount < GameManager.Instance.Player.inventory.operators.Count)
            {
                operatorSlots[SlotCount].data = (inventoryOperator.Value.data);
                // operatorSlots[SlotCount].UpdateDataIcon();
                operatorSlots[SlotCount].UpdateUsed(inventoryOperator.Value.isEquipped);
                SlotCount++;
            }
        }
        // 남은 슬롯 초기화
        for (int i = SlotCount; i < operatorSlots.Count; i++)
        {
            operatorSlots[i].ClearSlot();
        }
        
    }
    
    // 편성창 타워/오퍼레이터 버튼 슬롯변경기능
    public void SwapSlot()
    {
        if (towerSlot.gameObject.activeSelf)
        {
            swapButton.sprite = buttonSprite[1];
            operatorSlot.SetActive(true);
            towerSlot.SetActive(false);
        }
        else
        {
            swapButton.sprite = buttonSprite[0];
            operatorSlot.SetActive(false);
            towerSlot.SetActive(true);
        }
    }
    
    public bool CheckOrganize()
    {
        if (organizeSlotManger.SendTowerList())
        {
            return true;
        }
        else
        {
            // 편성 실패시 편성 실패 팝업
            organizedPopup.ShowFormationWarningPopup();
            return false;
        }
    }

    public void OpenOrganizeInfoPopup(int index)
    {
        var (operData, towerData) = organizeSlotManger.IndexGetData(index);
        
        organizeInfoPopup.OpenWindow(operData, towerData);
    }
    
    public void OpenInfoPopup(ScriptableObject data)
    {
        if (data is OperatorData operatorData)
        {
            PopupWindow window = operatorInfoPopup.GetComponent<PopupWindow>();
            window.OpenWindow(operatorData);
        }

        if (data is TowerData towerData)
        {
            PopupWindow window = towerInfoPopup.GetComponent<PopupWindow>();
            window.OpenWindow(towerData);
        }
    }

    public void OpenTutorialInfoPopup()
    {
        UIManager.Instance.OpenTutorial(TUTORIALTYPE.ORGANIZE);
    }
}
