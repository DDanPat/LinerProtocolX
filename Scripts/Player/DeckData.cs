using System;
using System.Collections.Generic;

/// <summary>
/// 편성시 타워 오퍼레이터의 조합 목록을 저장중
/// 게임 플레이시 GameManager.Instance.Player.deck의 towerList를 Clone하여 사용할 것(휘발성)
/// </summary>

[Serializable]
public class DeckData
{
    public List<TowerOperList> towerList = new List<TowerOperList>();
    public Dictionary<int, TowerOperList> towerOperList;
    public void Init()
    {
        ClearDeck();
        towerList = new List<TowerOperList>();
    }

    public void ClearDeck()
    {
        GameManager.Instance.Player.deck.towerList.Clear();
    }

    public void AddDeck(TowerData towerData, OperatorData operatorData)
    {
        TowerOperList newTowerOperList = new TowerOperList
        {
            towerData = towerData,
            operatorData = operatorData != null ? operatorData : null
        };

        towerList.Add(newTowerOperList);
    }

    public List<TowerOperList> Clone()
    {
        List<TowerOperList> clone = new List<TowerOperList>();

        foreach (var list in towerList)
        {
            TowerOperList copy = new TowerOperList();
            copy.towerData = list.towerData.Clone();        // ScriptableObject라면 Instantiate 사용
            copy.operatorData = list.operatorData?.Clone();  // 마찬가지로 복제
            clone.Add(copy);
        }

        return clone;
    }

}

[Serializable]
public class TowerOperList
{
    public TowerData towerData;
    public OperatorData operatorData;
}
