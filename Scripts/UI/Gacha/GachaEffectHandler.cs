using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GachaEffectHandler : MonoBehaviour
{
    public static GachaEffectHandler Instance { get; private set; }

    [Header("Oper Intro")]
    [SerializeField] private GameObject operIntroPanel;
    [SerializeField] private TextMeshProUGUI operIntroText;

    [Header("Oper Info")]
    [SerializeField] private Image operBg;
    [SerializeField] private Image operProfile;
    [SerializeField] private Image sterEffect;
    [SerializeField] private TextMeshProUGUI operNameText;
    [SerializeField] private TextMeshProUGUI operRoleText;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;

    private Sequence currentSequence; // 현재 진행 중인 Sequence 저장


    private Queue<Action> gachaEffectQueue = new Queue<Action>();
    private bool isPlaying = false;
    private OperatorTable currentOperInfo;

    private void Awake()
    {
        Instance = this;

        nextButton.onClick.AddListener(OnClikNextButton);
        skipButton.onClick.AddListener(OnClickSkipButton);

        gameObject.SetActive(false);
    }

    private void OnClikNextButton()
    {
        // 다음 연출 실행
        TryPlayNextEffect();
    }


    #region 외부에서 호출하는 메서드

    public void RequestGachaEffect(OperatorTable operInfo)
    {
        // 객체가 비활성화 상태라면 활성화
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        gachaEffectQueue.Enqueue(() =>
        {
            currentOperInfo = operInfo;
            GachaEffectOperInfoSetting(currentOperInfo);
            PlayEffectSequence();
        });

        TryPlayNextEffect();
    }

    #endregion

    #region 연출 시작 조건

    private void TryPlayNextEffect()
    {
        if (isPlaying) return;

        // 큐가 비어 있다면 자동 비활성화
        if (gachaEffectQueue.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
            

        isPlaying = true;
        var next = gachaEffectQueue.Dequeue();
        next.Invoke();
    }

    #endregion

    #region 연출 세팅

    private void GachaEffectOperInfoSetting(OperatorTable operInfo)
    {
        operIntroText.enableWordWrapping = true;
        operIntroText.text = operInfo.lineIntro;
        operNameText.text = operInfo.name;
        SetRole(operInfo.role);
        OperIconSetting(operInfo);
    }

    private void SetRole(int role)
    {
        switch (role)
        {
            case 1: operRoleText.text = "공격형"; break;
            case 2: operRoleText.text = "방어형"; break;
            case 3: operRoleText.text = "지원형"; break;
            default: operRoleText.text = ""; break;
        }
    }

    private void OperIconSetting(OperatorTable operInfo)
    {
        var inventory = GameManager.Instance.Player.inventory.operators;

        if (inventory.ContainsKey(operInfo.key))
        {
            Sprite operSprite = inventory[operInfo.key].data.illust;
            operBg.sprite = operSprite;
            operProfile.sprite = operSprite;
        }
    }

    #endregion

    #region 연출 실행

    private void PlayEffectSequence()
    {
        ResetEffectUI(); // ← 여기서 초기화 먼저 실행

        currentSequence = DOTween.Sequence(); // 현재 Sequence 저장

        currentSequence.AppendInterval(2f) // 1) 인트로 패널 2초간 유지
            .AppendCallback(() =>
            {
                operIntroPanel.SetActive(false);
            })
            .Append(sterEffect.transform.DOScale(Vector3.one * 8f, 1f).SetEase(Ease.OutBack))
            .OnUpdate(() =>
            {
                // 스케일이 7 이상 되었을 때 프로필 오픈
                if (!operProfile.gameObject.activeSelf && sterEffect.transform.localScale.x >= 7f)
                {
                    operProfile.gameObject.SetActive(true);
                }
            })
            .AppendCallback(() =>
            {
                // 7~8 반복 연출 시작
                sterEffect.transform.DOScale(Vector3.one * 7f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            })
            .AppendInterval(0.5f) // 연출 여유 시간
            .OnComplete(() =>
            {
                isPlaying = false;
                // 버튼 활성화 (다음으로 넘기기 가능)
                nextButton.gameObject.SetActive(true);
            });
    }

    #endregion
    #region 연출 스킵
    private void OnClickSkipButton()
    {
        if (currentSequence != null && currentSequence.IsActive())
        {
            currentSequence.Kill(); // 현재 연출 종료
        }

        // 최종 연출 완료 상태로 전환
        operIntroPanel.SetActive(false);
        sterEffect.transform.DOScale(Vector3.one * 7f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        operProfile.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        isPlaying = false;

        skipButton.gameObject.SetActive(false);
    }
    #endregion


    private void ResetEffectUI()
    {
        // 패널과 오브젝트 초기화
        operIntroPanel.SetActive(true);
        operProfile.gameObject.SetActive(false);

        // sterEffect 초기 스케일로 설정
        sterEffect.transform.localScale = Vector3.zero;

        // 이전 Tween 중지
        DOTween.Kill(sterEffect.transform);
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();
        currentSequence = null;

        // 버튼 비활성화
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true);
    }
}
