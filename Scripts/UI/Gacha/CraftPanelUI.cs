using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ResourcesType
{
    CorePiece,
    Core,
    Gem
}

public class CraftPanelUI : MonoBehaviour
{
    public static CraftPanelUI Instance;

    public int corePieceValue;
    public int coreValue;
    public int gemValue;

    [SerializeField] private TextMeshProUGUI corePieceValueText;
    [SerializeField] private TextMeshProUGUI coreValueText;
    [SerializeField] private TextMeshProUGUI gemValueText;

    [Header("Button")]
    [SerializeField] public Button returnButton;
    [SerializeField] public Button resultsButton;
    [SerializeField] public Button tutorialButton;

    [Header("Panel")]
    public GameObject resultsPanel;


    public List<GachaSlots> gachaSlotsList = new List<GachaSlots>();

    private Color activeButtonColor;


    private void Awake()
    {
        Instance = this;
        ColorUtility.TryParseHtmlString("#215F9A", out activeButtonColor);
    }

    private void Start()
    {
        SetResources();
        
        returnButton.onClick.AddListener(RetrunButtonOnClick);
        resultsButton.onClick.AddListener(ResultsButtonOnClick);
        tutorialButton.onClick.AddListener(OpenTutorial);

        returnButton.gameObject.SetActive(false);
        resultsPanel.SetActive(false);
    }

    public void SetResources()
    {
        corePieceValue = GameManager.Instance.Player.inventory.items[9500].amount;
        coreValue = GameManager.Instance.Player.inventory.items[9501].amount;
        gemValue = GameManager.Instance.Player.Gem;

        ResourcesTextUpdate();
    }

    public void ResourcesTextUpdate()
    {
        corePieceValueText.text = corePieceValue.ToString("N0");
        coreValueText.text = coreValue.ToString("N0");
        gemValueText.text = gemValue.ToString("N0");
    }

    private void RetrunButtonOnClick()
    {
        resultsPanel.SetActive(false);
    }

    private void ResultsButtonOnClick()
    {
        for (int i = 0; i < gachaSlotsList.Count; i++)
        {
            if (!gachaSlotsList[i].isRotated) gachaSlotsList[i].Rotate();
        }

        resultsButton.gameObject.SetActive(false);
    }

    public void ClearPreviousGachaResults()
    {
        foreach (var slot in gachaSlotsList)
        {
            if (slot != null)
            {
                slot.OnDespawn();
            }
        }
        gachaSlotsList.Clear();
    }
    public void CheckGachaSlotsRotation()
    {
        if (gachaSlotsList.Count == 0) return;

        bool allRotated = gachaSlotsList.All(slot => slot.isRotated);

        resultsButton.gameObject.SetActive(!allRotated);
        returnButton.gameObject.SetActive(allRotated);
    }

    public void ResultPanelUpdate(Sprite sprite)
    {
        resultsPanel.GetComponent<Image>().sprite = sprite;
    }

    private void OnEnable()
    {
        if (GameManager.Instance.Player != null) SetResources();
    }

    public void OpenTutorial()
    {
        UIManager.Instance.tutorialUIHandler.OpenTutorialPanel(TUTORIALTYPE.CRAFT);
    }
}
