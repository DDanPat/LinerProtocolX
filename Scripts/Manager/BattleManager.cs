using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public CurrentBattle CurrentBattle;
    public BattleUI BattleUI;

    public int curStage = 0;
    public StageData curStageData;

    public List<TowerOperList> inGameTowerOperList;
    public EnemyRoot enemyRoot; //적 스폰시 참조할 스텟
    private Dictionary<int, Coroutine> debuffCoroutines; // 참조 디버프 딕셔너리
    protected override void Awake()
    {
        base.Awake();
        inGameTowerOperList = GameManager.Instance.Player.deck.Clone(); // deck의 clone
        enemyRoot = new EnemyRoot();
        debuffCoroutines = new();
    }
    public void StageStart(StageData curStage)
    {
        this.curStageData = curStage;

        // 스테이지 시작
        CurrentBattle = FindObjectOfType<CurrentBattle>();
        BattleUI = FindObjectOfType<BattleUI>();
        // 순서 변경 - 불러오기 우선
        inGameTowerOperList = GameManager.Instance.Player.deck.Clone(); // deck의 clone
        CurrentBattle.Initialize();
        BattleUI.Initialize();
        CurrentBattle.BattleStart();
        LoadPassive();
        // 스테이지 전달
        BattleUI.SetStage(curStage.StageInfoTable);
    }
    public void TutorialStart(StageData curStage)
    {
        this.curStageData = curStage;
        // 튜토리얼 시작
        CurrentBattle = FindObjectOfType<CurrentBattle>();
        BattleUI = FindObjectOfType<BattleUI>();
        // 순서 변경 - 불러오기 우선
        inGameTowerOperList = new List<TowerOperList>();
        TowerOperList preset = new TowerOperList();
        preset.towerData = GameManager.Instance.Player.inventory.towers[3201].data.Clone();
        preset.operatorData = GameManager.Instance.Player.inventory.operators[5100].data.Clone();
        inGameTowerOperList.Add(preset);
        CurrentBattle.Initialize();
        BattleUI.Initialize();
        CurrentBattle.BattleStart();
    }

    public void StageEnd()
    {
        CurrentBattle = null;
    }

    // 강화 데이터 탐색
    public TowerOperList GetTowerInfo(int towerKey, int operatorKey)
    {
        var cardList = inGameTowerOperList;
        foreach (var info in cardList)
        {
            if (info.towerData.towerInfoTable.key != towerKey)
                continue;

            if (info.operatorData == null || info.operatorData.operatorInfoTable.key == operatorKey)
                return info;
        }
        // 찾지 못함
        return null;
    }
    public TowerOperList GetTowerInfo(int operatorKey)
    {
        var cardList = inGameTowerOperList;
        foreach (var info in cardList)
        {
            if (info.operatorData == null)
                continue;

            if (info.operatorData.operatorInfoTable.key == operatorKey)
                return info;
        }
        // 찾지 못함
        return null;
    }

    // 무작위 순번의 조합 반환
    public TowerOperList GetRandomInfo()
    {
        List<int> towers = new List<int>();
        for (int i = 0; i < inGameTowerOperList.Count; i++)
        {
            if (inGameTowerOperList[i].towerData.towerInfoTable.key >= 3100)
            {
                towers.Add(i);
            }
        }
        return inGameTowerOperList[towers[Random.Range(0, towers.Count)]];
    }
    public EnemyRoot GetEnemyRoot()
    {
        if (enemyRoot == null)
        {
            enemyRoot = new EnemyRoot();
            // 적 골드, 경험치 드랍량 및 확률
            enemyRoot.expScale = GetScale(6500);
            enemyRoot.goldChance = GetValue(6502);
            enemyRoot.goldScale = GetScale(6503);
            enemyRoot.itemChance = GetValue(6504);
        }
        return enemyRoot;
    }
    // 타워 키 목록 반환
    public List<int> GetTowerList()
    {
        List<int> res = new();
        foreach (var info in inGameTowerOperList)
        {
            if (info.towerData == null)
                res.Add(0);
            else
                res.Add(info.towerData.towerInfoTable.key);
        }
        // 6개 보장
        int remain = 6 - res.Count;
        for (int i = 0; i < remain; i++)
        {
            res.Add(0);
        }
        return res;
    }
    // 타워 갯수 반환
    public int GetTowerCount()
    {
        int res = 0;
        foreach (var info in inGameTowerOperList)
        {
            if (info.towerData == null)
                continue;
            res++;
        }
        return res;
    }
    // 오퍼레이터 키 목록 반환
    public List<int> GetOperatorList()
    {
        List<int> res = new();
        foreach (var info in inGameTowerOperList)
        {
            if (info.operatorData == null)
                res.Add(0);
            else
                res.Add(info.operatorData.operatorInfoTable.key);
        }
        // 6개 보장
        int remain = 6 - res.Count;
        for (int i = 0; i < remain; i++)
        {
            res.Add(0);
        }
        return res;
    }
    // 오퍼레이터 갯수 반환
    public int GetOperatorCount()
    {
        int res = 0;
        foreach (var info in inGameTowerOperList)
        {
            if (info.operatorData == null)
                continue;
            res++;
        }
        return res;
    }

    // EnemyRoot 디버프 함수
    // 속도 감소 상태로 소환
    public void LootSlow(int skillKey, float amount, float duration)
    {
        if (debuffCoroutines.TryGetValue(skillKey, out var defenseDebuff))
        {
            StopCoroutine(defenseDebuff);
            enemyRoot.speedScale = 1f;
        }

        StartCoroutine(LootSlow(amount, duration));
    }
    // 방어 감소 상태로 소환
    public void LootDefenseDown(int skillKey, float amount, float duration)
    {
        if (debuffCoroutines.TryGetValue(skillKey, out var defenseDebuff))
        {
            StopCoroutine(defenseDebuff);
            enemyRoot.speedScale = 1f;
        }
        StartCoroutine(LootDefenceDown(amount, duration));
    }
    private IEnumerator LootSlow(float amount, float duration)
    {
        enemyRoot.speedScale = amount;
        float elapsedTime = duration;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }
        enemyRoot.speedScale = 1f;
    }
    private IEnumerator LootDefenceDown(float amount, float duration)
    {
        enemyRoot.defenseScale = amount;
        float elapsedTime = duration;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }
        enemyRoot.defenseScale = 1f;
    }

    // 패시브 불러오기
    private void LoadPassive()
    {
        // 오퍼레이터별  스텟 증가 패시브 스킬 적용
        foreach (var oper in inGameTowerOperList)
        {
            var data = oper.operatorData;
            float attackScale = 1f;
            float attackRateScale = 1f;
            float rangeScale = 1f;
            float accuracyScale = 1f;
            float criticalRateScale = 1f;
            float criticalMultiplierScale = 1f;
            if (data != null)
            {
                int[] levels = { data.passive0LV, data.passive1LV, data.passive2LV };
                for (int i = 0; i < data.passiveSkillDatas.Count; i++)
                {
                    // 키로 스텟 구별, 증가량 합산
                    var skill = data.passiveSkillDatas[i];
                    if (skill != null)
                    {
                        int level = levels[i];
                        if (level != 0)
                        {
                            switch (data.passiveSkillDatas[i].skillTable.key)
                            {
                                case 6600:
                                    attackScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                                case 6601:
                                    attackRateScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                                case 6602:
                                    rangeScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                                case 6603:
                                    accuracyScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                                case 6604:
                                    criticalRateScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                                case 6605:
                                    criticalMultiplierScale += skill.skillTable.effectX[level - 1] / 100f;
                                    break;
                            }
                        }
                    }
                }
                //  소수점 반올림으로 증가
                data.operatorInfoTable.attack = Mathf.RoundToInt(data.operatorInfoTable.attack * attackScale);
                data.operatorInfoTable.attackRate = Mathf.RoundToInt(data.operatorInfoTable.attackRate * attackRateScale);
                data.operatorInfoTable.range = Mathf.RoundToInt(data.operatorInfoTable.range * rangeScale);
                data.operatorInfoTable.accuracy = Mathf.RoundToInt(data.operatorInfoTable.accuracy * accuracyScale);
                data.operatorInfoTable.criticalRate = Mathf.RoundToInt(data.operatorInfoTable.criticalRate * criticalRateScale);
                data.operatorInfoTable.criticalMultiplier = Mathf.RoundToInt(data.operatorInfoTable.criticalMultiplier * criticalMultiplierScale);
            }
        }
    }
    #region passiveSkillData
    // 오퍼레이터 데이터로 총합 배율(effectX) 반환 1.0 ~
    public float GetScale(int passiveKey)
    {
        float scale = 1f;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                // 오퍼레이터별 패시브 순회
                var passiveSkills = data.operatorData.passiveSkillDatas;
                for (int i = 0; i < passiveSkills.Count; i++)
                {
                    var skill = passiveSkills[i];
                    int level = Mathf.Max(1, levels[i]);
                    if (skill != null)
                    {
                        // 키랑 일치하면 배율 가져옴 20 -> +.20
                        if (skill.skillTable.key == passiveKey)
                        {
                            scale += skill.skillTable.effectX[level - 1] / 100f;
                        }
                    }
                }
            }
        }
        return scale;
    }
    // 오퍼레이터 데이터로 effectX 값 반환
    public int GetValue(int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                var passiveSkills = data.operatorData.passiveSkillDatas;
                int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                for (int i = 0; i < passiveSkills.Count; i++)
                {
                    var skill = passiveSkills[i];
                    int level = Mathf.Max(1, levels[i]);
                    if (skill != null)
                    {
                        // 키랑 일치하면 값
                        if (skill.skillTable.key == passiveKey)
                        {
                            sum += (int)skill.skillTable.effectX[level - 1];
                        }
                    }
                }
            }
        }
        return sum;
    }
    // 오퍼레이터 데이터 키로 구분하고 effectX 값 반환
    public int GetValue(int operatorKey, int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있고 키와 일치 한다면
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                if (data.operatorData.operatorInfoTable.key == operatorKey)
                {
                    var passiveSkills = data.operatorData.passiveSkillDatas;
                    int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                    for (int i = 0; i < passiveSkills.Count; i++)
                    {
                        var skill = passiveSkills[i];
                        int level = Mathf.Max(1, levels[i]);
                        if (skill != null)
                        {
                            // 키랑 일치하면 값
                            if (skill.skillTable.key == passiveKey)
                            {
                                sum += (int)skill.skillTable.effectX[level - 1];
                            }
                        }
                    }
                }
            }
        }
        return sum;
    }

    // 소지 오퍼레이터에서 처치 시 발생하는 패시브 키 리스트 반환
    public List<int> GetKillPassives(int operatorKey)
    {
        HashSet<int> keys = new();
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                if (data.operatorData.operatorInfoTable.key == operatorKey)
                {
                    // 오퍼레이터별 패시브 순회
                    var passiveSkills = data.operatorData.passiveSkillDatas;
                    for (int i = 0; i < passiveSkills.Count; i++)
                    {
                        var skill = passiveSkills[i];
                        if (skill != null)
                        {
                            // 타입(1) 처치 시 조건이면 추가
                            if (skill.skillTable.conditional == 1)
                            {
                                keys.Add(skill.skillTable.key);
                            }
                        }
                    }
                }
            }
        }
        return keys.ToList();
    }

    // 오퍼레이터 데이터로 EnemyCondition 값 반환
    public int GetEnemyCondition(int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                var passiveSkills = data.operatorData.passiveSkillDatas;
                int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                for (int i = 0; i < passiveSkills.Count; i++)
                {
                    var skill = passiveSkills[i];
                    int level = Mathf.Max(1, levels[i]);
                    if (skill != null)
                    {
                        // 키랑 일치하면 값
                        if (skill.skillTable.key == passiveKey)
                        {
                            sum += (int)skill.skillTable.enemyCondition[level - 1];
                        }
                    }
                }
            }
        }
        return sum;
    }
    // 오퍼레이터 데이터로 effectN 값 반환
    public int GetEffectN(int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                var passiveSkills = data.operatorData.passiveSkillDatas;
                int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                for (int i = 0; i < passiveSkills.Count; i++)
                {
                    var skill = passiveSkills[i];
                    int level = Mathf.Max(1, levels[i]);
                    if (skill != null)
                    {
                        // 키랑 일치하면 값
                        if (skill.skillTable.key == passiveKey)
                        {
                            sum += (int)skill.skillTable.effectN[level - 1];
                        }
                    }
                }
            }
        }
        return sum;
    }
    public int GetEffectN(int operatorKey, int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                if (data.operatorData.operatorInfoTable.key == operatorKey)
                {
                    // 오퍼레이터별 패시브 순회
                    var passiveSkills = data.operatorData.passiveSkillDatas;
                    int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                    for (int i = 0; i < passiveSkills.Count; i++)
                    {
                        var skill = passiveSkills[i];
                        int level = Mathf.Max(1, levels[i]);
                        // 키랑 일치하면 값
                        if (skill != null)
                        {
                            if (skill.skillTable.key == passiveKey)
                            {
                                sum += (int)skill.skillTable.effectN[level - 1];
                            }
                        }
                    }
                }
            }
        }
        return sum;
    }
    // 오퍼레이터 데이터로 effectN 값 반환
    public int GetDuration(int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                var passiveSkills = data.operatorData.passiveSkillDatas;
                int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                for (int i = 0; i < passiveSkills.Count; i++)
                {
                    var skill = passiveSkills[i];
                    int level = Mathf.Max(1, levels[i]);
                    if (skill != null)
                    {
                        // 키랑 일치하면 값
                        if (skill.skillTable.key == passiveKey)
                        {
                            sum += (int)skill.skillTable.duration[level - 1];
                        }
                    }
                }
            }
        }
        return sum;
    }
    // 오퍼레이터 데이터로 effectN 값 반환
    public int GetDuration(int operatorKey, int passiveKey)
    {
        int sum = 0;
        foreach (var data in inGameTowerOperList)
        {
            // 오퍼레이터 데이터가 있는 경우에만
            if (data.operatorData != null && data.operatorData.passiveSkillDatas != null)
            {
                // 오퍼레이터별 패시브 순회
                if (data.operatorData.operatorInfoTable.key == operatorKey)
                {
                    var passiveSkills = data.operatorData.passiveSkillDatas;
                    int[] levels = { data.operatorData.passive0LV, data.operatorData.passive1LV, data.operatorData.passive2LV };
                    for (int i = 0; i < passiveSkills.Count; i++)
                    {
                        var skill = passiveSkills[i];
                        int level = Mathf.Max(1, levels[i]);
                        if (skill != null)
                        {
                            // 키랑 일치하면 값
                            if (skill.skillTable.key == passiveKey)
                            {
                                sum += (int)skill.skillTable.duration[level - 1];
                            }
                        }
                    }
                }
            }
        }
        return sum;
    }
    #endregion

    public QuestConditions IsSquadState()
    {
        if (curStageData.StageInfoTable.key == 1000) return QuestConditions.none;

        int operCount = 0;
        int towerCount = inGameTowerOperList.Count;

        foreach (var data in inGameTowerOperList)
        {
            if (data.operatorData != null)
                operCount++;
        }

        if (towerCount == 6 && operCount == 6)
            return QuestConditions.fullSquad;

        if (towerCount == 6)
            return QuestConditions.halfSquad;

        if (operCount > 0)
            return QuestConditions.operatorOnDeck;

        return QuestConditions.none;
    }
}
