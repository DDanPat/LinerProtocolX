using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EquipmentTable
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
    public int type;

    /// <summary>
    /// 
    /// </summary>
    public int operatorKey;

    /// <summary>
    /// 
    /// </summary>
    public int grade;

    /// <summary>
    /// 
    /// </summary>
    public int effect0;

    /// <summary>
    /// 
    /// </summary>
    public int effect1;

    /// <summary>
    /// 
    /// </summary>
    public int effect3;

}
public class EquipmentTableLoader
{
    public List<EquipmentTable> ItemsList { get; private set; }
    public Dictionary<int, EquipmentTable> ItemsDict { get; private set; }

    public EquipmentTableLoader(string path = "JSON/EquipmentTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, EquipmentTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<EquipmentTable> Items;
    }

    public EquipmentTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public EquipmentTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
