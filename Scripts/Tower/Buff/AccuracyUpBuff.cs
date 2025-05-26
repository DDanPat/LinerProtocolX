// 명중률 버프
public class AccuracyUpBuff : ITowerBuff
{
    public float Duration { get; set; }
    public float Amount { get; set; }
    public BUFF_TYPE BUFF_TYPE { get; set; }
    public int ID { get; set; }    
    public AccuracyUpBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.ACCURACY_UP;
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