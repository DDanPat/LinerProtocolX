using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    // 저장/불러오기 경로
    private readonly string _filePath = Application.persistentDataPath + "/player.json";
    // 기본값 경로
    private readonly string _defaultPath = "DefaultPlayer";
    public void SavePlayer(Player player)
    {
        SavedPlayer savedPlayer = new(player);
        string json = JsonUtility.ToJson(savedPlayer, prettyPrint: true);
        File.WriteAllText(_filePath, json);
    }

    public void LoadPlayer(Player player)
    {
        if (File.Exists(_filePath))
        {
            // 파일 불러오기
            string json = File.ReadAllText(_filePath);
            SavedPlayer savedPlayer = JsonUtility.FromJson<SavedPlayer>(json);
            savedPlayer.SetPlayer(player);
        }
        else
        {
            // 기본 파일 생성하기
            string json = Resources.Load<TextAsset>(_defaultPath).text;
            SavedPlayer savedPlayer = JsonUtility.FromJson<SavedPlayer>(json);
            savedPlayer.SetPlayer(player);
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }
    [Serializable]
    private class SavedPlayer
    {
        public List<SavedTower> playerTowerList; // 전체 타워 목록
        public List<SavedOperator> operatorList; // 전체 오퍼레이터 목록
        public List<SavedItem> itemList; // 전체 아이템 목록
        public List<SavedTowerOper> towerOpers = new List<SavedTowerOper>(); // 덱 정보
        public List<SavedQuest> quests; // 쿼스트 정보
        public List<SavedStage> stages; // 스테이지 클리어, 도전 정보
        public List<SavedShopItem> dailyShops; // 일일 상점 목록 정보
        public List<SavedShopItem> resourcesShops; // 유료 상점 목록 정보
        public int playerCoin;
        public int playerCrystal;
        public int clearStage;
        public bool isGachaFrist = true;
        public int resetCount; // 일일 상점 초기화 가능 횟수
        public int favoriteKey; // 선호 오퍼레이터 키
        const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss"; // 날짜 포멧
        public string nextDailyResetTime; // 일일 상점 갱신 시간 문자열
        public string nextWeeklyResetTime; // 주간 상점 갱신 시간 문자열
        string playerId;

        int totalLoginDays; // 총 접속 일수
        int lastStagePlayed; // 마지막으로 성공한 스테이지
        int lastStageCleared; // 마지막 플레이한 스테이지
        bool isAdRemove; // 광고 제거 배너 구매 여부
        bool isTrippleFast;  // 3배속 배너 상품 구매 여부
        bool isGoldBoost; // 골드 부스터 배너 상품 구매 여부
        bool isMonthlyCrystal; // 월간 결정석 배너 상품 구매 여부
        Time timeGoldBoostActivated; // 골드 부스터 상품 구매 시점
        Time timeMonthlyCrystalActivated; // 월간 결정석 상품 구매 시점
        // dailyDailyShopPurchase // 일일 일일 상점 구매 목록


        public SavedPlayer(Player player)
        {
            playerTowerList = new List<SavedTower>();
            operatorList = new List<SavedOperator>();
            itemList = new List<SavedItem>();
            towerOpers = new List<SavedTowerOper>();
            quests = new List<SavedQuest>();
            stages = new List<SavedStage>();
            dailyShops = new List<SavedShopItem>();
            resourcesShops = new List<SavedShopItem>();


            // 현재 데이터를 저장
            foreach (var tower in player.inventory.towers)
            {
                playerTowerList.Add(new SavedTower(tower.Key, tower.Value.isOwned, tower.Value.isEquipped));
            }
            foreach (var oper in player.inventory.operators)
            {
                operatorList.Add(new SavedOperator(oper.Value));
            }
            foreach (var item in player.inventory.items)
            {
                itemList.Add(new SavedItem(item.Key, item.Value.amount));
            }
            foreach (var towerOper in player.deck.towerList)
            {
                var tower = towerOper.towerData.towerInfoTable.key;
                int operatorKey = 0;
                if (towerOper.operatorData != null)
                {
                    operatorKey = towerOper.operatorData.operatorInfoTable.key;
                }
                SavedTowerOper so = new SavedTowerOper(tower, operatorKey);
                towerOpers.Add(so);
            }

            // 스테이지 진행 여부 저장
            foreach (StageData stageData in StageManager.Instance.stageDataList)
            {
                stages.Add(new SavedStage(stageData.StageInfoTable.key, stageData.IsClear, stageData.StageClearCount, stageData.StagePlayCount));
            }
            clearStage = GameManager.Instance.Player.clearStage;

            // 자원 적용
            playerCoin = player.Gold;
            playerCrystal = player.Gem;

            // 퀘스트 진행 저장
            foreach (var dailyQuest in QuestManager.Instance.dailyQuestList)
            {
                quests.Add(new SavedQuest(dailyQuest.key,
                dailyQuest.currentConditionCount, dailyQuest.isClear));
            }
            foreach (var weeklyQuest in QuestManager.Instance.weeklyQuestList)
            {
                quests.Add(new SavedQuest(weeklyQuest.key,
                weeklyQuest.currentConditionCount, weeklyQuest.isClear));
            }
            foreach (var achievementQuest in QuestManager.Instance.achievementQuestList)
            {
                quests.Add(new SavedQuest(achievementQuest.key,
                achievementQuest.currentConditionCount, achievementQuest.isClear));
            }

            // 상점 데이터 저장
            //dailyShopList, resourcesShopList
            foreach (var item in player.dailyShopList)
            {
                dailyShops.Add(new SavedShopItem(item.itemInfo.key, item.isBuyOverlay));
            }
            foreach (var item in player.resourcesShopList)
            {
                resourcesShops.Add(new SavedShopItem(item.itemInfo.key, item.isBuyOverlay));
            }
            isGachaFrist = player.isGachaFrist;
            resetCount = GameManager.Instance.Player.ResetCount;

            // 상점 초기화 시간 저장
            nextDailyResetTime = QuestManager.Instance.GetNextDailyResetTime().ToString(DateTimeFormat);
            nextWeeklyResetTime = QuestManager.Instance.GetNextWeeklyResetTime().ToString(DateTimeFormat);

            // 선호 오퍼레이터 저장
            if (player.favoriteOperator == null)
                favoriteKey = 0;
            else
                favoriteKey = player.favoriteOperator.operatorInfoTable.key;


        }

        // 불러온 데이터를 적용
        public void SetPlayer(Player player)
        {
            // 타워에 적용
            foreach (var tower in playerTowerList)
            {
                if (player.inventory.towers.ContainsKey(tower.key))
                {
                    player.inventory.towers[tower.key].isOwned = tower.isOwned;
                    player.inventory.towers[tower.key].isEquipped = tower.isEquipped;
                }
            }
            // 오퍼레이터에 적용
            foreach (var oper in operatorList)
            {
                if (player.inventory.operators.ContainsKey(oper.key))
                {
                    player.inventory.operators[oper.key].data.activeLv = oper.activeLv;
                    if (oper.passive0 != 0)
                        player.inventory.operators[oper.key].data.passiveSkillDatas.Add(GetSkill(oper.passive0));
                    player.inventory.operators[oper.key].data.passive0LV = oper.passive0LV;
                    if (oper.passive1 != 0)
                        player.inventory.operators[oper.key].data.passiveSkillDatas.Add(GetSkill(oper.passive1));
                    player.inventory.operators[oper.key].data.passive1LV = oper.passive1LV;
                    if (oper.passive2 != 0)
                        player.inventory.operators[oper.key].data.passiveSkillDatas.Add(GetSkill(oper.passive2));
                    player.inventory.operators[oper.key].data.passive2LV = oper.passive2LV;

                    player.inventory.operators[oper.key].isOwned = oper.isOwned;
                    player.inventory.operators[oper.key].isEquipped = oper.isEquipped;
                    player.inventory.operators[oper.key].grade = oper.grade;
                    player.inventory.operators[oper.key].LevelUP(oper.level - 1);
                    player.inventory.operators[oper.key].exp = oper.exp;

                    player.inventory.operators[oper.key].equip0 = oper.equip0;
                    player.inventory.operators[oper.key].equip1 = oper.equip1;
                    player.inventory.operators[oper.key].equip2 = oper.equip2;
                    player.inventory.operators[oper.key].equip3 = oper.equip3;
                    
                    
                }
            }

            // 인벤토리에 적용
            foreach (var item in itemList)
            {

                if (player.inventory.items.ContainsKey(item.key))
                {
                    player.inventory.items[item.key].amount = item.amount;
                }
            }

            // 조합에 적용
            player.deck.towerList.Clear();
            foreach (var towerOper in towerOpers)
            {
                TowerData towerData = player.inventory.towers[towerOper.towerKey].data;
                OperatorData operatorData = null;

                if (towerOper.operatorKey != 0)
                {
                    operatorData = player.inventory.operators[towerOper.operatorKey].data;
                }
                player.deck.towerList.Add(new TowerOperList { towerData = towerData, operatorData = operatorData });
            }
            // 퀘스트 진행, 결과 불러오기
            foreach (var quest in quests)
            {
                bool findTarget = false;
                foreach (var dailyQuest in QuestManager.Instance.dailyQuestList)
                {
                    if (dailyQuest.key == quest.key)
                    {
                        dailyQuest.currentConditionCount = quest.currentConditionCount;
                        dailyQuest.isClear = quest.isClear;
                        findTarget = true;
                        break;
                    }
                }
                if (findTarget) continue;
                foreach (var weeklyQuest in QuestManager.Instance.weeklyQuestList)
                {
                    if (weeklyQuest.key == quest.key)
                    {
                        weeklyQuest.currentConditionCount = quest.currentConditionCount;
                        weeklyQuest.isClear = quest.isClear;
                        findTarget = true;
                        break;
                    }
                }
                if (findTarget) continue;
                foreach (var achievementQuest in QuestManager.Instance.achievementQuestList)
                {
                    if (achievementQuest.key == quest.key)
                    {
                        achievementQuest.currentConditionCount = quest.currentConditionCount;
                        achievementQuest.isClear = quest.isClear;
                        findTarget = true;

                        break;
                    }
                }
            }
            // 스테이지 정보 적용
            for (int i = 0; i < stages.Count; i++)
            {
                StageManager.Instance.stageDataList[i].IsClear = stages[i].isClear;
                StageManager.Instance.stageDataList[i].StageClearCount = stages[i].stageClearCount;
                StageManager.Instance.stageDataList[i].StagePlayCount = stages[i].StagePlayCount;
            }
            player.clearStage = clearStage;
            player.isGachaFrist = isGachaFrist;

            // 자원 값에 적용
            player.Gold = playerCoin;
            player.Gem = playerCrystal;

            // 상점 데이터 적용
            for (int i = 0; i < dailyShops.Count; i++)
            {
                player.dailyShopList.Add(new ShopItemData
                {
                    itemInfo = GameManager.Instance.DataManager.DailyShopTableLoader.GetByKey(dailyShops[i].key),
                    isBuyOverlay = dailyShops[i].isBuyOverlay
                });
                ;
            }
            for (int i = 0; i < resourcesShops.Count; i++)
            {
                player.resourcesShopList.Add(new ShopItemData
                {
                    itemInfo = GameManager.Instance.DataManager.DailyShopTableLoader.GetByKey(resourcesShops[i].key),
                    isBuyOverlay = resourcesShops[i].isBuyOverlay
                });
                ;
            }
            GameManager.Instance.Player.ResetCount = resetCount;

            // 기타 설정
            // 선호 오퍼레이터
            if (favoriteKey != 0)
                player.favoriteOperator = player.inventory.operators[favoriteKey].data;

            // 상점 갱신 시간 최신화, 공백 있으면 실패, 보정 없ㅣ
            if (DateTime.TryParseExact(
                nextDailyResetTime,
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dailyTime))
            {
                player.nextDailyResetTime = dailyTime;
            }
            else
            {
                player.nextDailyResetTime = DateTime.MinValue; // 오류시 최솟값
            }
            if (DateTime.TryParseExact(
                nextWeeklyResetTime,
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var weeklyTime))
            {
                player.nextWeeklyResetTime = weeklyTime;
            }
            else
            {
                player.nextWeeklyResetTime = DateTime.MinValue; // 오류시 최솟값
            }
        }
        public PassiveSkillData GetSkill(int skillId)
        {
            PassiveSkillData skill = Resources.Load<PassiveSkillData>($"Skill/PassiveSkill/{skillId}");
            if (skill == null)
                Debug.LogWarning($"스킬 {skillId} 못 찾음");
            return skill;
        }
    }
    [Serializable]
    private class SavedTower
    {
        public int key;
        public bool isOwned;
        public bool isEquipped;
        public SavedTower(int key, bool own, bool equip)
        {
            this.key = key;
            this.isOwned = own;
            this.isEquipped = equip;
        }
    }
    [Serializable]
    private class SavedOperator
    {
        // OwnedOperator.data
        public int key;
        public int activeLv;
        public int passive0;
        public int passive0LV;
        public int passive1;
        public int passive1LV;
        public int passive2;
        public int passive2LV;
        // OwnedOperator
        public bool isOwned;
        public bool isEquipped;
        public int grade;
        public int level;
        public int exp;

        public int equip0;
        public int equip1;
        public int equip2;
        public int equip3;
        public SavedOperator(OwnedOperator ownedTower)
        {
            this.key = ownedTower.data.operatorInfoTable.key;
            this.activeLv = ownedTower.data.activeLv;
            this.passive0LV = ownedTower.data.passive0LV;
            this.passive1LV = ownedTower.data.passive1LV;
            this.passive2LV = ownedTower.data.passive2LV;

            if (ownedTower.data.passiveSkillDatas != null)
            {
                if (ownedTower.data.passiveSkillDatas.Count > 0)
                    this.passive0 = ownedTower.data.passiveSkillDatas[0].skillTable.key;
                if (ownedTower.data.passiveSkillDatas.Count > 1)
                    this.passive1 = ownedTower.data.passiveSkillDatas[1].skillTable.key;
                if (ownedTower.data.passiveSkillDatas.Count > 2)
                    this.passive2 = ownedTower.data.passiveSkillDatas[2].skillTable.key;
            }

            this.isOwned = ownedTower.isOwned;
            this.isEquipped = ownedTower.isEquipped;
            this.grade = ownedTower.grade;
            this.level = ownedTower.level;
            this.exp = ownedTower.exp;
            this.equip0 = ownedTower.equip0;
            this.equip1 = ownedTower.equip1;
            this.equip2 = ownedTower.equip2;
            this.equip3 = ownedTower.equip3;
        }
    }
    [Serializable]
    private class SavedItem
    {
        public int key;
        public int amount;
        public SavedItem(int key, int amount)
        {
            this.key = key;
            this.amount = amount;
        }
    }
    [Serializable]
    private class SavedTowerOper
    {
        public int towerKey;
        public int operatorKey;
        public SavedTowerOper(int towerKey, int operatorKey)
        {
            this.towerKey = towerKey;
            this.operatorKey = operatorKey;
        }
    }
    [Serializable]
    private class SavedQuest
    {
        public int key;
        public int currentConditionCount;
        public bool isClear;
        public SavedQuest(int key, int currentConditionCount, bool isClear)
        {
            this.key = key;
            this.currentConditionCount = currentConditionCount;
            this.isClear = isClear;
        }
    }
    [Serializable]
    private class SavedStage
    {
        public int key;
        public bool isClear;
        public int stageClearCount;
        public int StagePlayCount;
        public SavedStage(int key, bool isClear, int clearCount, int playCount)
        {
            this.key = key;
            this.isClear = isClear;
            this.stageClearCount = clearCount;
            this.StagePlayCount = playCount;
        }
    }
    // 상점 내용
    [Serializable]
    private class SavedShopItem
    {
        public int key;
        public bool isBuyOverlay;
        public SavedShopItem(int key, bool isBuyOverlay)
        {
            this.key = key;
            this.isBuyOverlay = isBuyOverlay;
        }
    }
}

