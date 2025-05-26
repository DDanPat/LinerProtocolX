using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[System.Serializable]
public class GachaData
{
    public Sprite characterImage;
    public Sprite currencyIcon;
    public Sprite resultBG;
    public ResourcesType type;
    public int costPerDraw;
    public string bannerName;
    public int GachaKey;
}

public class GachaController : MonoBehaviour
{
    [Header("슬라이드 패널")]
    public GameObject[] panels;
    public float slideDuration = 0.5f; // 슬라이드 넘어가는 속도
    private int currentIndex = 0;

    private bool isSliding = false; // 슬라이드 상태 확인
    private Vector2 panelPosition;

    [Header("UI Elements")]
    public Image currencyIcon1; // 1회 뽑기에 사용되는 재화 이미지
    public Image currencyIcon10; // 10회 뽑기에 사용되는 재화 이미지
    public TextMeshProUGUI costText1; // 1회 뽑기에 사용되는 재화 코스트
    public TextMeshProUGUI costText10; // 10회 뽑기에 사용되는 재화 코스트
    public TextMeshProUGUI craftBannerText; // 뽑기 종류 배너

    public Button leftButton; // 왼쪽으로 넘기기 버튼
    public Button rightButton; // 오른쪽으로 넘기기 버튼

    public Button draw1Button; // 1회 뽑기 버튼
    public Button draw10Button; // 10회 뽑기 버튼


    [SerializeField] private GameObject fristGachaPanel;

    [Header("Gacha Data")]
    public List<GachaData> gachaDataList = new List<GachaData>(); // 가챠 뽑기 종류 테이블
    [SerializeField] private GachaSlots gachaSlotsPrefabs; // 가챠 뽑기 슬롯 프리팹
    [SerializeField] private Transform gachaSlotsPrefabsTransform; // 가챠 뽑기 슬롯 프리팹이 소환될 위치

    // 가챠 확률 테이블
    private List<ProductionPrimaryProbability> productionTable = new List<ProductionPrimaryProbability>();
    private List<ProductionCategoryProbability> catrgoryTable = new List<ProductionCategoryProbability>();
    private List<CategoryEntryTable> entryTable = new List<CategoryEntryTable>();

    ObjectPoolManager objectPoolManager;

    GachaData gachaData;

    private void Start()
    {
        objectPoolManager = ObjectPoolManager.Instance;

        currentIndex = 0;
        leftButton.onClick.AddListener(() => ShowPanel(1));
        rightButton.onClick.AddListener(() => ShowPanel(-1));
        draw1Button.onClick.AddListener(() => Draw(1));
        draw10Button.onClick.AddListener(() => Draw(10));

        UpdatePanelUI(currentIndex);
        SetPanel();
        SetTable();
    }

    private void FristGachaPanel(GachaData data = null)
    {
        data = data == null ? gachaDataList[0] : data;
        fristGachaPanel.SetActive(GameManager.Instance.Player.isGachaFrist && data == gachaDataList[0]);
    }


    private void SetTable()
    {
        productionTable = GameManager.Instance.DataManager.ProductionPrimaryProbabilityLoader.ItemsList;
        catrgoryTable = GameManager.Instance.DataManager.ProductionCategoryProbabilityLoader.ItemsList;
        entryTable = GameManager.Instance.DataManager.CategoryEntryTableLoader.ItemsList;
    }

    private void SetPanel()
    {
        for (int i = 0; i < gachaDataList.Count; i++)
        {
            panels[i].transform.GetChild(0).GetComponent<Image>().sprite = gachaDataList[i].characterImage;

            //panels[i].GetComponent<Image>().sprite = gachaDataList[i].characterImage;
        }
        CraftPanelUI.Instance.ResultPanelUpdate(gachaDataList[currentIndex].resultBG);
    }


