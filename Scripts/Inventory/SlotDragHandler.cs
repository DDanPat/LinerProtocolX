using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotDragHandler : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler
{
    private ScrollRect parentScroll;
    private GameObject dragIconObj;
    
    private Canvas dragCanvas;
    private Button button;
    private ScriptableObject itemdata;
    private InventorySlot slot;
    
    private bool isDragging = false;
    private float dragThreshold = 10f; // 픽셀 수 (마우스 이동량 기준)
    private Vector2 dragStartPos;

    private OrganizeInventorySlot OZInvenSlot;
    public Action dragAction;
    
    void Start()
    {
        parentScroll = GetComponentInParent<ScrollRect>();
        dragCanvas = DragCanvas.Instance.GetComponent<Canvas>();
        OZInvenSlot = GetComponentInParent<OrganizeInventorySlot>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
        isDragging = false;
        
        slot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        if (slot != null)
        {
            itemdata = slot.data;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(isDragging);
        
        if (!isDragging)
        {
            // 여기서 클릭 처리
            Debug.Log(slot);
            Debug.Log(itemdata);
            
            // 슬롯 클릭 시 원하는 처리 (예: 상세 정보 팝업)
            if (slot != null && itemdata != null && TutorialManager.Instance.tutorial_Organize == null)
            {
                OZInvenSlot.OpenInfoPopup();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 프리뷰 UI 띄우는 처리 등
        if (parentScroll != null)
            parentScroll.enabled = false;
        
        if (itemdata != null)
        {
            // 드래그 오브젝트 생성하여 아이템 슬롯에서 드래그시 마우스 따라갈 이미지 오브젝트 생성
            dragIconObj = new GameObject("DragIcon", typeof(Image), typeof(CanvasGroup));
            dragIconObj.transform.SetParent(dragCanvas.transform, false);
            var image = dragIconObj.GetComponent<Image>();
            image.sprite = slot.icon.sprite;
            image.raycastTarget = false; // 클릭 방지
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging && Vector2.Distance(eventData.position, dragStartPos) > dragThreshold)
        {
            isDragging = true;
        }

        if (dragIconObj != null)
            dragIconObj.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭 처리
        
        // 드롭된 위치에 Raycast 쏘기
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // 드래그 한 아이템 편성 슬롯에 집어 넣기
            OrganizeSlot organizeSlot = result.gameObject.GetComponentInParent<OrganizeSlot>();
            if (organizeSlot != null)
            {
                if (organizeSlot.CheckTypeData(itemdata))
                {

                    if (itemdata is TowerData tower)
                    {
                        organizeSlot.SetData(itemdata);
                        UIManager.Instance.organizeUI.organizeSlotManger.UpdateUsedInventory();
                        UIManager.Instance.organizeUI.InventoryDataUpdateSlots();
                    }
                    
                    else if (itemdata is OperatorData operatorData)
                    {
                        if (!UIManager.Instance.organizeUI.organizeSlotManger.IsAlreadyOrganized(itemdata))
                        {
                            organizeSlot.SetData(itemdata);
                            GameManager.Instance.Player.inventory.Organize(itemdata);
                            UIManager.Instance.organizeUI.organizeSlotManger.UpdateUsedInventory();
                            UIManager.Instance.organizeUI.InventoryDataUpdateSlots();
                        }
                    }
                    
                    // 아이템 슬롯 -> 편성 슬롯으로 입력완료 하면 편성슬롯들을 정렬함
                    UIManager.Instance.organizeUI.organizeSlotManger.SortSlots();
                    dragAction?.Invoke();
                    
                    break;
                }
                
            }
        }
        
        if (parentScroll != null)
            parentScroll.enabled = true;
        
        if (dragIconObj != null)
        {
            Destroy(dragIconObj);
        }
        
    }
}
