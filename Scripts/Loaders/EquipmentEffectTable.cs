using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class EquipmentEffectTable
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
    public SkillAttributes stat;

    /// <summary>
    /// %
    /// </summary>
    public float amount0;

    /// <summary>
    /// %
    /// </summary>
    public float amount1;

}
public class EquipmentEffectTableLoader
{
    public List<EquipmentEffectTable> ItemsList { get; private set; }
    public Dictionary<int, EquipmentEffectTable> ItemsDict { get; private set; }

    public EquipmentEffectTableLoader(string path = "JSON/EquipmentEffectTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, EquipmentEffectTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<EquipmentEffectTable> Items;
    }

    public EquipmentEffectTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public EquipmentEffectTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