    // 가챠 패널 넘기기
    public void ShowPanel(int direction)
    {
        int nextIndex = currentIndex + direction;

        if (nextIndex == currentIndex || isSliding) return;

        // nextIndex가 panels의 총 길이 보다 크면 인덱스 0번을
        // 총길이보다 작으면 인덱스 번호의 끝번호를 부여
        if (nextIndex >= panels.Length) nextIndex = 0;
        else if (nextIndex < 0) nextIndex = panels.Length - 1;

        isSliding = true;

        RectTransform fromPanel = panels[currentIndex].GetComponent<RectTransform>();
        RectTransform toPanel = panels[nextIndex].GetComponent<RectTransform>();

        fromPanel.transform.SetAsLastSibling();
        toPanel.transform.SetAsFirstSibling();

        float panelWidth = ((RectTransform)fromPanel.parent).rect.width;

        // 현재 패널 번호보다 인덱스가 크면 오른쪽에서 왼쪽으로 이동
        // 현재 패널 번호보다 인덱스가 작으면 왼쪽에서 오른쪽으로 이동
        if (currentIndex < nextIndex)
        {
            panelPosition = new Vector2(panelWidth, 0); // 오른쪽에서 in
        }
        else
        {
            panelPosition = new Vector2(-panelWidth, 0); // 왼쪽에서 in
        }


        //panelPosition = (currentIndex < nextIndex) ? new Vector2(Screen.width, 0) : new Vector2(-Screen.width, 0);

        toPanel.gameObject.SetActive(true);
        toPanel.anchoredPosition = panelPosition;

        fromPanel.DOAnchorPos(-panelPosition, slideDuration).SetEase(Ease.OutCubic);
        toPanel.DOAnchorPos(Vector2.zero, slideDuration).SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                fromPanel.gameObject.SetActive(false);
                fromPanel.anchoredPosition = Vector2.zero;
                currentIndex = nextIndex;
                UpdatePanelUI(currentIndex);
                isSliding = false;
            });
    }

    private void UpdatePanelUI(int index)
    {
        if (index < 0 || index >= gachaDataList.Count) return;

        var data = gachaDataList[index];

        currencyIcon1.sprite = data.currencyIcon;
        currencyIcon10.sprite = data.currencyIcon;

        costText1.text = $"X{data.costPerDraw}";
        costText10.text = $"X{data.costPerDraw * 10}";
        craftBannerText.text = data.bannerName;

        CraftPanelUI.Instance.ResultPanelUpdate(gachaDataList[currentIndex].resultBG);
        FristGachaPanel(data);
    }


    // 보유하고 있는 재화 수를 판단하여 bool값 부여
    private void Draw(int count)
    {
        if (isSliding) return;

        var data = gachaDataList[currentIndex];
        int totalCost = data.costPerDraw * count;
        int currentBalance = GetCurrentBalance(data.type);

        // 1) 팝업 띄우기
        PurchasePopup.Instance.Show(
            currency: GetCurrencyName(data.type),      // "골드", "보석" 등
            product: data.bannerName,                  // 배너 이름
            cost: totalCost,                           // 필요 코스트
            currentBalance: currentBalance,            // 현재 보유량
            confirmCallback: () =>
            {
                // 2) 확인 누르면 실제 뽑기 처리
                if (TryConsumeResource(data.type, totalCost))
                {
                    ExecuteGacha(count, data);
                }
                else
                {
                    Debug.LogWarning("재화가 부족합니다!");
                }
            },
            cancelCallback: () =>
            {
                // 가챠 뽑기를 취소
            }
        );
    }

    // 팝업 확인 후 실제 뽑기 실행 로직 분리
    private void ExecuteGacha(int count, GachaData data)
    {
        QuestManager.Instance.UpdateQuestProgress(QuestConditions.craft, QuestConditions.none, count);

        CraftPanelUI.Instance.ClearPreviousGachaResults();
        CraftPanelUI.Instance.resultsPanel.SetActive(true);
        CraftPanelUI.Instance.returnButton.gameObject.SetActive(false);
        CraftPanelUI.Instance.resultsButton.gameObject.SetActive(true);

        // 결과 0, 10개로 초기화
        List<int> results = new List<int>(10);
        for (int i = 0; i < 10; i++) results.Add(0);

        for (int i = 0; i < count; i++)
        {
            var slot = objectPoolManager.GetObject(ObjectType.GachaSlot,
                        gachaSlotsPrefabsTransform.position,
                        Quaternion.identity,
                        gachaSlotsPrefabsTransform)
                        .GetComponent<GachaSlots>();
            slot.transform.SetParent(gachaSlotsPrefabsTransform);
            CraftPanelUI.Instance.gachaSlotsList.Add(slot);

            int prodKey = GetProductionKey(data.GachaKey, count);
            var catResult = GetCatrgoryResult(prodKey);
            var result = GetRandomResult(catResult.categoryTable);

            slot.SetData(result);
            TakeToInventory(result.item, slot);
            results[i] = result.key;
        }
        // 임의 저장
        GameManager.Instance.SavePlayer();

        // 커스텀 이벤트 - 제작 결과 페이지
        string craftKey = "";
        switch (data.GachaKey)
        {
            case 0:
                craftKey = "일반";
                break;
            case 1:
                craftKey = "희귀";
                break;
            case 2:
                craftKey = "특별";
                break;
        }
        string craftResultJson = JsonConvert.SerializeObject(results);
        AnalyticsManager.Instance.SendEvent(Define.CraftPerformed, new Dictionary<string, object> {
            { Define.craftType, craftKey },
            { Define.craftCount, count},
            { Define.craftResultJson, craftResultJson},
            });
    }

    // enum → 한국어 명칭 변환 헬퍼
    private string GetCurrencyName(ResourcesType type)
    {
        switch (type)
        {
            case ResourcesType.CorePiece: return "코어 조각";
            case ResourcesType.Core: return "코어";
            case ResourcesType.Gem: return "보석";
            default: return "재화";
        }
    }

    // 현재 보유 재화 조회 헬퍼
    private int GetCurrentBalance(ResourcesType type)
    {
        // PurchasePopup 에서 표시만 하기 위한 조회, 실제 차감은 TryConsumeResource() 내부에서
        switch (type)
        {
            case ResourcesType.CorePiece: return CraftPanelUI.Instance.corePieceValue;
            case ResourcesType.Core: return CraftPanelUI.Instance.coreValue;
            case ResourcesType.Gem: return CraftPanelUI.Instance.gemValue;
            default: return 0;
        }
    }


    private bool TryConsumeResource(ResourcesType type, int amount)
    {
        int resourceRef = GetResourceRef(type);

        if (resourceRef < amount)
            return false;

        resourceRef -= amount;
        SetResourceAmount(type, amount);
        CraftPanelUI.Instance.SetResources();
        return true;
    }

    private int GetResourceRef(ResourcesType type)
    {
        switch (type)
        {
            case ResourcesType.CorePiece:
                return GameManager.Instance.Player.inventory.items[9500].amount;
            case ResourcesType.Core:
                return GameManager.Instance.Player.inventory.items[9501].amount;
            case ResourcesType.Gem:
                return GameManager.Instance.Player.Gem;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), $"처리되지 않은 자원 타입입니다: {type}");
        }
    }
    private void SetResourceAmount(ResourcesType type, int value)
    {
        switch (type)
        {
            case ResourcesType.CorePiece:
                GameManager.Instance.Player.inventory.items[9500].amount -= value;
                break;
            case ResourcesType.Core:
                GameManager.Instance.Player.inventory.items[9501].amount -= value;
                break;
            case ResourcesType.Gem:
                GameManager.Instance.Player.Gem -= value;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), $"처리되지 않은 자원 타입입니다: {type}");
        }
    }


    // 1차 가챠: 제작 방식 선택
    public int GetProductionKey(int target, int count)
    {
        if (count == 1 && GameManager.Instance.Player.isGachaFrist)
        {
            GameManager.Instance.Player.isGachaFrist = false;
            FristGachaPanel();
            // 커널 - 확정 단챠
            AnalyticsManager.Instance.SendStep(501);
            return 11006;
        }

        var filtered = productionTable.Where(e => e.target == target).ToList();
        float totalWeight = filtered.Sum(e => e.weight);
        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var entry in filtered)
        {
            cumulative += entry.weight;
            if (randomValue <= cumulative)
                return entry.key;
        }

        return -1;
    }

    // 2차 가챠 : 아이템 카테고리 선택
    public ProductionCategoryProbability GetCatrgoryResult(int productionKey)
    {
        var filtered = catrgoryTable
            .Where(e => e.productionTable == productionKey) // 특정 categoryKey와 일치하는 항목만 필터링
            .OrderBy(e => Random.value) // 동일 가중치 항목 간의 선택 편향을 방지하기 위해 무작위로 정렬
            .ToList();  // 결과를 List<T> 형태로 반환해 추후 루프 및 추첨에 사용

        float totalWeight = filtered.Sum(e => e.weight);
        float rand = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var entry in filtered)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry;
        }

        return null;
    }

    // 3차 가챠 : 아이템 선택
    public CategoryEntryTable GetRandomResult(ProductionCategory categoryKey)
    {
        var filtered = entryTable
            .Where(e => e.categoryTable == categoryKey)
            .OrderBy(e => Random.value)
            .ToList();
        float totalWeight = filtered.Sum(e => e.weight);
        float rand = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var entry in filtered)
        {
            cumulative += entry.weight;
            if (rand <= cumulative)
                return entry;
        }

        return null;
    }

    public void TakeToInventory(int key, GachaSlots gachaSlots)
    {
        var inventory = GameManager.Instance.Player.inventory;
        if (inventory.operators.ContainsKey(key))
        {
            if (inventory.operators[key].isOwned)
            {
                // 이미 보유한 오퍼레이터
                gachaSlots.isOPOwned = true;
                inventory.items[9104].amount += 1;
            }
            else
            {
                inventory.operators[key].isOwned = true;
                inventory.operators[key].Upgrade();
            }
            QuestManager.Instance.UpdateQuestProgress(QuestConditions.operatorGet, QuestConditions.none);

        }
        else if (inventory.items.ContainsKey(key))
        {
            inventory.items[key].amount += 1;
        }
        //else if (inventory.equipment.ContainsKey(key))
        //{
        //  장비 아이템
        //}

    }


}
