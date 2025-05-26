// 공격 속도 버프
public class AttackRateBuff : ITowerBuff
{
    public float Duration { get; set; }
    public float Amount { get; set; }
    public BUFF_TYPE BUFF_TYPE { get; set; }
    public int ID { get; set; }
    public AttackRateBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.ATK_RATE_UP;
        Duration = duration;
        Amount = amount;
        ID = id;
    }

    public void ApplyDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, Amount / 100f);
    }

    public void RemoveDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, -Amount / 100f);
    }
}
