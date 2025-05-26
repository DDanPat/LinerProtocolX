using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class ActiveSkillLevelTable
{
    /// <summary>
    /// 
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public int skillId;

    /// <summary>
    /// 
    /// </summary>
    public int level;

    /// <summary>
    /// t sec
    /// </summary>
    public float cooldown;

    /// <summary>
    /// u sec
    /// </summary>
    public float duration;

    /// <summary>
    /// n times
    /// </summary>
    public int iteration;

    /// <summary>
    /// r tiles
    /// </summary>
    public float range;

    /// <summary>
    /// 
    /// </summary>
    public SkillAttributes effectA;

    /// <summary>
    /// 
    /// </summary>
    public float valueA;

    /// <summary>
    /// 
    /// </summary>
    public SkillAttributes effectB;

    /// <summary>
    /// 
    /// </summary>
    public float valueB;

    /// <summary>
    /// 
    /// </summary>
    public SkillAttributes effectC;

    /// <summary>
    /// 
    /// </summary>
    public float valueC;

}
public class ActiveSkillLevelTableLoader
{
    public List<ActiveSkillLevelTable> ItemsList { get; private set; }
    public Dictionary<int, ActiveSkillLevelTable> ItemsDict { get; private set; }

    public ActiveSkillLevelTableLoader(string path = "JSON/ActiveSkillLevelTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, ActiveSkillLevelTable>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<ActiveSkillLevelTable> Items;
    }

    public ActiveSkillLevelTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public ActiveSkillLevelTable GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
