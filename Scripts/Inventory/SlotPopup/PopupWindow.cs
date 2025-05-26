using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    [Header("Popup Window Data")]
    [SerializeField] private GameObject popupWindow;
    [SerializeField] private SLOTTYPE slottype;
    [SerializeField] protected ScriptableObject popupData;    // 인벤토리에서 전달해 팝업창에서 사용할 SO데이터
    

    public void OpenWindow(ScriptableObject popupData)
    {
        popupWindow.SetActive(true);
        this.popupData = popupData;
        SetPopupUI();
    }

    public virtual void CloseWindow()
    {
        popupWindow?.SetActive(false);
    }

    public virtual void SetPopupUI()
    {
        
    }
}
