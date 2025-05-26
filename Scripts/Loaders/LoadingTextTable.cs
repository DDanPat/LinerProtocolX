using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LoadingTextTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public string text;

}
public class LoadingTextTableLoader
{
    public List<LoadingTextTable> ItemsList { get; private set; }
    public Dictionary<int, LoadingTextTable> ItemsDict { get; private set; }

    public LoadingTextTableLoader(string path = "JSON/LoadingTextTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, LoadingTextTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<LoadingTextTable> Items;
    }

    public LoadingTextTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public LoadingTextTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
