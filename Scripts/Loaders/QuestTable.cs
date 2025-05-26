using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class QuestTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public string name;

    /// <summary>
    /// 
    /// </summary>
    public int period;

    /// <summary>
    /// 
    /// </summary>
    public string conditionDesc;

    /// <summary>
    /// 
    /// </summary>
    public QuestConditions firstCondition;

    /// <summary>
    /// 
    /// </summary>
    public QuestConditions secondCondition;

    /// <summary>
    /// 
    /// </summary>
    public int conditionIdx;

    /// <summary>
    /// 
    /// </summary>
    public int conditionCount;

    /// <summary>
    /// 
    /// </summary>
    public string reward;

    /// <summary>
    /// 
    /// </summary>
    public int randomReward;

    /// <summary>
    /// 
    /// </summary>
    public List<int> rewardKey;

    /// <summary>
    /// 
    /// </summary>
    public int rewardNum;

}
public class QuestTableLoader
{
    public List<QuestTable> ItemsList { get; private set; }
    public Dictionary<int, QuestTable> ItemsDict { get; private set; }

    public QuestTableLoader(string path = "JSON/QuestTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, QuestTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<QuestTable> Items;
    }

    public QuestTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public QuestTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
