using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ProductionCategoryProbability
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public int productionTable;

    /// <summary>
    /// 
    /// </summary>
    public ProductionCategory categoryTable;

    /// <summary>
    /// 
    /// </summary>
    public float weight;

    /// <summary>
    /// 
    /// </summary>
    public string desc;

}
public class ProductionCategoryProbabilityLoader
{
    public List<ProductionCategoryProbability> ItemsList { get; private set; }
    public Dictionary<int, ProductionCategoryProbability> ItemsDict { get; private set; }

    public ProductionCategoryProbabilityLoader(string path = "JSON/ProductionCategoryProbability")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, ProductionCategoryProbability>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<ProductionCategoryProbability> Items;
    }

    public ProductionCategoryProbability GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public ProductionCategoryProbability GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
