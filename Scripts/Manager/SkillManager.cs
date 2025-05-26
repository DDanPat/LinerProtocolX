using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
public class SkillManager : Singleton<SkillManager>
{
    public LayerMask enemyLayer;
    public LayerMask towerLayer;
    public Dictionary<int, ActiveSkillTable> skillDict;
    public Dictionary<int, ActiveSkillLevelTable> skillLevelDict;
    public Dictionary<int, PassiveSkillTable> passiceDict;
    Dictionary<int, ISkill> skillHandlers = new Dictionary<int, ISkill>();

    private Dictionary<int, PassiveSkillData> passiveSkillCache = new(); // 패시브 교체시 사용할 전 패시브 목록 보관
    public void Start()
    {
        skillDict = GameManager.Instance.DataManager.ActiveSkillTableLoader.ItemsDict;
        skillLevelDict = GameManager.Instance.DataManager.ActiveSkillLevelTableLoader.ItemsDict;
        passiceDict = GameManager.Instance.DataManager.PassiveSkillTableLoader.ItemsDict;
        InitSkills();
    }

    // 스킬 딕셔너리 초기화 - 스킬 객체 생성
    public void InitSkills()
    {
        skillHandlers[6100] = new BombSkill();
        skillHandlers[6101] = new SpeedAttackSkill();
        skillHandlers[6102] = new BlankShotSkill();
        skillHandlers[6103] = new SoundBlastSkill();
        skillHandlers[6104] = new BombMultiSkill();
        skillHandlers[6105] = new TrickyTrickSkill();
        skillHandlers[6106] = new StageDominationSkill();
        skillHandlers[6107] = new ExcaliburSkill();
        skillHandlers[6108] = new SweetSilenceSkill();
        skillHandlers[6109] = new DarkWaltzSkill();
        skillHandlers[6110] = new BombChargeSkill();
        skillHandlers[6111] = new ReversalRushSkill();
        skillHandlers[6112] = new BurstSparkSkill();
        skillHandlers[6113] = new TacticalShootSkill();

        skillHandlers[6200] = new SlowSkill();
        skillHandlers[6201] = new PositionSlowSkill();
        skillHandlers[6202] = new SeduceAlgorithmSkill();
        skillHandlers[6203] = new DefenseDownStationSkill();
        skillHandlers[6204] = new DynamicShieldSkill();
        skillHandlers[6205] = new PowerOutShelter();
        skillHandlers[6206] = new ShieldBreakSkill();
        skillHandlers[6207] = new SandTimeSkill();

        skillHandlers[6300] = new CoolDownSkill();
        skillHandlers[6301] = new SmileDrinkSkill();
        skillHandlers[6302] = new LightBarrierSkill();
        skillHandlers[6303] = new HisenseConnectionSkill();
        skillHandlers[6304] = new NaturalBreezeSkill();
        skillHandlers[6305] = new ObstacleSkill();
        skillHandlers[6306] = new TacticalDeployment();
        skillHandlers[6307] = new FullSpeedDashSkill();
        skillHandlers[6308] = new SpeedExitSkill();
    }
    public void UseSkill(int skillKey, int operatorKey)
    {
        UseSkill(skillKey, operatorKey, Vector3.zero);
    }
    public void UseSkill(int skillKey, int operatorKey, Vector3 pos)
    {
        // 레벨에 맞는 스킬 정보
        ActiveSkillLevelTable skillInfo = skillLevelDict[skillKey];
        // 원본 스킬 ID
        int skillID = skillInfo.skillId;
        // 스킬
        if (skillHandlers.TryGetValue(skillID, out ISkill skill))
        {
            SkillData data = new SkillData(
                cooldown: skillInfo.cooldown,
                duration: skillInfo.duration,
                interation: skillInfo.iteration,
                range: skillInfo.range,
                valueA: skillInfo.valueA,
                valueB: skillInfo.valueB,
                valueC: skillInfo.valueC
            );
            skill.Execute(operatorKey, pos, data);
        }
        // 커널 - 튜토리얼 스킬 사용
        if (BattleManager.Instance.curStage == 0)
            AnalyticsManager.Instance.SendStep(5);
        // 커스텀 이벤트 - 스킬 사용
        AnalyticsManager.Instance.SendEvent(Define.SkillUsed, new Dictionary<string, object> {
            { Define.stageKey, BattleManager.Instance.curStage },
            { Define.operatorKey, operatorKey},
            });
    }

    // 설명문 반환
    public string GetDescript(int skillKey)
    {
        ActiveSkillLevelTable skill = skillLevelDict[skillKey];
        if (skill != null)
        {
            string description = skillDict[skill.skillId].desc;
            // StringBuilder를 사용하여 성능 최적화
            StringBuilder sb = new StringBuilder(description);
            sb.Replace("{a}", skill.valueA.ToString());
            sb.Replace("{b}", skill.valueB.ToString());
            sb.Replace("{c}", skill.valueC.ToString());
            sb.Replace("{t}", skill.cooldown.ToString());
            sb.Replace("{n}", skill.iteration.ToString());
            sb.Replace("{r}", skill.range.ToString());
            sb.Replace("{u}", skill.duration.ToString());
            sb.Replace("\\n", "\n");
            return sb.ToString();
        }

        // 오류
        return "올바르지 않은 키 값입니다.";
    }

    // 이름과 레벨 반환
    public string GetName(int skillKey)
    {
        if (!skillLevelDict.ContainsKey(skillKey))
        {
            Debug.LogError("SkillKey not found");
            return string.Empty; // 기본 반환값
        }

        ActiveSkillLevelTable skill = skillLevelDict[skillKey];
        int skillID = skill.skillId;

        // skillDict[skillID]를 한 번만 호출하여 변수에 저장
        if (!skillDict.ContainsKey(skillID))
        {
            Debug.LogError("SkillID not found");
            return string.Empty; // 기본 반환값
        }

        var skillData = skillDict[skillID];

        // 설명 생성
        string descript = $"<b>{skillData.name}</b>(LV.{skill.level})";

        return descript;
    }

    public PassiveSkillData GetSkill(int skillId)
    {
        if (passiveSkillCache.TryGetValue(skillId, out var skill))
            return skill;

        skill = Resources.Load<PassiveSkillData>($"Skill/PassiveSkill/{skillId}");
        if (skill != null)
            passiveSkillCache.Add(skillId, skill);
        else
            Debug.LogWarning($"스킬 {skillId} 못 찾음");

        return skill;
    }

}
