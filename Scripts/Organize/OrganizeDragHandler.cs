using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 편성슬롯의 드래그 드롭을 위한 핸들러
/// </summary>

public class OrganizeDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private float pressStartTime;
    private bool isPointerDown = false;
    private bool isLongPress = false;
    private float longPressThreshold = 0.5f; // 꾹 누르는 시간 기준 (초)
    
    private OrganizeSlotManger organizeOperatorSlotManger;
    [SerializeField] private int index; // 수동 등록
    
    private GameObject dragIconObj;
    
    private Canvas dragCanvas;
    private ScriptableObject itemdata;
    public Action action;
    
    void Start()
    {
        organizeOperatorSlotManger = GetComponentInParent<OrganizeSlotManger>();
        dragCanvas = DragCanvas.Instance.GetComponent<Canvas>();
    }
    
    void Update()
    {
        if (isPointerDown && !isLongPress)
        {
            if (Time.time - pressStartTime > longPressThreshold)
            {
                isLongPress = true;
                OnLongPress();
            }
        }
    }
    
    private void OnLongPress()
    {
        // 정보 확인 UI 띄우기 (현재는 로그만)
        OrganizeSlot slot = gameObject.GetComponent<OrganizeSlot>();
        
        if (slot != null)
        {
            itemdata = slot.data;
        }

        if (itemdata != null)
        {
            UIManager.Instance.organizeUI.OpenOrganizeInfoPopup(index);
            action?.Invoke();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        pressStartTime = Time.time;
        isPointerDown = true;
        isLongPress = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isPointerDown = false;  // 롱프레스 감지 중단
        if (isLongPress) return; // 꾹 눌러서 정보 본 거면 드래그 금지
        
        GameObject clickedObj = eventData.pointerPress ?? eventData.pointerPressRaycast.gameObject;
        OrganizeSlot slot = clickedObj.GetComponentInParent<OrganizeSlot>();
        

        if (slot != null)
        {
            itemdata = slot.data;
        }

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
        if (isLongPress) return;
        
        // 드래그 처리
        if(dragIconObj != null)
            dragIconObj.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭 처리
        if (isLongPress) return;
        isPointerDown = false;
        
        // 드롭된 위치에 Raycast 쏘기
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool unOrganized = true;

        foreach (var result in results)
        {
            // 드래그 한 아이템 편성 슬롯에 집어 넣기
            OrganizeSlot slot = result.gameObject.GetComponent<OrganizeSlot>();
            if (slot != null)
            {
                unOrganized = false;
                // 편성 슬롯의 데이터 종류 비교하여 맞으면 실행 아니면 넘어감
                if (slot.CheckTypeData(itemdata))
                {
                    // 편성 슬롯 내부에 데이터가 있는지 확인
                    if (slot.CheckData())
                    {
                        OrganizeSlot thisSlot = gameObject.GetComponent<OrganizeSlot>();
                        thisSlot.SwapSlot(slot);
                        break;
                    }
                    else
                    {
                        var image = dragIconObj.GetComponent<Image>();
                        slot.SetData(itemdata);
                        OrganizeSlot thisSlot = gameObject.GetComponent<OrganizeSlot>();
                        thisSlot.ClearSlot();
                        break;
                    }
                }
            }
        }
        
        // 허공에 드래그 드랍하여 레이캐스트에 잡히는게 없으면 드랍
        if (unOrganized)
        {
            OrganizeSlot thisSlot = gameObject.GetComponent<OrganizeSlot>();
            
            // 타워는 중복가능 오퍼레이터는 단일이라 드롭시 used 판정
            if(itemdata is OperatorData operatorData)
                GameManager.Instance.Player.inventory.UnOrganize(operatorData);
            
            thisSlot.ClearSlot();
            
            UIManager.Instance.organizeUI.organizeSlotManger.UpdateUsedInventory();
            UIManager.Instance.organizeUI.InventoryDataUpdateSlots();
        }
        
        if (dragIconObj != null)
        {
            Destroy(dragIconObj);
        }
        
        organizeOperatorSlotManger.SortSlots();
    }
}
