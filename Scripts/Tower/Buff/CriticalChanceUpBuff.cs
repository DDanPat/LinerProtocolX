// 크리티컬 확률 버프
public class CriticalChanceUpBuff : ITowerBuff
{
    public float Duration { get; set; }
    public float Amount { get; set; }
    public BUFF_TYPE BUFF_TYPE { get; set; }

    public int ID { get; set; }
    public CriticalChanceUpBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.CRI_CHANCE_UP;
        Duration = duration;
        Amount = amount;
        ID = id;
    }

    public void ApplyDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, Amount, 0);
    }

    public void RemoveDebuff(Tower target)
    {
        target.SetBuff(BUFF_TYPE, -Amount, 0);
    }
}