using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseDebuff : IDeBuffData
{
    public DEBUFF_TYPE DebuffType { get; set; }
    public Color DebuffColor { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    private float originDefece;
    public int ID { get; set; }

    public DefenseDebuff(float duration, float amount, int id = 0)
    {
        DebuffType = DEBUFF_TYPE.DEFENSE_DOWN;
        Duration = duration;
        Amount = amount;
        DebuffColor = new Color(153 / 255.0f, 0, 0);
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
