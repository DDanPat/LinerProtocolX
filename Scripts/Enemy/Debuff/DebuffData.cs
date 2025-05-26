using UnityEngine;
// 디버프 타입
public enum DEBUFF_TYPE
{
    SLOW,
    DEFENSE_DOWN,
    DAMAGE,
    DODGE_DOWN,
}
// 디버프 인터페이스
public interface IDeBuffData
{
    DEBUFF_TYPE DebuffType { get; set; }
    Color DebuffColor { get; set; }
    float Duration { get; set; }
    float Amount { get; set; }
    int ID { get; set; }
    void ApplyDebuff(EnemyState target);
    void RemoveDebuff(EnemyState target);
}