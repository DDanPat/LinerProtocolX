
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public Inventory inventory;
    public DeckData deck;

    public OperatorData favoriteOperator;

    // 스테이지 경험치 - 플레이어 LV
    // 아이테 경험치 - 오퍼레이터 LV 

    public static event System.Action OnResourceChanged;

    [SerializeField] private int gold;
    [SerializeField] private int gem;
    [SerializeField] public int clearStage = -1;

    public int Gold
    {
        get => gold;
        set
        {
            if (gold != value)
            {
                gold = value;
                OnResourceChanged?.Invoke();
            }
        }
    }
    public int Gem        // 유료재화
    {
        get => gem;
        set
        {
            if (gem != value)
            {
                gem = value;
                OnResourceChanged?.Invoke();
            }
        }
    }

    public List<ShopItemData> dailyShopList = new List<ShopItemData>();
    public List<ShopItemData> resourcesShopList = new List<ShopItemData>();

    public bool isGachaFrist = true;
    public DateTime nextDailyResetTime; // 일일 상점 갱신 시간
    public DateTime nextWeeklyResetTime; // 주간 상점 갱신 시간

    private int Count = 2;
    public int ResetCount { get { return Count; } set { Count = value; } }
}
