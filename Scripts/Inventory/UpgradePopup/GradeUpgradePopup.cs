using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GradeUpgradePopup : PopupWindow
{
    private OperatorData opData;
    public Button applyButton;

    [SerializeField] private Sprite[] gradeSprite = new Sprite[2];

    [Header("Next Upgrade Info")]
    [SerializeField] private GameObject nowGrade;
    [SerializeField] private Image[] nowGradeImage = new Image[5];
    [SerializeField] private Image nextImage;
    [SerializeField] private GameObject nextGrade;
    [SerializeField] private Image[] nextGradeImage = new Image[5];
    [SerializeField] private TextMeshProUGUI nowLevelText;
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private TextMeshProUGUI nextInfoText;


    [Header("Upgrade Required Info")]
    [SerializeField] private TextMeshProUGUI requireUpgradeGoldText;
    [SerializeField] private int showGrade;     // 1 : 1성 -> 2성
    [SerializeField] private int _requireUpgradeGold;

    [SerializeField] private List<InventorySlot> requiredItemSlots;
    [SerializeField] private List<TextMeshProUGUI> requiredItemTexts;

    public int requiredUpgradeGold
    {
        get { return _requireUpgradeGold; }
        set
        {
            _requireUpgradeGold = value;
            UpdateGoldText();
        }
    }

    private void UpdateGoldText()
    {
        requireUpgradeGoldText.text = _requireUpgradeGold.ToString("N0");
    }

    private void Start()
    {
        PopupOperatorWindow opWindow = GetComponentInParent<PopupOperatorWindow>();
        applyButton.onClick.AddListener(ApplyUpgrade);
        applyButton.onClick.AddListener(opWindow.SetPopupUI);
    }


    public override void SetPopupUI()
    {
        opData = popupData as OperatorData;
        if (opData == null) return;
        showGrade = GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade;

        for (int i = 0; i < requiredItemSlots.Count; i++)
        {
            requiredItemSlots[i].ClearSlot();
            requiredItemTexts[i].text = "";
            requireUpgradeGoldText.text = "";
        }


        switch (showGrade)
        {
            case 1: // 1-> 2 등급으로
            case 2:
            case 3:
            case 4:
                for (int i = 0; i < opData.GradeRequire[showGrade - 1].requiredItems.Count; i++)
                {
                    requiredItemSlots[i].data = opData.GradeRequire[showGrade - 1].requiredItems[i].item;
                }
                requiredUpgradeGold = opData.GradeRequire[showGrade - 1].requiredGold;
                applyButton.interactable = IsUpgradeRequired();
                SetInfoUI(showGrade);
                break;

            case 5: // 최종등급 강화 불가판정
                applyButton.interactable = false;
                SetInfoUIMAX();
                break;
        }

    }

    private void SetGradeStars(int gradeLevel)
    {
        for (int i = 0; i < nowGradeImage.Length; i++)
        {
            if (i < gradeLevel)
            {
                nowGradeImage[i].sprite = gradeSprite[1];
                nowGradeImage[i].color = Color.white;
            }
            else
            {
                nowGradeImage[i].sprite = gradeSprite[0];
                nowGradeImage[i].color = Color.black;
            }
        }

        for (int i = 0; i < nextGradeImage.Length; i++)
        {
            if (i < gradeLevel + 1)
            {
                nextGradeImage[i].sprite = gradeSprite[1];
                nextGradeImage[i].color = Color.white;
            }
            else
            {
                nextGradeImage[i].sprite = gradeSprite[0];
                nextGradeImage[i].color = Color.black;
            }
        }
    }

    private void SetInfoUI(int grade)
    {
        // List[0] = 1성 이미지, List[4] = 5성 이미지
        SetGradeStars(grade);
        nowGrade.SetActive(true);
        nextImage.enabled = true;
        nextGrade.SetActive(true);


        nowLevelText.text = "LV " + (20 + grade * 10).ToString();
        nextText.text = "최대레벨";
        nextLevelText.text = "LV " + (20 + (grade + 1) * 10).ToString();

        switch (grade)
        {
            case 1:
                nextInfoText.text = "액티브 스킬 1 해금";
                break;
            case 2:
            case 3:
            case 4:
                nextInfoText.text = $"패시브 스킬 {grade - 1} 해금";
                break;

        }
    }

    private void SetInfoUIMAX()
    {
        nowGrade.SetActive(false);
        nextGrade.SetActive(false);
        nextImage.enabled = false;

        nowLevelText.text = "";
        nextText.text = "최대 등급에 도달했습니다";
        nextLevelText.text = "";
        nextInfoText.text = "";
    }


    // 등급 업그레이드 조건 확인
    private bool IsUpgradeRequired()
    {
        bool result = true;

        for (int i = 0; i < requiredItemSlots.Count; i++)
        {
            if (requiredItemSlots[i].data != null && requiredItemSlots[i].data is ItemData itemData)
            {
                // 아이콘 슬롯 하단의 텍스트 항목에 소지중 / 업그레이드 필요갯수
                requiredItemTexts[i].text =
                    GameManager.Instance.Player.inventory.items[itemData.key].amount.ToString() + "/"
                    + opData.GradeRequire[showGrade - 1].requiredItems[i].count.ToString();

                if (GameManager.Instance.Player.inventory.items[itemData.key].amount
                    < opData.GradeRequire[showGrade - 1].requiredItems[i].count)
                {
                    // (소지중 / 업그레이드 갯수) 부족
                    requiredItemTexts[i].color = new Color(192 / 255f, 0 / 255f, 0 / 255f);
                    result = false;
                }
                else
                {
                    requiredItemTexts[i].color = new Color(0 / 255f, 176 / 255f, 80 / 255f);
                }
            }
        }

        // 소지중인 골드 / 업그레이드 필요한 골드 부족
        if (GameManager.Instance.Player.Gold < requiredUpgradeGold)
        {
            requireUpgradeGoldText.color = new Color(192 / 255f, 0 / 255f, 0 / 255f);
            result = false;
        }
        else
        {
            requireUpgradeGoldText.color = new Color(0 / 255f, 176 / 255f, 80 / 255f);
        }

        return result;
    }

    private void ApplyUpgrade()
    {

        // 커스텀 이벤트 - 승급
        AnalyticsManager.Instance.SendEvent(Define.OperatorGradeup, new Dictionary<string, object> {
        { Define.operatorKey, opData.operatorInfoTable.key },
        { Define.operatorGradeBefore, GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].grade },
        { Define.operatorLevel, GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].level },
        });
        GameManager.Instance.Player.inventory.operators[opData.operatorInfoTable.key].Upgrade();

        for (int i = 0; i < requiredItemSlots.Count; i++)
        {
            if (requiredItemSlots[i].data != null && requiredItemSlots[i].data is ItemData itemData)
            {
                GameManager.Instance.Player.inventory.items[itemData.key].amount -=
                    opData.GradeRequire[showGrade - 1].requiredItems[i].count;
            }
        }
        GameManager.Instance.Player.Gold -= requiredUpgradeGold;
        // 커스텀 이벤트 - 코인 변화
        AnalyticsManager.Instance.SendEvent(Define.CoinChanged, new Dictionary<string, object> {
            { Define.changeType, Define.spent },
            { Define.numberCoinChanged, requiredUpgradeGold},
            { Define.changeSource, 1},
            { Define.numberCoinBefore, GameManager.Instance.Player.Gold + requiredUpgradeGold},
        });
        // 퀘스트 - 등급업 반영
        QuestManager.Instance.UpdateQuestProgress(QuestConditions.operatorGradeup);

        var key = opData.operatorInfoTable.key;
        PassiveSkillData skill = null;
        if (1 < showGrade)
        {
            int newSkillKey = -1;

            if (opData.passiveSkillDatas?.Count > 0)
            {
                bool isDuplicated;

                do
                {
                    isDuplicated = false;
                    newSkillKey = SkillRandomizer.GetRandomSkillKey(opData.operatorInfoTable.role);

                    foreach (var passive in opData.passiveSkillDatas)
                    {
                        if (passive?.skillTable?.key == newSkillKey)
                        {
                            isDuplicated = true;
                            break;
                        }
                    }
                } while (isDuplicated);
            }

            else
            {
                newSkillKey = SkillRandomizer.GetRandomSkillKey(opData.operatorInfoTable.role);
            }

            skill = SkillManager.Instance.GetSkill(newSkillKey);

            if (skill == null)
            {
                Debug.LogWarning($"SkillKey {newSkillKey}에 해당하는 스킬이 없습니다.");
                return;
            }

        }

        if (opData.passiveSkillDatas == null)
        {
            opData.passiveSkillDatas = new List<PassiveSkillData>();
        }
        int requiredSlotCount = showGrade - 1; // Grade 2 → 패시브 1칸, Grade 3 → 패시브 2칸, Grade 4 → 패시브 3칸

        // 현재 개수보다 적으면 필요한 만큼만 null 채움
        while (opData.passiveSkillDatas.Count < requiredSlotCount)
        {
            opData.passiveSkillDatas.Add(null);
        }
        int insertIndex = showGrade - 2;
        switch (showGrade)
        {
            case 2:
                // 새 스킬 삽입
                opData.passiveSkillDatas[insertIndex] = skill;
                GameManager.Instance.Player.inventory.operators[key].data.passive0LV = 1;
                break;
            case 3:
                opData.passiveSkillDatas[insertIndex] = skill;
                GameManager.Instance.Player.inventory.operators[key].data.passive1LV = 1;
                break;
            case 4:
                opData.passiveSkillDatas[insertIndex] = skill;
                GameManager.Instance.Player.inventory.operators[key].data.passive2LV = 1;
                break;
        }
        // 임의 저장
        GameManager.Instance.SavePlayer();
        SetPopupUI();
    }


}