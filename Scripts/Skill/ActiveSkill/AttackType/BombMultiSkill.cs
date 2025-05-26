using UnityEngine;
using System.Threading.Tasks;
/// <summary>
/// 스킬 6104 - 정밀폭격
/// 지정된 영역을 중심으로 {r} 타일 내에 있는 모든 적들에게 타워 공격력의 {a}% 피해를 입히는 드론 폭격을 1초 간격으로 {n} 회 투하합니다. (쿨타임 {t} 초, 단발성)
/// </summary>
public class BombMultiSkill : ISkill
{
    private GameObject prefab;
    int count;
    TowerData towerInfo;
    GameObject skillObject;
    SkillData skillData;

    public BombMultiSkill()
    {
        prefab = Resources.Load<GameObject>(Define._SkillRoot + "/6100"); // 미리 불러오기
    }
    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        towerInfo = BattleManager.Instance.GetTowerInfo(operatorKey).towerData;
        skillObject = Object.Instantiate(prefab, pos, Quaternion.identity);
        skillData = data;
        count = data.interation;
        // 오브젝트 세팅
        skillObject.GetComponent<ProjectileSkill>().Set(towerInfo, skillData.range, skillData.valueA / 100f);
        count--;
        if (count > 0)
        {
            _ = ShowAgainAsync();
        }
    }
    async Task ShowAgainAsync()
    {
        await Task.Delay(1000);
        skillObject.GetComponent<ProjectileSkill>().Set(towerInfo, skillData.range, skillData.valueA / 100f);
        count--;
    }
}
