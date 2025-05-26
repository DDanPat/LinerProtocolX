using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Transform target;
    private Vector3 offset;

    private Sequence seq;
    private Camera mainCam;

    public void Set(string value, Transform followTarget, Vector3 offset, bool isCritical)
    {
        if (isCritical)
        {
            text.color = new Color(244 / 255f, 161 / 255f, 12 / 255f, 1f);
            text.fontSize = 60;
        }
        else
        {
            text.color = Color.white;
            text.fontSize = 50;
        }
        this.target = followTarget;
        this.offset = offset;
        this.text.text = value;

        mainCam = Camera.main;
        transform.position = target.position + offset;

        // 기존 애니메이션 중단
        if (seq != null && seq.IsActive()) seq.Kill();

        Color startColor = text.color;
        startColor.a = 1;
        text.color = startColor;

        // 애니메이션 시작
        seq = DOTween.Sequence();

        seq.Append(text.DOFade(0f, 1f).SetEase(Ease.OutCubic))
            .OnComplete(() => ObjectPoolManager.Instance.ReturnDamage(gameObject));
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 계속 적을 따라가게
        Vector3 followPos = target.position + offset;
        transform.position = new Vector3(followPos.x, transform.position.y, followPos.z);

        // 카메라 향하게
        transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                         mainCam.transform.rotation * Vector3.up);
    }
}
