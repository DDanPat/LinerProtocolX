using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 튜토리얼용 UI 매너지
// 튜토리얼 씬에서만 존재
public class TutorialUIManager : MonoBehaviour
{
    public static TutorialUIManager Instance;
    public List<GameObject> PopObjects;
    public GameObject shadowObject;
    public GameObject skillBlock;
    private int popupIndex = 0;
    private int towerPopup = 1;
    private int cardPopup = 3;
    private int skillPopup = 1;

    public RectTransform DragGuide; // 드래그 가이드용 오브젝트
    public float fromY = -500f; // 시작 위치 (아래)

    public float dragDuration = 1f;

    private bool isStop = false;
    Coroutine nextCoroutine;
    Coroutine dragCoroutine;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < PopObjects.Count; i++)
        {
            PopObjects[i].SetActive(i == 0);
        }
        skillBlock.SetActive(true);
        DragGuide.gameObject.SetActive(false);
    }
    public void OnClickPopup()
    {
        if (nextCoroutine != null)
            StopCoroutine(nextCoroutine);
        NextStep();
    }
    public void OnSkillDrag()
    {
        if (skillPopup > 0)
        {
            shadowObject.SetActive(false);
            ShowGuide(false);
        }
    }
    public void EndSkillDrag(bool use)
    {
        if (skillPopup > 0 && use)
        {
            NextStep();
            skillPopup--;
        }
    }
    public void OnCardDrag()
    {
        if (towerPopup > 0)
        {
            NextStep();
        }
        if (isStop)
            Time.timeScale = 0;
    }
    public void OnToolDrag()
    {
        if (isStop)
            Time.timeScale = 0;
    }
    public void OnTowerBuild()
    {
        if (towerPopup > 0)
        {
            NextStep();
            towerPopup--;
        }
        if (isStop)
            Time.timeScale = 0;
    }
    public void OnTowerFaild()
    {
        if (popupIndex <= 3)
        {
            PopObjects[popupIndex].SetActive(false);
            popupIndex = Mathf.Max(0, popupIndex - 1);
            PopObjects[popupIndex].SetActive(true);
        }
    }
    public void OnCardSelect()
    {
        if (cardPopup > 0)
        {
            NextStep();
            cardPopup--;
        }
    }
    public void NextStep()
    {

        PopObjects[popupIndex].SetActive(false);
        popupIndex++;
        if (popupIndex < PopObjects.Count)
            PopObjects[popupIndex].SetActive(true);

        if (popupIndex == 2)
        {
            // 타워 설치
            Time.timeScale = 0;
            isStop = true;
            ShowGuide(true, -200);
        }
        else if (popupIndex == 3)
        {
            Time.timeScale = 0;
            isStop = true;
            ShowGuide(false);
        }
        else if (popupIndex == 4)
        {
            // 상성 이미지
            Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
            isStop = true;
        }
        else if (popupIndex == 5)
        {
            nextCoroutine = StartCoroutine(PopupNext());
            isStop = false;
            Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
        }
        else if (popupIndex == 7)
        {
            // 스킬 사용 전
            Time.timeScale = 0;
            isStop = true;
            skillBlock.SetActive(false);
            ShowGuide(true, 200);
        }
        else if (popupIndex == 8)
        {
            // 스킬 사용 후
            isStop = false;
            Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
            ShowGuide(false);
        }
        else
        {
            isStop = false;
            Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
        }
    }
    IEnumerator PopupNext()
    {
        yield return new WaitForSeconds(3f);
        nextCoroutine = null;
        NextStep();
    }
    private void ShowGuide(bool active, float toY = 0)
    {
        DragGuide.gameObject.SetActive(active);
        if (active)
        {
            if (dragCoroutine != null)
                StopCoroutine(dragCoroutine);
            dragCoroutine = StartCoroutine(ScrollLoop(toY));
        }
        else
        {
            StopCoroutine(dragCoroutine);
        }
    }
    IEnumerator ScrollLoop(float toY)
    {
        float elapsed;
        while (true)
        {
            // 위치 초기화
            DragGuide.anchoredPosition = new Vector2(DragGuide.anchoredPosition.x, fromY);

            elapsed = 0f;

            while (elapsed < dragDuration)
            {
                elapsed += Time.unscaledDeltaTime;  // 시간 정지 무시
                float t = Mathf.Clamp01(elapsed / dragDuration);
                float currentY = Mathf.Lerp(fromY, toY, t);
                DragGuide.anchoredPosition = new Vector2(DragGuide.anchoredPosition.x, currentY);
                yield return null;
            }
            // 도착후 대기
            elapsed = 0f;
            while (elapsed < dragDuration * 0.5f)
            {
                elapsed += Time.unscaledDeltaTime;  // 시간 정지 무시
                DragGuide.anchoredPosition = new Vector2(DragGuide.anchoredPosition.x, toY);
                yield return null;
            }

            // 반복 (즉시 아래로 재위치 후 다시 이동 시작)
            yield return null;
        }
    }
}
