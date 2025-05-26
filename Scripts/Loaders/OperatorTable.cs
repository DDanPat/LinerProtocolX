using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class OperatorTable
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
    public string profile;

    /// <summary>
    /// 
    /// </summary>
    public string illust;

    /// <summary>
    /// 
    /// </summary>
    public string circle;

    /// <summary>
    /// 
    /// </summary>
    public int role;

    /// <summary>
    /// 
    /// </summary>
    public int attack;

    /// <summary>
    /// 
    /// </summary>
    public int attackRate;

    /// <summary>
    /// 
    /// </summary>
    public int range;

    /// <summary>
    /// 
    /// </summary>
    public int accuracy;

    /// <summary>
    /// 
    /// </summary>
    public int criticalRate;

    /// <summary>
    /// 
    /// </summary>
    public int criticalMultiplier;

    /// <summary>
    /// 
    /// </summary>
    public int skill;

    /// <summary>
    /// 
    /// </summary>
    public int dedicated;

    /// <summary>
    /// 
    /// </summary>
    public string lineIntro;

    /// <summary>
    /// 
    /// </summary>
    public string lineStart;

    /// <summary>
    /// 
    /// </summary>
    public string lineSkill;

    /// <summary>
    /// 
    /// </summary>
    public string lineWin;

    /// <summary>
    /// 
    /// </summary>
    public string lineDefeat;

    /// <summary>
    /// 
    /// </summary>
    public string lineLevelup;

    /// <summary>
    /// 
    /// </summary>
    public string operatorDesc;

}
public class OperatorTableLoader
{
    public List<OperatorTable> ItemsList { get; private set; }
    public Dictionary<int, OperatorTable> ItemsDict { get; private set; }

    public OperatorTableLoader(string path = "JSON/OperatorTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, OperatorTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<OperatorTable> Items;
    }

    public OperatorTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public OperatorTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
