using DG.Tweening;
using UnityEngine;

public class OrganizePopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup popupGroup;
    private float displayTime = 2.0f; // 보여질 시간
    private float fadeDuration = 0.5f; // 서서히 사라지는 시간

    public void ShowFormationWarningPopup()
    {
        popupGroup.gameObject.SetActive(true);
        popupGroup.alpha = 1f; // 완전 보이게
        
        // 기존 트윈이 있으면 취소
        DOTween.Kill(popupGroup);

        // 일정 시간 후에 사라지도록
        DOVirtual.DelayedCall(displayTime, () =>
        {
            popupGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                popupGroup.gameObject.SetActive(false);
            });
        });
    }
}
