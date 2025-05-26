using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DailyShopTable
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
    public int itemKey;

    /// <summary>
    /// 
    /// </summary>
    public int count;

    /// <summary>
    /// 0:false, 1:true
    /// </summary>
    public int isFree;

    /// <summary>
    /// $
    /// </summary>
    public float priceCash;

    /// <summary>
    /// 
    /// </summary>
    public int priceCoin;

    /// <summary>
    /// 
    /// </summary>
    public int priceCrystal;

    /// <summary>
    /// %
    /// </summary>
    public int discount;

}
public class DailyShopTableLoader
{
    public List<DailyShopTable> ItemsList { get; private set; }
    public Dictionary<int, DailyShopTable> ItemsDict { get; private set; }

    public DailyShopTableLoader(string path = "JSON/DailyShopTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, DailyShopTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<DailyShopTable> Items;
    }

    public DailyShopTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public DailyShopTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
