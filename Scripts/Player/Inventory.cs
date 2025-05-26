using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 소유할 인벤토리
/// 인게임에 필요한 모든 타워, 오퍼레이터, 재화를 저장한뒤 소유표시로 구분
/// 플레이어가 소유한 타워, 오퍼레이터, 재화를 저장하여 전달
/// </summary>

[Serializable]
public class Inventory
{
    public Dictionary<int, OwnedTower> towers;
    public Dictionary<int, OwnedOperator> operators;
    public Dictionary<int, Item> items;


    // 인스펙터 표시 확인, 저장용
    [SerializeField] private List<OwnedTower> towerList = new List<OwnedTower>();
    [SerializeField] private List<OwnedOperator> operatorList = new List<OwnedOperator>();
    [SerializeField] private List<Item> itemList;


    private void SaveLoad()
    {
        // gpt가 보여주는 인벤토리 저장불러오기 예시 참고해서 새로만들어야함
        //저장하기 예시
        string json = JsonUtility.ToJson(GameManager.Instance.Player.inventory);
        PlayerPrefs.SetString("playerInventory", json);

        //불러오기 예시
        json = PlayerPrefs.GetString("playerInventory");
        Inventory loaded = JsonUtility.FromJson<Inventory>(json);
        loaded.InitBuildDict(); // 꼭 호출!

    }

    // 인스펙터 저장용에 저장된 List -> Dictionary화
    public void InitBuildDict()
    {
        towers?.Clear();
        operators?.Clear();
        items?.Clear();
        if (towers == null) towers = new Dictionary<int, OwnedTower>();
        if (operators == null) operators = new Dictionary<int, OwnedOperator>();
        if (items == null) items = new Dictionary<int, Item>();

        foreach (OwnedTower tower in towerList)
        {
            if (tower.data != null)
            {
                tower.Initialize();
                towers[tower.data.towerInfoTable.key] = tower;
            }
        }
        foreach (OwnedOperator op in operatorList)
        {
            if (op.data != null)
            {
                op.Initialize();
                operators[op.data.operatorInfoTable.key] = op;
            }
        }
        foreach (var item in itemList)
        {
            if (item.data != null)
                items[item.data.key] = item;
        }
    }

    // 편성창에서 사용할 인벤토리 편성기능
    public void Organize(ScriptableObject data)
    {
        if (data is TowerData towerData)
        {
            towers[towerData.towerInfoTable.key].isEquipped = true;
        }

        else if (data is OperatorData operatorData)
        {
            operators[operatorData.operatorInfoTable.key].isEquipped = true;
        }


    }
    public void UnOrganize(ScriptableObject data)
    {
        if (data is TowerData towerData)
        {
            towers[towerData.towerInfoTable.key].isEquipped = false;
        }

        else if (data is OperatorData operatorData)
        {
            operators[operatorData.operatorInfoTable.key].isEquipped = false;
        }
    }
    public void ClearOrganize()
    {
        // 프리셋 변경이나 새로 편성할때마다 편성 리스트 체크하기 위해 인벤토리 전부 사용중 해제
        foreach (var ownedTower in towers)
        {
            ownedTower.Value.isEquipped = false;
        }

        foreach (var ownedOperator in operators)
        {
            ownedOperator.Value.isEquipped = false;
        }
    }

    public Item GetItem(int key)
    {
        items.TryGetValue(key, out var item);
        return item;
    }

    public void AddItem(int key, int count)
    {
        var item = GetItem(key);
        if (item != null) item.amount += count;
    }

    public void SubtractItem(int key, int count)
    {
        var item = GetItem(key);
        if (item != null) item.amount -= count;
    }
    // 아이템 갯수 반환
    public int GetItemCount(int key)
    {
        int res = 0;
        if (items.ContainsKey(key))
        {
            res += items[key].amount;
        }
        return res;
    }
    // 목록으로 합계 반환
    public int GetItemCount(params int[] itemKeys)
    {
        int res = 0;
        foreach (var key in itemKeys)
        {
            if (items.ContainsKey(key))
            {
                res += items[key].amount;
            }
        }
        return res;
    }
}

[Serializable]
public class OwnedTower
{
    public TowerData data;
    public bool isOwned;
    public bool isEquipped;

    public void Initialize()
    {
        data.Initialize();
    }
}



[Serializable]
public class OwnedOperator
{
    public OperatorData data;
    public bool isOwned;
    public bool isEquipped;

    public int grade;
    public int level;
    public int exp;

    public int equip0;      // 장비 key값?
    public int equip1;
    public int equip2;
    public int equip3;

    public void Initialize()
    {
        data.Initialize();
        // isOwned = false;
        // isEquipped = false;
        grade = 1;
        level = 1;
        exp = 0;
        equip0 = 0;
        equip1 = 0;
        equip2 = 0;
        equip3 = 0;
    }

    public void SetLevelStatus()
    {
        // data.operatorInfoTable += data.operatorInfoTable;
    }
    public void LevelUP(int PlusLevel)
    {
        level += PlusLevel;

        // TODO : 오퍼레이터 레벨업시 스탯반영 SO라서 한번 넣으면 수정된게 저장되므로 불러오기 기능 완료 후
        var bonus = data.RoleStatBonus[data.operatorInfoTable.role];

        data.operatorInfoTable.attack += (bonus.attack * PlusLevel);
        data.operatorInfoTable.attackRate += (bonus.attackRate * PlusLevel);
        data.operatorInfoTable.range += (bonus.range * PlusLevel);
        data.operatorInfoTable.accuracy += (bonus.accuracy * PlusLevel);
        data.operatorInfoTable.criticalRate += (bonus.criticalRate * PlusLevel);
        data.operatorInfoTable.criticalMultiplier += (bonus.criticalMultiplier * PlusLevel);
    }

    // 등급 강화 1성 -> 2성
    public void Upgrade()
    {
        grade++;
        // 스킬 해방
        switch (grade)
        {
            case 2:
                data.activeLv = 1;
                break;
            case 3:
                data.passive0LV = 1;
                break;
            case 4:
                data.passive1LV = 1;
                break;
            case 5:
                data.passive2LV = 1;
                break;
        }
    }

    public void SkillUpgrade(int skillType)
    {
        switch (skillType)
        {
            case 0:
                data.activeLv++;
                break;
            case 1:
                data.passive0LV++;
                break;
            case 2:
                data.passive1LV++;
                break;
            case 3:
                data.passive2LV++;
                break;

        }
    }

    public string GetPassiveSkillDescription(OwnedOperator op, int skillIndex)
    {
        var skillData = op.data.passiveSkillDatas[skillIndex];
        int skillLevel = 0;

        switch (skillIndex)
        {
            case 0:
                skillLevel = op.data.passive0LV - 1;
                break;
            case 1:
                skillLevel = op.data.passive1LV - 1;
                break;
            case 2:
                skillLevel = op.data.passive2LV - 1;
                break;
        }

        return skillData.GetFormattedDescription(skillLevel);
    }
}

[Serializable]
public class Item
{
    public ItemData data;
    public int amount;
}

