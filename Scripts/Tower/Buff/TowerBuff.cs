public enum BUFF_TYPE
{
    ATK_UP,
    ATK_RATE_UP,
    ACCURACY_UP,
    CRI_CHANCE_UP,
    CRI_SCALE_UP,
    RANGE_UP,
    IGNORE_DEFENCE,
    FOCUS,
    TYPE_CHAGE,
    ROOT_SLOW,
    ROOT_DEFENSE_DOWN,
}
public interface ITowerBuff
{
    BUFF_TYPE BUFF_TYPE { get; set; }
    float Duration { get; set; }
    float Amount { get; set; }
    int ID { get; set; }
    void ApplyDebuff(Tower target);
    void RemoveDebuff(Tower target);
}
