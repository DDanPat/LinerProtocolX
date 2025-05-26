using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PassiveSkillTable
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
    public int conditional;

    /// <summary>
    /// e value
    /// </summary>
    public List<float> enemyCondition;

    /// <summary>
    /// t value
    /// </summary>
    public List<float> duration;

    /// <summary>
    /// x value
    /// </summary>
    public List<float> effectX;

    /// <summary>
    /// n value
    /// </summary>
    public List<float> effectN;

    /// <summary>
    /// 
    /// </summary>
    public int profile;

}
public class PassiveSkillTableLoader
{
    public List<PassiveSkillTable> ItemsList { get; private set; }
    public Dictionary<int, PassiveSkillTable> ItemsDict { get; private set; }

    public PassiveSkillTableLoader(string path = "JSON/PassiveSkillTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, PassiveSkillTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<PassiveSkillTable> Items;
    }

    public PassiveSkillTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public PassiveSkillTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
