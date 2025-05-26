// 게임 씬에서 타워 클릭시 타워 정보 팝업을 표시

using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BattleTowerInfo : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button closeBG;

    [SerializeField] private Image TowerImage;
    [SerializeField] private GameObject OperatorMask;
    [SerializeField] private Image OperatorImage;
    [SerializeField] private TextMeshProUGUI totalName;
    [SerializeField] private TextMeshProUGUI attackValue;
    [SerializeField] private TextMeshProUGUI rateValue;
    [SerializeField] private TextMeshProUGUI rangeValue;
    [SerializeField] private TextMeshProUGUI accValue;
    [SerializeField] private TextMeshProUGUI criChanceValue;
    [SerializeField] private TextMeshProUGUI criScaleValue;
    [SerializeField] private TextMeshProUGUI killCount;
    private Tower tower;
    void Awake()
    {
        closeBtn.onClick.AddListener(Close);
        closeBG.onClick.AddListener(Close);
    }
    public void Set(Tower tower)
    {
        Time.timeScale = 0f;
        content.SetActive(true);
        this.tower = tower;
        tower.ShowRange(true);
        StringBuilder total = new();

        TowerImage.sprite = tower.towerData.profile;

        if (tower.operatorData != null)
        {
            OperatorMask.SetActive(true);
            OperatorImage.sprite = tower.operatorData.profile;
            total.Append(tower.operatorData.operatorInfoTable.name);
            total.Append(" + ");
        }
        else
        {
            OperatorMask.SetActive(false);
        }
        total.Append(tower.towerData.towerInfoTable.name);
        total.Append(" LV.");
        total.Append(tower.grade);
        totalName.text = total.ToString();

        this.killCount.text = tower.GetKillCount().ToString();

        float damage = tower.GetAttackPower(tower.towerData.towerInfoTable.attack, tower.grade);
        float range = tower.GetRange();
        float criticalChance = tower.towerData.towerInfoTable.criticalRate;
        float criticalCoefficient = tower.towerData.towerInfoTable.criticalCoefficient;
        float accuracy = tower.towerData.towerInfoTable.accuracy;
        if (tower.operatorData != null)
        {
            damage += damage * tower.operatorData.operatorInfoTable.attack * 0.01f;
            criticalChance += criticalChance * tower.operatorData.operatorInfoTable.criticalRate * 0.01f;
            criticalCoefficient += criticalCoefficient * tower.operatorData.operatorInfoTable.criticalMultiplier * 0.01f;
            accuracy += accuracy * tower.operatorData.operatorInfoTable.accuracy * 0.01f;
        }
        attackValue.text = damage.ToString("F1");
        rateValue.text =  tower.GetAttackRate().ToString("F1");
        rangeValue.text =  range.ToString("F1");
        accValue.text =  accuracy.ToString("F1");
        criChanceValue.text = criticalChance.ToString("F1");
        criScaleValue.text =  criticalCoefficient.ToString("F1");
    }
    public void Close()
    {
        content.SetActive(false);
        tower.ShowRange(false);
        Time.timeScale = BattleManager.Instance.BattleUI.SettingCurGameSpeed();
    }
}
