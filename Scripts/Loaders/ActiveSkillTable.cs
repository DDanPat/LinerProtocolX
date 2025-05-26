using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ActiveSkillTable
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
    public int trigger;

    /// <summary>
    /// 
    /// </summary>
    public string profile;

}
public class ActiveSkillTableLoader
{
    public List<ActiveSkillTable> ItemsList { get; private set; }
    public Dictionary<int, ActiveSkillTable> ItemsDict { get; private set; }

    public ActiveSkillTableLoader(string path = "JSON/ActiveSkillTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, ActiveSkillTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<ActiveSkillTable> Items;
    }

    public ActiveSkillTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public ActiveSkillTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
