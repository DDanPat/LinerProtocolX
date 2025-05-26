using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public StageInfoTable StageInfoTable;
    public bool IsClear; // 클리어 여부
    public int StageClearCount; // 클리어 횟수
    public int StagePlayCount; //  플레이 횟수
}

public class StageManager : Singleton<StageManager>
{
    public List<StageData> stageDataList = new List<StageData>();
    public StageHandler stageHandler;

    public void SetStageData()
    {
        stageDataList.Clear();

        foreach (var data in GameManager.Instance.DataManager.StageDataLoader.StageList)
        {
            stageDataList.Add(new StageData { 
                StageInfoTable = data,
                IsClear = false,
                StageClearCount = 0,
                StagePlayCount = 0,
            });
        }
    }
    public void SetHandler(StageHandler stageHandler)
    {
        this.stageHandler = stageHandler;
    }
}
