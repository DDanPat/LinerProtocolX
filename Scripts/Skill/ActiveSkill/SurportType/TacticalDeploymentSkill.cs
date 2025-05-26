using UnityEngine;
/// <summary>
/// 스킬 6306- 전술 배치
/// 즉시 배치할 수 있는 랜덤한 타워 카드 {n} 장을 획득합니다.
/// /// </summary>
public class TacticalDeployment : ISkill
{
    public TacticalDeployment()
    {

    }

    public void Execute(int operatorKey, Vector3 pos, SkillData data)
    {
        int bonus = data.interation;
        for (int i = 0; i < bonus; i++)
        {
            // 무작위 설치 카드 획득
            BattleManager.Instance.CurrentBattle.GetRandomTower();
        }

    }
}
