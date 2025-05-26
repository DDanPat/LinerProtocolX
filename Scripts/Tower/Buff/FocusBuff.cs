// 같은 오퍼레이터가 공격 시 추가 데미지
public class FocusBuff : ITowerBuff
{
    public BUFF_TYPE BUFF_TYPE { get; set; }
    public float Duration { get; set; }
    public float Amount { get; set; }
    public int ID { get; set; }
    public FocusBuff(float duration, float amount, int id = 0)
    {
        BUFF_TYPE = BUFF_TYPE.FOCUS;
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
