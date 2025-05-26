using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EnemyTable
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
    public int type;

    /// <summary>
    /// 
    /// </summary>
    public float size;

    /// <summary>
    /// 
    /// </summary>
    public float health;

    /// <summary>
    /// 
    /// </summary>
    public float shield;

    /// <summary>
    /// 
    /// </summary>
    public float attack;

    /// <summary>
    /// 
    /// </summary>
    public float defense;

    /// <summary>
    /// 
    /// </summary>
    public float speed;

    /// <summary>
    /// 
    /// </summary>
    public float dodge;

    /// <summary>
    /// 
    /// </summary>
    public float exp;

    /// <summary>
    /// 
    /// </summary>
    public string desc;

}
public class EnemyTableLoader
{
    public List<EnemyTable> ItemsList { get; private set; }
    public Dictionary<int, EnemyTable> EnemyDict { get; private set; }

    public EnemyTableLoader(string path = "JSON/EnemyTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        EnemyDict = new Dictionary<int, EnemyTable>();
        foreach (var item in ItemsList)
        {
            EnemyDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<EnemyTable> Items;
    }

    public EnemyTable GetByKey(int key)
    {
        if (EnemyDict.ContainsKey(key))
        {
            return EnemyDict[key];
        }
        return null;
    }
    public EnemyTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
