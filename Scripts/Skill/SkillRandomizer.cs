using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 스킬 교체시 사용하는 스킬 랜덤 반환
/// 액티브는 고정 패시브는 3개 랜덤 생성 및 아이템으로 교체하므로 패시브만 교체기능 있음
/// </summary>

public class SkillRandomizer : MonoBehaviour
{
    // 역할별 카테고리 확률
    private static readonly Dictionary<int, Dictionary<int, int>> roleCategoryRates = new()
    {
        { 1, new Dictionary<int, int> { { 0, 20 }, { 1, 50 }, { 2, 10 }, { 3, 20 } } }, // 공격형
        { 2, new Dictionary<int, int> { { 0, 20 }, { 1, 20 }, { 2, 50 }, { 3, 10 } } }, // 방어형
        { 3, new Dictionary<int, int> { { 0, 20 }, { 1, 10 }, { 2, 20 }, { 3, 50 } } }, // 지원형
    };
    
    private static readonly Dictionary<int, List<(int start, int end)>> passiveSkillKeyRanges = new()
    {
        { 0, new List<(int, int)> { (6500, 6514) } },                   // 일반 (6500 ~ 6513)
        { 1, new List<(int, int)> { (6600, 6613) } },                   // 공격 특화
        { 2, new List<(int, int)> { (6700, 6702), (6703, 6705) } },     // 방어 특화
        { 3, new List<(int, int)> { (6800, 6805) } },                   // 지원 특화
    };
    
    public static int GetRandomSkillKey(int role)
    {
        if (!roleCategoryRates.ContainsKey(role))
        {
            Debug.LogError($"잘못된 역할 입력: {role}");
            return -1;
        }

        // 역할에 맞는 카테고리 확률 가져오기
        var categoryRates = roleCategoryRates[role];

        // 확률로 카테고리 뽑기
        int total = categoryRates.Values.Sum();
        int randomValue = Random.Range(0, total);

        int cumulative = 0;
        int selectedCategory = 0;

        foreach (var pair in categoryRates)
        {
            cumulative += pair.Value;
            if (randomValue < cumulative)
            {
                selectedCategory = pair.Key;
                break;
            }
        }
        
        var ranges = passiveSkillKeyRanges[selectedCategory];
        var selectedRange = ranges[Random.Range(0, ranges.Count)];
        int randomSkillKey = Random.Range(selectedRange.start, selectedRange.end);
        return randomSkillKey;
    }
}
