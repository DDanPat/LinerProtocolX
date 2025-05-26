using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 속도 감소 디버프
public class SlowDebuff : IDeBuffData
{
    public DEBUFF_TYPE DebuffType { get; set; }
    public Color DebuffColor { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    public int ID { get; set; }
    public SlowDebuff(float duration, float amount, int id = 0)
    {
        DebuffType = DEBUFF_TYPE.SLOW;
        Duration = duration;
        Amount = amount;
        DebuffColor = new Color(153 / 255.0f, 217 / 255.0f, 234 / 255.0f);
        ID = id;
    }

    public void ApplyDebuff(EnemyState target)
    {
        //target.Animator.speed = Amount;
        target.SetBaseColor(DebuffColor);
        target.SetBuff(DebuffType, -Amount / 100f);
    }

    public void RemoveDebuff(EnemyState target)
    {
        //target.Animator.speed = 1f;
        target.RestoreColor();
        target.SetBuff(DebuffType, Amount / 100f);
    }
}
