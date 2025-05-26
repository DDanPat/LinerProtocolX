using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 슬롯으로 표시할 타워 정보
/// </summary>


[CreateAssetMenu(fileName ="Tower", menuName = "Data/Tower")]
public class TowerData : ScriptableObject
{
    [Header("Info")]
    public Sprite profile;      // 아이콘 표시 이미지
    public TowerTable towerInfoTable;
    // json -> TowerInfoTable생성후 TowerData에 적용후 apply로 Scriptable TowerData 내부 적용
    public void ApplyState(TowerTable state)
    {
        // itemSlots = Resources.Load<Sprite>(state.iconPath); // or Addressables
    }
    
    public void Initialize()
    {
        var dict = GameManager.Instance.DataManager.TowerTableLoader.ItemsDict;

        if (dict.TryGetValue(towerInfoTable.key, out var dataInfo))
        {
            towerInfoTable.key = dataInfo.key;
            towerInfoTable.name = dataInfo.name;
            towerInfoTable.desc = dataInfo.desc;
            towerInfoTable.bulletType = dataInfo.bulletType;
            towerInfoTable.attackType = dataInfo.attackType;
            towerInfoTable.range = dataInfo.range;
            towerInfoTable.attack = dataInfo.attack;
            towerInfoTable.antiAir = dataInfo.antiAir;
            towerInfoTable.attackRate = dataInfo.attackRate;
            towerInfoTable.accuracy = dataInfo.accuracy;
            towerInfoTable.criticalRate = dataInfo.criticalRate;
            towerInfoTable.criticalCoefficient = dataInfo.criticalCoefficient;
            towerInfoTable.tiles = dataInfo.tiles;
            towerInfoTable.shape = dataInfo.shape;
            towerInfoTable.sfx = dataInfo.sfx;
        }
    }
    
    public TowerData Clone()
    {
        return Instantiate(this);
    }
}
