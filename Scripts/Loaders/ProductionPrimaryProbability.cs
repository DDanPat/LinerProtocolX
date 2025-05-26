using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ProductionPrimaryProbability
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public int target;

    /// <summary>
    /// 
    /// </summary>
    public float weight;

    /// <summary>
    /// 
    /// </summary>
    public string desc;

}
public class ProductionPrimaryProbabilityLoader
{
    public List<ProductionPrimaryProbability> ItemsList { get; private set; }
    public Dictionary<int, ProductionPrimaryProbability> ItemsDict { get; private set; }

    public ProductionPrimaryProbabilityLoader(string path = "JSON/ProductionPrimaryProbability")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, ProductionPrimaryProbability>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<ProductionPrimaryProbability> Items;
    }

    public ProductionPrimaryProbability GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public ProductionPrimaryProbability GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
