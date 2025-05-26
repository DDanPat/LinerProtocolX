using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerHandler : MonoBehaviour
{
    // 일일 및 주간 리셋 시 호출할 콜백 이벤트
    public static event Action OnDailyReset;
    public static event Action OnWeeklyReset;

    // 다음 리셋 시각
    private DateTime nextDailyResetTime;
    private DateTime nextWeeklyResetTime;

    public void Init()
    {
        // 저장 된 시간 불러 오기
        nextDailyResetTime = GameManager.Instance.Player.nextDailyResetTime;
        nextWeeklyResetTime = GameManager.Instance.Player.nextWeeklyResetTime;

        //ScheduleNextDailyReset();
        //ScheduleNextWeeklyReset();
        StartCoroutine(CountdownCoroutine());
    }

    #region 스케줄 계산

    private void ScheduleNextDailyReset()
    {
        DateTime now = DateTime.Now;
        var todayNine = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
        nextDailyResetTime = now < todayNine ? todayNine : todayNine.AddDays(1);
        // 상점 변경 저장
        GameManager.Instance.SavePlayer();
    }

    private void ScheduleNextWeeklyReset()
    {
        DateTime now = DateTime.Now;
        // 오늘 오전 9시 기준으로 주간 리셋 기준일 계산
        // DayOfWeek.Monday == 1
        int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        var targetDay = now.Date.AddDays(daysUntilMonday).AddHours(9);

        // 만약 오늘이 월요일이고 아직 오전 9시 이전이라면, 그대로 이번 주 월요일 9시
        // 그 외에는 다음 주 월요일 9시
        nextWeeklyResetTime = now < targetDay ? targetDay : targetDay.AddDays(7);
    }

    #endregion

    private IEnumerator CountdownCoroutine()
    {
        while (true)
        {
            DateTime now = DateTime.Now;

            // 일일 리셋 체크
            if (now >= nextDailyResetTime)
            {
                OnDailyReset?.Invoke();
                ScheduleNextDailyReset();
                UIManager.Instance.ShouldShopDaily = true;
                UIManager.Instance.ShouldDailyLogin = true;
            }

            // 주간 리셋 체크
            if (now >= nextWeeklyResetTime)
            {
                OnWeeklyReset?.Invoke();
                ScheduleNextWeeklyReset();
            }

            // 남은 시간 계산
            TimeSpan remainDaily = nextDailyResetTime - now;
            TimeSpan remainWeekly = nextWeeklyResetTime - now;
            TimeUI(remainDaily, remainWeekly);

            yield return new WaitForSeconds(1f);
        }
    }

    private void TimeUI(TimeSpan remainDaily, TimeSpan remainWeekly)
    {
        // 일일: 항상 HH:MM:SS
        string dailyStr = string.Format("{0:D2}시{1:D2}분{2:D2}초",
            (int)remainDaily.TotalHours, remainDaily.Minutes, remainDaily.Seconds);

        // 주간: D일 HH:MM (D>0) or HH:MM:SS (D==0)
        string weeklyStr;
        if (remainWeekly.Days > 0)
        {
            weeklyStr = string.Format("{0}일 {1:D2}시{2:D2}분",
                remainWeekly.Days, remainWeekly.Hours, remainWeekly.Minutes);
        }
        else
        {
            weeklyStr = string.Format("{0:D2}시{1:D2}분{2:D2}초",
                (int)remainWeekly.TotalHours, remainWeekly.Minutes, remainWeekly.Seconds);
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.questUIHandler.SetUITimer(dailyStr, weeklyStr);
            UIManager.Instance.shopUIHandler.SetShopDailyTimer(dailyStr);
        }
    }
    public DateTime GetNextDailyResetTime()
    {
        return nextDailyResetTime;
    }
    public DateTime GetNextWeeklyResetTime()
    {
        return nextWeeklyResetTime;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnDailyReset?.Invoke();
            OnWeeklyReset?.Invoke();
            ScheduleNextDailyReset();
            ScheduleNextWeeklyReset();
        }
#endif
    }
}
