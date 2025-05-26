using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class StageUIHandler : MonoBehaviour
{
    [Header("Buttons")]
    public Button backStageButton;
    public Button nextStageButton;
    public Button gameStartButton;

    [Header("Stage Text")]
    public TextMeshProUGUI curStageText;
    public GameObject warningText;

    [Header("Button Images")]
    public Image backStageButtonImage;
    public Image nextStageButtonImage;

    private readonly Color activeColor = new Color32(0x21, 0x5F, 0x9A, 0xFF); // #215F9A
    private readonly Color inactiveColor = Color.white;

    /// <summary>
    /// 버튼 이벤트 초기화
    /// </summary>
    public void InitializeUI(System.Action onBack, System.Action onNext, System.Action onStart)
    {
        backStageButton.onClick.AddListener(() => onBack.Invoke());
        nextStageButton.onClick.AddListener(() => onNext.Invoke());
        gameStartButton.onClick.AddListener(() => onStart.Invoke());
    }

    /// <summary>
    /// 스테이지 번호 텍스트 갱신
    /// </summary>
    public void UpdateStageText(int curStage)
    {
        curStageText.text = $"{curStage}스테이지";
    }

    /// <summary>
    /// 버튼 색상 갱신
    /// </summary>
    public void UpdateNavigationButtons(int curStage, int maxStage, int clearStageCount)
    {
        backStageButtonImage.color = (curStage > 0) ? activeColor : inactiveColor;
        bool canGoNext = (curStage < maxStage - 1) && (curStage < clearStageCount);
        nextStageButtonImage.color = canGoNext ? activeColor : inactiveColor;
    }

    /// <summary>
    /// 경고 문구 표시/숨김
    /// </summary>
    public void ShowWarning(List<int> enemyKeys)
    {
        // 타워 총알 정보와 적의 방어 정보 비교
        foreach (var tower in GameManager.Instance.Player.deck.towerList)
        {
            foreach (var enemyKey in enemyKeys)
            {
                EnemyTable enemy = EnemyManager.Instance.GetEnemyData(enemyKey);
                if (enemy != null)
                {
                    if (enemy.type == tower.towerData.towerInfoTable.bulletType)
                    {
                        // 방어 타입과 공격 타입이 같음 = 유효하면 false(숨김)
                        // 마지막까지 유효하지 않으면 표시
                        if (warningText != null)
                        {
                            warningText.SetActive(false);
                            return;
                        }
                    }
                }
                else
                {
                    // 데이터가 없으면 숨김
                    // 마지막까지 유효하지 않으면 표시
                    if (warningText != null)
                    {
                        warningText.SetActive(false);
                        return;
                    }
                }
            }
        }
        // 마지막까지 유효하지 않으면 표시
        if (warningText != null)
        {
            warningText.SetActive(true);
        }
    }
}
