public static class Define
{
    public const string _towerRoot = "Tower";
    public const string _EnviromentRoot = "Environment";
    public const string _SkillRoot = "Skill";
    // 스테이지 리소스
    public const string _DarkDungeonPath = _EnviromentRoot + "/" + "Dark Dungeon";
    public const string _NeonCityPath = _EnviromentRoot + "/" + "Neon City";
    public const string _RedShelterPath = _EnviromentRoot + "/" + "Red Alien Shelter";
    public const string _RockHillPath = _EnviromentRoot + "/" + "Rock Hills";
    public const string _SpaceForestPath = _EnviromentRoot + "/" + "Space Forest";
    public const string _ParadisePath = _EnviromentRoot + "/" + "Paradise";
    public const string _VioletLandscapePath = _EnviromentRoot + "/" + "Violet Landscape";


    // 분석용 이벤트 이름
    public const string StageStarted = "StageStarted";
    public const string StageEnded = "StageEnded";
    public const string ItemStageEnded = "ItemStageEnded";
    public const string DeckStageStarted = "DeckStageStarted";
    public const string DeckStageEnded = "DeckStageEnded";
    public const string SkillUsed = "SkillUsed";
    public const string TowerPlaced = "TowerPlaced";
    public const string CardSelected = "CardSelected";
    public const string OperatorGradeup = "OperatorGradeup";
    public const string OperatorLevelup = "OperatorLevelup";
    public const string SkillUpgrade = "SkillUpgrade";
    public const string SkillReset = "SkillReset";
    public const string ItemEquip = "ItemEquip";
    public const string GoldItemUsed = "GoldItemUsed";
    public const string EqiupScrapped = "EqiupScrapped";
    public const string CraftPerformed = "CraftPerformed";
    public const string ProductPurchased = "ProductPurchased";
    public const string AdRemovePurchased = "AdRemovePurchased";
    public const string CrystalChanged = "CrystalChanged";
    public const string CoinChanged = "CoinChanged";




    // 분석용 파라미터 이름
    public const string stageKey = "stageKey";
    public const string numberPlayedBefore = "numberPlayedBefore";
    public const string numberClearedBefore = "numberClearedBefore";
    public const string numberDeckTower = "numberDeckTower";
    public const string numberDeckOperator = "numberDeckOperator";
    public const string playerCoinBalance = "playerCoinBalance";
    public const string playerGrowthItem = "playerGrowthItem";
    public const string playerSkillGrowthItemLow = "playerSkillGrowthItemLow";
    public const string playerSkillGrowthItemHigh = "playerSkillGrowthItemHigh";
    public const string isCleared = "isCleared";
    public const string StageKey = "stageKey";
    public const string stageEndCause = "stageEndCause";
    public const string stagePlayTime = "stagePlayTime";
    public const string numberTowerPlaced = "numberTowerPlaced";
    public const string numberSkillUsed = "numberSkillUsed";
    public const string stageExpGained = "stageExpGained";
    public const string stageCoinGained = "stageCoinGained";
    public const string gainedItemJson = "gainedItemJson";
    public const string deckTowerJson = "deckTowerJson";
    public const string deckOperatorJson = "deckOperatorJson";
    public const string operatorKey = "operatorKey";
    public const string towerKey = "towerKey";
    public const string numberTowerPlacedBefore = "numberTowerPlacedBefore";
    public const string cardKey = "cardKey";
    public const string operatorGradeBefore = "operatorGradeBefore";
    public const string operatorLevel = "operatorLevel";
    public const string operatorGrade = "operatorGrade";
    public const string operatorLevelBefore = "operatorLevelBefore";
    public const string operatorLevelAfter = "operatorLevelAfter";
    public const string numberBasicSDUsed = "numberBasicSDUsed";
    public const string numberPreciseSDUsed = "numberPreciseSDUsed";
    public const string numberSpecialSDUsed = "numberSpecialSDUsed";
    public const string numberCoreSDUsed = "numberCoreSDUsed";
    public const string skillKey = "skillKey";
    public const string skillLevelBefore = "skillLevelBefore";
    public const string skillKeyBefore = "skillKeyBefore";
    public const string skillKeyAfter = "skillKeyAfter";
    public const string equipmentKey = "equipmentKey";
    public const string equipmentGrade = "equipmentGrade";
    public const string equipmentEffect1 = "equipmentEffect1";
    public const string equipmentEffect2 = "equipmentEffect2";
    public const string equipmentEffect3 = "equipmentEffect3";
    public const string itemKey = "itemKey";
    public const string coinGained = "coinGained";
    public const string craftType = "craftType";
    public const string craftCount = "craftCount";
    public const string craftResultJson = "craftResultJson";
    public const string productKey = "productKey";
    public const string timePurchased = "timePurchased";
    public const string playerCrystalBefore = "playerCrystalBefore";
    public const string playerCoinBefore = "playerCoinBefore";
    public const string adWatchedBefore = "adWatchedBefore";
    public const string numberCardRefreshed = "numberCardRefreshed";
    public const string numberDailyShopRefreshed = "numberDailyShopRefreshed";
    public const string numberDailyShopAdWatched = "numberDailyShopAdWatched";
    public const string numberSingleCrystalAdWatched = "numberSingleCrystalAdWatched";
    public const string numberSingleCoinAdWatched = "numberSingleCoinAdWatched";
    public const string changeType = "changeType";
    public const string numberCrystalChanged = "numberCrystalChanged";
    public const string changeSource = "changeSource";
    public const string numberCrystalBefore = "numberCrystalBefore";
    public const string numberCoinChanged = "numberCoinChanged";
    public const string numberCoinBefore = "numberCoinBefore";
    public const string gained = "gained";
    public const string spent = "spent";
}

public enum BULLETTYPE
{
    AMMUNITION,
    ENERGY
}

public enum TUTORIALTYPE
{
    QUEST,
    ORGANIZE,
    CRAFT,
    INVENTORY,
    OPERATORINFO,
    SHOP
}