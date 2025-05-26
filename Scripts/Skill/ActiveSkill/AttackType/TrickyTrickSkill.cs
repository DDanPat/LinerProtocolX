using UnityEngine;
/// <summary>
/// 스킬 6105 - 트리키 트릭
/// 지정된 위치에 3초 후 폭발하는 함정을 설치합니다. 
// 폭발 시, {r} 타일 내에 있는 모든 적에게 타워 공격력의 {a}% 피해를 주고, 
// 적 유닛의 이동 속도를 {b}% 감소시킵니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class TrickyTrickSkill : ISkill
{
    GameObject prefab;
    public TrickyTrickSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6105"); // 미리 불러오기
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        GameObject skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        // 기본 데미지
        float damage = towerInfo.towerInfoTable.attack * data.valueA / 100f;
        // 오브젝트 세팅
        skillObject.GetComponent<DelayBomb>().Set(towerInfo, data.range, data.duration, damage, data.valueB / 100f, 3f);
    }
}
