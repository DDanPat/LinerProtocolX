using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeDebuff : IDeBuffData
{
    public DEBUFF_TYPE DebuffType { get; set; }
    public Color DebuffColor { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    public int ID { get; set; }
    float originDodge;

    public DodgeDebuff(float duration, float amount, Color debuffColor, int id = 0)
    {
        DebuffType = DEBUFF_TYPE.DODGE_DOWN;
        Duration = duration;
        Amount = amount;
        DebuffColor = debuffColor;
        ID = id;
    }

    public void ApplyDebuff(EnemyState target)
    {
        target.SetBaseColor(DebuffColor);
        originDodge = target.dodge;
        target.AddDodge(-originDodge);
    }

    public void RemoveDebuff(EnemyState target)
    {
        target.RestoreColor();
        target.AddDodge(originDodge);
    }
}
