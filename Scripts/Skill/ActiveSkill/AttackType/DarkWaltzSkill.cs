using UnityEngine;
/// <summary>
/// 스킬 6109 - 암흑무도
/// 지정된 위치에 일정 시간 동안 지속되는 {r} 타일 크기의 안개를 만든다. 
//  안개에 들어온 모든 적 유닛은 초 당 타워 공격력의 {a}% 피해를 입는다. (쿨타임 {t} 초, 지속시간 {u} 초)
/// </summary>
public class DarkWaltzSkill : ISkill
{
    GameObject prefab;
    public DarkWaltzSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6109"); // 미리 불러오기
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        GameObject skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        // 기본 데미지
        float damage = towerInfo.towerInfoTable.attack * data.valueA / 100f;
        // 오브젝트 세팅
        skillObject.GetComponent<ContainDamage>().Set(towerInfo, data.range, data.duration, damage);
    }
}
