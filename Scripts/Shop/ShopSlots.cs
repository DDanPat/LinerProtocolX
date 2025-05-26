using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopSlots : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI discount;
    [SerializeField] private TextMeshProUGUI price;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Image priceIcon;
    [SerializeField] private GameObject dicountObj;

    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject buyOverlay;

    private string resources; // 소모 재화
    private float resourcesValue; // 소모 재화 사용량

    int randomPrice;
    ShopItemData shopItemData;

    private void Start()
    {
        buyButton.onClick.AddListener(OnClickBuyButton);
    }

    private void OnClickBuyButton()
    {
        if (shopItemData.itemInfo.isFree == 1)
        {
            HandleAdOffer();
        }
        int cost = CalculateCost();
        int currentBalance = GetCurrentBalance();
        ShowPurchasePopup(cost, currentBalance);
    }

    // 광고 시청 처리
    private void HandleAdOffer()
    {
        // TODO 광고 기능 추가
        QuestManager.Instance.UpdateQuestProgress(QuestConditions.adWatch);
    }

    // 비용 계산
    private int CalculateCost()
    {
        return Mathf.CeilToInt(resourcesValue);
    }

    // 현재 잔액 조회
    private int GetCurrentBalance()
    {
        switch (resources)
        {
            case "골드":
                return GameManager.Instance.Player.Gold;
            case "보석":
                return GameManager.Instance.Player.Gem;
            case "현금":
                return int.MaxValue;
            default:
                return 0;
        }
    }

    // 구매 팝업 띄우기
    private void ShowPurchasePopup(int cost, int currentBalance)
    {
        PurchasePopup.Instance.Show(
            currency: resources,
            product: shopItemData.itemInfo.name,
            cost: cost,
            currentBalance: currentBalance,
            confirmCallback: () =>
            {
                ProcessPurchase(cost);
                if (shopItemData.itemInfo.key < 12200 || shopItemData.itemInfo.isFree == 1)
                {
                    if (shopItemData.itemInfo.key < 12200)
                    {
                        int index = GameManager.Instance.Player.dailyShopList.IndexOf(shopItemData);
                        GameManager.Instance.Player.dailyShopList[index].isBuyOverlay = true;
                    }
                    else
                    {
                        int index = GameManager.Instance.Player.resourcesShopList.IndexOf(shopItemData);
                        GameManager.Instance.Player.resourcesShopList[index].isBuyOverlay = true;
                    }

                    QuestManager.Instance.UpdateQuestProgress(QuestConditions.shopPurchase);
                    buyOverlay.SetActive(true);
                    buyButton.interactable = false;
                }
                // 커스터 이벤트 - 상점 구매
                int costGold = 0;
                int costGem = 0;
                if (resources == "골드")
                    costGold = cost;
                else if (resources == "보석")
                    costGem = cost;
                AnalyticsManager.Instance.SendEvent(Define.ProductPurchased, new Dictionary<string, object> {
                    { Define.productKey, shopItemData.itemInfo.key },
                    { Define.timePurchased, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")},
                    { Define.playerCrystalBefore, GameManager.Instance.Player.Gem + costGem},
                    { Define.playerCoinBefore, GameManager.Instance.Player.Gold + costGold},
                });
            },
            cancelCallback: () =>
            {
                //Debug.Log("구매를 취소하였습니다.");
            }
        );
    }

    // 실제 구매 처리
    private void ProcessPurchase(int cost)
    {
        DeductCost(cost);
        GrantShopItem();
    }

    // 비용 차감
    private void DeductCost(int cost)
    {
        switch (resources)
        {
            case "골드":
                GameManager.Instance.Player.Gold -= cost;
                break;
            case "보석":
                GameManager.Instance.Player.Gem -= cost;
                break;
            case "현금":
                // TODO: IAP 처리 루틴
                break;
        }
    }

    // 아이템 지급
    private void GrantShopItem()
    {
        var inv = GameManager.Instance.Player.inventory.items;
        if (inv.ContainsKey(shopItemData.itemInfo.itemKey))
            inv[shopItemData.itemInfo.itemKey].amount += shopItemData.itemInfo.count;
        else if (shopItemData.itemInfo.itemKey == 9900)
            GameManager.Instance.Player.Gem += shopItemData.itemInfo.count;
        else if (shopItemData.itemInfo.itemKey == 9901)
            GameManager.Instance.Player.Gold += shopItemData.itemInfo.count;
    }

    public void SetShopItem(ShopItemData item)
    {
        buyOverlay.SetActive(item.isBuyOverlay);
        buyButton.interactable = !item.isBuyOverlay;
        dicountObj.SetActive(false);
        shopItemData = item;
        itemName.text = $"{item.itemInfo.name} {item.itemInfo.count}개";
        SetPrice(item.itemInfo);
        SetItemIcon();
    }

    private void SetPrice(DailyShopTable item)
    {
        List<int> itemPriceValue = new List<int>();
        if (item.priceCash > 0) itemPriceValue.Add(0);
        if (item.priceCoin > 0) itemPriceValue.Add(1);
        if (item.priceCrystal > 0) itemPriceValue.Add(2);

        if (item.isFree == 0)
        {
            randomPrice = UnityEngine.Random.Range(0, itemPriceValue.Count);
            int priceValue = itemPriceValue[randomPrice];
            switch (priceValue)
            {
                case 0:
                    priceIcon.enabled = false;
                    price.text = $"${item.priceCash}";
                    resources = "현금";
                    resourcesValue = item.priceCash;
                    break;
                case 1:
                    priceIcon.sprite = Resources.Load<Sprite>(path: $"ItemIcons/12310_Coin");
                    price.text = $"{item.priceCoin}";
                    resources = "골드";
                    resourcesValue = item.priceCoin;
                    break;
                case 2:
                    priceIcon.sprite = Resources.Load<Sprite>(path: $"ItemIcons/12300_Crystal");
                    price.text = $"{item.priceCrystal}";
                    resources = "보석";
                    resourcesValue = item.priceCrystal;
                    break;
            }

        }
        else
        {
            priceIcon.enabled = false;
            price.text = "광고";
            resources = "광고";
            resourcesValue = 0;
        }

        if (item.discount > 0)
        {
            dicountObj.SetActive(true);
            discount.text = $"{item.discount}%";
        }
    }

    private void SetItemIcon()
    {
        var inventory = GameManager.Instance.Player.inventory.items;
        if (inventory.ContainsKey(shopItemData.itemInfo.itemKey))
        {
            itemIcon.sprite = inventory[shopItemData.itemInfo.itemKey].data.icon;
        }
        else
        {
            Sprite[] allSprites = Resources.LoadAll<Sprite>("ItemIcons");
            Sprite matchedSprite = allSprites
                .FirstOrDefault(sprite => sprite.name.Contains(shopItemData.itemInfo.key.ToString()));

            itemIcon.sprite = matchedSprite;
        }

        if (itemIcon.sprite == null)
        {
            itemIcon.sprite = Resources.Load<Sprite>(path: $"ItemIcons/12310_Coin");
        }
    }

    public void SetBuyOverlay()
    {
        buyOverlay.SetActive(false);
        buyButton.interactable = true;
    }
}
