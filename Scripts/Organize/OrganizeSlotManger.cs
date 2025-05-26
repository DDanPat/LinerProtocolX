using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 편성창의 슬롯을 관리하여 정렬 및 데이터 deck에 전달
/// 오퍼레이터는 중복체크 / 타워 중복체크 x
///
/// 타워 o / 오퍼레이터 o => 0
/// 타워 o / 오퍼레이터 x => 0
/// 타워 x / 오퍼레이터 o/x => X
/// </summary>

public class OrganizeSlotManger : MonoBehaviour
{
    [SerializeField]List<OrganizeSlot> organizeOperatorSlots = new List<OrganizeSlot>();
    [SerializeField]List<OrganizeSlot> organizeTowerSlots = new List<OrganizeSlot>();

    [SerializeField] private GameObject operatorSlot;
    [SerializeField] private GameObject towerSlot;

    private void Awake()
    {
        organizeOperatorSlots = operatorSlot.GetComponentsInChildren<OrganizeSlot>().ToList();
        organizeTowerSlots = towerSlot.GetComponentsInChildren<OrganizeSlot>().ToList();
    }

    private void OnEnable()
    {
        CheckDeckOrganized();
    }

    public void UpdateUsedInventory()
    {
        // 편성시 편성리스트 전부 검색하여 인벤토리에 사용중 표시하기
        // 프리셋 변경되면 목록 초기화하여 불러와서 인벤토리에 해제하기 위함
        GameManager.Instance.Player.inventory.ClearOrganize();

        foreach (var slot in organizeTowerSlots)
        {
            var slotData = slot.GetData();
            GameManager.Instance.Player.inventory.Organize(slotData);
        }

        foreach (var slot in organizeOperatorSlots)
        {
            var slotData = slot.GetData();
            GameManager.Instance.Player.inventory.Organize(slotData);
        }
    }
    
    public bool IsAlreadyOrganized(ScriptableObject data)
    {
        // 오퍼레이터 전용 편성이 중복되었는지 확인
        foreach (var slot in organizeOperatorSlots)
        {
            var slotData = slot.GetData();
            if (slotData != null && slotData == data)
            {
                return true;
            }
        }
        return false;
    }

    public bool SendTowerList()
    {
        // 테스트용으로 public 처리, 테스트 끝나면 private로 변경
        GameManager.Instance.Player.deck?.ClearDeck();

        HashSet<int> addedTowerIds = new HashSet<int>();

        for (int i = 0; i < organizeTowerSlots.Count; i++)
        {
            var towerData = organizeTowerSlots[i].data as TowerData;
            var operatorData = organizeOperatorSlots[i].data as OperatorData;

            // 타워 데이터가 없으면 넘어감
            if (towerData == null) continue;

            // 오퍼레이터가 있을 경우, 중복 여부 상관 없이 추가
            if (operatorData != null)
            {
                GameManager.Instance.Player.deck?.AddDeck(towerData, operatorData);
            }
            else
            {
                // 오퍼레이터가 없을 경우엔 타워 ID 중복 체크
                if (!addedTowerIds.Contains(towerData.towerInfoTable.key)) // key는 고유 식별자
                {
                    GameManager.Instance.Player.deck?.AddDeck(towerData, null);
                    addedTowerIds.Add(towerData.towerInfoTable.key);
                }
            }
        }

        return GameManager.Instance.Player.deck?.towerList.Count > 0;
    }
    
    public void SortSlots()
    {
        // 데이터가 있는 슬롯만 골라서 데이터만 추출
        var sortedOperatorData = organizeOperatorSlots
            .Where(slot => slot.data != null)
            .Select(slot => slot.data)
            .ToList();
        
        var sortedTowerData = organizeTowerSlots
            .Where(slot => slot.data != null)
            .Select(slot => slot.data)
            .ToList();

        // 순서대로 다시 슬롯에 배치
        // 오퍼레이터 편성창 정렬
        for (int i = 0; i < organizeOperatorSlots.Count; i++)
        {
            if (i < sortedOperatorData.Count)
            {
                organizeOperatorSlots[i].SetData(sortedOperatorData[i]);
            }
            else
            {
                organizeOperatorSlots[i].ClearSlot(); // 없으면 빈 슬롯으로
            }
        }
        
        // 타워 편성창 정렬
        for (int i = 0; i < organizeTowerSlots.Count; i++)
        {
            if (i < sortedTowerData.Count)
            {
                organizeTowerSlots[i].SetData(sortedTowerData[i]);
            }
            else
            {
                organizeTowerSlots[i].ClearSlot(); // 없으면 빈 슬롯으로
            }
        }
    }

    //편성창 진입시 deck에 저장된 정보를 바탕으로 슬롯에 표시
    private void CheckDeckOrganized()
    {
        if(GameManager.Instance.Player.deck.towerList.Count <=0)
            return;
        
        for (int i = 0; i < GameManager.Instance.Player.deck.towerList.Count; i++)
        {
            var deckSlot = GameManager.Instance.Player.deck.towerList[i];
            if (deckSlot == null || deckSlot.towerData == null)
            {
                organizeTowerSlots[i].ClearSlot();
                organizeOperatorSlots[i].ClearSlot();
                continue;
            }
            
            int towerKey = deckSlot.towerData.towerInfoTable.key;
            TowerData towerData = GameManager.Instance.Player.inventory.towers[towerKey].data;
            OperatorData operatorData = null;

            if (deckSlot.operatorData != null)
            {
                int operKey = deckSlot.operatorData.operatorInfoTable.key;
                operatorData = GameManager.Instance.Player.inventory.operators[operKey].data;
            }
            if (towerData != null)
            {
                organizeTowerSlots[i].SetData(towerData);
            }
            else if (towerData == null)
            {
                organizeTowerSlots[i].ClearSlot();
            }

            if (operatorData != null)
            {
                organizeOperatorSlots[i].SetData(operatorData);
            }
            else if (operatorData == null)
            {
                organizeOperatorSlots[i].ClearSlot();
            }
            
        }

        for (int i = GameManager.Instance.Player.deck.towerList.Count; i < organizeTowerSlots.Count; i++)
        {
            organizeTowerSlots[i].ClearSlot();
            organizeOperatorSlots[i].ClearSlot();
        }
    }

    public (OperatorData? opData, TowerData? towerData) IndexGetData(int index)
    {
        var opData = organizeOperatorSlots[index].data as OperatorData;
        var towerData = organizeTowerSlots[index].data as TowerData;
        
        return (opData, towerData);
    }

    public RectTransform GetTutorialSlotPosition(int i)
    {
        var pos = organizeTowerSlots[i].GetComponent<RectTransform>();
        return pos;
    }
}
