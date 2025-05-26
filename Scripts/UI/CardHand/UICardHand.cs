using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UICardHand : MonoBehaviour
{
    // UI 목록
    [SerializeField] private TowerFactory towerFactory;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private RectTransform cardGroup;
    [SerializeField] private RectTransform cardParent;
    [SerializeField] private RectTransform skillGroup;
    [SerializeField] private RectTransform skillParent;

    [SerializeField] private TextMeshProUGUI descriptText;
    [SerializeField] private CanvasGroup handGroup;
    [SerializeField] private CanvasGroup descriptGroup;
    [SerializeField] private CancelZone cancelZone;
    private GameObject tileViewer;

    private List<UICard> cards;
    private Dictionary<int, SkillButton> skills;

    void Awake()
    {
        cards = new();
        skills = new();
        descriptGroup.alpha = 0;
    }
    void Start()
    {
        if (tileViewer == null)
        {
            tileViewer = TileManager.Instance.tileBuilder.GetTileViewer();
        }
    }
    // 덱에 카드 획득
    public void SpawnCard(TowerOperList info, int operatorKey = -1)
    {
        int index = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].ItemKey == info.towerData.towerInfoTable.key
            && cards[i].OperatorKey == operatorKey)
            {
                index = i;
                break;
            }

        }
        // 카드 추가
        if (index != -1)
        {
            // 기존 카드에 추가
            cards[index].ChangeCount(1);
        }
        else
        {
            // 새롭게 생성
            UICard card = Instantiate(cardPrefab, cardParent).GetComponent<UICard>();
            card.Setup(info, operatorKey);
            card.OnStartDragAction += StartTowerDrg;
            card.OnEndDragAction += EndTowerDrag;
            cards.Add(card);
        }

        // 스킬 카드 생성
        if (operatorKey != -1 && !skills.ContainsKey(operatorKey))
        {
            AddSkill(info);
        }
    }
    // 기능 카드 추가
    public void SpawnCard(int itemKey)
    {
        int index = -1;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].ItemKey == itemKey)
            {
                index = i;
                break;
            }

        }
        // 카드 추가
        if (index != -1)
        {
            // 기존 카드에 추가
            cards[index].ChangeCount(1);
        }
        else
        {
            // 새롭게 생성
            UICard card = Instantiate(cardPrefab, cardParent).GetComponent<UICard>();
            card.Setup(itemKey, BattleManager.Instance.BattleUI.SelectCard.GetSprite(itemKey));
            card.OnStartDragAction += StartUnilDrg;
            card.OnEndDragAction += EndUtilDrag;
            cards.Add(card);
        }
    }
    private void StartTowerDrg(int towerKey, int operatorKey)
    {
        Time.timeScale = 0.1f;
        handGroup.alpha = 0;
        handGroup.blocksRaycasts = false;
        descriptGroup.alpha = 1;
        descriptGroup.blocksRaycasts = true;

        // 카드 정보 전달
        towerFactory.SetCard(towerKey, operatorKey);
        // 카드 이름 표시
        descriptText.text = GameManager.Instance.DataManager.TowerTableLoader.GetByKey(towerKey).name;
        // 강화 UI, 사거리 표시
        TileManager.Instance.ShowEnforce(towerKey, operatorKey, true);
        TileManager.Instance.ShowRange(towerKey, operatorKey, true);
        // 타일 표시
        if (tileViewer == null)
        {
            tileViewer = TileManager.Instance.tileBuilder.GetTileViewer();
        }
        if (tileViewer != null)
            tileViewer.SetActive(true);

        // 튜토리얼이라면 진행
        if (TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.OnCardDrag();
        }
    }
    private void EndTowerDrag(int towerKey)
    {
        Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed(); // 수정
        handGroup.blocksRaycasts = true;
        handGroup.alpha = 1;
        descriptGroup.alpha = 0;
        descriptGroup.blocksRaycasts = false;
        if (towerFactory.TryUseCard())
        {
            // 소환 성공
            // UI 요소 찾기
            int index = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ItemKey == towerKey)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                // 갯수 감소
                cards[index].ChangeCount(-1);
                // 0이 되면 삭제
                if (cards[index].IsEmpty())
                {
                    cards.Remove(cards[index]);
                }
            }
            // 튜토리얼이라면 진행
            if (TutorialUIManager.Instance != null)
            {
                TutorialUIManager.Instance.OnTowerBuild();
            }
        }
        else
        {
            // 튜토리얼 뒤로
            if (TutorialUIManager.Instance != null)
            {
                TutorialUIManager.Instance.OnTowerFaild();
            }
        }
        // 타일 숨기기
        if (tileViewer != null)
            tileViewer.SetActive(false);
    }
    // 스킬 버튼 드래그 시작
    private void StartSkillDrg(int skillKey, int operatorKey)
    {
        // 시간 천천히
        Time.timeScale = 0.1f;
        handGroup.alpha = 0;
        handGroup.blocksRaycasts = false;
        descriptGroup.alpha = 1;
        descriptGroup.blocksRaycasts = true;

        float range = GameManager.Instance.DataManager.ActiveSkillLevelTableLoader.GetByKey(skillKey).range;
        // 카드 이동
        towerFactory.SetCard(skillKey, operatorKey, range);
        descriptText.text = SkillManager.Instance.GetName(skillKey) + SkillManager.Instance.GetDescript(skillKey);

        // 튜토리얼이라면 진행
        if (TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.OnSkillDrag();
        }
    }
    // 스킬 버튼 드래그 종료
    private void EndSkillDrag(int skillKey)
    {
        handGroup.blocksRaycasts = true;
        handGroup.alpha = 1;
        descriptGroup.alpha = 0;
        descriptGroup.blocksRaycasts = false;
        // 시간 복귀
        Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed(); ;
        // 취소 확인
        if (cancelZone.onCancel)
        {
            towerFactory.CancelCard();
            // 튜토리얼 뒤로
            if (TutorialUIManager.Instance != null)
            {
                TutorialUIManager.Instance.OnToolDrag();
            }
            return;
        }
        else
        {
            // 스킬 사용
            towerFactory.TryUseCard();
            // UI 요소 찾기
            foreach (var skill in skills.Values)
            {
                if (skill.skillKey == skillKey)
                {
                    // 쿨타임 다시 적용
                    skill.ApplyCoolTime();
                    break;
                }
            }
            // 튜토리얼이라면 진행
            if (TutorialUIManager.Instance != null)
            {
                TutorialUIManager.Instance.EndSkillDrag(true);
            }
        }

    }

    // 스킬 버튼 추가
    public void AddSkill(TowerOperList info)
    {
        SkillButton skillButton = Instantiate(skillPrefab, skillParent).GetComponent<SkillButton>();
        skills[info.operatorData.operatorInfoTable.key] = skillButton;
        skillButton.Setup(info);
        skillButton.OnStartDragAction += StartSkillDrg;
        skillButton.OnEndDragAction += EndSkillDrag;
    }
    private void StartUnilDrg(int itemKey, int subKey = -1)
    {
        Time.timeScale = 0.1f;
        handGroup.alpha = 0;
        handGroup.blocksRaycasts = false;
        descriptGroup.alpha = 1;
        descriptGroup.blocksRaycasts = true;
        // 카드 이동
        towerFactory.SetCard(itemKey, subKey);
        // 타일 표시
        if (tileViewer == null)
        {
            tileViewer = TileManager.Instance.tileBuilder.GetTileViewer();
        }
        if (tileViewer != null)
            tileViewer.SetActive(true);
        // 카드 이름 표시
        descriptText.text = GameManager.Instance.DataManager.CardTableLoader.GetByKey(itemKey).desc.Replace("\\n", "\n");

        // 튜토리얼이라면
        if (TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.OnToolDrag();
        }
    }
    private void EndUtilDrag(int itemKey)
    {
        // 시간 복귀
        Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
        handGroup.blocksRaycasts = true;
        handGroup.alpha = 1;
        descriptGroup.alpha = 0;
        descriptGroup.blocksRaycasts = false;
        // 카드 사용
        if (towerFactory.TryUseCard())
        {
            // 소환 성공
            // UI 요소 찾기
            int index = -1;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].ItemKey == itemKey)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                // 갯수 감소
                cards[index].ChangeCount(-1);
                // 0이 되면 삭제
                if (cards[index].IsEmpty())
                {
                    cards.Remove(cards[index]);
                }
            }
        }
        // 타일 숨기기
        if (tileViewer != null)
            tileViewer.SetActive(false);

        // 튜토리얼이라면
        if (TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.OnToolDrag();
        }
    }
    // 사용 취소
    public void CancelCard()
    {
        // 타일 숨기기
        if (tileViewer != null)
            tileViewer.SetActive(false);
        towerFactory.CancelCard();
    }

    // 모든 스킬 쿨타임 감소(초), 제외 키
    public void ReduceSkillTime(float reduceTime, int except = -1)
    {
        foreach (var skill in skills.Values)
        {
            if (skill.GetOperatorKey() != except)
            {
                skill.ReduceTime(reduceTime);
            }
        }
    }
    // 모든 스킬 쿨타임 감소(초), 제외 키
    public void ReduceSkillTimePersent(float reducePercent, int except = -1)
    {
        foreach (var skill in skills.Values)
        {
            if (skill.GetOperatorKey() != except)
            {
                skill.ReduceTimePercent(reducePercent);
            }
        }
    }
}
