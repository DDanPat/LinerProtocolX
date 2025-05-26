using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GradeUpgradeData", menuName = "Data/Upgrade/OperatorGrade")]
public class OperatorGradeUpgradeData : ScriptableObject
{
    public int gradeLevel;      // 1성 -> 2성 업그레이드 할시 = 2
    public int requiredGold;    // 등급 상승시 필요한 골드재화
    public List<RequiredItem> requiredItems;  // 등급 상승시 필요한 재화 모음 오퍼레이터 등급은 신경망만추가
}
