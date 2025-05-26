using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class DropTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public string desc;

    /// <summary>
    /// 
    /// </summary>
    public List<int> specialItems;

    /// <summary>
    /// 
    /// </summary>
    public float weightSum;

}
public class DropTableLoader
{
    public List<DropTable> ItemsList { get; private set; }
    public Dictionary<int, DropTable> ItemsDict { get; private set; }

    public DropTableLoader(string path = "JSON/DropTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, DropTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<DropTable> Items;
    }

    public DropTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public DropTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
