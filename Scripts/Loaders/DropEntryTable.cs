using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DropEntryTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public int dropTable;

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
public class DropEntryTableLoader
{
    public List<DropEntryTable> ItemsList { get; private set; }
    public Dictionary<int, DropEntryTable> ItemsDict { get; private set; }

    public DropEntryTableLoader(string path = "JSON/DropEntryTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, DropEntryTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<DropEntryTable> Items;
    }

    public DropEntryTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public DropEntryTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
