using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class StageThemeTable
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
    public string floor;

    /// <summary>
    /// 
    /// </summary>
    public List<string> border;

    /// <summary>
    /// 
    /// </summary>
    public List<string> obstacle;

    /// <summary>
    /// 
    /// </summary>
    public string playerBase;

}
public class StageThemeTableLoader
{
    public List<StageThemeTable> ItemsList { get; private set; }
    public Dictionary<int, StageThemeTable> ItemsDict { get; private set; }

    public StageThemeTableLoader(string path = "JSON/StageThemeTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, StageThemeTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<StageThemeTable> Items;
    }

    public StageThemeTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public StageThemeTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
