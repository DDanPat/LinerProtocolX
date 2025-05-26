using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OverBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject originalBtn;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private Sprite ChangeSprite;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        originalBtn.GetComponent<Image>().sprite = ChangeSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        originalBtn.GetComponent<Image>().sprite = originalSprite;
    }
}
