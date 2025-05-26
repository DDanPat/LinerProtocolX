using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CardTable
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
    public string desc;

    /// <summary>
    /// 
    /// </summary>
    public int type;

    /// <summary>
    /// 
    /// </summary>
    public int towerAttack;

    /// <summary>
    /// 
    /// </summary>
    public int stat;

    /// <summary>
    /// 
    /// </summary>
    public float effect;

    /// <summary>
    /// 
    /// </summary>
    public int max;

}
public class CardTableLoader
{
    public List<CardTable> ItemsList { get; private set; }
    public Dictionary<int, CardTable> ItemsDict { get; private set; }

    public CardTableLoader(string path = "JSON/CardTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CardTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CardTable> Items;
    }

    public CardTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CardTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
