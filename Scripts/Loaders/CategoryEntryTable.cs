using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class CategoryEntryTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public ProductionCategory categoryTable;

    /// <summary>
    /// 
    /// </summary>
    public int item;

    /// <summary>
    /// 
    /// </summary>
    public string itemDesc;

    /// <summary>
    /// 
    /// </summary>
    public float weight;

}
public class CategoryEntryTableLoader
{
    public List<CategoryEntryTable> ItemsList { get; private set; }
    public Dictionary<int, CategoryEntryTable> ItemsDict { get; private set; }

    public CategoryEntryTableLoader(string path = "JSON/CategoryEntryTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, CategoryEntryTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<CategoryEntryTable> Items;
    }

    public CategoryEntryTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public CategoryEntryTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
