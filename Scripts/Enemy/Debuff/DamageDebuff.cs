using UnityEngine;
using UnityEngine.Events;

public class DamageDebuff : IDeBuffData
{
    public DEBUFF_TYPE DebuffType { get; set; }
    public Color DebuffColor { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    private float originDefece;
    public int ID { get; set; }
    UnityAction<int> killCallBack;

    public DamageDebuff(float duration, float amount, Color debuffColor, int id = 0, UnityAction<int> killCallBack = null)
    {
        DebuffType = DEBUFF_TYPE.DAMAGE;
        Duration = duration;
        Amount = amount;
        DebuffColor = debuffColor;
        this.killCallBack = killCallBack;
        ID = id;
    }

    public void ApplyDebuff(EnemyState target)
    {
        //target.SetBaseColor(DebuffColor);
        target.SetBuff(DebuffType, Amount, 0);
        target.SetKillCallback(killCallBack);
    }

    public void RemoveDebuff(EnemyState target)
    {
        //target.RestoreColor();
        target.SetBuff(DebuffType, -Amount, 0);
        target.ReSetKillCallback();
    }
}
