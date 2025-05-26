using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using DG.Tweening;

/// <summary>
/// 씬 전환 시 로딩 화면을 보여주는 컨트롤러 클래스
/// - Fade In/Out 처리
/// - 로딩 바 진행도
/// - "로딩중..." 점 애니메이션 효과
/// </summary>
public class LoadingSceneController : Singleton<LoadingSceneController>
{
    [Header("UI 컴포넌트")]
    [SerializeField] private CanvasGroup canvasGroup;              // 페이드 인/아웃을 위한 캔버스 그룹
    [SerializeField] private Image loadingBar;                     // 로딩 진행 바
    [SerializeField] private TextMeshProUGUI loadingText;          // "로딩중..." 텍스트
    [SerializeField] private TextMeshProUGUI tipText;              // 하단 TMI/팁 텍스트 (현재 미사용)
    [SerializeField] private Button loadingButton;

    private string loadSceneName;                                  // 로드할 씬 이름
    private Action onSceneLoadedAction;                            // 씬로드 완료 후 실행할 콜백

    private void Start()
    {
        loadingButton.onClick.AddListener(OnClickLoadingButton);
        // 처음엔 로딩 화면을 꺼두기
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 외부에서 씬을 로드할 때 호출하는 메서드
    /// </summary>
    public void LoadScene(string sceneName, Action onSceneLoaded = null)
    {
        SoundManager.Instance.StopBGM();

        //생성된 오브젝트 풀 제거
        ObjectPoolManager.Instance.RemoveNullObjectsInPool();

        loadingText.text = "로딩중";                // 초기 텍스트 설정
        tipText.text = RandemText();
        gameObject.SetActive(true);                // 로딩 오브젝트 활성화
        onSceneLoadedAction = onSceneLoaded;       // 콜백 저장
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 완료 시 호출될 콜백 등록
        loadSceneName = sceneName;                 // 로드할 씬 이름 저장
        StartCoroutine(LoadSceneProcess());        // 로딩 시작
    }

    /// <summary>
    /// 씬 비동기 로딩 + UI 업데이트 처리
    /// </summary>
    private IEnumerator LoadSceneProcess()
    {
        loadingBar.fillAmount = 0f;                      // 로딩 바 초기화
        yield return StartCoroutine(Fade(true));         // 페이드 인

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;                 // 로딩 완료 후 자동 전환 막기

        float dotTimer = 0f;     // 점 애니메이션 타이머
        int dotCount = 0;        // 점 개수 (1~6 반복)
        float timer = 0f;        // 로딩 완료 시 Lerp 보간용 시간 변수

        while (!op.isDone)
        {
            yield return null;

            // 1) 로딩바 채우기
            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress; // 0~0.9까지 실제 진행도 반영
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer); // 마지막 10% 부드럽게 채우기

                if (loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true; // 씬 전환 허용
                    yield break;
                }
            }

            // 2) "로딩중..." 점 애니메이션
            dotTimer += Time.unscaledDeltaTime;
            if (dotTimer >= 0.5f) // 0.5초마다 점 추가
            {
                dotTimer = 0f;
                dotCount = (dotCount % 6) + 1; // 1~6 순환
                loadingText.text = "로딩중" + new string('.', dotCount);
            }
        }
    }

    /// <summary>
    /// 씬 로드가 완료되었을 때 호출되는 메서드
    /// </summary>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == loadSceneName)
        {
            

            // 저장된 콜백 실행
            onSceneLoadedAction?.Invoke();
            onSceneLoadedAction = null;

            StartCoroutine(Fade(false));                 // 페이드 아웃 후 로딩창 숨기기
            SceneManager.sceneLoaded -= OnSceneLoaded;   // 이벤트 등록 해제

            SoundManager.Instance.PlayBGMForScene(arg0.name);
            if (UIManager.Instance != null)
            {
                // 버튼 활성/비활성
                UIManager.Instance.UpdateButton();
            }
        }
    }

    /// <summary>
    /// 화면을 서서히 밝히거나 어둡게 전환하는 페이드 처리
    /// </summary>
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        // 페이드 아웃 완료 후 로딩창 비활성화
        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    private string RandemText()
    {
        int random = UnityEngine.Random.Range(0, GameManager.Instance.DataManager.LoadingTextTableLoader.ItemsList.Count);
        string Ttext = GameManager.Instance.DataManager.LoadingTextTableLoader.ItemsList[random].text;
        string text = $"TIP : {Ttext}";
        return text;
    }

    private void OnClickLoadingButton()
    {
        tipText.text = RandemText();
    }
}

