using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorLevelUpPopup : PopupWindow
{
   private OperatorData opData;
   public Button applyButton;
   public Slider expSlider;


   [Header("Upgrade Required Info")]
   [SerializeField] private TextMeshProUGUI requireUpgradeGoldText;
   [SerializeField] private int showLevel;
   private int levelCap;
   public bool isLevelCap;
   public bool isGoldCap;
   private int windowSumLevel;
   [SerializeField] private int windowSumExp;   // 윈도우에서 버튼 클릭시 모이는 exp
   private int remainingExp;                    // 레벨 계산후 남은 경험치 보관
   [SerializeField] private int _requireLevelUpGold;

   [SerializeField] private List<InventorySlot> expItemSlots;

   [SerializeField] private TextMeshProUGUI nowLevelText;
   [SerializeField] private List<TextMeshProUGUI> nowStatText;
   [SerializeField] private TextMeshProUGUI nextLevelText;
   [SerializeField] private List<TextMeshProUGUI> nextStatText;

   public int requiredLevelUpGold
   {
      get { return _requireLevelUpGold; }
      set
      {
         _requireLevelUpGold = value;
         UpdateGoldText();
      }
   }

   private void UpdateGoldText()
   {
      requireUpgradeGoldText.text = _requireLevelUpGold.ToString("N0");
   }

   private void Start()
   {
      PopupOperatorWindow window = gameObject.GetComponentInParent<PopupOperatorWindow>();
      applyButton.onClick.AddListener(ApplyButton);
      applyButton.onClick.AddListener(window.SetPopupUI);
   }

   public override void SetPopupUI()
   {
      opData = popupData as OperatorData;
      if (opData == null) return;
      showLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].level;
      levelCap = 20 // 1성 30 / 2성 40 ... 5성 70
                 + 10 * GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade;
      windowSumLevel = 0;
      remainingExp = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].exp;
      requiredLevelUpGold = 0;

      isLevelCap = showLevel == levelCap;
      isGoldCap = false;

      for (int i = 0; i < expItemSlots.Count; i++)
      {
         var levelUpHandler = expItemSlots[i].GetComponent<SlotLevelUpHandler>();
         levelUpHandler.ClearSlot();
         expItemSlots[i].data = GameManager.Instance.Player.inventory.items[9100 + i].data;
      }

      applyButton.interactable = false;
      NowLevelUISet();

   }

   private void NowLevelUISet()
   {

      nowLevelText.text = "LV " + showLevel.ToString();
      nowStatText[0].text = "+" + opData.operatorInfoTable.attack;
      nowStatText[1].text = "+" + opData.operatorInfoTable.attackRate;
      nowStatText[2].text = "+" + opData.operatorInfoTable.range;
      nowStatText[3].text = "+" + opData.operatorInfoTable.accuracy;
      nowStatText[4].text = "+" + opData.operatorInfoTable.criticalRate;
      nowStatText[5].text = "+" + opData.operatorInfoTable.criticalMultiplier;

      RefreshNextLevelUI();

   }

   // 게이지가 넘어가 레벨업 미리보기 갱신할때 호출
   private void RefreshNextLevelUI()
   {

      nextLevelText.text = "LV " + (showLevel + windowSumLevel);
      nextStatText[0].text = "+" + (opData.operatorInfoTable.attack
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].attack);
      nextStatText[1].text = "+" + (opData.operatorInfoTable.attackRate
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].attackRate);
      nextStatText[2].text = "+" + (opData.operatorInfoTable.range
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].range);
      nextStatText[3].text = "+" + (opData.operatorInfoTable.accuracy
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].accuracy);
      nextStatText[4].text = "+" + (opData.operatorInfoTable.criticalRate
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].criticalRate);
      nextStatText[5].text = "+" + (opData.operatorInfoTable.criticalMultiplier
                                    + windowSumLevel * opData.RoleStatBonus[opData.operatorInfoTable.role].criticalMultiplier);

      expSlider.value = (float)remainingExp / GetExpRequiredForLevel(showLevel + windowSumLevel);
   }

   public void OnSlotChanged()
   {

      windowSumExp = 0;
      requiredLevelUpGold = 0;

      for (int i = 0; i < expItemSlots.Count; i++)
      {
         var levelupHandler = expItemSlots[i].GetComponent<SlotLevelUpHandler>();
         windowSumExp += levelupHandler.GetSlotTotalEXP();

         switch (i)
         {
            case 0:
               requiredLevelUpGold += 25 * levelupHandler.GetSlotAmount();
               break;
            case 1:
               requiredLevelUpGold += 40 * levelupHandler.GetSlotAmount();
               break;
            case 2:
               requiredLevelUpGold += 65 * levelupHandler.GetSlotAmount();
               break;
            case 3:
               requiredLevelUpGold += 140 * levelupHandler.GetSlotAmount();
               break;
         }
      }

      windowSumExp += GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].exp;

      isGoldCap = GameManager.Instance.Player.Gold < requiredLevelUpGold;


      UpdateEXPUI(windowSumExp);
   }

   private void UpdateEXPUI(int totalEXP)
   {
      var result = CalculateLevelUpProgress(showLevel, totalEXP, levelCap);
      windowSumLevel = result.levelUpCount;
      remainingExp = result.remainingExp;
      isLevelCap = result.isLevelCap;

      if (isGoldCap)
         requireUpgradeGoldText.color = new Color(192 / 255f, 0 / 255f, 0 / 255f);
      else
         requireUpgradeGoldText.color = new Color(0 / 255f, 176 / 255f, 80 / 255f);

      RefreshNextLevelUI();
      applyButton.interactable = ApplyInteractable();
   }

   // 레벨업 공식 n레벨의 경험치 요구량 = 50 * (1.07)^(n - 1)
   private int GetExpRequiredForLevel(int level)
   {
      // 소숫점 올림처리
      return Mathf.CeilToInt(50f * Mathf.Pow(1.07f, level - 1));
   }

   // 총 EXP 레벨캡에 제한 맞춘 계산
   private (int levelUpCount, int remainingExp, bool isLevelCap) CalculateLevelUpProgress(int currentLevel, int currentExp, int maxLevel)
   {
      int exp = currentExp;
      int level = currentLevel;
      bool reachedLevelCap = false;

      while (level < maxLevel)
      {
         int expNeeded = GetExpRequiredForLevel(level);
         if (exp < expNeeded)
            break;

         exp -= expNeeded;
         level++;
      }

      if (level >= maxLevel)
      {
         reachedLevelCap = true;
         exp = 0; // 초과 경험치는 무시
      }

      return (level - currentLevel, exp, reachedLevelCap);
   }
   int Curcount50 = 0;
   int Curcount70 = 0;
   private void CheckCurOperLevel()
   {
      foreach (var data in GameManager.Instance.Player.inventory.operators)
      {
         if (data.Value.isOwned)
         {
            if (data.Value.level >= 50)
            {
               Curcount50++;
            }
            if (data.Value.level >= 70)
            {
               Curcount70++;
            }
         }
      }
   }
   private void CheckOperLevel()
   {
      int count50 = 0;
      int count70 = 0;

      foreach (var data in GameManager.Instance.Player.inventory.operators)
      {
         if (data.Value.isOwned)
         {
            if (data.Value.level >= 50)
            {
               count50++;
            }
            if (data.Value.level >= 70)
            {
               count70++;
            }
         }
      }


      if (count50 - Curcount50 != 0) QuestManager.Instance.UpdateQuestProgress(QuestConditions.operatorLevelup50, QuestConditions.none, count50);
      if (count70 - Curcount70 != 0) QuestManager.Instance.UpdateQuestProgress(QuestConditions.operatorLevelup70, QuestConditions.none, count70);
   }



   private void ApplyButton()
   {
      CheckCurOperLevel();

      int beforeLevel = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].level;
      // 남은 경험치는 오퍼레이터에 저장 (레벨캡이 아닐 때만)
      GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].exp =
         isLevelCap ? 0 : remainingExp;

      GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].LevelUP(windowSumLevel);
      // 퀘스트 - 레벨업 반영
      QuestManager.Instance.UpdateQuestProgress(QuestConditions.operatorLevelup);


      for (int i = 0; i < expItemSlots.Count; i++)
      {
         var levelUpHandler = expItemSlots[i].GetComponent<SlotLevelUpHandler>();
         levelUpHandler.ApplyLevelUp();
      }

      // 커스텀 이벤트 - 레벨업
      AnalyticsManager.Instance.SendEvent(Define.OperatorLevelup, new Dictionary<string, object> {
      { Define.operatorKey, opData.operatorInfoTable.key },
      { Define.operatorGrade, GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade },
      { Define.operatorLevelBefore, beforeLevel },
      { Define.operatorLevelAfter, GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].level },
      //{ Define.numberBasicSDUsed, opData.operatorInfoTable.key },
      //{ Define.numberPreciseSDUsed, opData.operatorInfoTable.key },
      //{ Define.numberSpecialSDUsed, opData.operatorInfoTable.key },
      //{ Define.numberCoreSDUsed, opData.operatorInfoTable.key },
      });


      GameManager.Instance.Player.Gold -= requiredLevelUpGold;
      // 임의 저장
      GameManager.Instance.SavePlayer();
      // 커스텀 이벤트 - 코인 변화
      AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
            { Define.changeType, Define.spent },
            { Define.numberCoinChanged, requiredLevelUpGold},
            { Define.changeSource, 2},
            { Define.numberCoinBefore, GameManager.Instance.Player.Gold + requiredLevelUpGold},
      });
      CheckOperLevel();
      SetPopupUI();
   }

   private bool ApplyInteractable()
   {
      bool interactable = true;

      if (isGoldCap)
         interactable = false;
      if (windowSumLevel == 0 &&
         windowSumExp == GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].exp)
         interactable = false;


      return interactable;
   }

}
