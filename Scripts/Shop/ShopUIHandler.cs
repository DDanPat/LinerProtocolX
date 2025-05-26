using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemData
{
    public DailyShopTable itemInfo;
    public bool isBuyOverlay = false;
}

public class ShopUIHandler : MonoBehaviour
{
    List<ShopItemData> dailyShopAllList = new List<ShopItemData>();
    List<ShopItemData> dailyShopFreeList = new List<ShopItemData>();
    List<ShopItemData> resourcesShopList = new List<ShopItemData>();

    [SerializeField] private GameObject shopSlotPrefab;
    [SerializeField] private Transform dailyShopPos;
    [SerializeField] private int dailyShopCount = 6;
    [SerializeField] private Transform resourcesShopCoinPos;
    [SerializeField] private Transform resourcesShopCrystalPos;

    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI resetCountText;
    [SerializeField] private TextMeshProUGUI shopDailyTimerText;


    [SerializeField] private Button tutorialButton;

    public event System.Action OnShopUpdated;

    private bool isSubscribedToDailyReset = false;

    private void Awake()
    {
        resetButton.onClick.AddListener(OnClickResetButton);
        tutorialButton.onClick.AddListener(OpenTutorial);
    }
    private void OnEnable()
    {
        // 갱신 해야 되면 갱신
        if (UIManager.Instance.ShouldShopDaily)
        {
            DailyShopReset();
        }
        // 이미 구독되어 있지 않으면 구독
        if (!isSubscribedToDailyReset)
        {
            SubscribeToDailyResetEvent();
        }
    }
    private void Start()
    {
        SetResetCountText();
        SetShopList();
    }
    private void OnDestroy()
    {
        // 구독 해제
        if (isSubscribedToDailyReset)
        {
            TimerHandler.OnDailyReset -= DailyShopReset;
            isSubscribedToDailyReset = false;
        }
    }


    private void SetResetCountText()
    {
        resetCountText.text = $"초기화\n{GameManager.Instance.Player.ResetCount}/2";
    }

    private void OnClickResetButton()
    {
        if (GameManager.Instance.Player.ResetCount > 0)
        {
            foreach (var data in GameManager.Instance.Player.dailyShopList)
            {
                data.isBuyOverlay = false;
            }
            GameManager.Instance.Player.dailyShopList.Clear();
            DailyShopSetting();

            GameManager.Instance.Player.ResetCount--;
            SetResetCountText();
        }
    }

    private void SetShopList()
    {
        foreach (var item in GameManager.Instance.DataManager.DailyShopTableLoader.ItemsList)
        {
            if (item.key < 12200)
            {
                if (item.isFree == 1)
                    dailyShopFreeList.Add(new ShopItemData
                    {
                        itemInfo = item,
                        isBuyOverlay = false
                    });
                else dailyShopAllList.Add(new ShopItemData
                {
                    itemInfo = item,
                    isBuyOverlay = false
                });
            }
            else resourcesShopList.Add(new ShopItemData
            {
                itemInfo = item,
                isBuyOverlay = false
            });
        }
        DailyShopSetting();
        ResourcesShopSetting();
    }

    private void DailyShopSetting()
    {
        foreach (Transform child in dailyShopPos)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance.Player.dailyShopList.Count > 0)
        {
            List<ShopItemData> test = new();
            foreach (var item in GameManager.Instance.Player.dailyShopList)
            {
                test.Add(new ShopItemData
                {
                    itemInfo = GameManager.Instance.DataManager.DailyShopTableLoader.ItemsDict[item.itemInfo.key],
                    isBuyOverlay = item.isBuyOverlay
                });
            }

            GameManager.Instance.Player.dailyShopList.Clear();

            foreach (var item in test)
            {
                var slot = Instantiate(shopSlotPrefab, dailyShopPos).GetComponent<ShopSlots>();
                slot.SetShopItem(item);
                GameManager.Instance.Player.dailyShopList.Add(item);
            }
            return;
        }

        GameManager.Instance.Player.dailyShopList.Clear();


        bool isFree = true;

        for (int i = 0; i < dailyShopCount; i++)
        {
            ShopSlots shopSlots = Instantiate(shopSlotPrefab, dailyShopPos).GetComponent<ShopSlots>();
            ShopItemData shopItemData;

            if (dailyShopAllList.Count == 0 || dailyShopFreeList.Count == 0) return;

            if (isFree)
            {
                int random = Random.Range(0, dailyShopFreeList.Count);
                shopSlots.SetShopItem(dailyShopFreeList[random]);
                shopItemData = dailyShopFreeList[random];
            }
            else
            {
                int random = Random.Range(0, dailyShopAllList.Count);
                shopSlots.SetShopItem(dailyShopAllList[random]);
                shopItemData = dailyShopAllList[random];
            }

            GameManager.Instance.Player.dailyShopList.Add(shopItemData);
            isFree = false;
        }
    }

    private void ResourcesShopSetting()
    {
        foreach (Transform child in resourcesShopCrystalPos)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in resourcesShopCoinPos)
        {
            Destroy(child.gameObject);
        }

        if (GameManager.Instance.Player.resourcesShopList.Count > 0)
        {
            List<ShopItemData> test = new();
            foreach (var item in GameManager.Instance.Player.resourcesShopList)
            {
                test.Add(new ShopItemData
                {
                    itemInfo = GameManager.Instance.DataManager.DailyShopTableLoader.ItemsDict[item.itemInfo.key],
                    isBuyOverlay = item.isBuyOverlay
                });
            }

            GameManager.Instance.Player.resourcesShopList.Clear();

            foreach (var item in test)
            {
                SetresourcesShopList(item);
                GameManager.Instance.Player.resourcesShopList.Add(item);
            }
            return;
        }

        GameManager.Instance.Player.resourcesShopList.Clear();

        foreach (var item in resourcesShopList)
        {
            SetresourcesShopList(item);

            GameManager.Instance.Player.resourcesShopList.Add(item);
        }
    }

    private void SetresourcesShopList(ShopItemData item)
    {
        if (item.itemInfo.itemKey == 9900)
        {
            ShopSlots shopSlots = Instantiate(shopSlotPrefab, resourcesShopCrystalPos).GetComponent<ShopSlots>();
            shopSlots.SetShopItem(item);
        }
        else if (item.itemInfo.itemKey == 9901)
        {
            ShopSlots shopSlots = Instantiate(shopSlotPrefab, resourcesShopCoinPos).GetComponent<ShopSlots>();
            shopSlots.SetShopItem(item);
        }
    }

    public void SetShopDailyTimer(string daily)
    {
        shopDailyTimerText.text = $"다음 초기화까지 남은시간\n{daily}";
    }

    private void SubscribeToDailyResetEvent()
    {
        TimerHandler.OnDailyReset += DailyShopReset;
        isSubscribedToDailyReset = true;
    }


    public void DailyShopReset()
    {
        UIManager.Instance.ShouldShopDaily = false;
        GameManager.Instance.Player.dailyShopList.Clear();
        GameManager.Instance.Player.resourcesShopList.Clear();
        GameManager.Instance.Player.ResetCount = 2;
        SetResetCountText();
        DailyShopSetting();
        ResourcesShopListReset();
        ResourcesShopSetting();
        // 임의 저장
        GameManager.Instance.SavePlayer();
    }

    public void ResourcesShopListReset()
    {
        foreach (var item in GameManager.Instance.Player.resourcesShopList)
        {
            item.isBuyOverlay = false;
        }
    }

    public void OpenTutorial()
    {
        UIManager.Instance.tutorialUIHandler.OpenTutorialPanel(TUTORIALTYPE.SHOP);
    }
}
