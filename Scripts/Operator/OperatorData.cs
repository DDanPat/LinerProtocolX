using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 슬롯으로 표시할 오퍼레이터의 정보
/// </summary>

[CreateAssetMenu(fileName ="Operator", menuName = "Data/Operator")]
public class OperatorData : ScriptableObject
{
    [Header("Info")]
    public Sprite profile;      // 아이콘 표시 이미지
    public Sprite illust;       // 전신 이미지
    public OperatorInfoTable operatorInfoTable;

    public List<OperatorGradeUpgradeData> GradeRequire;     // 승급시 단계별 필요한 재화 목록
    
    public int activeLv;    // 액티브 스킬 레벨 0 - 미소지 1<= 소지 및 레벨
    public int passive0LV;  // 패시브 스킬 레벨 0 - 미소지 1<= 소지 및 레벨
    public int passive1LV;
    public int passive2LV;
    
    public ActiveSkillData activeSkillData;
    public List<PassiveSkillData> passiveSkillDatas;
    
    // json -> OperatorInfoTable생성후 OperatorData에 적용후 apply로 Scriptable OperatorData 내부 적용
    public void ApplyState(OperatorInfoTable infoTable)
    {
        // itemSlots = Resources.Load<Sprite>(state.iconPath); // or Addressables
    }

    public void Initialize()
    {
        var dict = GameManager.Instance.DataManager.OperatorTableLoader.ItemsDict;

        if (dict.TryGetValue(operatorInfoTable.key, out var dataInfo))
        {
            operatorInfoTable.name = dataInfo.name;
            operatorInfoTable.role = dataInfo.role;
            operatorInfoTable.attack = dataInfo.attack;
            operatorInfoTable.attackRate = dataInfo.attackRate;
            operatorInfoTable.range = dataInfo.range;
            operatorInfoTable.accuracy = dataInfo.accuracy;
            operatorInfoTable.criticalRate = dataInfo.criticalRate;
            operatorInfoTable.criticalMultiplier = dataInfo.criticalMultiplier;
            operatorInfoTable.lineIntro = dataInfo.lineIntro;
            operatorInfoTable.lineStart = dataInfo.lineStart;
            operatorInfoTable.lineSkill = dataInfo.lineSkill;
            operatorInfoTable.lineWin = dataInfo.lineWin;
            operatorInfoTable.lineDefeat = dataInfo.lineDefeat;
            operatorInfoTable.lineLevelUp = dataInfo.lineLevelup;
            operatorInfoTable.operatorDesc = dataInfo.operatorDesc;

            activeLv = 0;
            passive0LV = 0;
            passive1LV = 0;
            passive2LV = 0;
            
            // 액티브 - 고정 / 패시브 - 교체가능해서 초기화
            passiveSkillDatas = new List<PassiveSkillData>();
        }
    }
    
    // 오퍼레이터 역할에 따른 스탯 강화
    public readonly Dictionary<int, StatBonus> RoleStatBonus = new Dictionary<int, StatBonus>
    {
        // 1 공격형, 2 방어형 , 3 지원형
        { 1, new StatBonus(5, 5, 1, 1, 5, 5) },
        { 2, new StatBonus(1, 1, 3, 5, 3, 3) },
        { 3, new StatBonus(3, 3, 5, 3, 1, 1) },
    };
    
    public OperatorData Clone()
    {
        return Instantiate(this);
    }
}

[System.Serializable]
public class RequiredItem
{
    public ItemData item; // 어떤 아이템인지 (아이템 고유 데이터)
    public int count;     // 몇 개 필요한지
}


// 오퍼레이터 역할에 따른 스탯 강화 분류
public struct StatBonus
{
    public int attack;
    public int attackRate;
    public int range;
    public int accuracy;
    public int criticalRate;
    public int criticalMultiplier;

    public StatBonus(int atk, int spd, int rng, int acc, int critRate, int critMul)
    {
        attack = atk;
        attackRate = spd;
        range = rng;
        accuracy = acc;
        criticalRate = critRate;
        criticalMultiplier = critMul;
    }
}