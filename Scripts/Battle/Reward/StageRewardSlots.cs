using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType
{
    None,
    Item,
    Enemy,
}
public class StageRewardSlots : RewardSlotsBase
{
    [SerializeField] private SlotType slotType;
    private void Start()
    {
        itemButton.onClick.AddListener(OnClickItemButton);
    }

    private void OnClickItemButton()
    {
        // TODO 아이템 정보 패널 표시
        UIManager.Instance.lobbyUIHandler.ItemInfoPopup.OnPupup();
        switch (slotType)
        {
            case SlotType.Item:
                UIManager.Instance.lobbyUIHandler.ItemInfoPopup.SetItemPopupInfo(rewardItem, itemIcon.sprite);
                break;
            case SlotType.Enemy:
                UIManager.Instance.lobbyUIHandler.ItemInfoPopup.SetEnemyPopupInfo(enemydata, itemIcon.sprite);
                break;

        }
        
    }
}
