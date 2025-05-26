public class DataManager
{
    public ActiveSkillTableLoader ActiveSkillTableLoader { get; private set; }
    public ActiveSkillLevelTableLoader ActiveSkillLevelTableLoader { get; private set; }
    public CardTableLoader CardTableLoader { get; private set; }
    public CategoryEntryTableLoader CategoryEntryTableLoader { get; private set; }
    public DropEntryTableLoader DropEntryTableLoader { get; private set; }
    public DropTableLoader DropTableLoader { get; private set; }
    public EnemyTableLoader EnemyTableLoader { get; private set; }
    public EquipmentEffectTableLoader EquipmentEffectTableLoader { get; private set; }
    public EquipmentTableLoader EquipmentTableLoader { get; private set; }
    public ItemTableLoader ItemTableLoader { get; private set; }
    public OperatorTableLoader OperatorTableLoader { get; private set; }
    public PassiveSkillTableLoader PassiveSkillTableLoader { get; private set; }
    public ProductionCategoryProbabilityLoader ProductionCategoryProbabilityLoader { get; private set; }
    public ProductionPrimaryProbabilityLoader ProductionPrimaryProbabilityLoader { get; private set; }
    public TowerTableLoader TowerTableLoader { get; private set; }
    public StageInfoTableLoader StageDataLoader { get; private set; }
    public StageThemeTableLoader StageThemeTableLoader { get; private set; }
    public DailyShopTableLoader DailyShopTableLoader { get; private set; }

    public QuestTableLoader QuestTableLoader { get; private set; }
    public LoadingTextTableLoader LoadingTextTableLoader { get; private set; }


    public void Initialize()
    {
        ActiveSkillTableLoader = new ActiveSkillTableLoader();
        ActiveSkillLevelTableLoader = new ActiveSkillLevelTableLoader();
        CardTableLoader = new CardTableLoader();
        CategoryEntryTableLoader = new CategoryEntryTableLoader();
        DropEntryTableLoader = new DropEntryTableLoader();
        DropTableLoader = new DropTableLoader();
        EnemyTableLoader = new EnemyTableLoader();
        EquipmentEffectTableLoader = new EquipmentEffectTableLoader();
        EquipmentTableLoader = new EquipmentTableLoader();
        ItemTableLoader = new ItemTableLoader();
        OperatorTableLoader = new OperatorTableLoader();
        PassiveSkillTableLoader = new PassiveSkillTableLoader();
        ProductionCategoryProbabilityLoader = new ProductionCategoryProbabilityLoader();
        ProductionPrimaryProbabilityLoader = new ProductionPrimaryProbabilityLoader();
        TowerTableLoader = new TowerTableLoader();
        StageDataLoader = new StageInfoTableLoader();
        StageThemeTableLoader = new StageThemeTableLoader();
        DailyShopTableLoader = new DailyShopTableLoader();
        QuestTableLoader = new QuestTableLoader();
        LoadingTextTableLoader = new LoadingTextTableLoader();
    }
}
