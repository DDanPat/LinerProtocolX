// 사거리 증가
public class RangeUpBuff :  ITowerBuff
{
    public float Duration { get; set; }
    public float Amount { get; set; }
    public BUFF_TYPE BUFF_TYPE { get; set; }

    public int ID { get; set; }
    public RangeUpBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.RANGE_UP;
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