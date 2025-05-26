using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class StageInfoTable
{
    /// <summary>
    /// idx
    /// </summary>
    public int key;

    /// <summary>
    /// 
    /// </summary>
    public int design;

    /// <summary>
    /// 
    /// </summary>
    public List<int> enemy;

    /// <summary>
    /// 
    /// </summary>
    public int wave;

    /// <summary>
    /// 
    /// </summary>
    public int dropTable;

    /// <summary>
    /// 
    /// </summary>
    public List<int> spcialItems;

    /// <summary>
    /// 
    /// </summary>
    public int expItem;

    /// <summary>
    /// 
    /// </summary>
    public int theme;

    /// <summary>
    /// 
    /// </summary>
    public int width;

    /// <summary>
    /// 
    /// </summary>
    public int length;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> field;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> borderLeft;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> borderRight;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> borderBottom;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> unplaceableBottom;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> unplaceableTop;

    /// <summary>
    /// x1,y1,x2,y2
    /// </summary>
    public List<int> spawn;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> playerBase;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle0;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle1;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle2;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle3;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle4;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle5;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle6;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle7;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle8;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle9;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle10;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle11;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle12;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle13;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle14;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle15;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle16;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle17;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle18;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle19;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle20;

    /// <summary>
    /// x,y
    /// </summary>
    public List<int> obstacle21;

}
public class StageInfoTableLoader
{
    public List<StageInfoTable> StageList { get; private set; }
    public Dictionary<int, StageInfoTable> ItemsDict { get; private set; }

    public StageInfoTableLoader(string path = "JSON/StageInfoTable")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        StageList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, StageInfoTable>();
        foreach (var item in StageList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<StageInfoTable> Items;
    }

    public StageInfoTable GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public StageInfoTable GetByIndex(int index)
    {
        if (index >= 0 && index < StageList.Count)
        {
            return StageList[index];
        }
        return null;
    }
}
