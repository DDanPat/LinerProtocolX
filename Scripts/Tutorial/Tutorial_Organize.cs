using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class Tutorial_Organize : MonoBehaviour
{
    public RectTransform startPoint;
    public RectTransform endPoint;
    public RectTransform target;                    // 손가락등 표시가 이동할 오브젝트 UI
    public RectTransform target_organizeInfo;       // 꾹 누르기 튜토리얼 외곽선
    public RectTransform target_organizeInfoFill;   // 꾹 누르기 튜토리얼 꽉차는 효과
    
    public GameObject background;
    public GameObject lobbyBtnPrefab;               // 로비 버튼 잠금용 백그라운드 프리팹
    public GameObject background_LobbyBtn;          // 로비 버튼 잠금용 백그라운드 2
    public GameObject descPanel_Organize;
    public GameObject descPanel_particular;
    
    public float duration = 1f;

    public void BeforeDelayStart()
    {
        RectTransform parent = GameObject.Find("LobbyUI")?.GetComponent<RectTransform>();
        background_LobbyBtn = Instantiate(lobbyBtnPrefab, parent);
    }

    public IEnumerator DelayedTutorialStart()
    {
        yield return new WaitForSeconds(1f); // 패널 이동 시간만큼 조정

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.StartOrganizeTutorial_1();
    }

    public void SetTutorialTarget(RectTransform startPoint, RectTransform endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    public void StartOrganizeTutorial()
    {
        background.SetActive(true);
        target.gameObject.SetActive(true);
        descPanel_Organize.SetActive(true);
        
        target.position = startPoint.position;
        target.DOMove(endPoint.position, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Restart); 
    }

    public void StartParticularTutorial()
    {
        background.SetActive(true);
        target.gameObject.SetActive(true);
        descPanel_particular.SetActive(true);
        
        target_organizeInfo.gameObject.SetActive(true);
        target.position = endPoint.position;

        target_organizeInfoFill.DOScale(Vector3.one * 100f, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    public void StopOrganizeTutorial()
    {
        target.DOKill();
        target_organizeInfoFill.DOKill();
        
        background.SetActive(false);
        target.gameObject.SetActive(false);
        descPanel_Organize.SetActive(false);
        descPanel_particular.SetActive(false);
    }

    public void StopParticularTutorial()
    {
        target.DOKill();
        target_organizeInfoFill.DOKill();
        
        background.SetActive(false);
        target.gameObject.SetActive(false);
        descPanel_Organize.SetActive(false);
        descPanel_particular.SetActive(false);

        TutorialManager.Instance.EndOrganizeTutorial();
        
        Destroy(background_LobbyBtn);
        Destroy(gameObject);
    }
}
