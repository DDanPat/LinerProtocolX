using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
public class UISKillQueue : MonoBehaviour
{

    [SerializeField] private Image skillImage;
    [SerializeField] private Image operatorImage;
    [SerializeField] TextMeshProUGUI skillNameText;
    RectTransform _rect;
    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }
    private IEnumerator PlaySequence()
    {
        yield return null;
        // 슬라이드 인
        Vector2 originPos = _rect.anchoredPosition;
        _rect.anchoredPosition = originPos + Vector2.right * 300f;

        Tween inTween = _rect.DOAnchorPos(originPos, 0.4f);
        yield return inTween.WaitForCompletion(); // 정확한 완료 대기

        // 3초 대기
        yield return new WaitForSeconds(3f);

        // 슬라이드 아웃
        Tween outTween = _rect.DOAnchorPos(originPos + Vector2.right * 600f, 0.4f);
        yield return outTween.WaitForCompletion();

        Destroy(gameObject);
    }
    public void Set(Sprite operatorSprite, Sprite skillSprite, string skillName)
    {
        operatorImage.sprite = operatorSprite;
        skillImage.sprite = skillSprite;
        skillNameText.text = skillName;
        StartCoroutine(PlaySequence());
    }
}
