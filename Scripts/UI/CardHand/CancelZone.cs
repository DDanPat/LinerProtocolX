using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool onCancel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        onCancel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onCancel = false;
    }
}
