using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler

{
    public float longPressThreshold = 1f;   // 롱프레스 인식 시간 (초)
    public float repeatInterval = 0.1f;
    public bool isRepeating = false;        //  반복 실행 여부 설정

    
    private float pressTime = 0f;
    private bool isPressing = false;
    private bool longPressStarted = false;
    private float repeatTimer = 0f;
    
    public System.Action onClick;
    public System.Action onLongPress;

    void Update()
    {
        if (isPressing)
        {
            pressTime += Time.deltaTime;

            if (!longPressStarted && pressTime >= longPressThreshold)
            {
                longPressStarted = true;
                repeatTimer = 0f;
                onLongPress?.Invoke(); // 최초 1회 호출
            }

            if (longPressStarted && isRepeating)
            {
                repeatTimer += Time.deltaTime;
                if (repeatTimer >= repeatInterval)
                {
                    repeatTimer = 0f;
                    onLongPress?.Invoke(); // 반복 호출
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        pressTime = 0f;
        longPressStarted = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressing && !longPressStarted)
        {
            onClick?.Invoke(); // 일반 클릭
        }
        isPressing = false;
        longPressStarted = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPressing = false;
        longPressStarted = false;
    }
}
