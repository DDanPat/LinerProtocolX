using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraMoveButton : MonoBehaviour, IPointerDownHandler
{
    public bool isRight;
    private BattleCameraMove controller;
    void Awake()
    {
        // 버튼 이벤트 등록
        GetComponent<Button>().onClick.AddListener(PointerUp);
    }
    public void RegistController(BattleCameraMove controller)
    {
        this.controller = controller;
    }

    // 버튼 뗌
    public void PointerUp()
    {
        controller.Stop();
    }

    // 버튼 누름
    public void OnPointerDown(PointerEventData eventData)
    {
        controller.TryMove(isRight);
    }
}
