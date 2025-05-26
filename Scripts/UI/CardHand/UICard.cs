using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;


public class UICard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Image unitImage;
    [SerializeField] private Image operatorImage;
    [SerializeField] private GameObject operatorMask;
    [SerializeField] private TextMeshProUGUI unitCount;
    // 드래그 시작시 호출 함수
    public UnityAction<int, int> OnStartDragAction;
    // 드래그 종료시 호출 함수
    public UnityAction<int> OnEndDragAction;
    // 카드 키
    public int ItemKey { get; private set; }
    // 오퍼레이터 키
    public int OperatorKey { get; private set; }

    // 원래 위치
    private Vector3 originPos;
    // 갯수
    private int count;



    // 컴포넌트
    private RectTransform rt;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Setup(int itemKey, Sprite sprite)
    {
        ItemKey = itemKey;
        OperatorKey = -1;
        count = 1;
        unitImage.sprite = sprite;
        unitCount.text = count.ToString();
        operatorImage.gameObject.SetActive(false);
        operatorMask.SetActive(false);
    }
    public void Setup(TowerOperList info, int operatorKey = -1)
    {
        ItemKey = info.towerData.towerInfoTable.key;
        unitImage.sprite = info.towerData.profile;

        if (operatorKey == -1)
        {
            OperatorKey = -1;
            operatorImage.gameObject.SetActive(false);
            operatorMask.SetActive(false);
        }
        else
        {
            OperatorKey = info.operatorData.operatorInfoTable.key;
            operatorImage.gameObject.SetActive(true);
            operatorMask.SetActive(true);
            operatorImage.sprite = info.operatorData.profile;
        }

        count = 1;
        unitCount.text = count.ToString();
    }
    public void ChangeCount(int amount)
    {
        // 카드 갯수 증가
        count += amount;
        if (count <= 0)
        {
            Destroy(gameObject);
        }
        unitCount.text = count.ToString();
    }
    public bool IsEmpty()
    {
        return count <= 0;
    }


    #region MouseAction
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작
        OnStartDragAction?.Invoke(ItemKey, OperatorKey);
    }
    // 드래그 동안 위치 이동
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그시 위치 이동
        transform.position = eventData.position;
    }

    // 드래그 종료, 이동거리로 스폰 여부 판별
    public void OnEndDrag(PointerEventData eventData)
    {
        // layout에서 원위치
        gameObject.SetActive(false);
        gameObject.SetActive(true);

        // 콜백 실행
        OnEndDragAction?.Invoke(ItemKey);
    }
    #endregion
}
