using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class TowerTable
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
    public int bulletType;

    /// <summary>
    /// 
    /// </summary>
    public int attackType;

    /// <summary>
    /// 
    /// </summary>
    public float range;

    /// <summary>
    /// 
    /// </summary>
    public float attack;

    /// <summary>
    /// 
    /// </summary>
    public int antiAir;

    /// <summary>
    /// 
    /// </summary>
    public float attackRate;

    /// <summary>
    /// 
    /// </summary>
    public float accuracy;

    /// <summary>
    /// 
    /// </summary>
    public float criticalRate;

    /// <summary>
    /// 
    /// </summary>
    public float criticalCoefficient;

    /// <summary>
    /// 
    /// </summary>
    public int tiles;

    /// <summary>
    /// 
    /// </summary>
    public int shape;

    /// <summary>
    /// 
    /// </summary>
    public float dps;

    /// <summary>
    /// 
    /// </summary>
    public float dpsTile;

    /// <summary>
    /// 
    /// </summary>
    public string vfx;

    /// <summary>
    /// 
    /// </summary>
    public string sfx;

}
public class TowerTableLoader
{
    public List<TowerTable> ItemsList { get; private set; }
    public Dictionary<int, TowerTable> ItemsDict { get; private set; }

    public TowerTableLoader(string path = "JSON/TowerTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, TowerTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<TowerTable> Items;
    }

    public TowerTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public TowerTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
