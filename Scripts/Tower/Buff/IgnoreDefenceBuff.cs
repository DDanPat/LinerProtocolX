using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 방어력 무시 버프
public class IgnoreDefenceBuff : ITowerBuff
{
    public BUFF_TYPE BUFF_TYPE{ get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    public int ID { get; set; }
    public IgnoreDefenceBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.IGNORE_DEFENCE;
        Duration = duration;
        Amount = amount;
        ID = id;
    }

    public void ApplyDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, -Amount / 100f);
    }

    public void RemoveDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, Amount / 100f);
    }
}
