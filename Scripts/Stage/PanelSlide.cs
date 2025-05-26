using UnityEngine;
using DG.Tweening;

public class PanelSlider : MonoBehaviour
{
    public RectTransform[] panels; // UI 패널 5개를 연결할 배열
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 가동시간

    private int currentIndex = 0; // 현재 보여지고 있는 패널의 인덱스
    private bool isSliding = false; // 슬라이드 중인지 확인(중복 입력 방지)

    Vector2 panelPosition; // 전환될 패널의 시작 위치 (좌/우 방향 계산용)

    public LobbyUIHandler lobbyUIHandler;

    private void Start()
    {
        currentIndex = 2; // 시작 시 바로 나타날 패널의 인덱스 번호 지정
    }
    //public bool IsSliding() => isSliding; // 슬라이드가 진행 중인지를 반환하는 메서드

    public bool CheckOrganize()
    {
        if (currentIndex == 1 && !UIManager.Instance.organizeUI.CheckOrganize())
        {
            return true;
        }
        else return false;
    }

    public void ShowPanel(int index) // 버튼에서 호출되는 함수: index에 해당하는 패널로 전환
    {
        if (index == currentIndex || isSliding) return;

        CheckOrganize();

        isSliding = true; // 슬라이드 시작: 중복 입력 방지
        
        RectTransform fromPanel = panels[currentIndex]; // 현재 패널
        RectTransform toPanel = panels[index]; // 이동할 목표 패널

        fromPanel.transform.SetAsFirstSibling();
        int toIndex = fromPanel.transform.GetSiblingIndex();
        toPanel.transform.SetSiblingIndex(toIndex + 1);
        //toPanel.transform.SetAsLastSibling();

        float panelWidth = ((RectTransform)fromPanel.parent).rect.width;

        // 현재 패널 번호보다 인덱스가 크면 오른쪽에서 왼쪽으로 이동
        // 현재 패널 번호보다 인덱스가 작으면 왼쪽에서 오른쪽으로 이동
        if (currentIndex < index)
        {
            panelPosition = new Vector2(panelWidth, 0); // 오른쪽에서 in
        }
        else
        {
            panelPosition = new Vector2(-panelWidth, 0); // 왼쪽에서 in
        }

        // 이동할 패널을 활성화하고 시작 위치로 이동
        toPanel.gameObject.SetActive(true); // 비활성화된 패널 켜기
        toPanel.anchoredPosition = panelPosition; // 오른쪽 또는 왼쪽 밖에서 시작

        
        // DOTween 애니메이션 시작
        // 현재 패널을 화면 밖으로 이동 (반대 방향으로 슬라이드 아웃)
        fromPanel.DOAnchorPos(-panelPosition, slideDuration).SetEase(Ease.OutCubic);// 부드럽게 연출
        // 새 패널을 화면 중앙으로 이동
        toPanel.DOAnchorPos(Vector2.zero, slideDuration).SetEase(Ease.OutCubic) // 부드럽게 연출
            .OnComplete(() =>
            {
                fromPanel.gameObject.SetActive(false); // 이전 패널 보이지 않게 끄기
                fromPanel.anchoredPosition = Vector2.zero; // 위치 초기화
                currentIndex = index; // 현재 인덱스 갱신
                isSliding = false;    // 슬라이드 완료 → 입력 가능 상태로 전환

                // 슬라이드가 완료되면 버튼 클릭을 다시 가능하게 설정
                lobbyUIHandler.OnSlideComplete();
            });
    }
}
