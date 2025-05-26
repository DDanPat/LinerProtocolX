using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupTowerInfo : PopupWindow
{
    [SerializeField] private Image towerSlot;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI attackRateText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI criticalText;
    [SerializeField] private TextMeshProUGUI criticalCoefficientText;

    public override void SetPopupUI()
    {
        var towerData = popupData as TowerData;

        if (towerData != null)
        {
            towerSlot.sprite = towerData.profile;
            
            nameText.text = towerData.towerInfoTable.name;
            attackText.text = " " + towerData.towerInfoTable.attack;
            attackRateText.text = " " + towerData.towerInfoTable.attackRate;
            rangeText.text = " " + towerData.towerInfoTable.range;
            accuracyText.text = " " + towerData.towerInfoTable.accuracy;
            criticalText.text = " " + towerData.towerInfoTable.criticalRate;
            criticalCoefficientText.text = " " + towerData.towerInfoTable.criticalCoefficient;
        }
        
    }
}
