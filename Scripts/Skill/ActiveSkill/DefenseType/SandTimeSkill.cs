using UnityEngine;
/// <summary>
/// 스킬 6207 유사시간
/// 일정 시간 동안 유지되는 {r} 타일 크기의 퀵샌드 영역을 지정된 위치에 형성한다. 
/// 영역 안에 위치한 모든 적 유닛의 이동 속도를 {a}% 감소시킨다.
/// </summary>
public class SandTimeSkill : ISkill
{
    private GameObject prefab;
    public SandTimeSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6207"); // 미리 불러오기
    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        // 디버프 생성
        SlowDebuff debuff = new SlowDebuff(999f, data.valueA);
        GameObject skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        skillObject.GetComponent<DebuffArea>().Set(data.range, data.duration, debuff);
    }
}
