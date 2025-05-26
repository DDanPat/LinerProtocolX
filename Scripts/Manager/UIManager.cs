using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public InventoryUI inventoryUI;
    public OrganizeUI organizeUI;
    public QuestUIHandler questUIHandler;
    public ShopUIHandler shopUIHandler;
    public LobbyUIHandler lobbyUIHandler;
    public TutorialUIHandler tutorialUIHandler;

    public Image lobbyFavoriteOperator;
    private const string shouldShopDailyKey = "ShouldShopDaily";

    public bool ShouldShopDaily
    {
        get => PlayerPrefs.GetInt(shouldShopDailyKey, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(shouldShopDailyKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private const string shouldDailyLoginKey = "ShouldDailyLoginKey";

    public bool ShouldDailyLogin
    {
        get => PlayerPrefs.GetInt(shouldDailyLoginKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(shouldDailyLoginKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetFavoirte();
        UpdateButton();
    }

    public void SetFavoirte()
    {
        if (GameManager.Instance.Player.favoriteOperator != null)
        {
            lobbyFavoriteOperator.sprite = GameManager.Instance.Player.favoriteOperator.illust;
        }
    }

    // 진행 상태에 따른 버튼 활성 / 비활성
    public void UpdateButton()
    {
        lobbyUIHandler.UpdateButton();
    }

    public void OpenTutorial(TUTORIALTYPE tutorialType)
    {
        tutorialUIHandler.OpenTutorialPanel(tutorialType);
    }
}
