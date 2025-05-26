using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
public class AnalyticsManager : Singleton<AnalyticsManager>
{
    bool isInit = false;
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        isInit = true;
    }
    public void SendStep(int value)
    {
        if (isInit)
        {
            // 한번만 보내도록
            if (PlayerPrefs.GetInt("Step" + value, 0) != 0) return;

            // 커널 전송
            CustomEvent customEvent = new CustomEvent("Funnel"){
            { "funnelStep", value }
            };

            AnalyticsService.Instance.RecordEvent(customEvent);

            // 기록 저장
            PlayerPrefs.SetInt("Step" + value, 1);
        }
    }
    public void SendEvent(string eventName, Dictionary<string, object> content)
    {
        if (isInit)
        {
            // 커스텀 이벤트 전송
            CustomEvent customEvent = new CustomEvent(eventName);

            foreach (var kvp in content)
            {
                customEvent[kvp.Key] = kvp.Value;
            }
            AnalyticsService.Instance.RecordEvent(customEvent);
            //AnalyticsService.Instance.Flush(); // 즉시 전송
        }
    }
}