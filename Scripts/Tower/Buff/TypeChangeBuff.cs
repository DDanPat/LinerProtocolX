using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChangeBuff : ITowerBuff
{
    public BUFF_TYPE BUFF_TYPE { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    public int ID { get; set; }
    private float range;
    public TypeChangeBuff(float duration, float amount, float range, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.TYPE_CHAGE;
        Duration = duration;
        Amount = amount;
        this.range = range;
        ID = id;
    }

    public void ApplyDebuff(Tower target)
    {
        target.ForceChangeBulletType(range);
    }

    public void RemoveDebuff(Tower target)
    {
        target.RestoreBulletType();
    }
}
