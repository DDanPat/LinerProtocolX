using UnityEngine;
/// <summary>
/// 스킬 6100 - 폭탄 투하
/// 지정된 위치로부터 {r} 타일 내에 있는 모든 적들에게 타워 공격력의 {a}% 피해를 입히는 폭탄을 {n} 회 투하합니다. (쿨타임 {t} 초, 단발성)
/// </summary>

public class BombSkill : ISkill
{
    private GameObject prefab;
    public BombSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6100"); // 미리 불러오기
    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        TowerData towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        GameObject skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        skillObject.GetComponent<ProjectileSkill>().Set(towerInfo, data.range, data.valueA / 100f, data.interation);
    }

}
