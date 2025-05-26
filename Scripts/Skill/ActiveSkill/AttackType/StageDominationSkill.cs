using UnityEngine;
/// <summary>
/// 스킬 6106 - 사운드 블래스트
/// 고출력 음파를 발생시켜, 관리 타워의 사거리 안에 있는 모든 적 유닛에게 타워 공격력 {a}%의 피해를 입힙니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class StageDominationSkill : ISkill
{
    GameObject prefab;
    public StageDominationSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6106"); // 미리 불러오기
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        GameObject skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        float damage = towerInfo.towerInfoTable.attack * data.valueA / 100f;
        // 오브젝트 세팅
        skillObject.GetComponent<EnterDamage>().Set(towerInfo, data.range, data.duration, damage);
    }
}
