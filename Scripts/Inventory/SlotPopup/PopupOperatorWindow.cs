using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupOperatorWindow : PopupWindow
{
    [Header("PopupUI component")] 
    public Image mainIcon;
    public Image[] gradeIcon = new Image[5];
    public Image roleIcon;
    public Image favoriteIcon;
    [SerializeField] private Sprite[] _gradeIcons = new Sprite[2];
    [SerializeField] private Sprite[] _favoriteIcons = new Sprite[2];
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI roleText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI atkRateText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI criticalText;
    public TextMeshProUGUI criticalMultiplierText;
    public TextMeshProUGUI descriptionText;
    
    public GameObject bottomInfo;
    public SkillSlot[] skillSlots = new SkillSlot[4];
    
    public Button tutorialButton;
    
    [Header("Upgrade Window component")]
    public GameObject gradeUpWindow;
    public GameObject levelUpWindow;
    public GameObject skillUpWindow;
    public GameObject skillInfoPopup;
    
    public override void SetPopupUI()
    {
        if (popupData != null && popupData is OperatorData opData)
        {
            mainIcon.sprite = opData.profile;
            int gradeLv = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade;
            bool isFavorite = popupData == GameManager.Instance.Player.favoriteOperator;
            SetGradeStars(gradeLv);
            SetRole(opData.operatorInfoTable.role);
            SetFavorite(isFavorite);
            
            
            // 오퍼레이터 SO 텍스트 데이터 연동
            nameText.text = opData.operatorInfoTable.name;
            levelText.text = "LV " +GameManager.Instance.Player.inventory.
                operators[opData.operatorInfoTable.key].level.ToString();
            descriptionText.text = opData.operatorInfoTable.operatorDesc;

            if (opData.operatorInfoTable.role == 1)
            {
                roleText.text = "공격형";
            }
            else if (opData.operatorInfoTable.role == 2)
            {
                roleText.text = "방어형";
            }
            else if (opData.operatorInfoTable.role == 3)
            {
                roleText.text = "지원형";
            }
            
            atkText.text = "+" + opData.operatorInfoTable.attack.ToString();
            atkRateText.text = "+" + opData.operatorInfoTable.attackRate.ToString();
            rangeText.text = "+" + opData.operatorInfoTable.range.ToString();
            accuracyText.text = "+" + opData.operatorInfoTable.accuracy.ToString();
            criticalText.text = "+" + opData.operatorInfoTable.criticalRate.ToString();
            criticalMultiplierText.text = "+" + opData.operatorInfoTable.criticalMultiplier.ToString();

            SetSkillSlots();

        }
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        bottomInfo.SetActive(true);
        PopupWindow gradeWindow = gradeUpWindow.GetComponent<PopupWindow>();
        PopupWindow levelWindow = levelUpWindow.GetComponent<PopupWindow>();
        PopupWindow skillWindow = skillUpWindow.GetComponent<PopupWindow>();
        gradeWindow?.CloseWindow();
        levelWindow?.CloseWindow();
        skillWindow?.CloseWindow();
        
        UIManager.Instance.inventoryUI.InventoryDataUpdateSlots();
    }

    private void SetGradeStars(int gradeLevel)
    {
        for (int i = 0; i < gradeIcon.Length; i++)
        {
            if (i < gradeLevel)
            {
                gradeIcon[i].sprite = _gradeIcons[1];
                gradeIcon[i].color = Color.white;
            }
            else
            {
                gradeIcon[i].sprite = _gradeIcons[0];
                gradeIcon[i].color = Color.black;
            }
        }
    }

    private void SetFavorite(bool isFavorite)
    {
        if(!isFavorite)
            favoriteIcon.sprite = _favoriteIcons[0];
        else
        {
            favoriteIcon.sprite = _favoriteIcons[1];
        }
    }

    private void SetRole(int role)
    {
        switch (role)
        {
            case 1:
                roleIcon.color = Color.red;
                break;
            case 2:
                roleIcon.color = Color.blue;
                break;
            case 3:
                roleIcon.color = Color.yellow;
                break;
            default:
                roleIcon.color = Color.white;
                break;
        }
    }

    private void SetSkillSlots()
    {
        var opData = popupData as OperatorData;
        
        if (opData.activeSkillData != null && opData.activeLv > 0)
        {
            skillSlots[0].data = opData.activeSkillData;
            skillSlots[0].button.interactable = true;
            skillSlots[0].SetFillSprite();
        }
        else
        {
            skillSlots[0].ClearSlot();
            skillSlots[0].button.interactable = false;
            skillSlots[0].SetDefaultBackground();
        }

        
        
        for (int i = 1; i < skillSlots.Length; i++)
        {
            if (opData.passiveSkillDatas != null && i - 1 < opData.passiveSkillDatas.Count)
            {
                skillSlots[i].data = opData.passiveSkillDatas[i - 1];
            }
            else
            {
                skillSlots[i].ClearSlot();
                skillSlots[i].button.interactable = false;
                skillSlots[i].SetDefaultBackground();
            }

            switch (i)
            {
                case 1:
                    if (opData.passive0LV > 0)
                    {
                        skillSlots[i].button.interactable = true;
                        skillSlots[i].SetFillSprite();
                    }
                    break;
                case 2:
                    if (opData.passive1LV > 0)
                    {
                        skillSlots[i].button.interactable = true;
                        skillSlots[i].SetFillSprite();
                    }
                    break;
                case 3:
                    if (opData.passive2LV > 0)
                    {
                        skillSlots[i].button.interactable = true;
                        skillSlots[i].SetFillSprite();
                    }
                    break;
            }
        }
    }

    public void OpenBottomInfo()
    {
        bottomInfo.SetActive(true);
    }

    public void OpenUpgradePopup()
    {
        bottomInfo.SetActive(false);
        
        PopupWindow window = gradeUpWindow.GetComponent<PopupWindow>();
        window.OpenWindow(popupData);
        PopupWindow otherWindow = levelUpWindow.GetComponent<PopupWindow>();
        otherWindow.CloseWindow();
        PopupWindow otherWindow2 = skillUpWindow.GetComponent<PopupWindow>();
        otherWindow2.CloseWindow();
        
    }

    public void LevelupPopup()
    {
        bottomInfo.SetActive(false);
        
        PopupWindow window = levelUpWindow.GetComponent<PopupWindow>();
        window.OpenWindow(popupData);
        PopupWindow otherWindow = gradeUpWindow.GetComponent<PopupWindow>();
        otherWindow.CloseWindow();
        PopupWindow otherWindow2 = skillUpWindow.GetComponent<PopupWindow>();
        otherWindow2.CloseWindow();
    }

    public void SkillUpPopup()
    {
        bottomInfo.SetActive(false);
        
        PopupWindow window = skillUpWindow.GetComponent<PopupWindow>();
        window.OpenWindow(popupData);
    }

    public void SkillInfoPopup(int skillType)
    {
        PopupSkillInfo popup = skillInfoPopup.GetComponent<PopupSkillInfo>();
        popup.skillType = skillType;
        popup.OpenWindow(popupData);
    }

    public void SetFavoriteOperator()
    {
        var opData = popupData as OperatorData;
        GameManager.Instance.Player.favoriteOperator = opData;
        
        UIManager.Instance.SetFavoirte();
    }

    public void OpenTutorial()
    {
        UIManager.Instance.OpenTutorial(TUTORIALTYPE.OPERATORINFO);
    }
}
