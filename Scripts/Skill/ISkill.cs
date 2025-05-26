using UnityEngine;
// 액티브 스킬 인터페이스
public interface ISkill
{
    public void Execute(int operatorKey, Vector3 pos, SkillData data);
}
// 스킬 발동시 참조하는 구조체
public struct SkillData
{
    public float cooldown;
    public float duration;
    public int interation;
    public float range;
    public float valueA;
    public float valueB;
    public float valueC;

    public SkillData(float cooldown = 0, float duration = 0, int interation = 0, float range = 0, float valueA = 0, float valueB = 0, float valueC = 0, float valueD = 0)
    {
        this.cooldown = cooldown;
        this.duration = duration;
        this.interation = interation;
        this.range = range;
        this.valueA = valueA;
        this.valueB = valueB;
        this.valueC = valueC;
    }
}